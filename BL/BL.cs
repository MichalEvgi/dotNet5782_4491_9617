using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using IDAL;
using IBL.BO;
using IBL;
namespace IBL
{
    public class BL : IBL
    {
        private IDal dal;
        private List<DroneToList> drones;
        private static Random rand = new Random();
        public BL()
        {
            dal = new DalObject.DalObject();
            drones = new List<DroneToList>();
            initializeDrones();
        }
        private void initializeDrones()
        {
            foreach (var drone in dal.PrintDrones())
            {
                drones.Add(new DroneToList
                {
                    Id = drone.Id,
                    Model = drone.Model,
                    MaxWeight = (WeightCategories)drone.MaxWeight
                });
            }
        }
        /// <summary>
        ///send to DAL for adding the station to the list of stations
        /// </summary>
        public void AddStation(Station s)
        {
            dal.AddStation(new IDAL.DO.Station
            {
                Id = s.Id,
                Name = s.Name,
                Longitude = s.LocationS.Longitude,
                Lattitude = s.LocationS.Lattitude,
                ChargeSlots = s.AvailableSlots
            });
        }
        /// <summary>
        /// send to DAL for adding the drone to list of drones
        /// </summary>
        public void AddDrone(Drone d, int stationId)
        {
            d.Battery = rand.Next(20, 41);
            d.Status = DroneStatus.Maintenance;
            IDAL.DO.Station s = dal.GetStationById(stationId);
            d.CurrentLocation = new Location { Longitude = s.Longitude, Lattitude = s.Lattitude };
            dal.AddDrone(new IDAL.DO.Drone
            {
                Id = d.Id,
                Model = d.Model,
                MaxWeight = (IDAL.DO.WeightCategories)d.MaxWeight
            });
            dal.SendToCharge(d.Id, stationId);
            drones.Add(new DroneToList
            {
                Id = d.Id,
                Model = d.Model,
                MaxWeight = d.MaxWeight,
                Battery = d.Battery,
                Status = d.Status,
                CurrentLocation = d.CurrentLocation,
                ParcelId = 0
            });
        }
        /// <summary>
        /// send to DAL for adding the customer to the list of customeer
        /// </summary>
        public void AddCustomer(Customer c)
        {
            dal.AddCustomer(new IDAL.DO.Customer
            {
                Id = c.Id,
                Name = c.Name,
                Phone = c.Phone,
                Longitude = c.LocationC.Longitude,
                Lattitude = c.LocationC.Lattitude
            });
        }
        /// <summary>
        /// send to DAL for adding the parcel to the list of parcels
        /// </summary>
        public void AddParcel(Parcel p)
        {
            dal.AddParcel(new IDAL.DO.Parcel
            {
                Id = p.Id,
                SenderId = p.Sender.Id,
                TargetId = p.Target.Id,
                Weight = (IDAL.DO.WeightCategories)p.Weight,
                Priority = (IDAL.DO.Priorities)p.Priority,
                Requested = p.RequestedTime,
                DroneId = 0,
                Scheduled = p.ScheduledTime,
                PickedUp = p.PickedUpTime,
                Delivered = p.DeliveredTime
            });
        }
        /// <summary>
        /// delete drone by id
        /// </summary>
        /// <param name="id"></param>
        public void DeleteDrone(int id)
        {
            dal.DeleteDrone(id);
            int i = drones.FindIndex(d => d.Id == id);
            drones.RemoveAt(i);
        }
        /// <summary>
        /// delete station by id
        /// </summary>
        /// <param name="id"></param>
        public void DeleteStation(int id)
        {
            dal.DeleteStation(id);
        }
        /// <summary>
        /// delete customer by id
        /// </summary>
        /// <param name="id"></param>
        public void DeleteCustomer(int id)
        {
            dal.DeleteCustomer(id);
        }
        /// <summary>
        /// delete parcel by id
        /// </summary>
        /// <param name="id"></param>
        public void DeleteParcel(int id)
        {
            dal.DeleteParcel(id);
        }
        /// <summary>
        /// update the drone model
        /// </summary>
        /// <param name="id">drone id</param>
        /// <param name="model">new drone model</param>
        public void UpdateDrone(int id, string model)
        {
            IDAL.DO.Drone temp = dal.GetDroneById(id);
            dal.DeleteDrone(id);
            temp.Model = model;
            dal.AddDrone(temp);
            int index = drones.FindIndex(d => d.Id == id);
            drones[index].Model = model;
        }
        
