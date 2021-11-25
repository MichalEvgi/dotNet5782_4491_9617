using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    namespace BO
    {
        public struct DroneInCharging
        {
            /// <summary>
            /// properties
            /// </summary>
            public int Id { get; set; }
            public double Battery { get; set; }

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
