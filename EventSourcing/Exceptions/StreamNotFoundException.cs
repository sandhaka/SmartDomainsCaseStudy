using System;

namespace EventSourcing.Exceptions
{
    public class StreamNotFoundException : Exception
    {
        public StreamNotFoundException(string streamName)
        {
            StreamName = streamName;
        }

        public string StreamName { get; }
    }
}