        /// <summary>
        /// update the station
        /// </summary>
        /// <param name="id">station id</param>
        /// <param name="name">new station name</param>
        /// <param name="chargeSlots">new station charge slots</param>
        public void UpdateStation(int id, int name, int chargeSlots)
        {
            IDAL.DO.Station temp = dal.GetStationById(id);
            dal.DeleteStation(id);
            if (name != -1)
                temp.Name = name;
            if (chargeSlots != -1)
                temp.ChargeSlots = chargeSlots - dal.FullSlots(id);
            dal.AddStation(temp);
        }
        /// <summary>
        /// update the customer
        /// </summary>
        /// <param name="id">customer id</param>
        /// <param name="name">new customer name</param>
        /// <param name="phone">new customer phone</param>
        public void UpdateCustomer(int id, string name, string phone)
        {
            IDAL.DO.Customer temp = dal.GetCustomerById(id);
            dal.DeleteCustomer(id);
            if (name != "")
                temp.Name = name;
            if (phone != "")
                temp.Phone = phone;
            dal.AddCustomer(temp);
        }
        /// <summary>
        /// send drone to charge
        /// </summary>
        /// <param name="id">drone</param>
        public void SendToCharge(int id)
        {
            int index = drones.FindIndex(x => x.Id == id);
            DroneToList drone = drones[index];
            //if (drone.Status != DroneStatus.Available)
            // throw Exception;
            IEnumerable<IDAL.DO.Station> avStation = dal.AvailableStations();
            IDAL.DO.Station closestStation = avStation.First();
            double lon1 = drone.CurrentLocation.Longitude;
            double lat1 = drone.CurrentLocation.Lattitude;
            foreach(IDAL.DO.Station s in avStation)
            {
                double lon2 = closestStation.Longitude;
                double lat2 = closestStation.Lattitude;
                double lon3 = s.Longitude;
                double lat3 = s.Lattitude;
                if(dal.Distance(lon1,lat1,lon2,lat2)>dal.Distance(lon1,lat1,lon3,lat3))
                    closestStation = s;
            }
            double smallestDistance = dal.Distance(lon1, lat1, closestStation.Longitude, closestStation.Lattitude);
            //if (drone.Battery < smallestDistance * dal.ElectricityRequest().First())
            //   throw;
            drones[index].Battery -= smallestDistance * dal.ElectricityRequest().First();
            drones[index].CurrentLocation = new Location { Longitude = closestStation.Longitude, Lattitude = closestStation.Lattitude };
            drones[index].Status = DroneStatus.Maintenance;
            dal.SendToCharge(id, closestStation.Id);
        }
        /// <summary>
        /// release drone from charging
        /// </summary>
        /// <param name="id">drone's id</param>
        /// <param name="timeInCharging">charging time</param>
        public void ReleaseDrone(int id, double timeInCharging)
        {
            int index = drones.FindIndex(x => x.Id == id);
            // if (drones[index].Status != DroneStatus.Maintenance)
            //   throw;
            drones[index].Battery += timeInCharging * dal.ElectricityRequest().ElementAt(4);
            if (drones[index].Battery > 100)
                drones[index].Battery = 100;
            drones[index].Status = DroneStatus.Available;
            dal.ReleaseDrone(id);
        }        
        /// <summary>
        /// assign drone to parcel
        /// </summary>
        /// <param name="id"> drone's id</param>
        public void DroneToParcel(int id)
        {
            int index = drones.FindIndex(x => x.Id == id);
            // if (drones[index].Status != DroneStatus.Available)
            //   throw;
            IEnumerable<IDAL.DO.Parcel> unParcels = dal.UnassociatedParcel();
            unParcels.OrderBy(p => p);
            
        }
        /// <summary>
        /// pick parcel by drone
        /// </summary>
        /// <param name="id">drone's id</param>
        public void PickParcel(int id);
        /// <summary>
        /// deliver parcel to customer by drone
        /// </summary>
        /// <param name="id">drone's id</param>
        public void DeliverParcel(int id);
        /// <summary>
        /// return the description of a specific station
        /// </summary>
        /// <param name="id">station's id</param>
        /// <returns></returns>
        public string GetStation(int id)
        {
            IEnumerable<int> droneIDs = dal.DroneInChargeIds(id);
            IEnumerable<DroneInCharging> droneInChargings = droneIDs.Select(d => new DroneInCharging { Id = d, Battery = drones.Find(x => x.Id == d).Battery });                                             
            IDAL.DO.Station s = dal.GetStationById(id);
            Station station = new Station
            {
                Id=s.Id,
                Name=s.Name,
                LocationS= new Location { Longitude=s.Longitude, Lattitude=s.Lattitude},
                AvailableSlots=s.ChargeSlots,
                DronesInCharging= droneInChargings
            };
            return station.ToString();
        }
        /// <summary>
        /// return the description of a specific drone
        /// </summary>
        /// <param name="id">drone's id</param>
        /// <returns></returns>
        public string GetDrone(int id)
        {
            IDAL.DO.Drone d = dal.GetDroneById(id);
            DroneToList droneToList = drones.Find(d => d.Id == id);
            IDAL.DO.Parcel p = dal.GetTransferedParcel(id);
            IDAL.DO.Customer dalSender = dal.GetCustomerById(p.SenderId);
            IDAL.DO.Customer dalTarget = dal.GetCustomerById(p.TargetId);
            CustomerInParcel sender = new CustomerInParcel { Id = p.SenderId, Name = dalSender.Name };
            CustomerInParcel target = new CustomerInParcel { Id = p.TargetId, Name = dalTarget.Name };
            int parcelmode = 1;
            if (p.PickedUp != DateTime.MinValue)
                parcelmode = 2;
            ParcelInTransfer parcelInTransfer = new ParcelInTransfer
            {
                Id = p.Id,
                OnTheWay = parcelmode == 2,
                Priority = (Priorities)p.Priority,
                Weight = (WeightCategories)p.Weight,
                ParcelMode = (ParcelModes)parcelmode,
                Sender = sender,
                Target = target,
                SourceLocation = new Location { Longitude = dalSender.Longitude, Lattitude = dalSender.Lattitude },
                DestinationLocation = new Location { Longitude = dalTarget.Longitude, Lattitude = dalTarget.Lattitude },
                Distance = dal.Distance(dalSender.Longitude, dalSender.Lattitude, dalTarget.Longitude, dalTarget.Lattitude)
            };
            ///delivery
            Drone drone = new Drone
            {
                Id = d.Id,
                Model = d.Model,
                MaxWeight = (WeightCategories)d.MaxWeight,
                Battery = droneToList.Battery,
                Status = droneToList.Status,
                TransferedParcel = parcelInTransfer,
                CurrentLocation =droneToList.CurrentLocation
            };
            return drone.ToString();
        }

