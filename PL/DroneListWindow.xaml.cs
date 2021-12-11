using IBL.BO;
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

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneListWindow.xaml
    /// </summary>
    public partial class DroneListWindow : Window
    {
        IBL.IBL bl;
        public ObservableCollection<DroneToList> droneTos;
        public DroneListWindow(IBL.IBL bL)
        {
            bl = bL;
            InitializeComponent();
            var drones = bl.GetDronesList();
            droneTos = new ObservableCollection<DroneToList>();
            //List<DroneToList> drones = bl.GetDronesList().ToList();
            foreach(var d in drones)
            {
                droneTos.Add(d);
            }

            DroneListView.ItemsSource = droneTos;
            StatusSelector.ItemsSource = Enum.GetValues(typeof(IBL.BO.DroneStatus));
            WeightSelector.ItemsSource = Enum.GetValues(typeof(IBL.BO.WeightCategories));
        }

        private void StatusSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IBL.BO.DroneStatus status = (IBL.BO.DroneStatus)StatusSelector.SelectedItem;
            DroneListView.ItemsSource = bl.StatusDrone(status);
        }

        private void WeightSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IBL.BO.WeightCategories weight = (IBL.BO.WeightCategories)WeightSelector.SelectedItem;
            DroneListView.ItemsSource = bl.WeightDrone(weight);
        }

        private void AddDrone_Click(object sender, RoutedEventArgs e)
        {
            new DroneWindow(bl).Show();
        }

        private void DroneListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DroneListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           new DroneWindow(bl,(IBL.BO.DroneToList)(this.DroneListView.SelectedItem)).Show();
        }
    }
}
