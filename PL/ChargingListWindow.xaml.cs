using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public bool ClosingWindow { get; private set; } = true;
        /// <summary>
        /// opening the drones in charge list
        /// </summary>
        /// <param name="bL">IBL interface</param>
        /// <param name="s">the station the drones is charged in</param>
        public ChargingListWindow(IBL bL, Station s)
        {
            bl = bL;
            station = s;
            InitializeComponent();
            ChargingListView.ItemsSource = s.DronesInCharging.ToList();
        }

        /// <summary>
        /// close window event. close window if  ClosingWindow=false
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = ClosingWindow;
        }
        /// <summary>
        /// click on exit button event
        /// </summary>
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            ClosingWindow = false;
            Close();
        }
        /// <summary>
        /// open chosen drone window that charging in this station
        /// </summary>
        private void ChargingListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ChargingListView.SelectedItem != null)
                new DroneWindow(bl, (DroneInCharging)ChargingListView.SelectedItem).Show();
        }
    }
}
