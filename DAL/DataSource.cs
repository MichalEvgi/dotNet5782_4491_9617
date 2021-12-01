using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL.DO;


namespace DalObject
{
    class DataSource
    {
        /// <summary>
        /// lists of all the structs
        /// </summary>
        internal static List<Drone> drones = new List<Drone>(10);
        internal static List<DroneCharge> DroneCharges = new List<DroneCharge>(10);
        internal static List<Station> stations = new List<Station>(5);
        internal static List<Customer> customers = new List<Customer>(100);
        internal static List<Parcel> parcels = new List<Parcel>(1000);

        internal static Random r = new Random();

        internal class Config
        {
            public static int RunIndex = 1000;
            public static double Available { get; set; }
            public static double LightWeight { get; set; }
            public static double MediumWeight { get; set; }
            public static double HeavyWeight { get; set; }
            public static double ChargingRate { get; set; } //drone loading rate- precent per hour
        }
        public static void Initialize()
        {
            CreateDrone();
            CreateStation();
            CreateCustomer();
            CreateParcel();
            Config.Available = 0.5;
            Config.LightWeight = 1;
            Config.MediumWeight = 2;
            Config.HeavyWeight = 3;
            Config.ChargingRate = 25;
        }
        /// <summary>
        /// create 5 drones in the list of drone
        /// </summary>
        public static void CreateDrone()
        {
            for (int i = 0; i < 5; i++)
            {
                drones.Add(new Drone
                {
                    Id = r.Next(1000, 10000),
                    Model = ((DroneModel)i).ToString(),
                    MaxWeight = (WeightCategories)r.Next(0, 3)
                }
                    );
            }
        }
        /// <summary>
        /// create 2 stations in the list of station
        /// </summary>
        public static void CreateStation()
        {
            for (int i = 0; i < 2; i++)
            {
                stations.Add(new Station
                {
                    Id = i + 1,
                    Name = r.Next(100000, 1000000),
                    Longitude = r.NextDouble() * 200 - 100,
                    Lattitude = r.NextDouble() * 200 - 100,
                    ChargeSlots = 3
                }
                    );
            }
        }
        /// <summary>
        /// create 10 parcels in the list of parcels
        /// </summary>
        public static void CreateParcel()
        {
            for (int i = 0; i < 9; i++)
            {
                parcels.Add(new Parcel
                {
                    Id = Config.RunIndex,
                    SenderId = customers[r.Next(0, 10)].Id,
                    TargetId = customers[r.Next(0, 10)].Id,
                    Weight = (WeightCategories)r.Next(0, 3),
                    Priority = (Priorities)r.Next(0, 3),
                    Requested = DateTime.Now,
                    DroneId = 0,
                    Scheduled = DateTime.MinValue,
                    PickedUp = DateTime.MinValue,
                    Delivered = DateTime.MinValue
                }) ;
                Config.RunIndex++;
            }
            parcels.Add(new Parcel
            {
                Id = Config.RunIndex,
                SenderId = customers[r.Next(0, 10)].Id,
                TargetId = customers[r.Next(0, 10)].Id,
                Weight = (WeightCategories)r.Next(0, 3),
                Priority = (Priorities)r.Next(0, 3),
                Requested = DateTime.Now,
                DroneId = drones[r.Next(0, 5)].Id,
                Scheduled = DateTime.Now,
                PickedUp = DateTime.Now,
                Delivered = DateTime.Now

            });
            Config.RunIndex++;
        }
        /// <summary>
        /// create 10 customers in the lise of customers
        /// </summary>
        public static void CreateCustomer()
        {
            for (int i = 0; i < 10; i++)
            {
                customers.Add(new Customer
                {
                    Id = r.Next(100000000, 444444444),
                    Name = ((Names)i).ToString(),
                    Phone = "05" + r.Next(10000000, 99999999),
                    Longitude = r.NextDouble() * 200 - 100,
                    Lattitude = r.NextDouble() * 200 - 100
                }
                    );
            }
        }
    }
}
