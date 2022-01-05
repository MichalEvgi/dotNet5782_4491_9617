using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;

namespace DalApi
{
    public interface IDal
    {
        #region STATION
        /// <summary>
        /// add station to the list of stations
        /// </summary>
        public void AddStation(Station s);
        /// <summary>
        /// delete station by id
        /// </summary>
        /// <param name="id"></param>
        public void DeleteStation(int id);
        /// <summary>
        /// return the description of a specific station
        /// </summary>
        /// <param name="id">station's id</param>
        /// <returns></returns>
        public string ShowStation(int id);
        /// <summary>
        /// return list of stations
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Station> PrintStations();
        /// <summary>
        /// return all the stations with specific condition
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Station> FilteredStations(Predicate<Station> predi);
        /// <summary>
        /// return station by id
        /// </summary>
        /// <param name="id">station's id</param>
        /// <returns></returns>
        public Station GetStationById(int id);
        /// <summary>
        /// return if station with stationId=id is exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistStation(int id);
        /// <summary>
        /// return how many full slots in the station
        /// </summary>
        /// <param name="stationId">station id</param>
        /// <returns></returns>
        public int FullSlots(int stationId);
        /// <summary>
        /// return all the drone IDs that are charged in station(according to the station id)
        /// </summary>
        /// <param name="stationId">station id</param>
        /// <returns></returns>
        public IEnumerable<int> DroneInChargeIds(int stationId);
        #endregion
        #region DRONE
        /// <summary>
        /// add drone to list of drones
        /// </summary>
        public void AddDrone(Drone d);
        /// <summary>
        /// delete drone by id
        /// </summary>
        /// <param name="id"></param>
        public void DeleteDrone(int id);
        /// <summary>
        /// send drone to charge in station
        /// </summary>
        /// <param name="droneId">drone's id</param>
        /// <param name="stationId">station's id</param>
        public void SendToCharge(int droneId, int stationId);
        /// <summary>
        /// release drone from station and return the time in charging
        /// </summary>
        /// <param name="droneId">drone's id</param>
        public double ReleaseDrone(int droneId);
        /// <summary>
        /// return the description of a specific drone
        /// </summary>
        /// <param name="id">drone's id</param>
        /// <returns></returns>
        public string ShowDrone(int id);
        /// <summary>
        /// return list of drones
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Drone> PrintDrones();
        /// <summary>
        /// return drone by id
        /// </summary>
        /// <param name="id">drone's id</param>
        /// <returns></returns>
        public Drone GetDroneById(int id);
        /// <summary>
        /// return if drone with droneId=id is exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistDrone(int id);
        /// <summary>
        /// return drone charge by drone's id
        /// </summary>
        /// <param name="id">drone's id</param>
        /// <returns></returns>
        public DroneCharge GetDroneChargeById(int id);
        #endregion
        #region CUSTOMER
        /// <summary>
        /// add customer to the list of customeer
        /// </summary>
        public void AddCustomer(Customer c);
        /// <summary>
        /// delete customer by id
        /// </summary>
        /// <param name="id"></param>
        public void DeleteCustomer(int id);
        /// <summary>
        /// return the description of a specific customer
        /// </summary>
        /// <param name="id">customer's id</param>
        /// <returns></returns>
        public string ShowCustomer(int id);
        /// <summary>
        /// return list of customers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Customer> PrintCustomers();
        /// <summary>
        /// return customer by id
        /// </summary>
        /// <param name="id">customeer's id</param>
        /// <returns></returns>
        public Customer GetCustomerById(int id);
        /// <summary>
        /// return if customer with customerId=id is exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistCustomer(int id);
        #endregion
        #region PARCEL
        /// <summary>
        /// add parcel to the list of parcels
        /// </summary>
        public void AddParcel(Parcel p);
        /// <summary>
        /// delete parcel by id
        /// </summary>
        /// <param name="id"></param>
        public void DeleteParcel(int id);
        /// <summary>
        /// assign drone to parcel
        /// </summary>
        /// <param name="id"> parcel's id</param>
        /// <param name="droneId"> drone's id</param>
        public void DroneToParcel(int id, int droneId);
        /// <summary>
        /// pick parcel by drone
        /// </summary>
        /// <param name="id">parcel's id</param>
        public void PickParcel(int id);
        /// <summary>
        /// deliver parcel to customer
        /// </summary>
        /// <param name="id">parcel's id</param>
        public void DeliverParcel(int id);
        /// <summary>
        /// return the description of a specific parcel
        /// </summary>
        /// <param name="id">parcel's id</param>
        /// <returns></returns>
        public string ShowParcel(int id);
        /// <summary>
        /// return list of parcels
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Parcel> PrintParcels();
        /// <summary>
        /// return the list of parcels with specific condition
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Parcel> FilteredParcel(Predicate<Parcel> predi);
        /// /// <summary>
        /// return parcel by id
        /// </summary>
        /// <param name="id">parcel's id</param>
        /// <returns></returns>
        public Parcel GetParcelById(int id);
        /// <summary>
        /// return if parcel with parcelId=id is exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistParcel(int id);
        /// <summary>
        /// return the transfered parcel with the droneId
        /// </summary>
        /// <param name="droneId"></param>
        /// <returns></returns>
        public Parcel GetTransferedParcel(int droneId);
        #endregion
        #region HELP METHODS
        /// <summary>
        /// calculate the distance between two points of longitude and lattitude
        /// </summary>
        /// <param name="lon1"></param>
        /// <param name="lat1"></param>
        /// <param name="lon2"></param>
        /// <param name="lat2"></param>
        /// <returns></returns>
        public double Distance(double lon1, double lat1, double lon2, double lat2);
        /// <summary>
        /// request power consumption by drone
        /// return array of weight mode and charging rate
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double> ElectricityRequest();
        /// <summary>
        /// dreate string of sexagesimal longitude
        /// </summary>
        /// <param name="lng">longitude</param>
        /// <returns></returns>
        public static string Lng(double lng)
        {
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
        /// /// // <summary>
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
        /// Clear all the charging details when closing the program
        /// </summary>
        public void ClearDroneCharging();
    }
    #endregion
}