using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    namespace BO
    {
        public struct Drone
        {
            /// <summary>
            /// properties
            /// </summary>
            public int Id { get; set; }
            public string Model { get; set; }
            public WeightCategories MaxWeight { get; set; }

            /// <summary>
            /// to string
            /// </summary>
            public override string ToString()
            {
                return "Drone: Id: " + Id + " Model: " + Model + " Max Weight: " + MaxWeight;
            }

        }
    }
}

