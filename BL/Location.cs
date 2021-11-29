using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    namespace BO
    {
        public class Location
        {
            public double Longitude { get; set; }
            public double Lattitude { get; set; }
            /// <summary>
            /// to string
            /// </summary>
            public override string ToString()
            {
                return "Longitude:" + BL.Lng(Longitude) + " Lattitude:" + BL.Lat(Lattitude);
            }
        }
    }
}
