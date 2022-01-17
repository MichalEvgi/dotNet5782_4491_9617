using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.CompilerServices;
using DalApi;
using DO;

namespace Dal
{
   sealed class DalObject:IDal
    {
        #region INITIALIZE
        #region Singelton
        private static DalObject instance = null;
        private static readonly object padlock = new object();

       internal static DalObject Instance
        {
            get
            {
                if(instance==null)
                {
                    lock(padlock)
                    {
                        if(instance==null)
                        {
                            instance = new DalObject();
                        }
                    }
                }
                return instance;
            }
        }
        #endregion
        public DalObject() 
        {
            DataSource.Initialize();
        }
        #endregion
        #region STATION
        /// <summary>
        /// add station to the list of stations
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddStation(Station s)
        {
            if (DataSource.stations.Exists(stat => stat.Id == s.Id))
                throw new AlreadyExistsException("station");
            DataSource.stations.Add(s);
        }
        /// <summary>
        /// delete station by id
        /// </summary>
        /// <param name="id"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteStation(int id)
        {
            if (!DataSource.stations.Exists(stat => stat.Id == id))
                throw new NotFoundException("station");
            Station temp = GetStationById(id);
            DataSource.stations.Remove(temp);
        }
        /// <summary>
        /// return the description of a specific station
        /// </summary>
        /// <param name="id">station's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public string ShowStation(int id)
        {
            if (!DataSource.stations.Exists(stat => stat.Id == id))
                throw new NotFoundException("station");
            return GetStationById(id).ToString();
        }
        /// <summary>
        /// return list of stations
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Station> PrintStations() => DataSource.stations;
        /// <summary>
        /// return all the stations with specific condition
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Station> FilteredStations(Predicate<Station> predi)
        {
            return from Station s in DataSource.stations
                   where predi(s)
                   select s;
        }
        /// <summary>
        /// return if the station exists in stations or doesn't
        /// </summary>
        /// <param name="id">station's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool ExistStation(int id) => DataSource.stations.Exists(s => s.Id == id);
        /// <summary>
        /// return station by id
        /// </summary>
        /// <param name="id">station's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Station GetStationById(int id) => DataSource.stations.Find(s => s.Id == id);
        /// <summary>
        /// return how many full slots in the station
        /// </summary>
        /// <param name="stationId">station id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int FullSlots(int stationId)
        {
            if (!DataSource.stations.Exists(stat => stat.Id == stationId))
                throw new NotFoundException("station");
            int count = 0;
            foreach (var d in DataSource.DroneCharges)
            {
                if (d.StationId == stationId)
                    count++;
            }
            return count;
        }
        /// <summary>
        /// return all the drone IDs that are charged in station(according to the station id)
        /// </summary>
        /// <param name="stationId">station id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<int> DroneInChargeIds(int stationId)
        {
            if (!DataSource.stations.Exists(stat => stat.Id == stationId))
                throw new NotFoundException("station");
            return from DroneCharge d in DataSource.DroneCharges
                   where d.StationId == stationId
                   select d.DroneId;
        }
        #endregion
        #region DRONE
        /// <summary>
        /// add drone to list of drones
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddDrone(Drone d)
        {
            if (DataSource.drones.Exists(dron => dron.Id == d.Id))
                throw new AlreadyExistsException("drone");
            DataSource.drones.Add(d);
        }
        /// <summary>
        /// delete drone by id
        /// </summary>
        /// <param name="id"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteDrone(int id)
        {
            if (!DataSource.drones.Exists(dron => dron.Id == id))
                throw new NotFoundException("drone");
            Drone temp = GetDroneById(id);
            DataSource.drones.Remove(temp);
        }
        /// <summary>
        /// return the description of a specific drone
        /// </summary>
        /// <param name="id">drone's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public string ShowDrone(int id)
        {
            if (!DataSource.drones.Exists(dron => dron.Id == id))
                throw new NotFoundException("drone");
            return GetDroneById(id).ToString();
        }
        /// <summary>
        /// return list of drones
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Drone> PrintDrones() => DataSource.drones;
        /// <summary>
        /// return drone by id
        /// </summary>
        /// <param name="id">drone's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Drone GetDroneById(int id) => DataSource.drones.Find(d => d.Id == id);
        /// <summary>
        /// return if the drone exists in drones or doesn't
        /// </summary>
        /// <param name="id">drone's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool ExistDrone(int id) => DataSource.drones.Exists(d => d.Id == id);
        /// <summary>
        /// send drone to charge in station
        /// </summary>
        /// <param name="droneId">drone's id</param>
        /// <param name="stationId">station's id</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SendToCharge(int droneId, int stationId)
        {
            if (!DataSource.drones.Exists(dron => dron.Id == droneId))
                throw new NotFoundException("drone");
            if (!DataSource.stations.Exists(stat => stat.Id == stationId))
                throw new NotFoundException("station");
            Station tempS = GetStationById(stationId);
            //save station in temp
            DataSource.stations.Remove(tempS);
            //remove
            tempS.ChargeSlots--;
            //update
            DataSource.stations.Add(tempS);
            //add station back
            DataSource.DroneCharges.Add(new DroneCharge { DroneId = droneId, StationId = stationId , EntryTime=DateTime.Now}); //add drone charge
        }
        /// <summary>
        /// release drone from station
        /// </summary>
        /// <param name="droneId">drone's id</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public double ReleaseDrone(int droneId)
        {
            if (!DataSource.drones.Exists(dron => dron.Id == droneId))
                throw new NotFoundException("drone");
            DroneCharge temp = GetDroneChargeById(droneId);
            double timeCharge = (DateTime.Now - temp.EntryTime).TotalSeconds;
            Station tempS = GetStationById(temp.StationId);
            //save drone charge and station in temps
            DataSource.stations.Remove(tempS);
            DataSource.DroneCharges.Remove(temp);
            //remove+ remove drone charge
            tempS.ChargeSlots++;
            //update
            DataSource.stations.Add(tempS);
            //add station back
            return timeCharge; 
        }
        /// <summary>
        /// return drone charge by drone's id
        /// </summary>
        /// <param name="id">drone's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DroneCharge GetDroneChargeById(int id) => DataSource.DroneCharges.Find(dc => dc.DroneId == id);
        #endregion
        #region CUSTOMER
        /// <summary>
        /// add customer to the list of customer
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddCustomer(Customer c)
        {
            if (DataSource.customers.Exists(cust => cust.Id == c.Id))
                throw new AlreadyExistsException("customer");
            DataSource.customers.Add(c);
        }
        /// <summary>
        /// delete customer by id
        /// </summary>
        /// <param name="id"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteCustomer(int id)
        {
            if (!DataSource.customers.Exists(cust => cust.Id == id))
                throw new NotFoundException("customer");
            Customer temp = GetCustomerById(id);
            DataSource.customers.Remove(temp);
        }
        /// <summary>
        /// return the description of a specific customer
        /// </summary>
        /// <param name="id">customer's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public string ShowCustomer(int id)
        {
            if (!DataSource.customers.Exists(cust => cust.Id == id))
                throw new NotFoundException("customer");
            return GetCustomerById(id).ToString();
        }
        /// <summary>
        /// return list of customers
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Customer> PrintCustomers() => DataSource.customers;
        /// <summary>
        /// return if the customer exists in customers or doesn't
        /// </summary>
        /// <param name="id">customer's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool ExistCustomer(int id) => DataSource.customers.Exists(c => c.Id == id);
        /// <summary>
        /// return customer by id
        /// </summary>
        /// <param name="id">customeer's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Customer GetCustomerById(int id) => DataSource.customers.Find(c => c.Id == id);
        #endregion
        #region PARCEL
        /// <summary>
        /// add parcel to the list of parcels
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddParcel(Parcel p)
        {
            p.Id = DataSource.Config.RunIndex;
            DataSource.Config.RunIndex++;
            DataSource.parcels.Add(p);
        }
        /// <summary>
        /// delete parcel by id
        /// </summary>
        /// <param name="id"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteParcel(int id)
        {
            if (!DataSource.parcels.Exists(parc => parc.Id == id))
                throw new NotFoundException("parcel");
            Parcel temp = GetParcelById(id);
            DataSource.parcels.Remove(temp);
        }
        /// <summary>
        /// return the description of a specific parcel
        /// </summary>
        /// <param name="id">parcel's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public string ShowParcel(int id)
        {
            if (!DataSource.parcels.Exists(parc => parc.Id == id))
                throw new NotFoundException("parcel");
            return GetParcelById(id).ToString();
        }
        /// <summary>
        /// return list of parcels
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Parcel> PrintParcels() => DataSource.parcels;
        /// <summary>
        /// return the list of parcels with specific filter
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Parcel> FilteredParcel(Predicate<Parcel> predi)
        {
            return from Parcel p in DataSource.parcels
                   where predi(p)
                   select p;
        }
        /// <summary>
        /// return if the parcel exists in parcels or doesn't
        /// </summary>
        /// <param name="id">parcel's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool ExistParcel(int id) => DataSource.parcels.Exists(p => p.Id == id);
        /// <summary>
        /// return parcel by id
        /// </summary>
        /// <param name="id">parcel's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Parcel GetParcelById(int id) => DataSource.parcels.Find(p => p.Id == id);
        /// <summary>
        /// return the transfered parcel with the droneId
        /// </summary>
        /// <param name="droneId"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Parcel GetTransferedParcel(int droneId)
        {
            if (!DataSource.drones.Exists(dron => dron.Id == droneId))
                throw new NotFoundException("drone");
            foreach (Parcel p in DataSource.parcels)
            {
                if (p.DroneId == droneId && p.Delivered == null)
                    return p;
            }
            throw new NotFoundException("parcel"); //parcel not found
        }
        /// <summary>
        /// assign drone to parcel
        /// </summary>
        /// <param name="id"> parcel's id</param>
        /// <param name="droneId"> drone's id</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DroneToParcel(int id, int droneId)
        {
            if (!DataSource.parcels.Exists(par => par.Id == id))
                throw new NotFoundException("parcel");
            if (!DataSource.drones.Exists(dron => dron.Id == droneId))
                throw new NotFoundException("drone");
            Parcel temp = GetParcelById(id); //save parcel in temp
            DataSource.parcels.Remove(temp); //remove
            temp.DroneId = droneId;
            temp.Scheduled = DateTime.Now;
            //update
            DataSource.parcels.Add(temp); //add parcel back
        }
        /// <summary>
        /// pick parcel by drone
        /// </summary>
        /// <param name="id">parcel's id</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PickParcel(int id)
        {
            if (!DataSource.parcels.Exists(par => par.Id == id))
                throw new NotFoundException("parcel");
            Parcel temp = GetParcelById(id);
            //save parcel in temp
            DataSource.parcels.Remove(temp);
            //remove
            temp.PickedUp = DateTime.Now;
            //update
            DataSource.parcels.Add(temp);
            //add parcel back
        }
        /// <summary>
        /// deliver parcel to customer
        /// </summary>
        /// <param name="id">parcel's id</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeliverParcel(int id)
        {
            if (!DataSource.parcels.Exists(par => par.Id == id))
                throw new NotFoundException("parcel");
            Parcel temp = GetParcelById(id);
            //save parcel in temp
            DataSource.parcels.Remove(temp);
            //remove
            temp.Delivered = DateTime.Now;
            //update
            DataSource.parcels.Add(temp);
            //add parcel back
        }
        #endregion
        #region HELP METHODS
        /// <summary>
        /// create string of sexagesimal lattitude
        /// </summary>
        /// <param name="lat">lattitude</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
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
        [MethodImpl(MethodImplOptions.Synchronized)]
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
        /// <summary>
        /// request power consumption by drone
        /// return array of weight mode and charging rate
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<double> ElectricityRequest()
        {
            double[] arr = new double[5];
            arr[0] = DataSource.Config.Available;
            arr[1] = DataSource.Config.LightWeight;
            arr[2] = DataSource.Config.MediumWeight;
            arr[3] = DataSource.Config.HeavyWeight;
            arr[4] = DataSource.Config.ChargingRate;
            return arr;
        }
        /// <summary>
        /// calculate the distance between two points of longitude and lattitude
        /// </summary>
        /// <param name="lon1"></param>
        /// <param name="lat1"></param>
        /// <param name="lon2"></param>
        /// <param name="lat2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public double Distance(double lon1, double lat1, double lon2, double lat2)
        {
            var p = 0.017453292519943295;    // Math.PI / 180
            var a = 0.5 - Math.Cos((lat2 - lat1) * p) / 2 +
                    Math.Cos(lat1 * p) * Math.Cos(lat2 * p) *
                    (1 - Math.Cos((lon2 - lon1) * p)) / 2;

            return 12742 * Math.Asin(Math.Sqrt(a)); // 2 * R; R = 6371 km
        }
        #endregion
    }
}