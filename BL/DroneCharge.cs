using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BO
{
    public class DroneCharge
    {
        /// <summary>
        /// properties
        /// </summary>
        public int DroneId { get; set; }
        public int StationId { get; set; }
        /// <summary>
        /// to string
        /// </summary>
        public override string ToString()
        {
            return "Drone Charge: Drone Id: " + DroneId + " Station Id: " + StationId;
        }

    }
}