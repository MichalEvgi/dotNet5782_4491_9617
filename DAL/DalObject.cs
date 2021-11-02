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
        public static void DroneToParcel(int id, int droneId)
        {
            Parcel temp = GetParcelById(id);
            DataSource.parcels.Remove(temp);
            temp.DroneId = droneId;
            temp.Scheduled = DateTime.Now;
            DataSource.parcels.Add(temp);
        }
        public static void PickParcel(int id)
        {
            Parcel temp = GetParcelById(id);
            Drone tempD = GetDroneById(temp.DroneId);
            DataSource.parcels.Remove(temp);
            DataSource.drones.Remove(tempD);
            temp.PickedUp = DateTime.Now;
            tempD.Status = DroneStatuses.Delivery;
            DataSource.drones.Add(tempD);
            DataSource.parcels.Add(temp);
        }
        public static void DeliverParcel(int id)
        {
            Parcel temp = GetParcelById(id);
            Drone tempD = GetDroneById(temp.DroneId);
            DataSource.parcels.Remove(temp);
            DataSource.drones.Remove(tempD);
            temp.Delivered = DateTime.Now;
            tempD.Status = DroneStatuses.Available;
            DataSource.drones.Add(tempD);
            DataSource.parcels.Add(temp);
        }
        public static void SendToCharge(int droneId, int stationId)
        {
            Station tempS = GetStationById(stationId);
            Drone tempD = GetDroneById(droneId);
            DataSource.stations.Remove(tempS);
            DataSource.drones.Remove(tempD);
            tempS.ChargeSlots--;
            tempD.Status = DroneStatuses.Maintenance;
            DataSource.drones.Add(tempD);
            DataSource.stations.Add(tempS);
            DataSource.DroneCharges.Add(new DroneCharge { DroneId = droneId, StationId = stationId });
        }
        public static void ReleaseDrone(int droneId)
        {
            DroneCharge temp = GetDroneChargeById(droneId);
            Station tempS = GetStationById(temp.StationId);
            Drone tempD = GetDroneById(droneId);
            DataSource.stations.Remove(tempS);
            DataSource.drones.Remove(tempD);
            DataSource.DroneCharges.Remove(temp);
            tempS.ChargeSlots++;
            tempD.Status = DroneStatuses.Available;
            DataSource.drones.Add(tempD);
            DataSource.stations.Add(tempS);
        }
        public static Parcel GetParcelById(int id)=> DataSource.parcels.Find(p => p.Id == id);
        public static Drone GetDroneById(int id) => DataSource.drones.Find(d => d.Id == id);
        public static Station GetStationById(int id) => DataSource.stations.Find(s => s.Id == id);
        public static DroneCharge GetDroneChargeById(int id) => DataSource.DroneCharges.Find(d => d.DroneId == id);
        public static Customer GetCustomerById(int id) => DataSource.customers.Find(c => c.Id == id);

        public static string ShowStation(int id)
        {
            return GetStationById(id).ToString();
        }
        public static string ShowDrone(int id)
        {
            return GetDroneById(id).ToString();
        }
        public static string ShowCustomer(int id)
        {
            return GetCustomerById(id).ToString();
        }
        public static string ShowParcel(int id)
        {
            return GetParcelById(id).ToString();
        }

        public static List<Station> PrintStations()=>DataSource.stations;
        public static List<Drone> PrintDrones() => DataSource.drones;
        public static List<Customer> PrintCustomers() => DataSource.customers;
        public static List<Parcel> PrintParcels() => DataSource.parcels;
    }
}
