using System;

namespace EventSourcing.Exceptions
{
    public class DomainEventsToAddLimitException : Exception
    {
        public DomainEventsToAddLimitException(string message = null) : base(message)
        {

        }
    }
}