using RabbitMQ.Client;
using System.Collections.Generic;

namespace lb_QueueServices.Domain.Models
{
    /// <summary>
    /// Configuracion para conectar a RabbitMQ y definir la topologia de colas.
    /// </summary>
    public sealed class QueueContext
    {
        /// <summary>
        /// Nombre del host del broker.
        /// </summary>
        public string Host { get; init; } = default!;

        /// <summary>
        /// Puerto del broker (por defecto 5672).
        /// </summary>
        public int Port { get; init; } = 5672;

        /// <summary>
        /// Usuario de conexion.
        /// </summary>
        public string User { get; init; } = default!;

        /// <summary>
        /// Contrasena de conexion.
        /// </summary>
        public string Password { get; init; } = default!;

        /// <summary>
        /// Nombre del exchange.
        /// </summary>
        public string Exchange { get; init; } = default!;

        /// <summary>
        /// Tipo de exchange (por ejemplo ExchangeType.Topic o ExchangeType.Fanout).
        /// </summary>
        public string ExchangeType { get; set; } = default!;

        /// <summary>
        /// Nombre de cola opcional; si esta vacio se publica solo en exchange.
        /// </summary>
        public string? Queue { get; init; } = default!;

        /// <summary>
        /// Routing key para el binding y publicacion.
        /// </summary>
        public string RoutingKey { get; init; } = default!;

        /// <summary>
        /// Bandera de persistencia usada al construir propiedades basicas.
        /// </summary>
        public bool Persistent { get; init; } = true;

        /// <summary>
        /// Prioridad del mensaje usada al construir propiedades basicas.
        /// </summary>
        public byte Priority { get; init; } = default!;

        /// <summary>
        /// Habilita Basic QoS en el consumidor.
        /// </summary>
        public bool UseBasicQos { get; init; } = false;

        /// <summary>
        /// Prefetch size para Basic QoS.
        /// </summary>
        public uint BasicQosPrefetchSize { get; init; } = default!;

        /// <summary>
        /// Prefetch count para Basic QoS.
        /// </summary>
        public ushort BasicQosPrefetchCount { get; init; } = default!;

        /// <summary>
        /// Aplica QoS de forma global en el canal.
        /// </summary>
        public bool BasicQosGlobal { get; init; } = false;

        /// <summary>
        /// Argumentos de declaracion de cola (por ejemplo dead-letter).
        /// </summary>
        public Dictionary<string, object?> Arguments { get; init; } = new();

        /// <summary>
        /// Propiedades basicas opcionales para la publicacion.
        /// </summary>
        public BasicProperties? BasicProperties { get; init; }
    }
}
