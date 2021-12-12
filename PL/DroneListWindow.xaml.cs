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
            foreach(var d in drones)
            {
                droneTos.Add(d);
            }

            DroneListView.ItemsSource = droneTos;
            foreach (DroneStatus ds in (DroneStatus[])Enum.GetValues(typeof(DroneStatus)))
            {
                StatusSelector.Items.Add(ds);
            }
            StatusSelector.Items.Add("Clear filter");
            foreach (WeightCategories wc in (WeightCategories[])Enum.GetValues(typeof(WeightCategories)))
            {
                WeightSelector.Items.Add(wc);
            }
            WeightSelector.Items.Add("Clear filter");
        }

        private void StatusSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StatusSelector.SelectedItem.ToString() == "Clear filter")
            {
                if (WeightSelector.SelectedItem != null && WeightSelector.SelectedItem.ToString() != "Clear filter")

                    DroneListView.ItemsSource = bl.WeightDrone((WeightCategories)WeightSelector.SelectedItem);
                else

                    DroneListView.ItemsSource = bl.GetDronesList();
            }
            else if (WeightSelector.SelectedItem != null && WeightSelector.SelectedItem.ToString() != "Clear filter")
            {
                DroneListView.ItemsSource = bl.StatusAndWeight((DroneStatus)StatusSelector.SelectedItem, (WeightCategories)WeightSelector.SelectedItem);
            }
            else
            {
                DroneListView.ItemsSource = bl.StatusDrone((DroneStatus)StatusSelector.SelectedItem);
            }
   
        }

        private void WeightSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WeightSelector.SelectedItem.ToString() == "Clear filter")
            {
                if (StatusSelector.SelectedItem != null && StatusSelector.SelectedItem.ToString() != "Clear filter")
                    DroneListView.ItemsSource = bl.StatusDrone((DroneStatus)StatusSelector.SelectedItem);
                else
                    DroneListView.ItemsSource = bl.GetDronesList();
            }
            else if(StatusSelector.SelectedItem!=null&& StatusSelector.SelectedItem.ToString() != "Clear filter")
            {
                DroneListView.ItemsSource = bl.StatusAndWeight((DroneStatus)StatusSelector.SelectedItem, (WeightCategories)WeightSelector.SelectedItem);
            }
            else
            {
                DroneListView.ItemsSource = bl.WeightDrone((WeightCategories)WeightSelector.SelectedItem);
            }

            
        }

        private void AddDrone_Click(object sender, RoutedEventArgs e)
        {
            new DroneWindow(bl,droneTos).Show();
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
