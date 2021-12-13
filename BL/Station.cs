using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BO
{
    public class Station
    {
        /// <summary>
        /// properties
        /// </summary>
        public int Id { get; set; }
        public int Name { get; set; }
        public Location LocationS { get; set; }
        public int AvailableSlots { get; set; }
        public IEnumerable<DroneInCharging> DronesInCharging { get; set; }  //list of all the drones that charge in this station
        /// <summary>
        /// to string
        /// </summary>
        public override string ToString()
        {
            string dronelist = "";
            foreach (DroneInCharging d in DronesInCharging)
            {
                dronelist += d.ToString();   // all the toString of the list DronesInCharging
            }
            return "Id:" + Id + "\nName:" + Name + "\nLocation:" + LocationS + "\nAvailable slots:" + AvailableSlots + "\nDrones in charging:" + dronelist;
        }

    }
}