using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using IDAL.DO;
using IDAL;

namespace DalObject
{
    public class DalObject:IDal
    {
        public DalObject()
        {
            DataSource.Initialize();
        }
        /// <summary>
        /// add station to the list of stations
        /// </summary>
        public void AddStation(Station s)
        {
            DataSource.stations.Add(s);
        }
        /// <summary>
        /// add drone to list of drones
        /// </summary>
        public void AddDrone(Drone d)
        {
            DataSource.drones.Add(d);
        }
        /// <summary>
        /// add customer to the list of customeer
        /// </summary>
        public void AddCustomer(Customer c)
        {
            DataSource.customers.Add(c);
        }
        /// <summary>
        /// add parcel to the list of parcels
        /// </summary>
        public void AddParcel(Parcel p)
        {
            DataSource.parcels.Add(p);
        }
        /// <summary>
        /// assign drone to parcel
        /// </summary>
        /// <param name="id"> parcel's id</param>
        /// <param name="droneId"> drone's id</param>
        public void DroneToParcel(int id, int droneId)
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
        public void PickParcel(int id)
        {
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
        public void DeliverParcel(int id)
        {
            Parcel temp = GetParcelById(id);
            //save parcel in temp
            DataSource.parcels.Remove(temp);
            //remove
            temp.Delivered = DateTime.Now;
            //update
            DataSource.parcels.Add(temp);
            //add parcel back
        }
        /// <summary>
        /// send drone to charge in station
        /// </summary>
        /// <param name="droneId">drone's id</param>
        /// <param name="stationId">station's id</param>
        public void SendToCharge(int droneId, int stationId)
        {
            Station tempS = GetStationById(stationId);
            //save station in temp
            DataSource.stations.Remove(tempS);
            //remove
            tempS.ChargeSlots--;
            //update
            DataSource.stations.Add(tempS);
            //add station back
            DataSource.DroneCharges.Add(new DroneCharge { DroneId = droneId, StationId = stationId }); //add drone charge
        }
        /// <summary>
        /// release drone from station
        /// </summary>
        /// <param name="droneId">drone's id</param>
        public void ReleaseDrone(int droneId)
         {
            DroneCharge temp = GetDroneChargeById(droneId);
            Station tempS = GetStationById(temp.StationId);
            //save drone charge and station in temps
            DataSource.stations.Remove(tempS);
            //remove+ remove drone charge
            tempS.ChargeSlots++;
            //update
            DataSource.stations.Add(tempS);
            DataSource.DroneCharges.Remove(temp);
            //add station back
        }
        /// <summary>
        /// return parcel by id
        /// </summary>
        /// <param name="id">parcel's id</param>
        /// <returns></returns>
        public Parcel GetParcelById(int id)=> DataSource.parcels.Find(p => p.Id == id);
        /// <summary>
        /// return drone by id
        /// </summary>
        /// <param name="id">drone's id</param>
        /// <returns></returns>
        public Drone GetDroneById(int id) => DataSource.drones.Find(d => d.Id == id);
        /// <summary>
        /// return station by id
        /// </summary>
        /// <param name="id">station's id</param>
        /// <returns></returns>
        public Station GetStationById(int id) => DataSource.stations.Find(s => s.Id == id);
        /// <summary>
        /// return drone charge by drone's id
        /// </summary>
        /// <param name="id">drone's id</param>
        /// <returns></returns>
        public DroneCharge GetDroneChargeById(int id) => DataSource.DroneCharges.Find(dc => dc.DroneId == id);
        /// <summary>
        /// return customer by id
        /// </summary>
        /// <param name="id">customeer's id</param>
        /// <returns></returns>
        public Customer GetCustomerById(int id) => DataSource.customers.Find(c => c.Id == id);
        /// <summary>
        /// return the description of a specific station
        /// </summary>
        /// <param name="id">station's id</param>
        /// <returns></returns>
        public string ShowStation(int id)
        {
            return GetStationById(id).ToString();
        }
        /// <summary>
        /// return the description of a specific drone
        /// </summary>
        /// <param name="id">drone's id</param>
        /// <returns></returns>
        public string ShowDrone(int id)
        {
            return GetDroneById(id).ToString();
        }
        /// <summary>
        /// return the description of a specific customer
        /// </summary>
        /// <param name="id">customer's id</param>
        /// <returns></returns>
        public string ShowCustomer(int id)
        {
            return GetCustomerById(id).ToString();
        }
        /// <summary>
        /// return the description of a specific parcel
        /// </summary>
        /// <param name="id">parcel's id</param>
        /// <returns></returns>
        public string ShowParcel(int id)
        {
            return GetParcelById(id).ToString();
        }
        /// <summary>
        /// return list of stations
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Station> PrintStations()=>DataSource.stations;
        /// <summary>
        /// return list of drones
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Drone> PrintDrones() => DataSource.drones;
        /// <summary>
        /// return list of customers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Customer> PrintCustomers() => DataSource.customers;
        /// <summary>
        /// return list of parcels
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Parcel> PrintParcels() => DataSource.parcels;
        /// <summary>
        /// delete drone by id
        /// </summary>
        /// <param name="id"></param>
        public void DeleteDrone(int id)
        {
            Drone temp = GetDroneById(id);
            DataSource.drones.Remove(temp);
        }
        /// <summary>
        /// delete station by id
        /// </summary>
        /// <param name="id"></param>
        public void DeleteStation(int id)
        {
            Station temp = GetStationById(id);
            DataSource.stations.Remove(temp);
        }
        /// <summary>
        /// delete customer by id
        /// </summary>
        /// <param name="id"></param>
        public void DeleteCustomer(int id)
        {
            Customer temp = GetCustomerById(id);
            DataSource.customers.Remove(temp);
        }
        /// <summary>
        /// delete parcel by id
        /// </summary>
        /// <param name="id"></param>
        public void DeleteParcel(int id)
        {
            Parcel temp = GetParcelById(id);
            DataSource.parcels.Remove(temp);
        }
        /// <summary>
        /// return how many full slots in the station
        /// </summary>
        /// <param name="stationId">station id</param>
        /// <returns></returns>
        public int FullSlots(int stationId)
        {
            int count= 0;
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
        public IEnumerable<int> DroneInChargeIds(int stationId)
        {
            return from DroneCharge d in DataSource.DroneCharges
                   where d.StationId == stationId
                   select d.DroneId;
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
        /// <summary>
        /// request power consumption by drone
        /// return array of weight mode and charging rate
        /// </summary>
        /// <returns></returns>
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
    }
}