using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BlApi;
using BO;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IBL bL;
        /// <summary>
        /// open the maneger main window
        /// </summary>
        /// <param name="bl">IBL interface</param>
        public MainWindow(IBL bl)
        {
            try
            {
                bL = bl;
                InitializeComponent();
                
            }
            catch(DalConfigException)
            {
            }
        }
        /// <summary>
        /// open droneList window
        /// </summary>
        private void DroneListButton_Click(object sender, RoutedEventArgs e)
        {
            new DroneListWindow(bL).Show();
        }
        /// <summary>
        /// open customer list window
        /// </summary>
        private void CustomerListBtn_Click(object sender, RoutedEventArgs e)
        {
            new CustomerListWindow(bL).Show();
        }
        /// <summary>
        /// open station list window
        /// </summary>
        private void StationListBtn_Click(object sender, RoutedEventArgs e)
        {
            new StationListWindow(bL).Show();
        }
        /// <summary>
        /// open parcel list window
        /// </summary>
        private void ParcelListBtn_Click(object sender, RoutedEventArgs e)
        {
            new ParcelListWindow(bL).Show();
        }
    }
}
