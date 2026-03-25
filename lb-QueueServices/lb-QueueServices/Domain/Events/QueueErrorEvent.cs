using System;

namespace lb_QueueServices.Domain.Events
{
    /// <summary>
    /// Represents an error raised by a queue consumer.
    /// </summary>
    public class QueueErrorEvent
    {
        /// <summary>
        /// The exception that triggered the error.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Creates a new error event wrapper.
        /// </summary>
        /// <param name="exception">The exception to expose.</param>
        public QueueErrorEvent(Exception exception)
        {
            Exception = exception;
        }
    }
}
