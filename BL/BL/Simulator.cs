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
        double intialBattery = 0;
        public Simulator(int droneId, Action reportProgress, Func<bool> stopDelgate, BL bl)
        {
            lock (bl)
            {
                dro = bl.drones.Where(d => d.Id == droneId).FirstOrDefault();
            }
            while (!stopDelgate())
            {
                lock (bl)
                {
                    drone = bl.GetDrone(droneId);
                }
                switch (drone.Status)
                {
                    case DroneStatus.Available:
                        lock (bl)
                        {
                            try
                            {
                                bl.DroneToParcel(droneId);
                            }
                            catch (Exception)
                            {
                                //try
                                //{
                                bl.SendToCharge(droneId);
                                reportProgress();
                                //}
                                //catch (BatteryException)
                                //{
                                //    Thread.Sleep(DELAY);
                                //}
                            }
                        }
                        break;
                    case DroneStatus.Maintenance:
                        lock (bl)
                        {
                            bl.ReleaseDrone(droneId);
                            if (drone.Battery < 100)
                                bl.SendToCharge(droneId);
                            reportProgress();
                        }
                        break;
                    case DroneStatus.Delivery:
                        double flyTime = drone.TransferedParcel.Distance / SPEED;
                        Location dest;
                        double progresLon;
                        double progresLat;
                        double progresBat;
                        
                        if (bl.GetParcel(drone.TransferedParcel.Id).PickedUpTime == null)//haven't picked up yet
                        {
                            dest = drone.TransferedParcel.SourceLocation;
                            progresLon = (dest.Longitude - drone.CurrentLocation.Longitude) / flyTime;
                            progresLat = (dest.Lattitude - drone.CurrentLocation.Lattitude) / flyTime;
                            lock (bl)
                            {
                                progresBat = bl.dal.ElectricityRequest().First() * drone.TransferedParcel.Distance;
                            }
                            if (check < drone.TransferedParcel.Distance)
                            {
                                dro.CurrentLocation.Longitude += progresLon;
                                dro.CurrentLocation.Lattitude += progresLat;
                                dro.Battery -= progresBat / flyTime;
                                intialBattery+= progresBat / flyTime;
                                check += 1;
                                break;
                            }
                            dro.Battery += intialBattery;
                            intialBattery = 0;
                            lock (bl)
                            {
                                bl.PickParcel(droneId);
                            }
                            check = 0;
                            break;
                        }
                        else //already picked up
                        {
                            dest = drone.TransferedParcel.DestinationLocation;
                            progresLon = (dest.Longitude - drone.CurrentLocation.Longitude) / flyTime;
                            progresLat = (dest.Lattitude - drone.CurrentLocation.Lattitude) / flyTime;
                            lock (bl)
                            {
                                progresBat = bl.dal.ElectricityRequest().ElementAt((int)drone.TransferedParcel.Weight + 1) * drone.TransferedParcel.Distance;
                            }
                            check += 1;
                            if (check < drone.TransferedParcel.Distance)
                            {
                                dro.CurrentLocation.Longitude += progresLon;
                                dro.CurrentLocation.Lattitude += progresLat;
                                dro.Battery -= progresBat / flyTime;
                                intialBattery+= progresBat / flyTime;
                                break;
                            }
                            dro.Battery += intialBattery;
                            intialBattery = 0;
                            lock (bl)
                            {
                                bl.DeliverParcel(droneId);
                            }
                            check = 0;
                            break;
                        }
                }
                reportProgress();
                Thread.Sleep(DELAY);
            }
        }
    }
}
        

