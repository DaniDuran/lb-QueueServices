# lb-QueueServices

lb-QueueServices es una librería de clases para .NET diseñada para
simplificar la integración con RabbitMQ, proporcionando componentes
reutilizables para la publicación y consumo de mensajes en colas
mediante una arquitectura desacoplada basada en Dependency Injection.

La librería encapsula la gestión de conexiones, exchanges, colas y bindings,
permitiendo a las aplicaciones enfocarse únicamente en la lógica de negocio.

------------------------------------------------------------------------

# Características

-	Publicación de mensajes en RabbitMQ
-	Consumo asíncrono de colas
-	Configuración mediante QueueContext
-	Integración con Dependency Injection
-	Manejo de Exchange (Fanout / Topic)
-	Soporte para headers y propiedades de mensaje
-	Control de prefetch y QoS
-	Manejo de errores y eventos de consumo
-	Escalabilidad para sistemas orientados a eventos

------------------------------------------------------------------------

# Versiones

## v1.0.0

Creación de la librería base para gestión de colas.

Incluye:

-   Configuración base de conexión a RabbitMQ
-   Creación de exchanges
-   Creación de colas
-   Abstracción mediante interfaces
-   Soporte para inyección de dependencias

------------------------------------------------------------------------

## v2.0.2

Implementación de publicación y consumo de colas.

Incluye:

-   Publicación de mensajes (RabbitPublisher)
-   Consumo de mensajes (RabbitConsumer)
-   Manejo de eventos de consumo
-   Ack/Nack automático
-   Manejo de errores

------------------------------------------------------------------------

## v2.0.3

Soporte para prioridad y control de registros disponibles.

Incluye:

-   Prioridad de mensajes
-   Headers personalizados
-   Control de QoS
-   Configuración de prefetch
-   Optimización para alto throughput

------------------------------------------------------------------------

# Arquitectura del Proyecto

Estructura general del proyecto:

lb-QueueServices

-   Consumers
    -   RabbitConsumer.cs
-   Publishers
    -   RabbitPublisher.cs
-   Context
    -   QueueContext.cs
-   Events
    -   QueueMessageReceivedEvent.cs
    -   QueueErrorEvent.cs
-   Interfaces
    -   IQueueConsumer.cs
    -   IQueuePublisher.cs
    -   IQueueMonitor.cs
-   Extensions
    -   ServiceCollectionExtensions.cs
-   Retry
    -   RetryPolicy.cs

------------------------------------------------------------------------

# Componentes principales

## RabbitPublisher

Encargado de publicar mensajes en RabbitMQ.

Funciones principales:

-   Crear exchange
-   Crear colas
-   Enlazar colas
-   Publicar mensajes

------------------------------------------------------------------------

## RabbitConsumer

Encargado de consumir mensajes de una cola.

Características:

-   Consumo asincrónico
-   Manejo automático de ACK
-   Manejo de errores
-   Eventos de consumo

------------------------------------------------------------------------

# Instalación

La librería se distribuye como **NuGet Package**.

Instalación mediante CLI:

dotnet add package lb-QueueServices

O desde NuGet Package Manager:
Install-Package lb-QueueServices

------------------------------------------------------------------------

# Configuración en Program.cs

Para habilitar los servicios en el contenedor de dependencias:

using QueueServices.Extensions;

builder.Services.AddQueueServices();

Internamente registra:

services.AddScoped\<IQueueConsumer, RabbitConsumer\>();
services.AddScoped\<IQueueMonitor, RabbitConsumer\>();
services.AddScoped\<IQueuePublisher, RabbitPublisher\>();

------------------------------------------------------------------------

# Configuración de QueueContext

Ejemplo:

var context = new QueueContext 
	{ 
		Host = "localhost",
		Port = 5672, User = "guest",
		Password = "guest",
		Exchange = "orders.exchange",
		ExchangeType = ExchangeType.Topic,
		Queue = "orders.queue",
		RoutingKey = "orders.created",
		UseBasicQos = true,
		BasicQosPrefetchCount = 10
	};

------------------------------------------------------------------------

# Publicación de mensajes

public class OrderService { private readonly IQueuePublisher
\_publisher;

    public OrderService(IQueuePublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task PublishOrderAsync(OrderCreated order)
    {
        var context = new QueueContext
        {
            Host = "localhost",
            Port = 5672,
            User = "guest",
            Password = "guest",
            Exchange = "orders.exchange",
            ExchangeType = ExchangeType.Topic,
            Queue = "orders.queue",
            RoutingKey = "orders.created"
        };

        await _publisher.PublishAsync(order, context);
    }

}

------------------------------------------------------------------------

# Consumo de mensajes

public class OrderConsumerService : BackgroundService { private readonly
IQueueConsumer \_consumer;

    public OrderConsumerService(IQueueConsumer consumer)
    {
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var context = new QueueContext
        {
            Host = "localhost",
            Port = 5672,
            User = "guest",
            Password = "guest",
            Exchange = "orders.exchange",
            ExchangeType = ExchangeType.Topic,
            Queue = "orders.queue",
            RoutingKey = "orders.created"
        };

        _consumer.MessageReceived += async (sender, message) =>
        {
            var json = Encoding.UTF8.GetString(message.Body);
            Console.WriteLine($"Message received: {json}");
            await message.AckAsync();
        };

        await _consumer.StartAsync(context);
    }

}

------------------------------------------------------------------------

# Prioridad de mensajes

var properties = new BasicProperties 
	{ 
		Persistent = true,
		Priority = 5
	};

var context = new QueueContext
	{
		Exchange = "orders.exchange",
		Queue = "orders.queue",
		RoutingKey = "orders.created",
		BasicProperties = properties
	};

------------------------------------------------------------------------

# QoS (Control de consumo)

var context = new QueueContext
	{
		UseBasicQos = true,
		BasicQosPrefetchCount = 20
	};

Esto evita sobrecargar consumidores.

------------------------------------------------------------------------

# DLQ (Dead Letter Queue)

var context = new QueueContext
	{
		Exchange = "orders.exchange",
		Queue = "orders.queue",
		RoutingKey = "orders.created",
		Arguments = new Dictionary<string, object?>
		{
			{ "x-dead-letter-exchange", "orders.dlx" },
			{ "x-dead-letter-routing-key", "orders.dlq" }
		}
	};

Esto envia mensajes rechazados a la cola DLQ.

------------------------------------------------------------------------

# Retries avanzados

var retryPolicy = new RetryPolicy
	{
		MaxRetries = 5
	};

await _consumer.StartAsync(context, retryPolicy);

_consumer.MessageReceived += async (sender, message) =>
{
	// Reencola el mensaje e incrementa x-retry-count
	await message.NackAsync(requeue: true);
};

Puedes combinar con TTL agregando "x-message-ttl" en Arguments.

------------------------------------------------------------------------

# Manejo de errores

\_consumer.Error += (sender, error) =\> {
Console.WriteLine(error.Exception.Message); };

------------------------------------------------------------------------

# Buenas prácticas

-   Usar prefetchCount para evitar saturar consumidores
-   Manejar correctamente eventos de error
-   Usar BackgroundService para consumidores
-   Utilizar exchanges durables

------------------------------------------------------------------------

# Licencia

MIT License