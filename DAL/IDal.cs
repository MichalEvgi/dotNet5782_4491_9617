using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL.DO;

namespace IDAL
{
    public interface IDal
    {
        /// <summary>
        /// add station to the list of stations
        /// </summary>
        /// <param name="id"> station's id</param>
        /// <param name="name"> station's name</param>
        /// <param name="lng"> station's longitude</param>
        /// <param name="lat"> station's lattitude</param>
        /// <param name="chargeslots"> number of available charge slots</param>
        public void AddStation(int id, int name, double lng, double lat, int chargeslots);
        /// <summary>
        /// add drone to list of drones
        /// </summary>
        /// <param name="id"> drone's id</param>
        /// <param name="model"> drone's model</param>
        /// <param name="maxWeight">drone's max weight of parcel</param>
        public void AddDrone(int id, string model, int maxWeight);
        /// <summary>
        /// add customer to the list of customeer
        /// </summary>
        /// <param name="id">customer's id</param>
        /// <param name="name">customer's name</param>
        /// <param name="phone">customer's phone</param>
        /// <param name="lng">customer's longitude</param>
        /// <param name="lat">customer's lattitude</param>
        public void AddCustomer(int id, string name, string phone, double lng, double lat);
        /// <summary>
        /// add parcel to the list of parcels
        /// </summary>
        /// <param name="sender">sender customer id</param>
        /// <param name="target">target customer id</param>
        /// <param name="weight"> parcel's weight</param>
        /// <param name="priority">parcel's priority</param>
        public void AddParcel(int sender, int target, int weight, int priority);
        // <summary>
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
        /// send drone to charge in station
        /// </summary>
        /// <param name="droneId">drone's id</param>
        /// <param name="stationId">station's id</param>
        public void SendToCharge(int droneId, int stationId);
        /// <summary>
        /// release drone from station
        /// </summary>
        /// <param name="droneId">drone's id</param>
        public void ReleaseDrone(int droneId);
        // <summary>
        /// return the description of a specific station
        /// </summary>
        /// <param name="id">station's id</param>
        /// <returns></returns>
        public string ShowStation(int id);
        /// <summary>
        /// return the description of a specific drone
        /// </summary>
        /// <param name="id">drone's id</param>
        /// <returns></returns>
        public string ShowDrone(int id);
        /// <summary>
        /// return the description of a specific customer
        /// </summary>
        /// <param name="id">customer's id</param>
        /// <returns></returns>
        public string ShowCustomer(int id);
        /// <summary>
        /// return the description of a specific parcel
        /// </summary>
        /// <param name="id">parcel's id</param>
        /// <returns></returns>
        public string ShowParcel(int id);
        /// <summary>
        /// return list of stations
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Station> PrintStations();
        /// <summary>
        /// return list of drones
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Drone> PrintDrones();
        /// <summary>
        /// return list of customers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Customer> PrintCustomers();
        /// <summary>
        /// return list of parcels
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Parcel> PrintParcels();
 }
}
