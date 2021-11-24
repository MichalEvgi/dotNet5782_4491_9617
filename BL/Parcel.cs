using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    namespace BO
    {
        public struct Parcel
        {
            /// <summary>
            /// properities
            /// </summary>
            public int Id { get; set; }
            public int SenderId { get; set; }
            public int TargetId { get; set; }
            public WeightCategories Weight { get; set; }
            public Priorities Priority { get; set; }
            public DateTime Requested { get; set; }
            public int DroneId { get; set; }
            public DateTime Scheduled { get; set; }
            public DateTime PickedUp { get; set; }
            public DateTime Delivered { get; set; }
            /// <summary>
            /// to string
            /// </summary>
            public override string ToString()
            {
                return "Parcel: Id:" + Id + " Sender Id:" + SenderId + " Target Id:" + TargetId + " Weight:" + Weight + " Priority:" + Priority + " Requested: " + Requested + " Drone Id: " + DroneId + " Scheduled: " + Scheduled + " Pick up: " + PickedUp + " Delivered: " + Delivered;
            }
        }
    }
}
