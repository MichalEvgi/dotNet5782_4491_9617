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
        public class BatteryException : Exception
        {
            public BatteryException() : base() { }
            public BatteryException(string message) : base(message) { }
            public BatteryException(string message, Exception inner) : base(message, inner) { }
            protected BatteryException(SerializationInfo info, StreamingContext context) : base(info, context) { }
            override public string ToString()
            {
                return  Message;
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
        [Serializable]
        public class DroneStatusException : Exception
        {
            public DroneStatusException() : base() { }
            public DroneStatusException(string message) : base(message) { }
            public DroneStatusException(string message, Exception inner) : base(message, inner) { }
            protected DroneStatusException(SerializationInfo info, StreamingContext context) : base(info, context) { }
            override public string ToString()
            {
                return  Message;
            }
        }
        [Serializable]
        public class EmptyListException : Exception
        {
            public EmptyListException() : base() { }
            public EmptyListException(string message) : base(message) { }
            public EmptyListException(string message, Exception inner) : base(message, inner) { }
            protected EmptyListException(SerializationInfo info, StreamingContext context) : base(info, context) { }
            override public string ToString()
            {
                return Message;
            }
        }
        [Serializable]
        public class ParcelModeException : Exception
        {
            public ParcelModeException() : base() { }
            public ParcelModeException(string message) : base(message) { }
            public ParcelModeException(string message, Exception inner) : base(message, inner) { }
            protected ParcelModeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
            override public string ToString()
            {
                return Message;
            }
        }
    }
}
