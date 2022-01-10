using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.CompilerServices;
using BlApi;
using DalApi;
using BO;

namespace BL
{
    class BL : IBL
    {
        #region INITIALIZE
        internal IDal dal;
        private List<DroneToList> drones;
        private static Random rand = new Random();

        #region Singelton
        private static BL instance = null;
        private static readonly object padlock = new object();

        public static BL Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new BL();
                        }
                    }
                }
                return instance;
            }
        }
        #endregion
        public BL()
        {
            // create DAL objects of all entites
            try
            {
                dal = DalFactory.GetDal();
                // init drones BL
                drones = new List<DroneToList>();
                initializeDrones();
            }
            catch (DalApi.DalConfigException ex)
            {
                throw new BO.DalConfigException(ex.Message);
            }
        }
        private void initializeDrones()
        {
            // init battery variables
            double available = dal.ElectricityRequest().ElementAt(0);
            double lightWeight = dal.ElectricityRequest().ElementAt(1);
            double mediumWeight = dal.ElectricityRequest().ElementAt(2);
            double heavyWeight = dal.ElectricityRequest().ElementAt(3);
            double chargingRate = dal.ElectricityRequest().ElementAt(4);

            // loop on droneS in order to insert to BL
            foreach (var drone in dal.PrintDrones())
            {
                //set the details of droneToList according to the drone in DAL
                DroneToList droneToList = new DroneToList
                {
                    Id = drone.Id,
                    Model = drone.Model,
                    MaxWeight = (WeightCategories)drone.MaxWeight
                };

                bool flag = false;// flag if the drone is in delivery
                //search in parcels from DAL to find if the drone is in delivery
                foreach (var parcel in dal.PrintParcels())
                {
                    //if the drone is in delivery with this parcel
                    if ((parcel.DroneId == drone.Id) && (parcel.Delivered == null))
                    {
                        flag = true;
                        //set the details of droneToList
                        droneToList.Status = DroneStatus.Delivery;
                        droneToList.ParcelId = parcel.Id;
                        //calulate the minimum battery to deliver the parcel and go to station for charging
                        DO.Customer senderCustomer = dal.GetCustomerById(parcel.SenderId);
                        double senderLon = senderCustomer.Longitude;
                        double senderLat = senderCustomer.Lattitude;
                        //if the parcel is not picked up
                        if (parcel.PickedUp == null)
                        {
                            DO.Station closestStation = ClosestStation(dal.PrintStations(), senderLon, senderLat);
                            //set the location
                            droneToList.CurrentLocation = new Location { Longitude = closestStation.Longitude, Lattitude = closestStation.Lattitude };
                        }
                        else
                        {
                            //if the parcel was picked up
                            //set the location
                            droneToList.CurrentLocation = new Location { Longitude = senderLon, Lattitude = senderLat };
                        }
                        //get the minimum battery
                        double minBattery = MinBattery(parcel, droneToList.CurrentLocation.Longitude, droneToList.CurrentLocation.Lattitude);
                        //if the min battery is over 100%
                        if (minBattery > 100)
                            throw new BatteryException("The battery is over than 100%, the consumption for drone:" + drone.Id + " is " + minBattery + "%");
                        //set the battery to a random battery between minBattery to 100%
                        droneToList.Battery = minBattery + rand.NextDouble() * (100 - minBattery);
                        break;
                    }
                }
                //if the drione is not associated to parcel
                if (!flag)
                {
                    droneToList.Status = (DroneStatus)rand.Next(0, 2);
                    //if the dronein maintenance
                    if (droneToList.Status == DroneStatus.Maintenance)
                    {
                        //set a random location (from the available stations locations)
                        DO.Station randStation = dal.FilteredStations(s => s.ChargeSlots > 0).ElementAt(rand.Next(0, dal.FilteredStations(s => s.ChargeSlots > 0).Count()));
                        droneToList.CurrentLocation = new Location { Longitude = randStation.Longitude, Lattitude = randStation.Lattitude };
                        dal.SendToCharge(droneToList.Id, randStation.Id);
                        //set a random battery between 0-20
                        droneToList.Battery = rand.NextDouble() * 20;
                    }
                    else
                    {
                        //if the drone is available
                        //set a random location from the delivered parcels target locations
                        try
                        {
                            DO.Parcel randParcel = dal.FilteredParcel(p => p.Delivered != null).ElementAt(rand.Next(0, dal.FilteredParcel(p => p.Delivered != null).Count()));
                            DO.Customer targetRandParcel = dal.GetCustomerById(randParcel.TargetId);
                            droneToList.CurrentLocation = new Location { Longitude = targetRandParcel.Longitude, Lattitude = targetRandParcel.Lattitude };
                            //find the closest station with available charging slots
                            DO.Station closeToDrone = ClosestStation(dal.FilteredStations(s => s.ChargeSlots > 0), droneToList.CurrentLocation.Longitude, droneToList.CurrentLocation.Lattitude);
                            //calculate the minimum battery to go to the closest station for charging
                            double minBattery = dal.Distance(droneToList.CurrentLocation.Longitude, droneToList.CurrentLocation.Lattitude, closeToDrone.Longitude, closeToDrone.Lattitude) * available;
                            //if the min battery is over 100%
                            if (minBattery > 100)
                                throw new BatteryException("The battery is over than 100%, the consumption for drone:" + drone.Id + " is " + minBattery + "%");
                            //set a random battery between min battery to 100%
                            droneToList.Battery = rand.NextDouble() * (100 - minBattery) + minBattery;
                        }
                        catch (Exception)
                        {
                            droneToList.CurrentLocation = new Location { Longitude = 35.05, Lattitude = 31.05 };
                            droneToList.Battery = rand.NextDouble() * (50) + 50;
                        }
                    }

                }
                //add the droneToList to the list in BL
                drones.Add(droneToList);
            }
        }
        #endregion
        #region STATION
        /// <summary>
        ///send to DAL for adding the station to the list of stations
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddStation(Station s)
        {
            // validation station fields on add             
            if (s.Id < 1)
                throw new InvalidInputException("id should be bigger than 0");
            if (s.AvailableSlots < 0)
                throw new InvalidInputException("available chargeslots cannot be negative");
            if (s.Name < 0)
                throw new InvalidInputException("name cannot be negative");
            if (s.LocationS.Longitude > 35.2 || s.LocationS.Longitude < 35 || s.LocationS.Lattitude > 31.2 || s.LocationS.Lattitude < 31)
                throw new InvalidInputException("The location is out of range of shipments the longitude should be 35-35.2 and the lattitude should be 31-31.2");
            try
            {
                lock (dal)
                {
                    // add new station to DAL
                    dal.AddStation(new DO.Station
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Longitude = s.LocationS.Longitude,
                        Lattitude = s.LocationS.Lattitude,
                        ChargeSlots = s.AvailableSlots
                    });

                }
            }
            catch (DO.AlreadyExistsException ex)
            {
                throw new AlreadyExistsException(ex.Message);
            }

        }
        /// <summary>
        /// delete station by id
        /// </summary>
        /// <param name="id"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteStation(int id)
        {
            try
            {
                // delele station from DAL
                dal.DeleteStation(id);
            }
            catch (DO.NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
        }
        /// <summary>
        /// update the station
        /// </summary>
        /// <param name="id">station id</param>
        /// <param name="name">new station name</param>
        /// <param name="chargeSlots">new station charge slots</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateStation(int id, int name, int chargeSlots)
        {
            // validation station fields on update 
            if (chargeSlots < -1)
                throw new InvalidInputException("The number of chargeslots cannot be negative ");
            if (name < -1)
                throw new InvalidInputException("name cannot be negative");
            lock (dal)
            {
                if (!dal.ExistStation(id))
                    throw new NotFoundException("station");
                int fullslots = dal.FullSlots(id);
                if (chargeSlots - fullslots < 0 && chargeSlots != -1)
                    throw new InvalidInputException("The number of chargeslots cannot be " + chargeSlots + ", there are " + fullslots + " fullslots");


                // get the object of station by ID
                DO.Station temp = dal.GetStationById(id);

                // delete the OLD station before update

                // delete from DAL
                dal.DeleteStation(id);

                // update temp station object from input
                if (name != -1)
                    temp.Name = name;
                if (chargeSlots != -1)
                    temp.ChargeSlots = chargeSlots - fullslots;

                // add to DAL
                dal.AddStation(temp);
            }
        }
        /// <summary>
        /// return the description of a specific station
        /// </summary>
        /// <param name="id">station's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Station GetStation(int id)
        {
            lock (dal)
            {
                // validation check that this station ID exists
                if (!dal.ExistStation(id))
                    throw new NotFoundException("station");
                // get from DAL
                DO.Station s = dal.GetStationById(id);
                // get drone ids that  chargin in this stations
                IEnumerable<int> droneIDs = dal.DroneInChargeIds(id);
                IEnumerable<DroneInCharging> droneInChargings = droneIDs.Select(d => new DroneInCharging { Id = d, Battery = drones.Find(x => x.Id == d).Battery });
                Station station = new Station
                {
                    Id = s.Id,
                    Name = s.Name,
                    LocationS = new Location { Longitude = s.Longitude, Lattitude = s.Lattitude },
                    AvailableSlots = s.ChargeSlots,
                    DronesInCharging = droneInChargings
                };
                return station;
            }
        }
        /// <summary>
        /// return list of stations
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<StationToList> GetStationsList()
        {
            lock (dal)
            {
                // list of stations
                return dal.PrintStations().Select(s => convertTostationToList(s));
            }
        }
        /// <summary>
        /// return the list of the base stations with available charging slots
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<StationToList> AvailableStations()
        {
            lock (dal)
            {
                //get the avalible stations that have at least 1 place to charge
                return dal.FilteredStations(s => s.ChargeSlots > 0).Select(s => convertTostationToList(s));
            }
        }
        /// <summary>
        /// convert station from DAL to stationtolist from BL
        /// </summary>
        /// <param name="s">do station</param>
        /// <returns></returns>
        private StationToList convertTostationToList(DO.Station s)
        {
            // convert station to stationtolist
            StationToList station = new StationToList
            {
                Id = s.Id,
                Name = s.Name,
                AvailableSlots = s.ChargeSlots,
                FullSlots = dal.FullSlots(s.Id) //calculate how many full slots in s
            };
            return station;
        }
        /// <summary>
        /// return the closest station from "stations" to a specific location
        /// </summary>
        /// <param name="stations">collection of stations</param>
        /// <param name="lon">longitude</param>
        /// <param name="lat">lattitude</param>
        /// <returns></returns>
        internal DO.Station ClosestStation(IEnumerable<DO.Station> stations, double lon, double lat)
        {
            // validation to check count of stations
            if (stations.Count() == 0)
                throw new EmptyListException("there is no available station");
            // loop on stations list to find the closet station 
            // accroding the long and lat
            DO.Station closestStation = stations.First();
            foreach (DO.Station s in stations)
            {
                double lon2 = closestStation.Longitude;
                double lat2 = closestStation.Lattitude;
                double lon3 = s.Longitude;
                double lat3 = s.Lattitude;
                // if the current distance is closest update closet station 
                if (dal.Distance(lon, lat, lon2, lat2) > dal.Distance(lon, lat, lon3, lat3))
                    closestStation = s;
            }
            return closestStation;
        }
        #endregion
        #region DRONE
        /// <summary>
        /// play the simulator
        /// </summary>
        /// <param name="droneId"> drone id</param>
        /// <param name="updateDelgate"> delgate for updating the show</param>
        /// <param name="stopDelgate"> delgate for stoping the simulator</param>
        public void playSimulator(int droneId, Action updateDelgate, Func<bool> stopDelgate)
        {
            new Simulator(droneId, updateDelgate, stopDelgate, this);
        }
        /// <summary>
        /// send to DAL for adding the drone to list of drones
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddDrone(Drone d, int stationId)
        {
            // check validation 
            if (d.Id < 1)
                throw new InvalidInputException("drone id should be bigger than 0");
            lock (dal)
            {
                if (!dal.ExistStation(stationId))
                    throw new NotFoundException("station");
                // battery set to random number
                d.Battery = rand.Next(20, 41);
                d.Status = DroneStatus.Maintenance;
                DO.Station s = dal.GetStationById(stationId);
                d.CurrentLocation = new Location { Longitude = s.Longitude, Lattitude = s.Lattitude };
                try
                {
                    // add drone
                    dal.AddDrone(new DO.Drone
                    {
                        Id = d.Id,
                        Model = d.Model,
                        MaxWeight = (DO.WeightCategories)d.MaxWeight
                    });
                    // add drone to station and put in charge
                    dal.SendToCharge(d.Id, stationId);
                    // add drone to drone list in BL
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
                catch (DO.AlreadyExistsException ex)
                {
                    throw new AlreadyExistsException(ex.Message);
                }
            }
        }
        /// <summary>
        /// delete drone by id
        /// </summary>
        /// <param name="id"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteDrone(int id)
        {
            try
            {
                lock (dal)
                {
                    // delete drone
                    dal.DeleteDrone(id);
                    // remove drone from BL
                    int i = drones.FindIndex(d => d.Id == id);
                    drones.RemoveAt(i);
                }
            }
            catch (DO.NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
        }
        /// <summary>
        /// update the drone model
        /// </summary>
        /// <param name="id">drone id</param>
        /// <param name="model">new drone model</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateDrone(int id, string model)
        {
            lock (dal)
            {
                // validation drone update
                if (!dal.ExistDrone(id))
                    throw new NotFoundException("drone");
                // get drone from DAL by ID
                DO.Drone temp = dal.GetDroneById(id);
                // first delete the drone and then update
                dal.DeleteDrone(id);
                // update the model
                temp.Model = model;
                // add to DAL
                dal.AddDrone(temp);
            }
            // update model in BL
            int index = drones.FindIndex(d => d.Id == id);
            drones[index].Model = model;
        }
        /// <summary>
        /// send drone to charge
        /// </summary>
        /// <param name="id">drone</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SendToCharge(int id)
        {
            // validate drone exists
            int index = drones.FindIndex(x => x.Id == id);
            if (index == -1)
                throw new NotFoundException("drone:" + id);
            // validate drone status
            DroneToList drone = drones[index];
            if (drone.Status != DroneStatus.Available)
                throw new DroneStatusException("The drone must be available for sending to charge");

            lock (dal)
            {
                // send drone to closet station that availble to charge
                IEnumerable<DO.Station> avStation = dal.FilteredStations(s => s.ChargeSlots > 0);
                double lon1 = drone.CurrentLocation.Longitude;
                double lat1 = drone.CurrentLocation.Lattitude;
                DO.Station closestStation = ClosestStation(avStation, lon1, lat1);
                double smallestDistance = dal.Distance(lon1, lat1, closestStation.Longitude, closestStation.Lattitude);
                // check if drone can enough battery to reach to the closet station 
                if (drone.Battery < smallestDistance * dal.ElectricityRequest().First())
                    throw new BatteryException("there is no enough battery for sending to charge");
                // update DAL
                dal.SendToCharge(id, closestStation.Id);

                // update battery/ station/ status in BL
                drones[index].Battery -= smallestDistance * dal.ElectricityRequest().First();
                drones[index].CurrentLocation = new Location { Longitude = closestStation.Longitude, Lattitude = closestStation.Lattitude };
                drones[index].Status = DroneStatus.Maintenance;
            }

        }
        /// <summary>
        /// release drone from charging
        /// </summary>
        /// <param name="id">drone's id</param>
        /// <param name="timeInCharging">charging time</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ReleaseDrone(int id)
        {
            // validation drone exists
            int index = drones.FindIndex(x => x.Id == id);
            if (index == -1)
                throw new NotFoundException("drone:" + id);
            if (drones[index].Status != DroneStatus.Maintenance)
                throw new DroneStatusException("The drone must be in maintence status to be released");

            lock (dal)
            {
                // update drone release in DAL 
                double timeInCharging = dal.ReleaseDrone(id);

                // update BL
                // update battery by charge rate 4th element
                drones[index].Battery += timeInCharging / 60 * dal.ElectricityRequest().ElementAt(4);
                if (drones[index].Battery > 100)
                    drones[index].Battery = 100;
                drones[index].Status = DroneStatus.Available;
            }

        }
        /// <summary>
        /// return a specific drone
        /// </summary>
        /// <param name="id">drone's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Drone GetDrone(int id)
        {
            lock (dal)
            {
                // validation
                if (!dal.ExistDrone(id))
                    throw new NotFoundException("drone");
                // get drone by ID
                DO.Drone d = dal.GetDroneById(id);
                // get drone from BL
                DroneToList droneToList = drones.Find(d => d.Id == id);
                DO.Parcel p;
                try
                {
                    // find the parcel 
                    p = dal.GetTransferedParcel(id);
                }
                catch (DO.NotFoundException)
                {
                    // if the drone without parcel
                    Drone drone1 = new Drone
                    {
                        Id = d.Id,
                        Model = d.Model,
                        MaxWeight = (WeightCategories)d.MaxWeight,
                        Battery = droneToList.Battery,
                        Status = droneToList.Status,
                        TransferedParcel = null,
                        CurrentLocation = droneToList.CurrentLocation
                    };
                    return drone1;
                }

                // the drone have parcel
                // take sender and target customer details 
                DO.Customer dalSender = dal.GetCustomerById(p.SenderId);
                DO.Customer dalTarget = dal.GetCustomerById(p.TargetId);
                CustomerInParcel sender = new CustomerInParcel { Id = p.SenderId, Name = dalSender.Name };
                CustomerInParcel target = new CustomerInParcel { Id = p.TargetId, Name = dalTarget.Name };

                // default mode is associated
                int parcelmode = 1;
                double distanc = dal.Distance(droneToList.CurrentLocation.Longitude, droneToList.CurrentLocation.Lattitude, dalSender.Longitude, dalSender.Lattitude);

                // update mode to Collected
                if (p.PickedUp != null)
                {
                    parcelmode = 2;
                    distanc = dal.Distance(dalSender.Longitude, dalSender.Lattitude, dalTarget.Longitude, dalTarget.Lattitude);
                }

                // get all parcel details in order to add it to drone
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
                    Distance = distanc
                };


                // the final drone object
                Drone drone = new Drone
                {
                    Id = d.Id,
                    Model = d.Model,
                    MaxWeight = (WeightCategories)d.MaxWeight,
                    Battery = droneToList.Battery,
                    Status = droneToList.Status,
                    TransferedParcel = parcelInTransfer,
                    CurrentLocation = droneToList.CurrentLocation
                };
                return drone;
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DroneToList GetDroneTo(int id)
        {
            foreach (DroneToList d in drones)
            {
                if (d.Id == id)
                    return d;
            }
            return new DroneToList();
        }
        /// <summary>
        /// return list of drones
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DroneToList> GetDronesList()
        {
            return drones;
        }
        #endregion
        #region CUSTOMER
        /// <summary>
        /// send to DAL for adding the customer to the list of customeer
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddCustomer(Customer c)
        {
            // validate ID number
            if (c.Id < 100000000 || c.Id > 444444444)
                throw new InvalidInputException("id should be 9 digits");
            if (c.Phone.Length != 10)
                throw new InvalidInputException("phone number should be 10 digits");
            if (c.LocationC.Longitude > 35.2 || c.LocationC.Longitude < 35 || c.LocationC.Lattitude > 31.2 || c.LocationC.Lattitude < 31)
                throw new InvalidInputException("The location is out of range of shipments the longitude should be 35-35.2 and the lattitude should be 31-31.2");
            try
            {
                lock (dal)
                {
                    // add customer to DAL
                    dal.AddCustomer(new DO.Customer
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Phone = c.Phone,
                        Longitude = c.LocationC.Longitude,
                        Lattitude = c.LocationC.Lattitude
                    });
                }
            }
            catch (DO.AlreadyExistsException ex)
            {
                throw new AlreadyExistsException(ex.Message);
            }

        }
        /// <summary>
        /// delete customer by id
        /// </summary>
        /// <param name="id"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteCustomer(int id)
        {
            try
            {
                lock (dal)
                {
                    // delete from DAL
                    dal.DeleteCustomer(id);
                }
            }
            catch (DO.NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
        }
        /// <summary>
        /// update the customer
        /// </summary>
        /// <param name="id">customer id</param>
        /// <param name="name">new customer name</param>
        /// <param name="phone">new customer phone</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateCustomer(int id, string name, string phone)
        {
            // validation input in order to update
            if (phone.Length != 10 && phone != "")
                throw new InvalidInputException("phone number should be 10 digits");
            lock (dal)
            {
                if (!dal.ExistCustomer(id))
                    throw new NotFoundException("customer");
                // get object 
                DO.Customer temp = dal.GetCustomerById(id);
                // delete from DAL in order to add the updateed object after
                dal.DeleteCustomer(id);
                if (name != "")
                    temp.Name = name;
                if (phone != "")
                    temp.Phone = phone;
                // add to dal
                dal.AddCustomer(temp);
            }
        }
        /// <summary>
        /// return a specific customer
        /// </summary>
        /// <param name="id">customer's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Customer GetCustomer(int id)
        {
            lock (dal)
            {
                // validation
                if (!dal.ExistCustomer(id))
                    throw new NotFoundException("customer");
                // get parcel from/to
                IEnumerable<ParcelInCustomer> parcelFrom = dal.FilteredParcel(p => p.SenderId == id).Select(p => convertToParcelInCustomer(p, p.TargetId));
                IEnumerable<ParcelInCustomer> parcelTo = dal.FilteredParcel(p => p.TargetId == id).Select(p => convertToParcelInCustomer(p, p.SenderId));
                // get from DAL
                DO.Customer c = dal.GetCustomerById(id);
                Customer customer = new Customer
                {
                    Id = c.Id,
                    Name = c.Name,
                    Phone = c.Phone,
                    LocationC = new Location { Longitude = c.Longitude, Lattitude = c.Lattitude },
                    FromCustomer = parcelFrom,
                    ToCustomer = parcelTo
                };
                return customer;
            }
        }
        /// <summary>
        /// return list of customers
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<CustomerToList> GetCustomersList()
        {
            lock (dal)
            {
                return dal.PrintCustomers().Select(c => convertToCustomerToList(c));
            }
        }
        /// <summary>
        /// return list of drones with status=status
        /// </summary>
        /// <param name="status">status</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DroneToList> StatusDrone(DroneStatus status)
        {
            return from DroneToList d in drones
                   where d.Status == status
                   select d;
        }
        /// <summary>
        /// return list of drones with weight=weight
        /// </summary>
        /// <param name="weight">weight</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DroneToList> WeightDrone(WeightCategories weight)
        {
            return from DroneToList d in drones
                   where d.MaxWeight == weight
                   select d;
        }
        /// <summary>
        /// eturn list of drones with status=status and weight=weight
        /// </summary>
        /// <param name="status">status</param>
        /// <param name="weight">weight</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DroneToList> StatusAndWeight(DroneStatus status, WeightCategories weight)
        {
            return from DroneToList d in drones
                   where (d.Status == status) && (d.MaxWeight == weight)
                   select d;
        }
        /// <summary>
        /// convert from DAL customet to BL customertolist
        /// </summary>
        /// <param name="c">DO customer</param>
        /// <returns></returns>
        private CustomerToList convertToCustomerToList(DO.Customer c)
        {
            int supplied = 0, notSupplied = 0, arrived = 0, notArrived = 0;
            lock (dal)
            {
                // loop on dal parcels to get customer parcels and their status 
                foreach (DO.Parcel p in dal.PrintParcels())
                {
                    if (p.SenderId == c.Id)
                    {
                        if (p.Delivered == null)
                            notSupplied++;
                        else
                            supplied++;
                    }
                    if (p.TargetId == c.Id)
                    {
                        if (p.Delivered == null)
                            notArrived++;
                        else
                            arrived++;
                    }
                }
                // return customer
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
        }
        #endregion
        #region PARCEL
        /// <summary>
        /// send to DAL for adding the parcel to the list of parcels
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddParcel(Parcel p)
        {
            lock (dal)
            {
                // validation
                if (!dal.ExistCustomer(p.Sender.Id))
                    throw new NotFoundException("customer:" + p.Sender.Id);
                if (!dal.ExistCustomer(p.Target.Id))
                    throw new NotFoundException("customer:" + p.Target.Id);
                // add parcel to dal
                dal.AddParcel(new DO.Parcel
                {
                    SenderId = p.Sender.Id,
                    TargetId = p.Target.Id,
                    Weight = (DO.WeightCategories)p.Weight,
                    Priority = (DO.Priorities)p.Priority,
                    Requested = DateTime.Now,
                    DroneId = null,
                    Scheduled = null,
                    PickedUp = null,
                    Delivered = null
                });
            }
        }
        /// <summary>
        /// delete parcel by id
        /// </summary>
        /// <param name="id"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteParcel(int id)
        {
            try
            {
                lock (dal)
                {
                    // delete parcel from DAL
                    dal.DeleteParcel(id);
                }
            }
            catch (DO.NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
        }
        /// <summary>
        /// assign drone to parcel
        /// </summary>
        /// <param name="id"> drone's id</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DroneToParcel(int id)
        {
            // validation
            int index = drones.FindIndex(x => x.Id == id);
            if (index == -1)
                throw new NotFoundException("drone:" + id);
            if (drones[index].Status != DroneStatus.Available)
                throw new DroneStatusException("Drone status must be available to be associated");
            // get drone location
            double lonD = drones[index].CurrentLocation.Longitude;
            double latD = drones[index].CurrentLocation.Lattitude;
            lock (dal)
            {
                //get the unassociated parcels
                IEnumerable<DO.Parcel> unParcels = dal.FilteredParcel(p => p.DroneId == null);
                if (unParcels.Count() == 0) //there's no unassociated parcels
                    throw new EmptyListException("there is no unassociated parcels");
                //remove the parcels with over weight 
                unParcels = from p in unParcels
                            where (int)p.Weight <= (int)drones[index].MaxWeight
                            select p;
                //order by the distance between drone to the sender customer
                unParcels = unParcels.OrderBy(p => dal.Distance(lonD, latD, dal.GetCustomerById(p.SenderId).Longitude, dal.GetCustomerById(p.SenderId).Lattitude));
                //order from the heavy to light weight
                unParcels = unParcels.OrderByDescending(p => (int)p.Weight);
                //order from urgent to regular
                unParcels = unParcels.OrderByDescending(p => (int)p.Priority);
                bool found = false; //found=parcel was found
                DO.Parcel chosenParcel;
                // loop on the ordered un-associated parcel list
                foreach (var parcel in unParcels)
                {
                    // if enough battary, choose it and exit the loop 
                    if (drones[index].Battery >= MinBattery(parcel, lonD, latD))
                    {
                        found = true;
                        chosenParcel = parcel;
                        drones[index].Status = DroneStatus.Delivery;
                        drones[index].ParcelId = chosenParcel.Id;
                        dal.DroneToParcel(chosenParcel.Id, drones[index].Id);
                        break;
                    }
                }
                // not found any drone with battery
                if (!found)
                    throw new NotFoundException("Parcel that close enough for the drone:" + id + " battery,");
            }
        }
        /// <summary>
        /// pick parcel by drone
        /// </summary>
        /// <param name="id">drone's id</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PickParcel(int id)
        {
            // validations
            int index = drones.FindIndex(d => d.Id == id);
            if (index == -1)
                throw new NotFoundException("drone: " + id);
            if (drones[index].Status != DroneStatus.Delivery)
                throw new DroneStatusException("drone status must be in delivery status");
            lock (dal)
            {
                // get all parcels from DAL
                IEnumerable<DO.Parcel> parcels = dal.PrintParcels();
                DO.Parcel parcel = new DO.Parcel
                {
                    Id = 0,
                    SenderId = 0,
                    TargetId = 0,
                    Weight = 0,
                    Priority = 0,
                    Requested = null,
                    DroneId = null,
                    Scheduled = null,
                    PickedUp = null,
                    Delivered = null
                };
                // loop on parcels to find the parcel in delivery status
                // that associated to this drone
                foreach (DO.Parcel p in parcels)
                {
                    if (p.DroneId == id && p.Delivered == null)
                        parcel = p;
                }
                // if not found any parcel
                if (parcel.Id == 0)
                    throw new NotFoundException("parcel with drone id: " + id);
                // the parcel already picked up
                if (parcel.PickedUp != null)
                    throw new ParcelModeException("The parcel is already picked up");

                DO.Customer c = dal.GetCustomerById(parcel.SenderId);
                // update DAL
                dal.PickParcel(parcel.Id);

                // get distance in order to update battery
                double dis = dal.Distance(drones[index].CurrentLocation.Longitude, drones[index].CurrentLocation.Lattitude, c.Longitude, c.Lattitude);
                // update BL
                drones[index].Battery -= dis * dal.ElectricityRequest().First();
                drones[index].CurrentLocation = new Location { Longitude = c.Longitude, Lattitude = c.Lattitude };
            }
        }
        /// <summary>
        /// deliver parcel to customer by drone
        /// </summary>
        /// <param name="id">drone's id</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeliverParcel(int id)
        {
            // VALIDATIONS
            int index = drones.FindIndex(d => d.Id == id);
            if (index == -1)
                throw new NotFoundException("drone: " + id);
            if (drones[index].Status != DroneStatus.Delivery)
                throw new DroneStatusException("Drone status must be in delivery status");
            lock (dal)
            {
                // loop on parcels to find the parcel in delivery status
                // that associated to this drone 
                IEnumerable<DO.Parcel> parcels = dal.PrintParcels();
                DO.Parcel parcel = new DO.Parcel
                {
                    Id = 0,
                    SenderId = 0,
                    TargetId = 0,
                    Weight = 0,
                    Priority = 0,
                    Requested = null,
                    DroneId = null,
                    Scheduled = null,
                    PickedUp = null,
                    Delivered = null
                };
                foreach (DO.Parcel p in parcels)
                {
                    if (p.DroneId == id && p.Delivered == null)
                        parcel = p;
                }
                // if not found any parcel
                if (parcel.Id == 0)
                    throw new NotFoundException("parcel with drone id: " + id);
                // if parcel is not picked up yet
                if (parcel.PickedUp == null)
                    throw new ParcelModeException("The parcel is not picked up yet");
                DO.Customer c = dal.GetCustomerById(parcel.TargetId);
                // update DAL
                dal.DeliverParcel(parcel.Id);

                // get distance in order to update battery
                double dis = dal.Distance(drones[index].CurrentLocation.Longitude, drones[index].CurrentLocation.Lattitude, c.Longitude, c.Lattitude);
                // update BL
                drones[index].Battery -= dis * dal.ElectricityRequest().ElementAt((int)parcel.Weight + 1);
                drones[index].CurrentLocation = new Location { Longitude = c.Longitude, Lattitude = c.Lattitude };
                drones[index].Status = DroneStatus.Available;
                drones[index].ParcelId = 0;
            }
        }
        /// <summary>
        /// return a specific parcel
        /// </summary>
        /// <param name="id">parcel's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Parcel GetParcel(int id)
        {
            lock (dal)
            {
                // validation
                if (!dal.ExistParcel(id))
                    throw new NotFoundException("parcel");
                //get parcel from DAL
                DO.Parcel p = dal.GetParcelById(id);
                //get the sender and target customers details
                DO.Customer cSender = dal.GetCustomerById(p.SenderId);
                DO.Customer cTarget = dal.GetCustomerById(p.TargetId);
                CustomerInParcel senderInParcel = new CustomerInParcel { Id = cSender.Id, Name = cSender.Name };
                CustomerInParcel targetInParcel = new CustomerInParcel { Id = cTarget.Id, Name = cTarget.Name };
                //if the parcel isn't associated
                if (p.DroneId == 0 || p.DroneId == null)
                {
                    //get all the details and return it
                    Parcel parcel1 = new Parcel
                    {
                        Id = p.Id,
                        Sender = senderInParcel,
                        Target = targetInParcel,
                        Weight = (WeightCategories)p.Weight,
                        Priority = (Priorities)p.Priority,
                        DroneP = null,
                        RequestedTime = p.Requested,
                        ScheduledTime = p.Scheduled,
                        PickedUpTime = p.PickedUp,
                        DeliveredTime = p.Delivered
                    };
                    return parcel1;
                }
                //if the parcel is associated
                //get the drone in parcel details
                DroneToList d = drones.Find(drone => drone.Id == p.DroneId);
                DroneInParcel droneInParcel = new DroneInParcel { Id = d.Id, Battery = d.Battery, CurrentLocation = d.CurrentLocation };
                //get all the details to parcel and return it
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
                return parcel;
            }
        }
        /// <summary>
        /// return list of parcels
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<ParcelToList> GetParcelsList()
        {
            lock (dal)
            {
                return dal.PrintParcels().Select(p => convertToParcelToList(p));
            }
        }
        /// <summary>
        /// return the list of parcels that not associated yet with drone
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<ParcelToList> UnassociatedParcel()
        {
            lock (dal)
            {
                return dal.FilteredParcel(p => p.DroneId == null).Select(p => convertToParcelToList(p));
            }
        }
        /// <summary>
        /// convert from DAL parcel to BL parcelincustomer
        /// </summary>
        /// <param name="p">do parcel</param>
        /// <param name="otherId">id of the customer in the other side of the delivery</param>
        /// <returns></returns>
        private ParcelInCustomer convertToParcelInCustomer(DO.Parcel p, int otherId)
        {
            // default not associated
            int parcelMode = 0;
            if (p.Scheduled != null)
            {
                //if parcel is associated
                if (p.PickedUp == null)
                    parcelMode = 1;
                else
                {
                    //if the parcel was picked up
                    if (p.Delivered == null)
                        parcelMode = 2;
                    else
                        //if the parcel was delivered
                        parcelMode = 3;
                }
            }
            lock (dal)
            {
                //the name of customer on the other side in this parcel
                string otherName = dal.GetCustomerById(otherId).Name;
                //get all the details of parcel-in-customer and return it
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
        }
        /// <summary>
        /// convert from DAL parcel to BL parceltolist
        /// </summary>
        /// <param name="p">do parcel</param>
        /// <returns></returns>
        private ParcelToList convertToParcelToList(DO.Parcel p)
        {
            lock (dal)
            {
                //get the names of the sender and target customers
                string senderName = dal.GetCustomerById(p.SenderId).Name;
                string targetName = dal.GetCustomerById(p.TargetId).Name;
                // default not associated
                int parcelMode = 0;
                if (p.Scheduled != null)
                {
                    //if parcel is associated
                    if (p.PickedUp == null)
                        parcelMode = 1;
                    else
                    {
                        //if the parcel was picked up
                        if (p.Delivered == null)
                            parcelMode = 2;
                        else
                            //if the parcel was delivered
                            parcelMode = 3;
                    }
                }
                //get all the details of parcel in list and return it
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
        }
        #endregion
        #region HELP METHODS
        /// <summary>
        /// return the minimum battery to pick the parcel, deliver it and go the closest station for charging
        /// </summary>
        /// <param name="parcel">the parcel that is needed to delivery</param>
        /// <param name="lonD">drone's longitude</param>
        /// <param name="latD">drone's lattitudes</param>
        /// <returns></returns>
        private double MinBattery(DO.Parcel parcel, double lonD, double latD)
        {
            // variables
            DO.Customer senderCustomer = dal.GetCustomerById(parcel.SenderId);
            DO.Customer stargetCustomer = dal.GetCustomerById(parcel.TargetId);
            DO.Station closeToTarget;
            double senderLon, senderLat, targetLon, targetLat;
            double senderDistance, deliveryDistance, chargeDistance;
            // assign value to variables
            senderLon = senderCustomer.Longitude;
            senderLat = senderCustomer.Lattitude;
            targetLon = stargetCustomer.Longitude;
            targetLat = stargetCustomer.Lattitude;
            // get the closet station to target in order to charge drone
            closeToTarget = ClosestStation(dal.FilteredStations(s => s.ChargeSlots > 0), targetLon, targetLat);
            // calculte the total battery according the toatal distance = to sender + to target + back to station
            senderDistance = dal.Distance(senderLon, senderLat, lonD, latD);
            deliveryDistance = dal.Distance(senderLon, senderLat, targetLon, targetLat);
            chargeDistance = dal.Distance(targetLon, targetLat, closeToTarget.Longitude, closeToTarget.Lattitude);

            return (senderDistance + chargeDistance) * dal.ElectricityRequest().First() + deliveryDistance * dal.ElectricityRequest().ElementAt((int)parcel.Weight + 1);
        }
        /// <summary>
        /// create string of sexagesimal lattitude
        /// </summary>
        /// <param name="lat">lattitude</param>
        /// <returns></returns>
        public static string Lat(double lat)
        {
            // convert the lat to string format
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
            // convert the long to string format
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
        #endregion
    }
}