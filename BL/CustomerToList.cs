using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    namespace BO
    {
        public class CustomerToList
        {
            /// <summary>
            /// properties
            /// </summary>
            public int Id { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public int Supplied { get; set; }  //amount parcels that the customer sent and supplied
            public int NotSupplied { get; set; } //amount parcels that the customer sent and not supplied
            public int Arrived { get; set; }  //amount parcels that the customer got and arrived
            public int NotArrived { get; set; } ////amount parcels that the customer got and not arrived
            /// <summary>
            /// to string
            /// </summary>
            public override string ToString()
            {
                return ToolStringClass.ToStringProperty(this);
            }
        }
    }
}
