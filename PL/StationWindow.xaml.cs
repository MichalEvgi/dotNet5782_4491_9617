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
using System.Windows.Shapes;
using BlApi;
using BO;

namespace PL
{
    /// <summary>
    /// Interaction logic for StationWindow.xaml
    /// </summary>
    public partial class StationWindow : Window
    {
        IBL bl;
        private StationListWindow st;
        public StationWindow(IBL bL, StationListWindow slw)
        {
            bl = bL;
            st = slw;
            InitializeComponent();
            addStaion.Visibility = Visibility.Visible;
            actions.Visibility = Visibility.Hidden;
        }
        //close window by cancel button
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

        }
        public Station selectedStation;
        public StationWindow(IBL bL, StationToList station, StationListWindow dlw)
        {
            bl = bL;
            InitializeComponent();
            selectedStation = bl.GetStation(station.Id);
            DataContext = selectedStation;
            addStaion.Visibility = Visibility.Hidden;
            actions.Visibility = Visibility.Visible;
        }

        private void Exitbt_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Updatebt_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
