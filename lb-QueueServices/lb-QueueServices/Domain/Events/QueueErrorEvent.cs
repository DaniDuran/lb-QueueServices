using System;

namespace lb_QueueServices.Domain.Events
{
    /// <summary>
    /// Representa un error emitido por un consumidor de colas.
    /// </summary>
    public class QueueErrorEvent
    {
        /// <summary>
        /// Excepcion que origino el error.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Crea un evento de error.
        /// </summary>
        /// <param name="exception">Excepcion a exponer.</param>
        public QueueErrorEvent(Exception exception)
        {
            Exception = exception;
        }
    }
}
