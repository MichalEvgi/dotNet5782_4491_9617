using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    namespace BO
    {
        public class Parcel
        {
            /// <summary>
            /// properities
            /// </summary>
            public int Id { get; set; }
            public CustomerInParcel Sender { get; set; }
            public CustomerInParcel Target { get; set; }
            public WeightCategories Weight { get; set; }
            public Priorities Priority { get; set; }
            public DroneInParcel DroneP { get; set; }
            public DateTime? RequestedTime { get; set; }
            public DateTime? ScheduledTime { get; set; }
            public DateTime? PickedUpTime { get; set; }
            public DateTime? DeliveredTime { get; set; }
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
