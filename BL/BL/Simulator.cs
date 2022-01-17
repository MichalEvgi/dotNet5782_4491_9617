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
                                    bl.SendToCharge(droneId);
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
                            bl.ReleaseDrone(droneId);
                            if (drone.Battery < 100)
                                bl.SendToCharge(droneId);
                            drone = bl.GetDrone(droneId);
                        }
                        break;
                    case DroneStatus.Delivery:
                        if (bl.GetParcel(drone.TransferedParcel.Id).PickedUpTime == null)//haven't picked up yet
                        {
                            dest = drone.TransferedParcel.SourceLocation;
                            dis = bl.dal.Distance(drone.CurrentLocation.Longitude, drone.CurrentLocation.Lattitude,dest.Longitude, dest.Lattitude);
                            flyTime = dis / SPEED;
                            progresLon = (dest.Longitude - drone.CurrentLocation.Longitude) / flyTime;
                            progresLat = (dest.Lattitude - drone.CurrentLocation.Lattitude) / flyTime;
                            lock (bl)
                            {
                                progresBat = bl.dal.ElectricityRequest().First() * dis;
                            }
                            if (check < dis)
                            {
                                dro.CurrentLocation.Longitude += progresLon;
                                dro.CurrentLocation.Lattitude += progresLat;
                                dro.Battery -= progresBat / flyTime;
                                check += 1;
                                break;
                            }
                            lock (bl)
                            {
                                bl.PickParcel(droneId);
                            }
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
                            progresLon = (dest.Longitude - drone.CurrentLocation.Longitude) / flyTime;
                            progresLat = (dest.Lattitude - drone.CurrentLocation.Lattitude) / flyTime;
                            lock (bl)
                            {
                                progresBat = bl.dal.ElectricityRequest().ElementAt((int)drone.TransferedParcel.Weight + 1) * dis;
                            }
                            if (check < dis)
                            {
                                dro.CurrentLocation.Longitude += progresLon;
                                dro.CurrentLocation.Lattitude += progresLat;
                                dro.Battery -= progresBat / flyTime;
                                check += 1;
                                break;
                            }
                            lock (bl)
                            {
                                bl.DeliverParcel(droneId);
                            }
                            dro.Battery += progresBat;
                            check = 0;
                            lock (bl)
                            {
                                drone = bl.GetDrone(droneId);
                            }
                            break;
                        }
                }
                reportProgress();
                Thread.Sleep(DELAY);
            }
        }
    }
}
        

