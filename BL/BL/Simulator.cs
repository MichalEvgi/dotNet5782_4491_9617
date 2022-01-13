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
        const int DELAY= 1200;
        const double SPEED= 1;
        Drone drone;
        public Simulator(int droneId, Action reportProgress, Func<bool> stopDelgate, BL bl)
        {
           while(!stopDelgate())
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
                                reportProgress();
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
                                    reportProgress();
                                    Thread.Sleep(DELAY);
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
                            reportProgress();
                        }
                        Thread.Sleep(DELAY);
                        break;
                    case DroneStatus.Delivery:
                        double flyTime = drone.TransferedParcel.Distance * 1000.0 / SPEED;
                        Thread.Sleep((int)flyTime);
                        if (bl.GetParcel(drone.TransferedParcel.Id).PickedUpTime==null)//haven't picked up yet
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
                        reportProgress();
                        Thread.Sleep(DELAY);
                        break;
                }
            }
        }
    }
}
