using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using static BL.BL;
using BlApi;
using BO;

namespace BL
{

    class Simulator
    {
        const int DELAY = 1000;
        const double SPEED = 1;
        Drone drone;
        DroneToList dro;
        int check;
        Location dest;
        double progresLon, progresLat, progresBat;
        double dis, flyTime;
        DO.Station st;
        /// <summary>
        /// play the simulator 
        /// </summary>
        /// <param name="droneId">drone id</param>
        /// <param name="reportProgress">delgete that update the changes</param>
        /// <param name="stopDelgate">delgete to when the simulator is stoped</param>
        /// <param name="bl">IBL interface</param>
        public Simulator(int droneId, Action reportProgress, Func<bool> stopDelgate, BL bl)
        {
            lock (bl)
            {
                dro = bl.drones.Where(d => d.Id == droneId).FirstOrDefault();
                drone = bl.GetDrone(droneId);
            }
            while (!stopDelgate())
            {
                switch (drone.Status)
                {
                    case DroneStatus.Available:
                        lock (bl)
                        {
                            try
                            {
                                bl.DroneToParcel(droneId);
                                drone = bl.GetDrone(droneId);
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    if (dro.Status == DroneStatus.Available)
                                    {
                                        st = bl.ClosestStation(bl.dal.FilteredStations(s => s.ChargeSlots > 0), dro.CurrentLocation.Longitude, dro.CurrentLocation.Lattitude);
                                        dis = bl.dal.Distance(drone.CurrentLocation.Longitude, drone.CurrentLocation.Lattitude, st.Longitude, st.Lattitude);
                                        progresBat = bl.dal.ElectricityRequest().First() * dis;
                                        flyTime = dis / SPEED;
                                        if (flyTime != 0)
                                        {
                                            progresLon = (st.Longitude - drone.CurrentLocation.Longitude) / flyTime;
                                            progresLat = (st.Lattitude - drone.CurrentLocation.Lattitude) / flyTime;
                                            }
                                        bl.SendToCharge(droneId);
                                        if (flyTime != 0)
                                        {
                                            dro.Battery += progresBat;
                                            dro.CurrentLocation.Lattitude = drone.CurrentLocation.Lattitude;
                                            dro.CurrentLocation.Longitude = drone.CurrentLocation.Longitude;
                                        }
                                    }

                                    if (check <= dis && flyTime!=0)
                                    {
                                        dro.CurrentLocation.Longitude += progresLon;
                                        dro.CurrentLocation.Lattitude += progresLat;
                                        dro.Battery -= progresBat / flyTime;
                                        check += 1;
                                        break;
                                    }

                                    drone = bl.GetDrone(droneId);
                                }
                                catch (BatteryException)
                                {
                                }
                            }
                        }
                        break;
                    case DroneStatus.Maintenance:
                        lock (bl)
                        {
                            if (dro.Battery < 100)
                            {
                                dro.Battery += bl.dal.ElectricityRequest().ElementAt(4) / 60;
                                if (dro.Battery >= 100)
                                    bl.ReleaseDrone(droneId);
                            }
                            drone = bl.GetDrone(droneId);
                        }
                        break;
                    case DroneStatus.Delivery:

                        lock (bl)
                        {
                            if (bl.GetParcel(drone.TransferedParcel.Id).PickedUpTime == null)//haven't picked up yet
                            {
                                dest = drone.TransferedParcel.SourceLocation;
                                lock (bl)
                                {
                                    dis = bl.dal.Distance(drone.CurrentLocation.Longitude, drone.CurrentLocation.Lattitude, dest.Longitude, dest.Lattitude);
                                }
                                flyTime = dis / SPEED;
                                if (flyTime != 0)
                                {
                                    progresLon = (dest.Longitude - drone.CurrentLocation.Longitude) / flyTime;
                                    progresLat = (dest.Lattitude - drone.CurrentLocation.Lattitude) / flyTime;
                                    lock (bl)
                                    {
                                        progresBat = bl.dal.ElectricityRequest().First() * dis;
                                    }
                                    if (check <= dis)
                                    {
                                        dro.CurrentLocation.Longitude += progresLon;
                                        dro.CurrentLocation.Lattitude += progresLat;
                                        dro.Battery -= progresBat / flyTime;
                                        check += 1;
                                        break;
                                    }
                                }
                                lock (bl)
                                {
                                    bl.PickParcel(droneId);
                                }
                                if(flyTime!=0)
                                dro.Battery += progresBat;
                                check = 0;
                                lock (bl)
                                {
                                    drone = bl.GetDrone(droneId);
                                }
                                break;
                            }
                            else //already picked up
                            {
                                dest = drone.TransferedParcel.DestinationLocation;
                                dis = bl.dal.Distance(drone.CurrentLocation.Longitude, drone.CurrentLocation.Lattitude, dest.Longitude, dest.Lattitude);
                                flyTime = dis / SPEED;
                                if (flyTime != 0)
                                {
                                    progresLon = (dest.Longitude - drone.CurrentLocation.Longitude) / flyTime;
                                    progresLat = (dest.Lattitude - drone.CurrentLocation.Lattitude) / flyTime;
                                    lock (bl)
                                    {
                                        progresBat = bl.dal.ElectricityRequest().ElementAt((int)drone.TransferedParcel.Weight + 1) * dis;
                                    }
                                    if (check <= dis)
                                    {
                                        dro.CurrentLocation.Longitude += progresLon;
                                        dro.CurrentLocation.Lattitude += progresLat;
                                        dro.Battery -= progresBat / flyTime;
                                        check += 1;
                                        break;
                                    }
                                }
                                lock (bl)
                                {
                                    bl.DeliverParcel(droneId);
                                }
                                if(flyTime!=0)
                                dro.Battery += progresBat;
                                check = 0;
                                lock (bl)
                                {
                                    drone = bl.GetDrone(droneId);
                                }
                                break;
                            }
                        }


                }
                reportProgress();
                Thread.Sleep(DELAY);
            }
        }
    }
}
        