        /// <summary>
        /// return the description of a specific customer
        /// </summary>
        /// <param name="id">customer's id</param>
        /// <returns></returns>
        public string GetCustomer(int id)
        {
            IEnumerable<ParcelInCustomer> parcelFrom = from IDAL.DO.Parcel p in dal.PrintParcels()
                                                where p.SenderId == id
                                                select convertToParcelInCustomer(p,p.TargetId);
            IEnumerable<ParcelInCustomer> parcelTo= from IDAL.DO.Parcel p in dal.PrintParcels()
                                                where p.TargetId == id
                                                select convertToParcelInCustomer(p, p.SenderId); 
            IDAL.DO.Customer c = dal.GetCustomerById(id);
            Customer customer = new Customer
            {
                Id = c.Id,
                Name = c.Name,
                Phone = c.Phone,
                LocationC = new Location { Longitude = c.Longitude, Lattitude = c.Lattitude },
                FromCustomer =parcelFrom,
                ToCustomer =parcelTo
            };
            return customer.ToString();
        }
        /// <summary>
        /// return the description of a specific parcel
        /// </summary>
        /// <param name="id">parcel's id</param>
        /// <returns></returns>
        public string GetParcel(int id)
        {
            IDAL.DO.Parcel p = dal.GetParcelById(id);
            IDAL.DO.Customer cSender = dal.GetCustomerById(p.SenderId);
            IDAL.DO.Customer cTarget = dal.GetCustomerById(p.TargetId);
            CustomerInParcel senderInParcel = new CustomerInParcel { Id = cSender.Id, Name = cSender.Name };
            CustomerInParcel targetInParcel = new CustomerInParcel { Id = cTarget.Id, Name = cTarget.Name };
            DroneToList d = drones.Find(drone => drone.Id == p.DroneId);
            DroneInParcel droneInParcel = new DroneInParcel { Id = d.Id, Battery = d.Battery, CurrentLocation = d.CurrentLocation };
            Parcel parcel = new Parcel
            {
                Id = p.Id,
                Sender = senderInParcel,
                Target = targetInParcel,
                Weight = (WeightCategories)p.Weight,
                Priority = (Priorities)p.Priority,
                DroneP = droneInParcel,
                RequestedTime = p.Requested,
                ScheduledTime = p.Scheduled,
                PickedUpTime = p.PickedUp,
                DeliveredTime = p.Delivered
            };
            return parcel.ToString();
        }
        /// <summary>
        /// return list of stations
        /// </summary>
        /// <returns></returns>
        public IEnumerable<StationToList> GetStationsList()
        {
            return dal.PrintStations().Select(s => convertTostationToList(s));
        }
        /// <summary>
        /// return list of drones
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DroneToList> GetDronesList()
        {
            return drones;
        }
        /// <summary>
        /// return list of customers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CustomerToList> GetCustomersList()
        {
            return dal.PrintCustomers().Select(c => convertToCustomerToList(c));
        }
        /// <summary>
        /// return list of parcels
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ParcelToList> GetParcelsList()
        {
            return dal.PrintParcels().Select(p => convertToParcelToList(p));
        }
        /// <summary>
        /// return the list of parcels that not associated yet with drone
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ParcelToList> UnassociatedParcel()
        {
            return from IDAL.DO.Parcel p in dal.PrintParcels()
                   where p.DroneId== 0
                   select convertToParcelToList(p);
        }
        /// <summary>
        /// return the list of the base stations with available charging slots
        /// </summary>
        /// <returns></returns>
        public IEnumerable<StationToList> AvailableStations()
        {
            return from IDAL.DO.Station s in dal.PrintStations()
                   where s.ChargeSlots > 0
                   select convertTostationToList(s);
        }
        private ParcelInCustomer convertToParcelInCustomer(IDAL.DO.Parcel p, int otherId)
        {
            int parcelMode = 0;
            if (p.Scheduled != DateTime.MinValue)
            {
                if (p.PickedUp == DateTime.MinValue)
                    parcelMode = 1;
                else
                {
                    if (p.Delivered == DateTime.MinValue)
                        parcelMode = 2;
                    else
                        parcelMode = 3;
                }
            }
            string otherName = dal.GetCustomerById(otherId).Name;
            ParcelInCustomer parcelInCustomer = new ParcelInCustomer
            {
                Id = p.Id,
                Weight = (WeightCategories)p.Weight,
                Priority = (Priorities)p.Priority,
                ParcelMode = (ParcelModes)parcelMode,
                OtherCustomer = new CustomerInParcel { Id = otherId, Name = otherName }
            };
            return parcelInCustomer;
        }
        private CustomerToList convertToCustomerToList(IDAL.DO.Customer c)
        {
            int supplied = 0, notSupplied = 0, arrived = 0, notArrived = 0;
            foreach (IDAL.DO.Parcel p in dal.PrintParcels())
            {
                if(p.SenderId==c.Id)
                {
                    if (p.Delivered == DateTime.MinValue)
                        notSupplied++;
                    else
                        supplied++;
                }
                if(p.TargetId==c.Id)
                {
                    if (p.Delivered == DateTime.MinValue)
                        notArrived++;
                    else
                        arrived++;
                }
            }
            CustomerToList customer = new CustomerToList
            {
                Id = c.Id,
                Name = c.Name,
                Phone = c.Phone,
                Supplied = supplied,
                NotSupplied = notSupplied,
                Arrived = arrived,
                NotArrived = notArrived
            };
            return customer;
        }
        private ParcelToList convertToParcelToList(IDAL.DO.Parcel p)
        {
            string senderName = dal.GetCustomerById(p.SenderId).Name;
            string targetName = dal.GetCustomerById(p.TargetId).Name;
            int parcelMode = 0;
            if (p.Scheduled != DateTime.MinValue)
            {
                if (p.PickedUp == DateTime.MinValue)
                    parcelMode = 1;
                else
                {
                    if (p.Delivered == DateTime.MinValue)
                        parcelMode = 2;
                    else
                        parcelMode = 3;
                }
            }
            ParcelToList parcel = new ParcelToList
            {
                Id = p.Id,
                SenderName = senderName,
                TargetName = targetName,
                Weight = (WeightCategories)p.Weight,
                Priority = (Priorities)p.Priority,
                ParcelMode = (ParcelModes)parcelMode
            };
            return parcel;
        }
        private StationToList convertTostationToList(IDAL.DO.Station s)
        {
            StationToList station = new StationToList
            {
                Id = s.Id,
                Name = s.Name,
                AvailableSlots = s.ChargeSlots,
                FullSlots = dal.FullSlots(s.Id)
            };
            return station;
        }
        /// <summary>
        /// create string of sexagesimal lattitude
        /// </summary>
        /// <param name="lat">lattitude</param>
        /// <returns></returns>
        public static string Lat(double lat)
        {
            string ch;
            if (lat < 0)
            {
                ch = "S";
                lat *= -1;
            }
            else
                ch = "N";
            int deg = (int)lat;
            double dif = lat - deg;
            int min = (int)(dif * 60);
            double sec = (dif * 3600 - min * 60);
            sec = Math.Round(sec, 4);
            return $"{deg}° {min}' {sec}'' {ch}";
        }
        /// <summary>
        /// dreate string of sexagesimal longitude
        /// </summary>
        /// <param name="lng">longitude</param>
        /// <returns></returns>
        public static string Lng(double lng)
        {
            string ch;
            if (lng < 0)
            {
                ch = "W";
                lng *= -1;
            }
            else
                ch = "E";
            int deg = (int)lng;
            double dif = lng - deg;
            int min = (int)(dif * 60);
            double sec = (dif) * 3600 - min * 60;
            sec = Math.Round(sec, 4);
            return $"{deg}° {min}' {sec}'' {ch}";
        }
    }
}
