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
using System.ComponentModel;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneListWindow.xaml
    /// </summary>
    public partial class DroneListWindow : Window
    {
        IBL bl;
        public bool ClosingWindow { get; private set; } = true;
        /// <summary>
        /// open drones list window
        /// </summary>
        /// <param name="bL">IBL interface</param>
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
        /// <summary>
        /// refresh the drone list view
        /// </summary>
        public void SelectorChanges()
        {
            if ((StatusSelector.SelectedItem == null || StatusSelector.SelectedItem.ToString() == "Clear filter") && (WeightSelector.SelectedItem == null || WeightSelector.SelectedItem.ToString() == "Clear filter"))
            {
                //if both fiter aren't set
                DroneListView.ItemsSource = bl.GetDronesList().Select(x => x);
            }
            else
            {
                if (StatusSelector.SelectedItem == null || StatusSelector.SelectedItem.ToString() == "Clear filter")
                    //if there is only weight filter
                    DroneListView.ItemsSource = bl.GetDronesList().Where(d => d.MaxWeight == (WeightCategories)WeightSelector.SelectedItem);
                else
                {
                    if (WeightSelector.SelectedItem == null || WeightSelector.SelectedItem.ToString() == "Clear filter")
                        //if there is only status filter
                        DroneListView.ItemsSource = bl.GetDronesList().Where(d => d.Status == (DroneStatus)StatusSelector.SelectedItem);
                    else
                        //if the two filter were selected
                        DroneListView.ItemsSource = bl.GetDronesList().Where(d => (d.MaxWeight == (WeightCategories)WeightSelector.SelectedItem) && (d.Status == (DroneStatus)StatusSelector.SelectedItem));
                }
            }
        }
        /// <summary>
        /// drone status selected
        /// </summary>
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
        /// <summary>
        /// drone max weight selected
        /// </summary>
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
            else if (StatusSelector.SelectedItem != null && StatusSelector.SelectedItem.ToString() != "Clear filter")
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
        /// <summary>
        /// open drone window with add drone state
        /// </summary>
        private void AddDrone_Click(object sender, RoutedEventArgs e)
        {
            new DroneWindow(bl, this).Show();
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
            this.Close();
        }
        /// <summary>
        ///open drone window with action state
        /// </summary>
        private void DroneListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DroneListView.SelectedItem != null)
                new DroneWindow(bl, (DroneToList)(DroneListView.SelectedItem), this).Show();
        }
        /// <summary>
        /// group the list by status
        /// </summary>
        private void StatusGroup_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<IGrouping<DroneStatus, DroneToList>> droneGroup = from dro in bl.GetDronesList() group dro by dro.Status;
            List<DroneToList> droneTos = new();

            foreach (var group in droneGroup)
            {
                foreach (var dro in group)
                {
                    droneTos.Add(dro);
                }
            }
            DroneListView.ItemsSource = droneTos;


            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(DroneListView.ItemsSource);
            if (view.GroupDescriptions.Count < 1)
            {
                PropertyGroupDescription groupDescription = new PropertyGroupDescription("Status");
                view.GroupDescriptions.Add(groupDescription);
            }
        }
        /// <summary>
        /// refresh grouping
        /// </summary>
        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(DroneListView.ItemsSource);
            view.GroupDescriptions.Clear();
            SelectorChanges();
        }
    }
}
