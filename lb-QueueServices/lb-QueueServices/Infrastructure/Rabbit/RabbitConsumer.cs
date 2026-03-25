using lb_QueueServices.Domain.Contracts;
using lb_QueueServices.Domain.Events;
using lb_QueueServices.Domain.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace lb_QueueServices.Infrastructure.Rabbit
{
    public sealed class RabbitConsumer :IQueueConsumer, IQueueMonitor, IAsyncDisposable
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private AsyncEventingBasicConsumer? _consumer;

        private readonly SemaphoreSlim _channelLock = new(1, 1);
        private RetryPolicy _retryPolicy = new();

        private bool _disposed;

        //public event EventHandler<QueueMessageReceivedEvent>? MessageReceived;
        public event Func<object?, QueueMessageReceivedEvent, Task>? MessageReceived;
        public event EventHandler<QueueMessageReceivedEvent>? MessageObserved;
        public event EventHandler<QueueErrorEvent>? Error;

        #region Start

        public async Task StartAsync(QueueContext context, RetryPolicy? retry = null)
        {
            EnsureNotDisposed();
            _retryPolicy = retry ?? new RetryPolicy();

            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = context.Host,
                    Port = context.Port,
                    UserName = context.User,
                    Password = context.Password,                   
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
                };

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                if (context.UseBasicQos) {
                    await _channel.BasicQosAsync(
                        prefetchSize: context.BasicQosPrefetchSize,
                        prefetchCount: context.BasicQosPrefetchCount,
                        global: context.BasicQosGlobal
                    );
                }

                await _channel.ExchangeDeclareAsync(
                    exchange: context.Exchange,
                    type: context.ExchangeType,
                    durable: true);

                await _channel.QueueDeclareAsync(
                    queue: context.Queue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: context.Arguments);

                await _channel.QueueBindAsync(
                    queue: context.Queue,
                    exchange: context.Exchange,
                    routingKey: context.RoutingKey);

                _consumer = new AsyncEventingBasicConsumer(_channel);
                _consumer.ReceivedAsync += OnMessageReceivedAsync;

                await _channel.BasicConsumeAsync(
                    queue: context.Queue,
                    autoAck: false,
                    consumer: _consumer);
            }
            catch (Exception ex)
            {
                Error?.Invoke(this, new QueueErrorEvent(ex));
                throw;
            }
        }

        public async Task StartMonitoringAsync(QueueContext context)
        {
            EnsureNotDisposed();

            // Variante futura:
            // autoAck = true
            // ReceivedAsync -> MessageObserved
        }

        #endregion

        #region Handlers

        private static IReadOnlyDictionary<string, object>? SanitizeHeaders( IDictionary<string, object?>? headers)
        {
            if (headers == null || headers.Count == 0)
                return null;

            return headers
                .Where(kv => kv.Value != null)
                .ToDictionary(
                    kv => kv.Key,
                    kv => kv.Value!);
        }

        private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs args)
        {
            try
            {
                var sanitizedHeaders = SanitizeHeaders(args.BasicProperties?.Headers);
                var channel = _channel!;

                QueueMessageReceivedEvent eventArgs = null!;

                eventArgs = new QueueMessageReceivedEvent(
                    queueName: args.RoutingKey,
                    body: args.Body.ToArray(),
                    ack: async () =>
                    {
                        await _channelLock.WaitAsync();
                        try
                        {
                            await channel!.BasicAckAsync(args.DeliveryTag, false);
                        }
                        finally
                        {
                            _channelLock.Release();
                        }
                    },
                    nack: async (requeue) =>
                    {
                        await _channelLock.WaitAsync();

                        try
                        {
                            var retry = eventArgs.GetRetryCount();
                            if (retry >= _retryPolicy.MaxRetries)
                            {
                                await channel!.BasicNackAsync(args.DeliveryTag, false, false);
                                return;
                            }
                            else if (requeue) {
                                retry++;

                                var originalProps = args.BasicProperties;

                                var props = new BasicProperties
                                {
                                    Persistent = true,
                                    ContentType = originalProps?.ContentType,
                                    ContentEncoding = originalProps?.ContentEncoding,
                                    CorrelationId = originalProps?.CorrelationId,
                                    MessageId = originalProps?.MessageId,
                                    Type = originalProps?.Type,
                                    AppId = originalProps?.AppId,
                                    Priority = originalProps?.Priority ?? 0,
                                    Headers = new Dictionary<string, object?>()
                                };

                                if (originalProps?.Headers != null)
                                {
                                    foreach (var h in originalProps.Headers)
                                    {
                                        props.Headers[h.Key] = h.Value;
                                    }
                                }

                                props.Headers["x-retry-count"] = retry;

                                var body = args.Body.ToArray();

                                await channel!.BasicPublishAsync(
                                    exchange: args.Exchange,
                                    routingKey: args.RoutingKey,
                                    mandatory: false,
                                    basicProperties: props,
                                    body: body
                                );

                                await channel.BasicAckAsync(args.DeliveryTag, false);
                            }
                            else
                            {
                                await channel.BasicNackAsync(args.DeliveryTag, false, false);
                            }
                        }
                        finally
                        {
                            _channelLock.Release();
                        }
                    },
                    headers: sanitizedHeaders);

                if (MessageReceived != null)
                {
                    var handlers = MessageReceived.GetInvocationList();

                    foreach (var handler in handlers)
                    {
                        var asyncHandler = (Func<object?, QueueMessageReceivedEvent, Task>)handler;
                        await asyncHandler(this, eventArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                Error?.Invoke(this, new QueueErrorEvent(ex));
            }
        }

        #endregion

        #region Stop & Dispose

        public async Task StopAsync()
        {
            await DisposeAsync();
        }

        public async Task StopMonitoringAsync()
        {
            await DisposeAsync();
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;

            try
            {
                if (_consumer != null)
                    _consumer.ReceivedAsync -= OnMessageReceivedAsync;

                if (_channel?.IsOpen == true)
                    await _channel.CloseAsync();

                if (_connection?.IsOpen == true)
                    await _connection.CloseAsync();
            }
            catch
            {
                // Dispose nunca debe lanzar
            }
            finally
            {
                _channel?.Dispose();
                _connection?.Dispose();
            }
        }

        public void Dispose()
            => DisposeAsync().AsTask().GetAwaiter().GetResult();

        private void EnsureNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(RabbitConsumer));
        }

        #endregion


    }

}
