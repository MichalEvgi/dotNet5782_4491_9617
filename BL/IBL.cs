using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBL.BO;

namespace IBL
{
    public interface IBL
    {
        #region STATION
        /// <summary>
        ///send to DAL for adding the station to the list of stations
        /// </summary>
        public void AddStation(Station s);
        /// <summary>
        /// delete station by id
        /// </summary>
        /// <param name="id"></param>
        public void DeleteStation(int id);
        /// <summary>
        /// update the station
        /// </summary>
        /// <param name="id">station id</param>
        /// <param name="name">new station name</param>
        /// <param name="chargeSlots">new station charge slots</param>
        public void UpdateStation(int id, int name, int chargeSlots);
        /// <summary>
        /// return the description of a specific station
        /// </summary>
        /// <param name="id">station's id</param>
        /// <returns></returns>
        public string GetStation(int id);
        /// <summary>
        /// return list of stations
        /// </summary>
        /// <returns></returns>
        public IEnumerable<StationToList> GetStationsList();
        /// <summary>
        /// return the list of the base stations with available charging slots
        /// </summary>
        /// <returns></returns>
        public IEnumerable<StationToList> AvailableStations();
        #endregion
        #region DRONE
        /// <summary>
        /// send to DAL for adding the drone to list of drones
        /// </summary>
        public void AddDrone(Drone d, int stationId);
        /// <summary>
        /// delete drone by id
        /// </summary>
        /// <param name="id"></param>
        public void DeleteDrone(int id);
        /// <summary>
        /// update the drone model
        /// </summary>
        /// <param name="id">drone id</param>
        /// <param name="model">new drone model</param>
        public void UpdateDrone(int id, string model);
        /// <summary>
        /// send drone to charge
        /// </summary>
        /// <param name="id">drone</param>
        public void SendToCharge(int id);
        /// <summary>
        /// release drone from charging
        /// </summary>
        /// <param name="id">drone's id</param>
        /// <param name="timeInCharging">charging time</param>
        public void ReleaseDrone(int id, double timeInCharging);
        /// <summary>
        /// return the description of a specific drone
        /// </summary>
        /// <param name="id">drone's id</param>
        /// <returns></returns>
        public string GetDrone(int id);
        /// <summary>
        /// return list of drones
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DroneToList> GetDronesList();
        #endregion
        #region CUSTOMER
        /// <summary>
        /// send to DAL for adding the customer to the list of customeer
        /// </summary>
        public void AddCustomer(Customer c);
        /// <summary>
        /// delete customer by id
        /// </summary>
        /// <param name="id"></param>
        public void DeleteCustomer(int id);
        /// <summary>
        /// update the customer
        /// </summary>
        /// <param name="id">customer id</param>
        /// <param name="name">new customer name</param>
        /// <param name="phone">new customer phone</param>
        public void UpdateCustomer(int id, string name, string phone);
        /// <summary>
        /// return the description of a specific customer
        /// </summary>
        /// <param name="id">customer's id</param>
        /// <returns></returns>
        public string GetCustomer(int id);
        /// <summary>
        /// return list of customers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CustomerToList> GetCustomersList();
        #endregion
        #region PARCEL
        /// <summary>
        /// send to DAL for adding the parcel to the list of parcels
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
        /// <param name="id"> drone's id</param>
        public void DroneToParcel(int id);
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
        /// return the description of a specific parcel
        /// </summary>
        /// <param name="id">parcel's id</param>
        /// <returns></returns>
        public string GetParcel(int id);
        /// <summary>
        /// return list of parcels
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ParcelToList> GetParcelsList();
        /// <summary>
        /// return the list of parcels that not associated yet with drone
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ParcelToList> UnassociatedParcel();
        #endregion
    }
}
