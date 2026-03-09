using System;
using System.Collections.Generic;
using System.Text;


namespace lb_QueueServices.Domain.Events
{
    public class QueueErrorEvent
    {
        public Exception Exception { get; }

        public QueueErrorEvent(Exception exception)
        {
            Exception = exception;
        }
    }
}
