﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IBL
{
    namespace BO
    {
        public class Customer
        {
            /// <summary>
            /// properties
            /// </summary>
            public int Id { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public Location LocationC { get; set; }
            public IEnumerable<ParcelInCustomer> FromCustomer { get; set; }
            public IEnumerable<ParcelInCustomer> ToCustomer { get; set; }
            /// <summary>
            /// to string
            /// </summary>
            public override string ToString()
            {
                return ToolStringClass.ToStringProperty(this);
            }

        }
    }
}
