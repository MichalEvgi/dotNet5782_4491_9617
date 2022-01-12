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
using BO;
using BlApi;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneListWindow.xaml
    /// </summary>
    public partial class DroneListWindow : Window
    {
        IBL bl;
        
        public DroneListWindow(IBL bL)
        {
            bl = bL;
            InitializeComponent();
            DroneListView.ItemsSource = bl.GetDronesList();
            //set the items of StatusSelector to DroneStatus
            
            foreach (DroneStatus ds in (DroneStatus[])Enum.GetValues(typeof(DroneStatus)))
            {
                StatusSelector.Items.Add(ds);
            }
            //add option of clear filter to StatusSelector
            StatusSelector.Items.Add("Clear filter");
            //set the items of WeightSelector to WeightCategories
            foreach (WeightCategories wc in (WeightCategories[])Enum.GetValues(typeof(WeightCategories)))
            {
                WeightSelector.Items.Add(wc);
            }
            //add option of clear filter to WeightSelector
            WeightSelector.Items.Add("Clear filter");
        }
        private void DroneListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectorChanges();
        }
        //private void DroneListView_CollectionChanged(object sender, SelectionChangedEventArgs e)
        //{

        //}
        public void SelectorChanges()
        {
            if((StatusSelector.SelectedItem==null||StatusSelector.SelectedItem.ToString() == "Clear filter") && (WeightSelector.SelectedItem==null||WeightSelector.SelectedItem.ToString() == "Clear filter"))
            {
                //if both fiter aren't set
                DroneListView.ItemsSource = bl.GetDronesList().Select(x => x);
            }
            else
            {
                if (StatusSelector.SelectedItem == null || StatusSelector.SelectedItem.ToString() == "Clear filter")
                    //if there is only weight filter
                    DroneListView.ItemsSource =bl.GetDronesList().Where(d => d.MaxWeight == (WeightCategories)WeightSelector.SelectedItem);
                else
                {
                    if(WeightSelector.SelectedItem == null || WeightSelector.SelectedItem.ToString() == "Clear filter")
                        //if there is only status filter
                        DroneListView.ItemsSource = bl.GetDronesList().Where(d => d.Status == (DroneStatus)StatusSelector.SelectedItem);
                    else
                        //if the two filter were selected
                        DroneListView.ItemsSource = bl.GetDronesList().Where(d => (d.MaxWeight == (WeightCategories)WeightSelector.SelectedItem) && (d.Status == (DroneStatus)StatusSelector.SelectedItem));
                }
            }
        }

        private void StatusSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StatusSelector.SelectedItem.ToString() == "Clear filter")
            {
                if (WeightSelector.SelectedItem != null && WeightSelector.SelectedItem.ToString() != "Clear filter")
                    //only WeightCategories was selected
                    DroneListView.ItemsSource = bl.WeightDrone((WeightCategories)WeightSelector.SelectedItem);
                else
                    //no filter was selected
                    DroneListView.ItemsSource = bl.GetDronesList();
            }
            else if (WeightSelector.SelectedItem != null && WeightSelector.SelectedItem.ToString() != "Clear filter")
            {
                //both filter were selected
                DroneListView.ItemsSource = bl.StatusAndWeight((DroneStatus)StatusSelector.SelectedItem, (WeightCategories)WeightSelector.SelectedItem);
            }
            else
            {
                //only drone status was selected
                DroneListView.ItemsSource = bl.StatusDrone((DroneStatus)StatusSelector.SelectedItem);
            }
   
        }

        private void WeightSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WeightSelector.SelectedItem.ToString() == "Clear filter")
            {
                if (StatusSelector.SelectedItem != null && StatusSelector.SelectedItem.ToString() != "Clear filter")
                    //only drone status was selected
                    DroneListView.ItemsSource = bl.StatusDrone((DroneStatus)StatusSelector.SelectedItem);
                else
                    //no filter was selected
                    DroneListView.ItemsSource = bl.GetDronesList();
            }
            else if(StatusSelector.SelectedItem!=null&& StatusSelector.SelectedItem.ToString() != "Clear filter")
            {
                //both filter were selected
                DroneListView.ItemsSource = bl.StatusAndWeight((DroneStatus)StatusSelector.SelectedItem, (WeightCategories)WeightSelector.SelectedItem);
            }
            else
            {
                //only WeightCategories was selected
                DroneListView.ItemsSource = bl.WeightDrone((WeightCategories)WeightSelector.SelectedItem);
            }

            
        }
        //open drone window with add drone state
        private void AddDrone_Click(object sender, RoutedEventArgs e)
        {
            new DroneWindow(bl,this).Show();
        }
        //close window
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //open drone window with action state
        private void DroneListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           new DroneWindow(bl,(DroneToList)(this.DroneListView.SelectedItem),this).Show();
        }
    }
}
