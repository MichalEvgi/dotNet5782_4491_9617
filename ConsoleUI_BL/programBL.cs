using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using IBL.BO;
using IBL;
namespace ConsoleUI_BL
{
    public class programBL
    {
        static void Main(string[] args)
        {
            #region MAIN
            try
            {
                IBL.IBL ibl = new BL();
                int choice;
                Console.WriteLine("To add press 1");
                Console.WriteLine("To update press 2");
                Console.WriteLine("To view an entity, press 3");
                Console.WriteLine("To view a list press 4");
                Console.WriteLine("To exit press 5");
                int.TryParse(Console.ReadLine(), out choice);
                //ask the user which kind of action he wants to operate
                while (choice != 5)
                {
                    switch (choice)
                    {
                        case 1: Adding(ibl); break;
                        case 2: Updating(ibl); break;
                        case 3: Showing(ibl); break;
                        case 4: ShowList(ibl); break;
                        default: Console.WriteLine("error! enter a number between 1 to 5"); break; //ERROR
                    }
                    Console.WriteLine("enter another choice");
                    int.TryParse(Console.ReadLine(), out choice);
                }
            }
            catch(BatteryException ex) //exception in the BL constructor
            {
                Console.WriteLine(ex);
                Console.WriteLine("Run the program again");
            }
            #endregion
        }
        #region ADDING
        /// <summary>
        /// ask the user which object to add
        /// </summary>
        public static void Adding(IBL.IBL ibl)
        {
            int c;
            Console.WriteLine("To add station press 1");
            Console.WriteLine("To add drone press 2");
            Console.WriteLine("To add customer press 3");
            Console.WriteLine("To add parcel press 4");
            int.TryParse(Console.ReadLine(), out c);
            switch (c)
            {
                case 1: AddStation(ibl); break;
                case 2: AddDrone(ibl); break;
                case 3: AddCustomer(ibl); break;
                case 4: AddParcel(ibl); break;
                default:
                    Console.WriteLine("error!");
                    Console.WriteLine("To add press 1");
                    Console.WriteLine("To update press 2");
                    Console.WriteLine("To view an entity, press 3");
                    Console.WriteLine("To view a list press 4");
                    Console.WriteLine("To exit press 5"); break;
            }
        }
        /// <summary>
        /// add station to the stations list
        /// </summary>
        public static void AddStation(IBL.IBL ibl)
        {
            int id, name, charge;
            double lng, lat;
            Console.WriteLine("enter id");
            int.TryParse(Console.ReadLine(), out id);
            Console.WriteLine("enter name(number)");
            int.TryParse(Console.ReadLine(), out name);
            Console.WriteLine("enter longitude between 35 to 35.2");
            double.TryParse(Console.ReadLine(), out lng);
            Console.WriteLine("enter lattitude between 31 to 31.2");
            double.TryParse(Console.ReadLine(), out lat);
            Console.WriteLine("enter available charge slots");
            int.TryParse(Console.ReadLine(), out charge);
            try
            {
                ibl.AddStation(new Station { Id = id, Name = name, LocationS = new Location { Longitude = lng, Lattitude = lat }, AvailableSlots = charge });
            }
            catch (InvalidInputException ex)
            {
                Console.WriteLine(ex);
            }
            catch (AlreadyExistsException ex)
            {
                Console.WriteLine(ex);
            }
        }
        /// <summary>
        /// add drone to the drones list
        /// </summary>
        public static void AddDrone(IBL.IBL ibl)
        {
            int id;
            string model;
            int maxWeight;
            int stationId;
            Console.WriteLine("enter id");
            int.TryParse(Console.ReadLine(), out id);
            Console.WriteLine("enter model");
            model = Console.ReadLine();
            Console.WriteLine("enter max weight: 0 for light, 1 for Medium and 2 for Heavy");
            int.TryParse(Console.ReadLine(), out maxWeight);
            Console.WriteLine("enter station number for initial charging");
            int.TryParse(Console.ReadLine(), out stationId);
            try
            {
                ibl.AddDrone(new Drone { Id = id, Model = model, MaxWeight = (WeightCategories)maxWeight }, stationId);
            }
            catch (NotFoundException ex)
            {
                Console.WriteLine(ex);
            }
            catch (InvalidInputException ex)
            {
                Console.WriteLine(ex);
            }
            catch (AlreadyExistsException ex)
            {
                Console.WriteLine(ex);
            }
        }
        /// <summary>
        /// add customer to the customers list
        /// </summary>
        public static void AddCustomer(IBL.IBL ibl)
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
            while (!phone.All(char.IsDigit)) //check if the phone that was entered was all digits
            {
                Console.WriteLine("phone should be digits only");
                Console.WriteLine("enter phone");
                phone = Console.ReadLine();
            }
            Console.WriteLine("enter longitude between 35 to 35.2");
            double.TryParse(Console.ReadLine(), out lng);
            Console.WriteLine("enter lattitude between 31 to 31.2");
            double.TryParse(Console.ReadLine(), out lat);
            try
            {
                ibl.AddCustomer(new Customer { Id = id, Name = name, Phone = phone, LocationC = new Location { Longitude = lng, Lattitude = lat } });
            }
            catch (InvalidInputException ex)
            {
                Console.WriteLine(ex);
            }
            catch (AlreadyExistsException ex)
            {
                Console.WriteLine(ex);
            }
        }
        // add parcel to the parecls list
        public static void AddParcel(IBL.IBL ibl)
        {
            int sender, target, weight, priority;
            Console.WriteLine("enter sender id");
            int.TryParse(Console.ReadLine(), out sender);
            Console.WriteLine("enter target id");
            int.TryParse(Console.ReadLine(), out target);
            Console.WriteLine("enter parcel's weight: 0 for light, 1 for Medium and 2 for Heavy");
            int.TryParse(Console.ReadLine(), out weight);
            Console.WriteLine("enter priority: 0 for Regular, 1 for Express and 2 for Urgent");
            int.TryParse(Console.ReadLine(), out priority);
            try
            {
                ibl.AddParcel(new Parcel { Sender = new CustomerInParcel { Id = sender }, Target = new CustomerInParcel { Id = target }, Weight = (WeightCategories)weight, Priority = (Priorities)priority });
            }
            catch (NotFoundException ex)
            {
                Console.WriteLine(ex);
            }
        }
        #endregion
        #region UPDATING
        /// <summary>
        /// ask the user which update to do
        /// </summary>
        public static void Updating(IBL.IBL ibl)
        {
            int c;
            Console.WriteLine("To update drone details press 1");
            Console.WriteLine("To update station details 2");
            Console.WriteLine("To update customer details press 3");
            Console.WriteLine("To send a drone for charging press 4");
            Console.WriteLine("To release drone from charging press 5");
            Console.WriteLine("To assign parcel to drone press 6");
            Console.WriteLine("To collect parcel by drone press 7");
            Console.WriteLine("To Delivery parcel to customer press 8");
            int.TryParse(Console.ReadLine(), out c);
            switch (c)
            {
                case 1: UpdateDrone(ibl); break;
                case 2: UpdateStation(ibl); break;
                case 3: UpdateCustomer(ibl); break;
                case 4: SendDrone(ibl); break;
                case 5: ReleaseDrone(ibl); break;
                case 6: ParcelToDrone(ibl); break;
                case 7: CollectParcel(ibl); break;
                case 8: DeliverParcel(ibl); break;
                default:
                    Console.WriteLine("error!");
                    Console.WriteLine("To add press 1");
                    Console.WriteLine("To update press 2");
                    Console.WriteLine("To view an entity, press 3");
                    Console.WriteLine("To view a list press 4");
                    Console.WriteLine("To exit press 5"); break;
            }
        }
        /// <summary>
        /// update the model of the drone
        /// </summary>
        public static void UpdateDrone(IBL.IBL ibl)
        {
            int droneId;
            string newModel;
            Console.WriteLine("enter drone id for updating");
            int.TryParse(Console.ReadLine(), out droneId);
            Console.WriteLine("enter the new model");
            newModel = Console.ReadLine();
            try
            {
                ibl.UpdateDrone(droneId, newModel);
            }
            catch (NotFoundException ex)
            {
                Console.WriteLine(ex);
            }
        }
        /// <summary>
        /// update the the station name or the station charging slots or both of them
        /// </summary>
        public static void UpdateStation(IBL.IBL ibl)
        {
            int stationId, newName, newChargeSlots;
            Console.WriteLine("enter station id for updating");
            int.TryParse(Console.ReadLine(), out stationId);
            Console.WriteLine("enter the new name(number) for updating (if you don't want to update enter -1)");
            int.TryParse(Console.ReadLine(), out newName);
            Console.WriteLine("enter the new charging slosts amount for updating (if you don't want to update enter -1)");
            int.TryParse(Console.ReadLine(), out newChargeSlots);
            try
            {
                ibl.UpdateStation(stationId, newName, newChargeSlots);
            }
            catch (NotFoundException ex)
            {
                Console.WriteLine(ex);
            }
            catch (InvalidInputException ex)
            {
                Console.WriteLine(ex);
            }
        }
        /// <summary>
        /// update the customer name or the customer phone or both of them
        /// </summary>
        public static void UpdateCustomer(IBL.IBL ibl)
        {
            int customerId;
            string newName, newPhone;
            Console.WriteLine("enter customer id for updating");
            int.TryParse(Console.ReadLine(), out customerId);
            Console.WriteLine("enter the new name for updating (if you don't want to update press enter)");
            newName = Console.ReadLine();
            Console.WriteLine("enter the new phone number for updating (if you don't want to update press enter)");
            newPhone = Console.ReadLine();
            while (!newPhone.All(char.IsDigit) && newPhone != "")
            {
                Console.WriteLine("phone should be digits only");
                Console.WriteLine("enter the new phone");
                newPhone = Console.ReadLine();
            }
            try
            {
                ibl.UpdateCustomer(customerId, newName, newPhone);
            }
            catch (NotFoundException ex)
            {
                Console.WriteLine(ex);
            }
            catch (InvalidInputException ex)
            {
                Console.WriteLine(ex);
            }
        }
        /// <summary>
        /// send drone to charge in station
        /// </summary>
        public static void SendDrone(IBL.IBL ibl)
        {
            int droneId;
            Console.WriteLine("enter drone id for charging");
            int.TryParse(Console.ReadLine(), out droneId);
            try
            {
                ibl.SendToCharge(droneId);
            }
            catch (NotFoundException ex)
            {
                Console.WriteLine(ex);
            }
            catch (DroneStatusException ex)
            {
                Console.WriteLine(ex);
            }
            catch (BatteryException ex)
            {
                Console.WriteLine(ex);
            }
            catch (EmptyListException ex)
            {
                Console.WriteLine(ex);
            }
        }
        /// <summary>
        /// release drone from charge station
        /// </summary>
        public static void ReleaseDrone(IBL.IBL ibl)
        {
            int droneId;
            double timeInCharging;
            Console.WriteLine("enter drone id for releasing");
            int.TryParse(Console.ReadLine(), out droneId);
            Console.WriteLine("enter time in charging (in hours)");
            double.TryParse(Console.ReadLine(), out timeInCharging);
            try
            {
                ibl.ReleaseDrone(droneId, timeInCharging);
            }
            catch (NotFoundException ex)
            {
                Console.WriteLine(ex);
            }
            catch (DroneStatusException ex)
            {
                Console.WriteLine(ex);
            }
        }
        /// <summary>
        /// assign parcel to drone
        /// </summary>
        public static void ParcelToDrone(IBL.IBL ibl)
        {
            int droneId;
            Console.WriteLine("enter drone for association");
            int.TryParse(Console.ReadLine(), out droneId);
            try
            {
                ibl.DroneToParcel(droneId);
            }
            catch (NotFoundException ex)
            {
                Console.WriteLine(ex);
            }
            catch (DroneStatusException ex)
            {
                Console.WriteLine(ex);
            }
            catch (EmptyListException ex)
            {
                Console.WriteLine(ex);
            }
        }
        /// <summary>
        /// pick up parcel by drone
        /// </summary>
        public static void CollectParcel(IBL.IBL ibl)
        {
            int id;
            Console.WriteLine("enter drone id for collecting parcel");
            int.TryParse(Console.ReadLine(), out id);
            try
            {
                ibl.PickParcel(id);
            }
            catch (NotFoundException ex)
            {
                Console.WriteLine(ex);
            }
            catch (DroneStatusException ex)
            {
                Console.WriteLine(ex);
            }
            catch (ParcelModeException ex)
            {
                Console.WriteLine(ex);
            }
        }
        /// <summary>
        /// deliever parcel to customer
        /// </summary>
        public static void DeliverParcel(IBL.IBL ibl)
        {
            int id;
            Console.WriteLine("enter drone id for delievering parcel");
            int.TryParse(Console.ReadLine(), out id);
            try
            {
                ibl.DeliverParcel(id);
            }
            catch (NotFoundException ex)
            {
                Console.WriteLine(ex);
            }
            catch (DroneStatusException ex)
            {
                Console.WriteLine(ex);
            }
            catch (ParcelModeException ex)
            {
                Console.WriteLine(ex);
            }
        }
        #endregion
        #region SHOWING
        /// <summary>
        /// ask the user which object to show
        /// </summary>
        public static void Showing(IBL.IBL ibl)
        {
            int c;
            Console.WriteLine("To view a station press 1");
            Console.WriteLine("To view a drone press 2");
            Console.WriteLine("To view a customer press 3");
            Console.WriteLine("To view a parcel press 4");
            int.TryParse(Console.ReadLine(), out c);
            switch (c)
            {
                case 1: viewStation(ibl); break;
                case 2: viewDrone(ibl); break;
                case 3: viewCustomer(ibl); break;
                case 4: viewParcel(ibl); break;
                default:
                    Console.WriteLine("error!");
                    Console.WriteLine("To add press 1");
                    Console.WriteLine("To update press 2");
                    Console.WriteLine("To view an entity, press 3");
                    Console.WriteLine("To view a list press 4");
                    Console.WriteLine("To exit press 5"); break;
            }
        }
        /// <summary>
        /// prints the requested station
        /// </summary>
        public static void viewStation(IBL.IBL ibl)
        {
            int id;
            Console.WriteLine("enter station id to show");
            int.TryParse(Console.ReadLine(), out id);
            try
            {
                Console.WriteLine(ibl.GetStation(id));
            }
            catch (NotFoundException ex)
            {
                Console.WriteLine(ex); ;
            }
        }
        /// <summary>
        /// Prints the requested drone
        /// </summary>
        public static void viewDrone(IBL.IBL ibl)
        {
            int id;
            Console.WriteLine("enter drone id to show");
            int.TryParse(Console.ReadLine(), out id);
            try
            {
                Console.WriteLine(ibl.GetDrone(id));
            }
            catch (NotFoundException ex)
            {
                Console.WriteLine(ex); ;
            }
        }
        /// <summary>
        /// Prints the requested customeer
        /// </summary>
        public static void viewCustomer(IBL.IBL ibl)
        {
            int id;
            Console.WriteLine("enter customer id to show");
            int.TryParse(Console.ReadLine(), out id);
            try
            {
                Console.WriteLine(ibl.GetCustomer(id));
            }
            catch (NotFoundException ex)
            {
                Console.WriteLine(ex); ;
            }
        }
        /// <summary>
        /// Prints the requested parcel
        /// </summary>
        public static void viewParcel(IBL.IBL ibl)
        {
            int id;
            Console.WriteLine("enter parcel id to show");
            int.TryParse(Console.ReadLine(), out id);
            try
            {
                Console.WriteLine(ibl.GetParcel(id));
            }
            catch (NotFoundException ex)
            {
                Console.WriteLine(ex); ;
            }
        }
        #endregion
        #region SHOWLIST
        /// <summary>
        /// ask the user which list to show
        /// </summary>
        public static void ShowList(IBL.IBL ibl)
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
                case 1: ListStations(ibl); break;
                case 2: ListDrones(ibl); break;
                case 3: ListCustomers(ibl); break;
                case 4: ListParcels(ibl); break;
                case 5: ListUnassociated(ibl); break;
                case 6: ListAvailable(ibl); break;
                default:   
                    Console.WriteLine("error!");
                    Console.WriteLine("To add press 1");
                    Console.WriteLine("To update press 2");
                    Console.WriteLine("To view an entity, press 3");
                    Console.WriteLine("To view a list press 4");
                    Console.WriteLine("To exit press 5"); break;
            }
        }
        /// <summary>
        /// prints the stations list
        /// </summary>
        public static void ListStations(IBL.IBL ibl)
        {
            IEnumerable<StationToList> stations = ibl.GetStationsList();
            foreach (StationToList s in stations)
            {
                Console.WriteLine(s);
            }
        }
        /// <summary>
        ///  prints the drones list
        /// </summary>
        public static void ListDrones(IBL.IBL ibl)
        {
            IEnumerable<DroneToList> drones = ibl.GetDronesList();
            foreach (DroneToList d in drones)
            {
                Console.WriteLine(d);
            }
        }
        /// <summary>
        ///  prints the customers list
        /// </summary>
        public static void ListCustomers(IBL.IBL ibl)
        {
            IEnumerable<CustomerToList> customers = ibl.GetCustomersList();
            foreach (CustomerToList c in customers)
            {
                Console.WriteLine(c);
            }
        }
        /// <summary>
        ///  prints the parcels list
        /// </summary>
        public static void ListParcels(IBL.IBL ibl)
        {
            IEnumerable<ParcelToList> parcels = ibl.GetParcelsList();
            foreach (ParcelToList p in parcels)
            {
                Console.WriteLine(p);
            }
        }
        /// <summary>
        /// prints the list of parcels that not associated yet with drone
        /// </summary>
        public static void ListUnassociated(IBL.IBL ibl)
        {
            IEnumerable<ParcelToList> parcels = ibl.UnassociatedParcel();
            foreach (ParcelToList p in parcels)
            {
                    Console.WriteLine(p);
            }
        }
        /// <summary>
        /// prints the base stations with available charging slots
        /// </summary>
        public static void ListAvailable(IBL.IBL ibl)
        {
            IEnumerable<StationToList> stations = ibl.AvailableStations(); 
            foreach (StationToList s in stations)
            {
                    Console.WriteLine(s);
            }
        }
        #endregion
    }
}