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
        [Serializable]
        public class NotFound : Exception
        {
            public NotFound() : base() { }
            public NotFound(string message) : base(message) { }
            public NotFound(string message, Exception inner) : base(message, inner) { }
            protected NotFound(SerializationInfo info, StreamingContext context) : base(info, context) { }
            override public string ToString()
            {
                return "This item wasn't found" + Message;
            }
        }
            [Serializable]
            public class AlreadyExists : Exception
            {
                public AlreadyExists() : base() { }
                public AlreadyExists(string message) : base(message) { }
                public AlreadyExists(string message, Exception inner) : base(message, inner) { }
                protected AlreadyExists(SerializationInfo info, StreamingContext context) : base(info, context) { }
                override public string ToString()
                {
                    return "This "+ Message+" already exists";
                }
            }
    }