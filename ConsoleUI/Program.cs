using System;
using System.Collections.Generic;
using DalObject;
using IDAL.DO;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            int choice;
            Console.WriteLine("To add press 1");
            Console.WriteLine("To update press 2");
            Console.WriteLine("To view an entity, press 3");
            Console.WriteLine("To view a list press 4");
            choice = int.Parse(Console.ReadLine());
            while (choice != 5)
            {
                switch (choice)
                {
                    case 1: Adding(); break;
                    case 2: Apdating(); break;
                    case 3: Showing(); break;
                    case 4: ShowList(); break;
                }
                Console.WriteLine("enter another choice");
                choice = int.Parse(Console.ReadLine());
            }
        }
        public static void Adding()
        {
            int c;
            Console.WriteLine("To add station press 1");
            Console.WriteLine("To add drone press 2");
            Console.WriteLine("To add customer press 3");
            Console.WriteLine("To add parcel press 4");
            c = int.Parse(Console.ReadLine());
            switch (c)
            {
                case 1: AddStation(); break;
                case 2: AddDrone(); break;
                case 3: AddCustomer(); break;
                case 4: AddParcel(); break;
            }
        }
        public static void Apdating()
        {
            int c;
            Console.WriteLine("To assign parcel to drone press 1");
            Console.WriteLine("To collect parcel by drone press 2");
            Console.WriteLine("To Delivery parcel to customer press 3");
            Console.WriteLine("To send a drone for charging press 4");
            Console.WriteLine("To release drone from charging press 5");
            c = int.Parse(Console.ReadLine());
            switch (c)
            {
                case 1: ParcelToDrone(); break;
                case 2: CollectParcel(); break;
                case 3: DeliverParcel(); break;
                case 4: SendDrone(); break;
                case 5: ReleaseDrone(); break;
            }
        }
        public static void Showing()
        {
            int c;
            Console.WriteLine("To view a station press 1");
            Console.WriteLine("To view a drone press 2");
            Console.WriteLine("To view a customer press 3");
            Console.WriteLine("To view a parcel press 4");
            c = int.Parse(Console.ReadLine());
            switch (c)
            {
                case 1: viewStation(); break;
                case 2: viewDrone(); break;
                case 3: viewCustomer(); break;
                case 4: viewParcel(); break;
            }
        }
        public static void ShowList()
        {
            int c;
            Console.WriteLine("To view a list of stations press 1");
            Console.WriteLine("To view a list of drones press 2");
            Console.WriteLine("To view a list of customers press 3");
            Console.WriteLine("To view a list of parcels press 4");
            Console.WriteLine("To view a list of unassociated parcels press 5");
            Console.WriteLine("To view a list of stations with available charging stands press 6");
            c = int.Parse(Console.ReadLine());
            switch (c)
            {
                case 1: ListStations(); break;
                case 2: ListDrones(); break;
                case 3: ListCustomers(); break;
                case 4: ListParcels(); break;
                case 5: ListUnassociated(); break;
                case 6: ListAvailable(); break;
            }
        }
        public static void AddStation()
        {
            int id, name, charge;
            double lng, lat;
            Console.WriteLine("enter id");
            id = int.Parse(Console.ReadLine());
            Console.WriteLine("enter name(number)");
            name = int.Parse(Console.ReadLine());
            Console.WriteLine("enter longitude");
            lng = double.Parse(Console.ReadLine());
            Console.WriteLine("enter lattitude");
            lat = double.Parse(Console.ReadLine());
            Console.WriteLine("enter available charge slots");
            charge = int.Parse(Console.ReadLine());
            DalObject.DalObject.AddStation(id, name, lng, lat, charge);
        }
        public static void AddDrone()
        {
            int id;
            string model;
            int maxWeight, status = 0;
            double battery;
            Console.WriteLine("enter id");
            id = int.Parse(Console.ReadLine());
            Console.WriteLine("enter model");
            model = Console.ReadLine();
            Console.WriteLine("enter 0 for light, 1 for Medium and 2 for Heavy");
            maxWeight = int.Parse(Console.ReadLine());
            Console.WriteLine("enter battery");
            battery = double.Parse(Console.ReadLine());
            DalObject.DalObject.AddDrone(id, model, maxWeight, status, battery);
        }
        public static void AddCustomer()
        {
            int id;
            string name, phone;
            double lng, lat;
            Console.WriteLine("enter id");
            id = int.Parse(Console.ReadLine());
            Console.WriteLine("enter name");
            name = Console.ReadLine();
            Console.WriteLine("enter phone");
            phone = Console.ReadLine();
            Console.WriteLine("enter longitude");
            lng = double.Parse(Console.ReadLine());
            Console.WriteLine("enter lattitude");
            lat = double.Parse(Console.ReadLine());
            DalObject.DalObject.AddCustomer(id, name, phone, lng, lat);
        }
        public static void AddParcel()
        {
            int sender, target, weight, priority;
            Console.WriteLine("enter sender id");
            sender = int.Parse(Console.ReadLine());
            Console.WriteLine("enter target id");
            target = int.Parse(Console.ReadLine());
            Console.WriteLine("enter 0 for light, 1 for Medium and 2 for Heavy");
            weight = int.Parse(Console.ReadLine());
            Console.WriteLine("enter 0 for Regular, 1 for Express and 2 for Urgent");
            priority = int.Parse(Console.ReadLine());
            DalObject.DalObject.AddParcel(sender, target, weight, priority);
        }
        public static void ParcelToDrone()
        {
            int parcelId, droneId;
            Console.WriteLine("enter parcel id for association");
            parcelId = int.Parse(Console.ReadLine());
            Console.WriteLine("enter drone for association");
            droneId = int.Parse(Console.ReadLine());
            DalObject.DalObject.DroneToParcel(parcelId, droneId);
        }
        public static void CollectParcel()
        {
            int id;
            Console.WriteLine("enter parcel id to collect");
            id = int.Parse(Console.ReadLine());
            DalObject.DalObject.PickParcel(id);
        }
        public static void DeliverParcel()
        {
            int id;
            Console.WriteLine("enter parcel id for delievery");
            id = int.Parse(Console.ReadLine());
            DalObject.DalObject.DeliverParcel(id);
        }
        public static void SendDrone()
        {
            int droneId, stationId;
            Console.WriteLine("enter drone id for charging");
            droneId = int.Parse(Console.ReadLine());
            Console.WriteLine("enter station id for charging");
            stationId = int.Parse(Console.ReadLine());
            DalObject.DalObject.SendToCharge(droneId, stationId);
        }
        public static void ReleaseDrone()
        {
            int droneId;
            Console.WriteLine("enter drone id for releasing");
            droneId = int.Parse(Console.ReadLine());
            DalObject.DalObject.ReleaseDrone(droneId);
        }
        public static void viewStation()
        {
            int id;
            Console.WriteLine("enter station id to show");
            id = int.Parse(Console.ReadLine());
            Console.WriteLine(DalObject.DalObject.ShowStation(id));
        }
        public static void viewDrone()
        {
            int id;
            Console.WriteLine("enter drone id to show");
            id = int.Parse(Console.ReadLine());
            Console.WriteLine(DalObject.DalObject.ShowDrone(id));
        }
        public static void viewCustomer()
        {
            int id;
            Console.WriteLine("enter customer id to show");
            id = int.Parse(Console.ReadLine());
            Console.WriteLine(DalObject.DalObject.ShowCustomer(id));
        }
        public static void viewParcel()
        {
            int id;
            Console.WriteLine("enter parcel id to show");
            id = int.Parse(Console.ReadLine());
            Console.WriteLine(DalObject.DalObject.ShowParcel(id));
        }
        public static void ListStations()
        {
            List<Station> stations = DalObject.DalObject.PrintStations();
            foreach (Station s in stations)
            {
                Console.WriteLine(s);
            }
        }
        public static void ListDrones()
        {
            List<Drone> drones = DalObject.DalObject.PrintDrones();
            foreach (Drone d in drones)
            {
                Console.WriteLine(d);
            }
        }
        public static void ListCustomers()
        {
            List<Customer> customers = DalObject.DalObject.PrintCustomers();
            foreach (Customer c in customers)
            {
                Console.WriteLine(c);
            }
        }
        public static void ListParcels()
        {
            List<Parcel> parcels = DalObject.DalObject.PrintParcels();
            foreach (Parcel p in parcels)
            {
                Console.WriteLine(p);
            }
        }
        public static void ListUnassociated()
        {
            List<Parcel> parcels = DalObject.DalObject.PrintParcels();
            foreach (Parcel p in parcels)
            {
                if (p.DroneId == 0)
                    Console.WriteLine(p);
            }
        }
        public static void ListAvailable()
        {
            List<Station> stations = DalObject.DalObject.PrintStations();
            foreach (Station s in stations)
            {
                if (s.ChargeSlots > 0)
                    Console.WriteLine(s);
            }
        }
    }
}
