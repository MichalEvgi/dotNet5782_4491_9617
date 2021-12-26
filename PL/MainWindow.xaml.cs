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
        public MainWindow()
        {
            try
            {
                bL = BlFactory.GetBl();
                InitializeComponent();
            }
            catch(DalConfigException)
            {
            }
        }
        //open droneList window
        private void DroneListButton_Click(object sender, RoutedEventArgs e)
        {
            new DroneListWindow(bL).Show();
        }

        private void CustomerListBtn_Click(object sender, RoutedEventArgs e)
        {
            new CustomerListWindow(bL).Show();
        }

        private void StationListBtn_Click(object sender, RoutedEventArgs e)
        {
            new StationListWindow(bL).Show();
        }

        private void ParcelListBtn_Click(object sender, RoutedEventArgs e)
        {
            new ParcelListWindow(bL).Show();
        }
    }
}
