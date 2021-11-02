using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL.DO;

namespace DalObject
{
    public class DalObject
    {
        public DalObject()
        {
            DataSource.Initialize();
        }
        /// <summary>
        /// add station to the list of stations
        /// </summary>
        /// <param name="id"> station's id</param>
        /// <param name="name"> station's name</param>
        /// <param name="lng"> station's longitude</param>
        /// <param name="lat"> station's lattitude</param>
        /// <param name="chargeslots"> number of available charge slots</param>
        public static void AddStation(int id, int name, double lng, double lat, int chargeslots)
        {
            DataSource.stations.Add(new Station
            {
                Id = id,
                Name = name,
                Longitude = lng,
                Lattitude = lat,
                ChargeSlots = chargeslots
            });
        }
        /// <summary>
        /// add drone to list of drones
        /// </summary>
        /// <param name="id"> drone's id</param>
        /// <param name="model"> drone's model</param>
        /// <param name="maxWeight">drone's max weight of parcel</param>
        /// <param name="status">drone's status</param>
        /// <param name="battery">drone's battery</param>
        public static void AddDrone(int id, string model, int maxWeight, int status, double battery)
        {
            DataSource.drones.Add(new Drone
            {
                Id = id,
                Model = model,
                MaxWeight = (WeightCategories)maxWeight,
                Status = (DroneStatuses)status,
                Battery = battery
            });
        }
        /// <summary>
        /// add customer to the list of customeer
        /// </summary>
        /// <param name="id">customer's id</param>
        /// <param name="name">customer's name</param>
        /// <param name="phone">customer's phone</param>
        /// <param name="lng">customer's longitude</param>
        /// <param name="lat">customer's lattitude</param>
        public static void AddCustomer(int id, string name, string phone, double lng, double lat)
        {
            DataSource.customers.Add(new Customer
            {
                Id = id,
                Name = name,
                Phone = phone,
                Longitude = lng,
                Lattitude = lat
            });
        }
        /// <summary>
        /// add parcel to the list of parcels
        /// </summary>
        /// <param name="sender">sender customer id</param>
        /// <param name="target">target customer id</param>
        /// <param name="weight"> parcel's weight</param>
        /// <param name="priority">parcel's priority</param>
        public static void AddParcel(int sender, int target, int weight, int priority)
        {
            DataSource.parcels.Add(new Parcel
            {
                Id = DataSource.Config.index,
                SenderId = sender,
                TargetId = target,
                Weight = (WeightCategories)weight,
                Priority = (Priorities)priority,
                Requested = DateTime.Now,
                DroneId = 0
            });
            DataSource.Config.index++;
        }
        /// <summary>
        /// assign drone to parcel
        /// </summary>
        /// <param name="id"> parcel's id</param>
        /// <param name="droneId"> drone's id</param>
        public static void DroneToParcel(int id, int droneId)
        {
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
        public static void PickParcel(int id)
        {
            Parcel temp = GetParcelById(id);
            Drone tempD = GetDroneById(temp.DroneId);
            //save parcel and drone in temps
            DataSource.parcels.Remove(temp);
            DataSource.drones.Remove(tempD);
            //remove
            temp.PickedUp = DateTime.Now;
            tempD.Status = DroneStatuses.Delivery;
            //update
            DataSource.drones.Add(tempD);
            DataSource.parcels.Add(temp);
            //add parcel and drone back
        }
        /// <summary>
        /// deliver parcel to customer
        /// </summary>
        /// <param name="id">parcel's id</param>
        public static void DeliverParcel(int id)
        {
            Parcel temp = GetParcelById(id);
            Drone tempD = GetDroneById(temp.DroneId);
            //save parcel and drone in temps
            DataSource.parcels.Remove(temp);
            DataSource.drones.Remove(tempD);
            //remove
            temp.Delivered = DateTime.Now;
            tempD.Status = DroneStatuses.Available;
            //update
            DataSource.drones.Add(tempD);
            DataSource.parcels.Add(temp);
            //add parcel and drone back
        }
        /// <summary>
        /// send drone to charge in station
        /// </summary>
        /// <param name="droneId">drone's id</param>
        /// <param name="stationId">station's id</param>
        public static void SendToCharge(int droneId, int stationId)
        {
            Station tempS = GetStationById(stationId);
            Drone tempD = GetDroneById(droneId);
            //save station and drone in temps
            DataSource.stations.Remove(tempS);
            DataSource.drones.Remove(tempD);
            //remove
            tempS.ChargeSlots--;
            tempD.Status = DroneStatuses.Maintenance;
            //update
            DataSource.drones.Add(tempD);
            DataSource.stations.Add(tempS);
            //add station and drone back
            DataSource.DroneCharges.Add(new DroneCharge { DroneId = droneId, StationId = stationId }); //add drone charge
        }
        /// <summary>
        /// release drone from station
        /// </summary>
        /// <param name="droneId">drone's id</param>
        public static void ReleaseDrone(int droneId)
        {
            DroneCharge temp = GetDroneChargeById(droneId);
            Station tempS = GetStationById(temp.StationId);
            Drone tempD = GetDroneById(droneId);
            //save drone charge, station and drone in temps
            DataSource.stations.Remove(tempS);
            DataSource.drones.Remove(tempD);
            DataSource.DroneCharges.Remove(temp);
            //remove+ remove drone charge
            tempS.ChargeSlots++;
            tempD.Status = DroneStatuses.Available;
            //update
            DataSource.drones.Add(tempD);
            DataSource.stations.Add(tempS);
            //add station and drone back
        }
        /// <summary>
        /// return parcel by id
        /// </summary>
        /// <param name="id">parcel's id</param>
        /// <returns></returns>
        public static Parcel GetParcelById(int id)=> DataSource.parcels.Find(p => p.Id == id);
        /// <summary>
        /// return drone by id
        /// </summary>
        /// <param name="id">drone's id</param>
        /// <returns></returns>
        public static Drone GetDroneById(int id) => DataSource.drones.Find(d => d.Id == id);
        /// <summary>
        /// return station by id
        /// </summary>
        /// <param name="id">station's id</param>
        /// <returns></returns>
        public static Station GetStationById(int id) => DataSource.stations.Find(s => s.Id == id);
        /// <summary>
        /// return drone charge by drone's id
        /// </summary>
        /// <param name="id">drone's id</param>
        /// <returns></returns>
        public static DroneCharge GetDroneChargeById(int id) => DataSource.DroneCharges.Find(d => d.DroneId == id);
        /// <summary>
        /// return customer by id
        /// </summary>
        /// <param name="id">customeer's id</param>
        /// <returns></returns>
        public static Customer GetCustomerById(int id) => DataSource.customers.Find(c => c.Id == id);
        /// <summary>
        /// return the description of a specific station
        /// </summary>
        /// <param name="id">station's id</param>
        /// <returns></returns>
        public static string ShowStation(int id)
        {
            return GetStationById(id).ToString();
        }
        /// <summary>
        /// return the description of a specific drone
        /// </summary>
        /// <param name="id">drone's id</param>
        /// <returns></returns>
        public static string ShowDrone(int id)
        {
            return GetDroneById(id).ToString();
        }
        /// <summary>
        /// return the description of a specific customer
        /// </summary>
        /// <param name="id">customer's id</param>
        /// <returns></returns>
        public static string ShowCustomer(int id)
        {
            return GetCustomerById(id).ToString();
        }
        /// <summary>
        /// return the description of a specific parcel
        /// </summary>
        /// <param name="id">parcel's id</param>
        /// <returns></returns>
        public static string ShowParcel(int id)
        {
            return GetParcelById(id).ToString();
        }
        /// <summary>
        /// return list of stations
        /// </summary>
        /// <returns></returns>
        public static List<Station> PrintStations()=>DataSource.stations;
        /// <summary>
        /// return list of drones
        /// </summary>
        /// <returns></returns>
        public static List<Drone> PrintDrones() => DataSource.drones;
        /// <summary>
        /// return list of customers
        /// </summary>
        /// <returns></returns>
        public static List<Customer> PrintCustomers() => DataSource.customers;
        /// <summary>
        /// return list of parcels
        /// </summary>
        /// <returns></returns>
        public static List<Parcel> PrintParcels() => DataSource.parcels;
    }
}