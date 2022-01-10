using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using DalApi;
using DO;

namespace Dal
{
    sealed class DalXml: IDal
    {
        #region INITIALIZE
        #region Singelton
        private static DalXml instance = null;
        private static readonly object padlock = new object();

        public static DalXml Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new DalXml();
                        }
                    }
                }
                return instance;
            }
        }
        #endregion
        public DalXml()
        {
            List<DroneCharge> chargingList = XMLTools.LoadListFromXMLSerializer<DroneCharge>(droneChargePath);
            foreach (DroneCharge d in chargingList)
            {
                ReleaseDrone(d.DroneId);
            }
            chargingList.Clear();
            XMLTools.SaveToXMLSerializer(chargingList, droneChargePath);
        }
        string stationPath = @"StationXml.xml";//XMLSerializer
        string dronePath = @"DroneXml.xml";//XMLSerializer
        string parcelPath = @"ParcelXml.xml";//XMLSerializer
        string customerPath = @"CustomerXml.xml";//XElement
        string droneChargePath = @"DroneChargeXml.xml";//XMLSerializer
        string configPath = @"config.xml";//XElement
        #endregion
        #region STATION
        /// <summary>
        /// add station to the list of stations
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddStation(Station s)
        {
            List<Station> stationList = XMLTools.LoadListFromXMLSerializer<Station>(stationPath);
            if (stationList.Exists(x => x.Id == s.Id))
                throw new AlreadyExistsException("station");
            stationList.Add(s);
            XMLTools.SaveToXMLSerializer<Station>(stationList, stationPath);
        }
        /// <summary>
        /// delete station by id
        /// </summary>
        /// <param name="id"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteStation(int id)
        {
            List<Station> stationList = XMLTools.LoadListFromXMLSerializer<Station>(stationPath);
            if (!stationList.Exists(stat => stat.Id == id))
                throw new NotFoundException("station");
            Station temp = GetStationById(id);
            stationList.Remove(temp);
            XMLTools.SaveToXMLSerializer<Station>(stationList, stationPath);
        }
        /// <summary>
        /// return the description of a specific station
        /// </summary>
        /// <param name="id">station's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public string ShowStation(int id)
        {
            if (!XMLTools.LoadListFromXMLSerializer<Station>(stationPath).Exists(stat => stat.Id == id))
                throw new NotFoundException("station");
            return GetStationById(id).ToString();
        }
        /// <summary>
        /// return list of stations
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Station> PrintStations() => XMLTools.LoadListFromXMLSerializer<Station>(stationPath).Select(s=>s);
        /// <summary>
        /// return all the stations with specific condition
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Station> FilteredStations(Predicate<Station> predi)
        {
            return from Station s in XMLTools.LoadListFromXMLSerializer<Station>(stationPath)
                   where predi(s)
                   select s;
        }
        /// <summary>
        /// return if the station exists in stations or doesn't
        /// </summary>
        /// <param name="id">station's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool ExistStation(int id) => XMLTools.LoadListFromXMLSerializer<Station>(stationPath).Exists(s => s.Id == id);
        /// <summary>
        /// return station by id
        /// </summary>
        /// <param name="id">station's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Station GetStationById(int id) => XMLTools.LoadListFromXMLSerializer<Station>(stationPath).Find(s => s.Id == id);
        /// <summary>
        /// return how many full slots in the station
        /// </summary>
        /// <param name="stationId">station id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int FullSlots(int stationId)
        {
            if (!XMLTools.LoadListFromXMLSerializer<Station>(stationPath).Exists(stat => stat.Id == stationId))
                throw new NotFoundException("station");
            int count = 0;
            foreach (var d in XMLTools.LoadListFromXMLSerializer<DroneCharge>(droneChargePath))
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
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<int> DroneInChargeIds(int stationId)
        {
            if (!XMLTools.LoadListFromXMLSerializer<Station>(stationPath).Exists(stat => stat.Id == stationId))
                throw new NotFoundException("station");
            return from DroneCharge d in XMLTools.LoadListFromXMLSerializer<DroneCharge>(droneChargePath)
                   where d.StationId == stationId
                   select d.DroneId;
        }
        #endregion
        #region DRONE
        /// <summary>
        /// add drone to list of drones
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddDrone(Drone d)
        {
            List<Drone> droneList = XMLTools.LoadListFromXMLSerializer<Drone>(dronePath);
            if (droneList.Exists(x => x.Id == d.Id))
                throw new AlreadyExistsException("drone");
            droneList.Add(d);
            XMLTools.SaveToXMLSerializer<Drone>(droneList, dronePath);
        }
        /// <summary>
        /// delete drone by id
        /// </summary>
        /// <param name="id"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteDrone(int id)
        {
            List<Drone> droneList = XMLTools.LoadListFromXMLSerializer<Drone>(dronePath);
            if (!droneList.Exists(d => d.Id == id))
                throw new NotFoundException("drone");
            Drone temp = GetDroneById(id);
            droneList.Remove(temp);
            XMLTools.SaveToXMLSerializer(droneList, dronePath);
        }
        /// <summary>
        /// return the description of a specific drone
        /// </summary>
        /// <param name="id">drone's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public string ShowDrone(int id)
        {
            if (!XMLTools.LoadListFromXMLSerializer<Drone>(dronePath).Exists(dron => dron.Id == id))
                throw new NotFoundException("drone");
            return GetDroneById(id).ToString();
        }
        /// <summary>
        /// return list of drones
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Drone> PrintDrones() => XMLTools.LoadListFromXMLSerializer<Drone>(dronePath).Select(d=>d);
        /// <summary>
        /// return drone by id
        /// </summary>
        /// <param name="id">drone's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Drone GetDroneById(int id) => XMLTools.LoadListFromXMLSerializer<Drone>(dronePath).Find(d => d.Id == id);
        /// <summary>
        /// return if the drone exists in drones or doesn't
        /// </summary>
        /// <param name="id">drone's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool ExistDrone(int id) => XMLTools.LoadListFromXMLSerializer<Drone>(dronePath).Exists(d => d.Id == id);
        /// <summary>
        /// send drone to charge in station
        /// </summary>
        /// <param name="droneId">drone's id</param>
        /// <param name="stationId">station's id</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SendToCharge(int droneId, int stationId)
        {
            if (!XMLTools.LoadListFromXMLSerializer<Drone>(dronePath).Exists(dron => dron.Id == droneId))
                throw new NotFoundException("drone");
            List<Station> stationList = XMLTools.LoadListFromXMLSerializer<Station>(stationPath);
            List<DroneCharge> droneChargeList = XMLTools.LoadListFromXMLSerializer<DroneCharge>(droneChargePath);
            if (!stationList.Exists(stat => stat.Id == stationId))
                throw new NotFoundException("station");
            Station tempS = GetStationById(stationId);
            //save station in temp
            stationList.Remove(tempS);
            //remove
            tempS.ChargeSlots--;
            //update
            stationList.Add(tempS);
            //add station back
           droneChargeList.Add(new DroneCharge { DroneId = droneId, StationId = stationId, EntryTime = DateTime.Now }); //add drone charge
            XMLTools.SaveToXMLSerializer(stationList, stationPath);
            XMLTools.SaveToXMLSerializer(droneChargeList, droneChargePath);
        }
        /// <summary>
        /// release drone from station
        /// </summary>
        /// <param name="droneId">drone's id</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public double ReleaseDrone(int droneId)
        {
            if (!XMLTools.LoadListFromXMLSerializer<Drone>(dronePath).Exists(dron => dron.Id == droneId))
                throw new NotFoundException("drone");
            List<Station> stationList = XMLTools.LoadListFromXMLSerializer<Station>(stationPath);
            List<DroneCharge> droneChargeList = XMLTools.LoadListFromXMLSerializer<DroneCharge>(droneChargePath);
            DroneCharge temp = GetDroneChargeById(droneId);
            double timeCharge = (DateTime.Now - temp.EntryTime).TotalSeconds;
            Station tempS = GetStationById(temp.StationId);
            //save drone charge and station in temps
            stationList.Remove(tempS);
            droneChargeList.Remove(temp);
            //remove+ remove drone charge
            tempS.ChargeSlots++;
            //update
            stationList.Add(tempS);
            //add station back
            XMLTools.SaveToXMLSerializer(stationList, stationPath);
            XMLTools.SaveToXMLSerializer(droneChargeList, droneChargePath);
            return timeCharge;
        }
        /// <summary>
        /// return drone charge by drone's id
        /// </summary>
        /// <param name="id">drone's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DroneCharge GetDroneChargeById(int id) => XMLTools.LoadListFromXMLSerializer<DroneCharge>(droneChargePath).Find(dc => dc.DroneId == id);
        #endregion
        #region CUSTOMER
        /// <summary>
        /// add customer to the list of customer
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddCustomer(Customer c)
        {
            XElement customerRoot = XMLTools.LoadListFromXMLElement(customerPath);
            var customerElem = (from customer in customerRoot.Elements()
                                  where customer.Element("Id").Value == c.Id.ToString()
                                  select customer).FirstOrDefault();
            if (customerElem != null)
                throw new AlreadyExistsException("customer");
            XElement newcustomerElem = new XElement("Customer"
                , new XElement("Id", c.Id),
                new XElement("Name", c.Name),
                new XElement("Phone", c.Phone),
                new XElement("Longitude", c.Longitude),
                new XElement("Lattitude", c.Lattitude));
            customerRoot.Add(newcustomerElem);
            XMLTools.SaveListToXMLElement(customerRoot, customerPath);
        }
        /// <summary>
        /// delete customer by id
        /// </summary>
        /// <param name="id"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteCustomer(int id)
        {
            XElement customerRoot = XMLTools.LoadListFromXMLElement(customerPath);
            var customerElem = (from customer in customerRoot.Elements()
                                where customer.Element("Id").Value == id.ToString()
                                select customer).FirstOrDefault();
            if (customerElem == null)
                throw new NotFoundException("customer");

            customerElem.Remove();
            XMLTools.SaveListToXMLElement(customerRoot, customerPath);
        }
        /// <summary>
        /// return the description of a specific customer
        /// </summary>
        /// <param name="id">customer's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public string ShowCustomer(int id)
        {
            XElement customerRoot = XMLTools.LoadListFromXMLElement(customerPath);
            var customerElem = (from customer in customerRoot.Elements()
                                where customer.Element("Id").Value == id.ToString()
                                select customer).FirstOrDefault();
            if (customerElem == null)
                throw new NotFoundException("customer");
            return new Customer
            {
                Id = Convert.ToInt32(customerElem.Element("Id").Value),
                Name = (customerElem.Element("Name").Value),
                Phone = (customerElem.Element("Phone").Value),
                Longitude = Convert.ToDouble(customerElem.Element("Longitude").Value),
                Lattitude = Convert.ToDouble(customerElem.Element("Lattitude").Value)
            }.ToString();
        }
        /// <summary>
        /// return list of customers
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Customer> PrintCustomers()
        {
            XElement customerRoot = XMLTools.LoadListFromXMLElement(customerPath);
            var allCustomers = from customer in customerRoot.Elements()
                                select new Customer
                                       {
                                    Id = Convert.ToInt32(customer.Element("Id").Value),
                                    Name = (customer.Element("Name").Value),
                                    Phone = (customer.Element("Phone").Value),
                                    Longitude = Convert.ToDouble(customer.Element("Longitude").Value),
                                    Lattitude = Convert.ToDouble(customer.Element("Lattitude").Value)
                                };
            return allCustomers;
        }
        /// <summary>
        /// return if the customer exists in customers or doesn't
        /// </summary>
        /// <param name="id">customer's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool ExistCustomer(int id)
        {
            XElement customerRoot = XMLTools.LoadListFromXMLElement(customerPath);
            var customerElem = (from customer in customerRoot.Elements()
                                where customer.Element("Id").Value == id.ToString()
                                select customer).FirstOrDefault();
            return customerElem != null;
        }
        /// <summary>
        /// return customer by id
        /// </summary>
        /// <param name="id">customeer's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Customer GetCustomerById(int id)
        {
            XElement customerRoot = XMLTools.LoadListFromXMLElement(customerPath);
            var customerElem = (from customer in customerRoot.Elements()
                                where customer.Element("Id").Value == id.ToString()
                                select customer).FirstOrDefault();
            if (customerElem == null)
                throw new NotFoundException("customer");
            return new Customer
            {
                Id = Convert.ToInt32(customerElem.Element("Id").Value),
                Name = customerElem.Element("Name").Value,
                Phone = customerElem.Element("Phone").Value,
                Longitude = Convert.ToDouble(customerElem.Element("Longitude").Value),
                Lattitude = Convert.ToDouble(customerElem.Element("Lattitude").Value)
            };
        }
        #endregion
        #region PARCEL
        /// <summary>
        /// add parcel to the list of parcels
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddParcel(Parcel p)
        {
            List<Parcel> parcelList = XMLTools.LoadListFromXMLSerializer<Parcel>(parcelPath);
            XElement configRoot= XMLTools.LoadListFromXMLElement(configPath);
            int index = Convert.ToInt32(configRoot.Descendants("runIndex").First().Value);
            p.Id = index;
            index++;
            configRoot.Descendants("runIndex").First().Value = index.ToString();
            XMLTools.SaveListToXMLElement(configRoot, configPath);
            parcelList.Add(p);
            XMLTools.SaveToXMLSerializer(parcelList, parcelPath);
        }
        /// <summary>
        /// delete parcel by id
        /// </summary>
        /// <param name="id"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteParcel(int id)
        {
            List<Parcel> parcelList = XMLTools.LoadListFromXMLSerializer<Parcel>(parcelPath);
            if (!parcelList.Exists(p => p.Id == id))
                throw new NotFoundException("parcel");
            Parcel temp = GetParcelById(id);
            parcelList.Remove(temp);
            XMLTools.SaveToXMLSerializer(parcelList, parcelPath);
        }
        /// <summary>
        /// return the description of a specific parcel
        /// </summary>
        /// <param name="id">parcel's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public string ShowParcel(int id)
        {
            if (!XMLTools.LoadListFromXMLSerializer<Parcel>(parcelPath).Exists(parc => parc.Id == id))
                throw new NotFoundException("parcel");
            return GetParcelById(id).ToString();
        }
        /// <summary>
        /// return list of parcels
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Parcel> PrintParcels() => XMLTools.LoadListFromXMLSerializer<Parcel>(parcelPath).Select(p=>p);
        /// <summary>
        /// return the list of parcels with specific filter
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Parcel> FilteredParcel(Predicate<Parcel> predi)
        {
            return from Parcel p in XMLTools.LoadListFromXMLSerializer<Parcel>(parcelPath)
                   where predi(p)
                   select p;
        }
        /// <summary>
        /// return if the parcel exists in parcels or doesn't
        /// </summary>
        /// <param name="id">parcel's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool ExistParcel(int id) => XMLTools.LoadListFromXMLSerializer<Parcel>(parcelPath).Exists(p => p.Id == id);
        /// <summary>
        /// return parcel by id
        /// </summary>
        /// <param name="id">parcel's id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Parcel GetParcelById(int id) => XMLTools.LoadListFromXMLSerializer<Parcel>(parcelPath).Find(p => p.Id == id);
        /// <summary>
        /// return the transfered parcel with the droneId
        /// </summary>
        /// <param name="droneId"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Parcel GetTransferedParcel(int droneId)
        {
            if (!XMLTools.LoadListFromXMLSerializer<Drone>(dronePath).Exists(dron => dron.Id == droneId))
                throw new NotFoundException("drone");
            foreach (Parcel p in XMLTools.LoadListFromXMLSerializer<Parcel>(parcelPath))
            {
                if (p.DroneId == droneId && p.Delivered == null)
                    return p;
            }
            throw new NotFoundException("parcel"); //parcel not found
        }
        /// <summary>
        /// assign drone to parcel
        /// </summary>
        /// <param name="id"> parcel's id</param>
        /// <param name="droneId"> drone's id</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DroneToParcel(int id, int droneId)
        {
            List<Parcel> parcelList = XMLTools.LoadListFromXMLSerializer<Parcel>(parcelPath);
            if (!parcelList.Exists(par => par.Id == id))
                throw new NotFoundException("parcel");
            if (!XMLTools.LoadListFromXMLSerializer<Drone>(dronePath).Exists(dron => dron.Id == droneId))
                throw new NotFoundException("drone");
            Parcel temp = GetParcelById(id); //save parcel in temp
            parcelList.Remove(temp); //remove
            temp.DroneId = droneId;
            temp.Scheduled = DateTime.Now;
            //update
           parcelList.Add(temp); //add parcel back
           XMLTools.SaveToXMLSerializer(parcelList, parcelPath);
        }
        /// <summary>
        /// pick parcel by drone
        /// </summary>
        /// <param name="id">parcel's id</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PickParcel(int id)
        {
            List<Parcel> parcelList = XMLTools.LoadListFromXMLSerializer<Parcel>(parcelPath);
            if (!parcelList.Exists(par => par.Id == id))
                throw new NotFoundException("parcel");
            Parcel temp = GetParcelById(id);
            //save parcel in temp
           parcelList.Remove(temp);
            //remove
            temp.PickedUp = DateTime.Now;
            //update
            parcelList.Add(temp);
            //add parcel back
            XMLTools.SaveToXMLSerializer(parcelList, parcelPath);
        }
        /// <summary>
        /// deliver parcel to customer
        /// </summary>
        /// <param name="id">parcel's id</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeliverParcel(int id)
        {
            List<Parcel> parcelList = XMLTools.LoadListFromXMLSerializer<Parcel>(parcelPath);
            if (!parcelList.Exists(par => par.Id == id))
                throw new NotFoundException("parcel");
            Parcel temp = GetParcelById(id);
            //save parcel in temp
            parcelList.Remove(temp);
            //remove
            temp.Delivered = DateTime.Now;
            //update
            parcelList.Add(temp);
            //add parcel back
            XMLTools.SaveToXMLSerializer(parcelList, parcelPath);
        }
        #endregion
        #region HELP METHODS
        /// <summary>
        /// create string of sexagesimal lattitude
        /// </summary>
        /// <param name="lat">lattitude</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
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
        [MethodImpl(MethodImplOptions.Synchronized)]
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
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<double> ElectricityRequest()
        {
            XElement configRoot = XMLTools.LoadListFromXMLElement(configPath);
            double[] arr = new double[5];
            arr[0] = Convert.ToDouble(configRoot.Descendants("Available").First().Value);
            arr[1] = Convert.ToDouble(configRoot.Descendants("LightWeight").First().Value);
            arr[2] = Convert.ToDouble(configRoot.Descendants("MediumWeight").First().Value); 
            arr[3] = Convert.ToDouble(configRoot.Descendants("HeavyWeight").First().Value); 
            arr[4] = Convert.ToDouble(configRoot.Descendants("ChargingRate").First().Value); 
            return arr;
        }
        /// <summary>
        /// calculate the distance between two points of longitude and lattitude
        /// </summary>
        /// <param name="lon1"></param>
        /// <param name="lat1"></param>
        /// <param name="lon2"></param>
        /// <param name="lat2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public double Distance(double lon1, double lat1, double lon2, double lat2)
        {
            var p = 0.017453292519943295;    // Math.PI / 180
            var a = 0.5 - Math.Cos((lat2 - lat1) * p) / 2 +
                    Math.Cos(lat1 * p) * Math.Cos(lat2 * p) *
                    (1 - Math.Cos((lon2 - lon1) * p)) / 2;

            return 12742 * Math.Asin(Math.Sqrt(a)); // 2 * R; R = 6371 km
        }
        #endregion
    }
}
