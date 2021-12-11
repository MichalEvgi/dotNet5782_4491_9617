using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    namespace BO
    {
        public class DroneToList : INotifyPropertyChanged
        {
            /// <summary>
            /// properties
            /// </summary>
            public int Id { get; set; }
            public string Model { get; set; } 
            public WeightCategories MaxWeight { get; set; }
            public double Battery { get; set; }
            public DroneStatus Status { get; set; }
            public Location CurrentLocation { get; set; }
            public int ParcelId { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            public void NotifyPropertyChanged(string propName)
            {
                if (this.PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }

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
