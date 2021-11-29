using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    namespace BO
    {
        public class ParcelInTransfer
        {
            /// <summary>
            /// properities
            /// </summary>
            public int Id { get; set; }
            public bool OnTheWay { get; set; } //true- on the way false- waiting 
            public Priorities Priority { get; set; }
            public WeightCategories Weight { get; set; }
            public ParcelModes ParcelMode { get; set; }
            public CustomerInParcel Sender { get; set; }
            public CustomerInParcel Target { get; set; }
            public Location SourceLocation { get; set; }
            public Location DestinationLocation { get; set; }
            public double Distance { get; set; }

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
