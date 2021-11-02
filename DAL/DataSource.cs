﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL.DO;


namespace DalObject
{
    class DataSource
    {
        internal static List<Drone> drones = new List<Drone>(10);
        internal static List<DroneCharge> DroneCharges = new List<DroneCharge>(10);
        internal static List<Station> stations = new List<Station>(5);
        internal static List<Customer> customers = new List<Customer>(100);
        internal static List<Parcel> parcels = new List<Parcel>(1000);

        internal static Random r = new Random();

        internal class Config
        {
            internal static int index = 1000;
        }
       public static void Initialize()
        {
            CreateDrone();
            CreateStation();
            CreateCustomer();
            CreateParcel();
            CreateDroneCharge();
        }
        public static void CreateDrone()
        {
            for (int i = 0; i < 5; i++)
            {
                drones.Add(new Drone
                {
                    Id = r.Next(1000, 10000),
                    Model=((DroneModel)i).ToString(),
                    MaxWeight=(WeightCategories)r.Next(0,3),
                    Status= (DroneStatuses)r.Next(0,2),
                    Battery= r.Next(0,101)
                }
                    );
            }
        }
        public static void CreateStation()
        {
            for (int i = 0; i < 2; i++)
            {
                stations.Add(new Station
                {
                    Id = i + 1,
                    Name=r.Next(100000,1000000),
                    Longitude=r.NextDouble()*200-100,
                    Lattitude=r.NextDouble()*200-100,
                    ChargeSlots= 3
                }
                    );
            }
        }
        public static void CreateParcel()
        {
            parcels.Add(new Parcel
            {
                Id = r.Next(100, 999),
                SenderId = customers[r.Next(0, 10)].Id,
                TargetId = customers[r.Next(0, 10)].Id,
                Weight = (WeightCategories)r.Next(0, 3),
                Priority = (Priorities)r.Next(0, 3),
                Requested = DateTime.Now,
                DroneId = 0,
                Scheduled =DateTime.MinValue,
                PickedUp=DateTime.MinValue,
                Delivered=DateTime.MinValue

            });
        }
        public static void CreateCustomer()
        {
            for (int i = 0; i < 10; i++)
            {
                customers.Add(new Customer
                {
                    Id = r.Next(100000000, 444444444),
                    Name = ((Names)i).ToString(),
                    Phone="05"+r.Next(10000000,99999999),
                    Longitude = r.NextDouble() * 200 - 100,
                    Lattitude = r.NextDouble() * 200 - 100
                }
                    ) ;
            }
        }
        public static void CreateDroneCharge()
        {
            for (int i = 0; i < 5; i++)
            {
                DroneCharges.Add(new DroneCharge
                {
                   DroneId=drones[i].Id,
                   StationId=stations[r.Next(0,2)].Id
                });
            }
        }
    }
}
