using System;

namespace EventSourcing.Exceptions
{
    public class StreamEmptyException : Exception
    {
        public StreamEmptyException(string streamName)
        {
            StreamName = streamName;
        }

        public string StreamName { get; }
    }
}