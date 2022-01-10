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
        const int DELAY= 750;
        const double SPEED= 0.75;
        Drone drone;
        public Simulator(int droneId, Action updateDelgate, Func<bool> stopDelgate, BL bl)
        {
            lock (bl)
            {
                drone = bl.GetDrone(droneId);
            }
           while(!stopDelgate())
            {
                switch(drone.Status)
                {
                    case DroneStatus.Available:
                        lock (bl)
                        {
                            try
                            {
                                bl.DroneToParcel(droneId);
                                Thread.Sleep(DELAY);
                            }
                            catch (EmptyListException)
                            {
                                Thread.Sleep(DELAY);
                            }
                            catch (NotFoundException)
                            {
                                try
                                {
                                    bl.SendToCharge(droneId);
                                }
                                catch (BatteryException)
                                {
                                    Thread.Sleep(DELAY);
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
                        }
                        Thread.Sleep(DELAY);
                        break;
                    case DroneStatus.Delivery:
                        double flyTime = drone.TransferedParcel.Distance * 1000.0 / SPEED;
                        Thread.Sleep((int)flyTime);
                        if (!drone.TransferedParcel.OnTheWay)//haven't picked up yet
                        {
                            lock (bl)
                            {
                                bl.PickParcel(droneId);
                            }
                        }
                        else //already picked up
                        {
                            lock (bl)
                            {
                                bl.DeliverParcel(droneId);
                            }
                        }
                        Thread.Sleep(DELAY);
                        break;
                }
            }
        }
    }
}
