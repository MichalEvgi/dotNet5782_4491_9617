using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IBL
{
    namespace BO
    {
        public struct Station
        {
            /// <summary>
            /// properties
            /// </summary>
            public int Id { get; set; }
            public int Name { get; set; }
            public Location LocationS { get; set; }
            public int AvailableSlots { get; set; }
            public List<DroneInCharging> DronesInCharging { get; set; }
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
