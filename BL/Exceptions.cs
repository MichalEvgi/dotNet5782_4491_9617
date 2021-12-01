using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace IBL
{
    namespace BO
    {
        [Serializable]
        public class NotFoundException : Exception
        {
            public NotFoundException() : base() { }
            public NotFoundException(string message) : base(message) { }
            public NotFoundException(string message, Exception inner) : base(message, inner) { }
            protected NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
            override public string ToString()
            {
                return "This " + Message +" wasn't found";
            }
        }
        [Serializable]
        public class AlreadyExistsException : Exception
        {
            public AlreadyExistsException() : base() { }
            public AlreadyExistsException(string message) : base(message) { }
            public AlreadyExistsException(string message, Exception inner) : base(message, inner) { }
            protected AlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
            override public string ToString()
            {
                return "This " + Message + " already exists";
            }
        }
        [Serializable]
        public class FullBatteryException : Exception
        {
            public FullBatteryException() : base() { }
            public FullBatteryException(string message) : base(message) { }
            public FullBatteryException(string message, Exception inner) : base(message, inner) { }
            protected FullBatteryException(SerializationInfo info, StreamingContext context) : base(info, context) { }
            override public string ToString()
            {
                return "The drone's battery consumption is over 100% " + Message;
            }
        }
        [Serializable]
        public class InvalidInputException : Exception
        {
            public InvalidInputException() : base() { }
            public InvalidInputException(string message) : base(message) { }
            public InvalidInputException(string message, Exception inner) : base(message, inner) { }
            protected InvalidInputException(SerializationInfo info, StreamingContext context) : base(info, context) { }
            override public string ToString()
            {
                return "Invalid input: " + Message;
            }
        }
    }
}
