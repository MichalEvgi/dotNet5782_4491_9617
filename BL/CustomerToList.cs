using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    namespace BO
    {
        public struct CustomerToList
        {
            /// <summary>
            /// properties
            /// </summary>
            public int Id { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public int Supplied { get; set; }
            public int NotSupplied { get; set; }
            public int Arrived { get; set; }
            public int NotArrived { get; set; }
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
