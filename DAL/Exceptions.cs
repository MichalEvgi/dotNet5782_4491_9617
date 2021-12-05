using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using IDAL.DO;

namespace IDAL
{
    namespace DO
    {
        /// <summary>
        /// if the value not found throw this exception
        /// </summary>
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
        /// <summary>
        /// if the value already exist throw this exception
        /// </summary>
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
    }
}