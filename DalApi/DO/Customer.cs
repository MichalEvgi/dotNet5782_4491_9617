using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DalApi;
using DO;


namespace DO
{
    public struct Customer
    {
        /// <summary>
        /// properties
        /// </summary>
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public double Longitude { get; set; }
        public double Lattitude { get; set; }
        /// <summary>
        /// to string
        /// </summary>
        public override string ToString()
        {
            return "Customer: Id:" + Id + " Name:" + Name + " Phone:" + Phone + " Longitude:" + DalApi.IDal.Lng(Longitude) + " Lattitude:" + DalApi.IDal.Lat(Lattitude);
        }

    }
}

