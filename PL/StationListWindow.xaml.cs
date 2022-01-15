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
    /// Interaction logic for StationListWindow.xaml
    /// </summary>
    public partial class StationListWindow : Window
    {
        IBL bl;
        public StationListWindow(IBL bL)
        {
            bl = bL;
            InitializeComponent();
            StationListView.ItemsSource = bl.GetStationsList();
            SlotSelector.Items.Add("Availble slots");
            SlotSelector.Items.Add("Clear filter");

        }

        private void StationListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectorChanges();
        }
        private void SelectorChanges()
        {
            if (SlotSelector.SelectedItem == null || SlotSelector.SelectedItem.ToString() == "Clear filter")
                StationListView.ItemsSource = bl.GetStationsList();
            else
                StationListView.ItemsSource = bl.AvailableStations();
        }

        //close window
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddStation_Click(object sender, RoutedEventArgs e)
        {
            new StationWindow(bl, this).Show();
        }

        private void StationListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (StationListView.SelectedItem != null)
                new StationWindow(bl, (StationToList)(StationListView.SelectedItem), this).Show();
        }

        private void SlotsSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectorChanges();
        }
        //bool grouped;
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

        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(StationListView.ItemsSource);
            view.GroupDescriptions.Clear();
            SelectorChanges();
        }
    }
}
