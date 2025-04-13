using Newtonsoft.Json;
using QueueServices.contracts;
using QueueServices.dtos;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using RabbitMQ.Client.Exceptions;

namespace QueueServices.services
{
    public class QueueHandler : IQueueHandler
    {
        public event EventHandler<ReceivedEventArgs>? ReceivedEvent;
        public event EventHandler<ReceivedSetupFailedEventArgs>? ReceivedSetupFailedEvent;
        public event EventHandler<ReceivedStartedEventArgs>? ReceivedStartedEvent;

        public event EventHandler<EmitterEventArgs>? EmitterEvent;

        private IChannel _channel;
        private IConnection _connection;
        private bool disposed = false;

        public void Dispose()
        {
            DisposeAsync(true).GetAwaiter().GetResult();
            GC.SuppressFinalize(this);
        }

        protected virtual async Task DisposeAsync(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_channel != null)
                    {
                        try
                        {
                            await _channel.DisposeAsync();
                        }
                        catch (Exception ex)
                        {
                            // Registrar el error si lo deseas
                            Console.WriteLine($"Error disposing _channel: {ex.Message}");
                        }
                    }

                    if (_connection != null)
                    {
                        try
                        {
                            await _connection.DisposeAsync();
                        }
                        catch (Exception ex)
                        {
                            // Registrar el error si lo deseas
                            Console.WriteLine($"Error disposing _connection: {ex.Message}");
                        }
                    }
                }

                disposed = true;
            }
        }

        public async Task StartConsume(MQSettings mQSettings, string queueName)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = mQSettings.Host,
                    Port = mQSettings.Port,
                    UserName = mQSettings.Uid,
                    Password = mQSettings.Pwd
                };

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                await _channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += Consumer_ReceivedAsync;

                await _channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);

                ReceivedStartedEvent?.Invoke(this, new ReceivedStartedEventArgs { QueueName = queueName });
            }
            catch (BrokerUnreachableException ex)
            {
                // Manejo específico para errores de conexión                
                ReceivedSetupFailedEvent?.Invoke(this, new ReceivedSetupFailedEventArgs { Message = ex.Message, QueueName = queueName });
            }
            catch (OperationInterruptedException ex)
            {
                // Manejo específico para errores de operación interrumpida                
                ReceivedSetupFailedEvent?.Invoke(this, new ReceivedSetupFailedEventArgs { Message = ex.Message, QueueName = queueName });
            }
            catch (Exception ex)
            {
                // Manejo genérico de excepciones               
                ReceivedSetupFailedEvent?.Invoke(this, new ReceivedSetupFailedEventArgs { Message = ex.Message, QueueName = queueName });
            }
        }

        private async Task Consumer_ReceivedAsync(object sender, BasicDeliverEventArgs @event)
        {
            byte[] body = @event.Body.ToArray();
            string payload = Encoding.UTF8.GetString(body);

            try
            {
                ReceivedEvent?.Invoke(this, new ReceivedEventArgs { Payload = payload });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en el evento ReceivedEvent: {ex.Message}");
            }

            await Task.Delay(1);
        }

        public async Task StopConsume()
        {
            if (_channel != null)
            {
                try
                {
                    await _channel.AbortAsync();
                }
                catch (Exception ex)
                {
                    // Registrar error
                    Console.WriteLine($"Error al abortar el canal: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("El canal no está inicializado.");
            }
        }

        public async Task SendMessageAsync<T>(T data, MQSettings mQSettings, string queueName)
        {
            string payload = JsonConvert.SerializeObject(data);
            await SendMessageAsync(payload, mQSettings, queueName);            
        }

        public async Task SendMessageAsync(string payload, MQSettings mQSettings, string queueName)
        {
            try
            {
                // Asegúrate de que la configuración de la conexión es válida
                if (string.IsNullOrEmpty(mQSettings.Host) || mQSettings.Port <= 0 || string.IsNullOrEmpty(mQSettings.Uid) || string.IsNullOrEmpty(mQSettings.Pwd))
                {
                    throw new InvalidOperationException("La configuración de RabbitMQ es inválida.");
                }

                // Crear una nueva instancia de ConnectionFactory
                var factory = new ConnectionFactory
                {
                    HostName = mQSettings.Host,
                    Port = mQSettings.Port,
                    UserName = mQSettings.Uid,
                    Password = mQSettings.Pwd
                };

                Console.WriteLine("Intentando conectar a RabbitMQ...");

                // Intentar crear la conexión y canal
                using (var connection = await factory.CreateConnectionAsync())
                using (var channel = await connection.CreateChannelAsync())
                {
                    // Asegúrate de que la conexión y el canal se hayan creado correctamente
                    if (connection == null || channel == null)
                    {
                        throw new InvalidOperationException("La conexión o el canal no se pudieron inicializar.");
                    }

                    Console.WriteLine("Conexión y canal creados exitosamente.");

                    // Declarar la cola
                    await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                    byte[] body = Encoding.UTF8.GetBytes(payload);

                    // Crear un objeto de propiedades básicas
                    var properties = new BasicProperties();
                    properties.Persistent = true;  // Marcar el mensaje como persistente

                    // Publicar el mensaje en la cola
                    await channel.BasicPublishAsync(
                        exchange: "",
                        routingKey: queueName,
                        mandatory: true,
                        basicProperties: properties,
                        body: body);

                    Console.WriteLine("Mensaje enviado a la cola.");

                    // Invocar evento de éxito
                    EmitterEvent?.Invoke(this, new EmitterEventArgs { QueueName= queueName, Payload = payload, Success = true });
                }
            }
            catch (Exception ex)
            {
                // Registrar el error
                Console.WriteLine($"Error al enviar mensaje a RabbitMQ: {ex.Message}");

                // Invocar evento de error
                EmitterEvent?.Invoke(this, new EmitterEventArgs { StatusMessage = ex.Message, Success = false, QueueName = queueName });
            }
        }
    }
}
