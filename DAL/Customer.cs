using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    namespace DO
    {
        public struct Customer
        {
            /// <summary>
            /// properties
            /// </summary>
            public int Id { get; set;}
            public string Name { get; set;}
            public string Phone { get; set;}
            public double Longitude { get; set;}
            public double Lattitude { get; set;}
            /// <summary>
            /// to string
            /// </summary>
            public override string ToString()
            {
                return "Customer: Id:" + Id + " Name:" + Name + " Phone:" + Phone + " Longitude:" + Longitude + " Lattitude:" + Lattitude;
            }

        }
    }
}
