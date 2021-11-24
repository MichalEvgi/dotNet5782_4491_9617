using System;
using System.Collections.Generic;
using DalObject;
using IDAL.DO;
using IDAL;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            IDal dalObject = new DalObject.DalObject();
            int choice;
            Console.WriteLine("To add press 1");
            Console.WriteLine("To update press 2");
            Console.WriteLine("To view an entity, press 3");
            Console.WriteLine("To view a list press 4");
            int.TryParse(Console.ReadLine(),out choice);
            //ask the user which kind of action he wants to operate
            while (choice != 5)
            {
                switch (choice)
                {
                    case 1: Adding(); break;
                    case 2: Updating(); break;
                    case 3: Showing(); break;
                    case 4: ShowList(); break;
                }
                Console.WriteLine("enter another choice");
                int.TryParse(Console.ReadLine(), out choice);
            }
        }
        /// <summary>
        /// ask the user which object to add
        /// </summary>
        public static void Adding()
        {
            int c;
            Console.WriteLine("To add station press 1");
            Console.WriteLine("To add drone press 2");
            Console.WriteLine("To add customer press 3");
            Console.WriteLine("To add parcel press 4");
            int.TryParse(Console.ReadLine(), out c);
            switch (c)
            {
                case 1: AddStation(); break;
                case 2: AddDrone(); break;
                case 3: AddCustomer(); break;
                case 4: AddParcel(); break;
            }
        }
        /// <summary>
        /// ask the user which update to do
        /// </summary>
        public static void Updating()
        {
            int c;
            Console.WriteLine("To assign parcel to drone press 1");
            Console.WriteLine("To collect parcel by drone press 2");
            Console.WriteLine("To Delivery parcel to customer press 3");
            Console.WriteLine("To send a drone for charging press 4");
            Console.WriteLine("To release drone from charging press 5");
            int.TryParse(Console.ReadLine(), out c);
            switch (c)
            {
                case 1: ParcelToDrone(); break;
                case 2: CollectParcel(); break;
                case 3: DeliverParcel(); break;
                case 4: SendDrone(); break;
                case 5: ReleaseDrone(); break;
            }
        }
        /// <summary>
        /// ask the user which object to show
        /// </summary>
        public static void Showing()
        {
            int c;
            Console.WriteLine("To view a station press 1");
            Console.WriteLine("To view a drone press 2");
            Console.WriteLine("To view a customer press 3");
            Console.WriteLine("To view a parcel press 4");
            int.TryParse(Console.ReadLine(), out c);
            switch (c)
            {
                case 1: viewStation(); break;
                case 2: viewDrone(); break;
                case 3: viewCustomer(); break;
                case 4: viewParcel(); break;
            }
        }
        /// <summary>
        /// ask the user which list to show
        /// </summary>
        public static void ShowList()
        {
            int c;
            Console.WriteLine("To view a list of stations press 1");
            Console.WriteLine("To view a list of drones press 2");
            Console.WriteLine("To view a list of customers press 3");
            Console.WriteLine("To view a list of parcels press 4");
            Console.WriteLine("To view a list of unassociated parcels press 5");
            Console.WriteLine("To view a list of stations with available charging stands press 6");
            int.TryParse(Console.ReadLine(), out c);
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
        /// <summary>
        /// add station to the stations list
        /// </summary>
        public static void AddStation()
        {
            int id, name, charge;
            double lng, lat;
            Console.WriteLine("enter id");
            int.TryParse(Console.ReadLine(), out id);
            Console.WriteLine("enter name(number)");
            int.TryParse(Console.ReadLine(), out name);
            Console.WriteLine("enter longitude");
            double.TryParse(Console.ReadLine(), out lng);
            Console.WriteLine("enter lattitude");
            double.TryParse(Console.ReadLine(), out lat);
            Console.WriteLine("enter available charge slots");
            int.TryParse(Console.ReadLine(), out charge);
            DalObject.DalObject.AddStation(id, name, lng, lat, charge);
        }
        /// <summary>
        /// add drone to the drones list
        /// </summary>
        public static void AddDrone()
        {
            int id;
            string model;
            int maxWeight;
            Console.WriteLine("enter id");
            int.TryParse(Console.ReadLine(), out id);
            Console.WriteLine("enter model");
            model = Console.ReadLine();
            Console.WriteLine("enter 0 for light, 1 for Medium and 2 for Heavy");
            int.TryParse(Console.ReadLine(), out maxWeight);
            DalObject.DalObject.AddDrone(id, model, maxWeight);
        }
        /// <summary>
        /// add customer to the customers list
        /// </summary>
        public static void AddCustomer()
        {
            int id;
            string name, phone;
            double lng, lat;
            Console.WriteLine("enter id");
            int.TryParse(Console.ReadLine(), out id);
            Console.WriteLine("enter name");
            name = Console.ReadLine();
            Console.WriteLine("enter phone");
            phone = Console.ReadLine();
            Console.WriteLine("enter longitude");
            double.TryParse(Console.ReadLine(), out lng);
            Console.WriteLine("enter lattitude");
            double.TryParse(Console.ReadLine(), out lat);
            DalObject.DalObject.AddCustomer(id, name, phone, lng, lat);
        }
        // add parcel to the parecls list
        public static void AddParcel()
        {
            int sender, target, weight, priority;
            Console.WriteLine("enter sender id");
            int.TryParse(Console.ReadLine(), out sender);
            Console.WriteLine("enter target id");
            int.TryParse(Console.ReadLine(), out target);
            Console.WriteLine("enter 0 for light, 1 for Medium and 2 for Heavy");
            int.TryParse(Console.ReadLine(), out weight);
            Console.WriteLine("enter 0 for Regular, 1 for Express and 2 for Urgent");
            int.TryParse(Console.ReadLine(), out priority);
            DalObject.DalObject.AddParcel(sender, target, weight, priority);
        }
        /// <summary>
        /// assign parcel to drone
        /// </summary>
        public static void ParcelToDrone()
        {
            int parcelId, droneId;
            Console.WriteLine("enter parcel id for association");
            int.TryParse(Console.ReadLine(), out parcelId);
            Console.WriteLine("enter drone for association");
            int.TryParse(Console.ReadLine(), out droneId);
            DalObject.DalObject.DroneToParcel(parcelId, droneId);
        }
        /// <summary>
        /// pick up parcel by drone
        /// </summary>
        public static void CollectParcel()
        {
            int id;
            Console.WriteLine("enter parcel id to collect");
            int.TryParse(Console.ReadLine(), out id);
            DalObject.DalObject.PickParcel(id);
        }
        /// <summary>
        /// deliever parcel to customer
        /// </summary>
        public static void DeliverParcel()
        {
            int id;
            Console.WriteLine("enter parcel id for delievery");
            int.TryParse(Console.ReadLine(), out id);
            DalObject.DalObject.DeliverParcel(id);
        }
        /// <summary>
        /// send drone to charge in station
        /// </summary>
        public static void SendDrone()
        {
            int droneId, stationId;
            Console.WriteLine("enter drone id for charging");
            int.TryParse(Console.ReadLine(), out droneId);
            Console.WriteLine("enter station id for charging");
            int.TryParse(Console.ReadLine(), out stationId);
            DalObject.DalObject.SendToCharge(droneId, stationId);
        }
        /// <summary>
        /// release drone from charge station
        /// </summary>
        public static void ReleaseDrone()
        {
            int droneId;
            Console.WriteLine("enter drone id for releasing");
            int.TryParse(Console.ReadLine(), out droneId);
            DalObject.DalObject.ReleaseDrone(droneId);
        }
        /// <summary>
        /// prints the requested station
        /// </summary>
        public static void viewStation()
        {
            int id;
            Console.WriteLine("enter station id to show");
            int.TryParse(Console.ReadLine(), out id);
            Console.WriteLine(DalObject.DalObject.ShowStation(id));
        }
        /// <summary>
        /// Prints the requested drone
        /// </summary>
        public static void viewDrone()
        {
            int id;
            Console.WriteLine("enter drone id to show");
            int.TryParse(Console.ReadLine(), out id);
            Console.WriteLine(DalObject.DalObject.ShowDrone(id));
        }
        /// <summary>
        /// Prints the requested customeer
        /// </summary>
        public static void viewCustomer()
        {
            int id;
            Console.WriteLine("enter customer id to show");
            int.TryParse(Console.ReadLine(), out id);
            Console.WriteLine(DalObject.DalObject.ShowCustomer(id));
        }
        /// <summary>
        /// Prints the requested parcel
        /// </summary>
        public static void viewParcel()
        {
            int id;
            Console.WriteLine("enter parcel id to show");
            int.TryParse(Console.ReadLine(), out id);
            Console.WriteLine(DalObject.DalObject.ShowParcel(id));
        }
        /// <summary>
        /// prints the stations list
        /// </summary>
        public static void ListStations()
        {
            IEnumerable<Station> stations = DalObject.DalObject.PrintStations();
            foreach (Station s in stations)
            {
                Console.WriteLine(s);
            }
        }
        /// <summary>
        ///  prints the drones list
        /// </summary>
        public static void ListDrones()
        {
            IEnumerable<Drone> drones = DalObject.DalObject.PrintDrones();
            foreach (Drone d in drones)
            {
                Console.WriteLine(d);
            }
        }
        /// <summary>
        ///  prints the customers list
        /// </summary>
        public static void ListCustomers()
        {
            IEnumerable<Customer> customers = DalObject.DalObject.PrintCustomers();
            foreach (Customer c in customers)
            {
                Console.WriteLine(c);
            }
        }
        /// <summary>
        ///  prints the parcels list
        /// </summary>
        public static void ListParcels()
        {
            IEnumerable<Parcel> parcels = DalObject.DalObject.PrintParcels();
            foreach (Parcel p in parcels)
            {
                Console.WriteLine(p);
            }
        }
        /// <summary>
        /// prints the list of parcels that not associated yet with drone
        /// </summary>
        public static void ListUnassociated()
        {
            IEnumerable<Parcel> parcels = DalObject.DalObject.PrintParcels();
            foreach (Parcel p in parcels)
            {
                if (p.DroneId == 0) //if the parcel is not associated with drone
                    Console.WriteLine(p);
            }
        }
        /// <summary>
        /// prints the base stations with available charging slots
        /// </summary>
        public static void ListAvailable()
        {
            IEnumerable<Station> stations = DalObject.DalObject.PrintStations();
            foreach (Station s in stations)
            {
                if (s.ChargeSlots > 0) //if there are any available charging slots in the station
                    Console.WriteLine(s);
            }
        }
    }
}