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
using System.ComponentModel;

namespace PL
{
    /// <summary>
    /// Interaction logic for StationListWindow.xaml
    /// </summary>
    public partial class StationListWindow : Window
    {
        IBL bl;
        public bool ClosingWindow { get; private set; } = true;
        /// <summary>
        /// open stations list window
        /// </summary>
        /// <param name="bL">IBL interface</param>
        public StationListWindow(IBL bL)
        {
            bl = bL;
            InitializeComponent();
            StationListView.ItemsSource = bl.GetStationsList();
            SlotSelector.Items.Add("Availble slots");
            SlotSelector.Items.Add("Clear filter");

        }
        
        /// <summary>
        /// refresh the list view
        /// </summary>
        private void SelectorChanges()
        {
            if (SlotSelector.SelectedItem == null || SlotSelector.SelectedItem.ToString() == "Clear filter")
                StationListView.ItemsSource = bl.GetStationsList();
            else
                StationListView.ItemsSource = bl.AvailableStations();
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
        /// open add station window
        /// </summary>
        private void AddStation_Click(object sender, RoutedEventArgs e)
        {
            new StationWindow(bl, this).Show();
        }
        /// <summary>
        /// open station action window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StationListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (StationListView.SelectedItem != null)
                new StationWindow(bl, (StationToList)(StationListView.SelectedItem), this).Show();
        }
        /// <summary>
        /// selected filter event
        /// </summary>
        private void SlotsSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectorChanges();
        }
        /// <summary>
        /// grouping clicked
        /// </summary>
        private void amountbtn_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<IGrouping<int, StationToList>> stationGroup = from st in bl.GetStationsList() group st by st.AvailableSlots;
            List<StationToList> stationTos = new();

            foreach (var group in stationGroup)
            {
                foreach (var st in group)
                {
                    stationTos.Add(st);
                }
            }
            StationListView.ItemsSource = stationTos;


            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(StationListView.ItemsSource);
            if (view.GroupDescriptions.Count < 1)
            {
                PropertyGroupDescription groupDescription = new PropertyGroupDescription("AvailableSlots");
                view.GroupDescriptions.Add(groupDescription);
            }
        }
        /// <summary>
        /// refresh grouping
        /// </summary>
        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(StationListView.ItemsSource);
            view.GroupDescriptions.Clear();
            SelectorChanges();
        }
    }
}
