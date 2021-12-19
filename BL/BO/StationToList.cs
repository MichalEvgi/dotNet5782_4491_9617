using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class StationToList
    {
        /// <summary>
        /// properties
        /// </summary>
        public int Id { get; set; }
        public int Name { get; set; }
        public int AvailableSlots { get; set; }
        public int FullSlots { get; set; }
        /// <summary>
        /// to string
        /// </summary>
        public override string ToString()
        {
            return ToolStringClass.ToStringProperty(this);
        }
    }
}