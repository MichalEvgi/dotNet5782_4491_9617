using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using BlApi;
using BO;

namespace PL
{
    /// <summary>
    /// Interaction logic for ChargingListWindow.xaml
    /// </summary>
    public partial class ChargingListWindow : Window
    {
        IBL bl;
        Station station;
        public ChargingListWindow(IBL bL, Station s)
        {
            bl = bL;
            station = s;
            InitializeComponent();
            ChargingListView.ItemsSource = s.DronesInCharging.ToList();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ChargingListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            new DroneWindow(bl, (DroneInCharging)ChargingListView.SelectedItem).Show();
        }
    }
}
