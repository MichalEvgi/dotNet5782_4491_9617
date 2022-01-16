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
    /// Interaction logic for ParcelListWindow.xaml
    /// </summary>
    public partial class ParcelListWindow : Window
    {
        IBL bl;
        public ParcelListWindow(IBL bL)
        {
            bl = bL;
            InitializeComponent();
            ParcelListView.ItemsSource = bl.GetParcelsList().ToList();
            foreach (ParcelModes pm in (ParcelModes[])Enum.GetValues(typeof(ParcelModes)))
            {
                ParcelModecmb.Items.Add(pm);
            }
            ParcelModecmb.Items.Add("Clear filter");

            foreach (Priorities pm in (Priorities[])Enum.GetValues(typeof(Priorities)))
            {
                Prioritycmb.Items.Add(pm);
            }
            Prioritycmb.Items.Add("Clear filter");

        }
        private void ParcelListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ParcelListView.SelectedItem != null)
                new ParcelWindow(bl, (ParcelToList)(ParcelListView.SelectedItem), this).Show();
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddParcel_Click(object sender, RoutedEventArgs e)
        {
            new ParcelWindow(bl, this).Show();
        }

        private void SenderGrop_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<IGrouping<string, ParcelToList>> parcelGroup = from par in bl.GetParcelsList() group par by par.SenderName;
            List<ParcelToList> parcelTos = new();

            foreach (var group in parcelGroup)
            {
                foreach (var par in group)
                {
                    parcelTos.Add(par);
                }
            }
            ParcelListView.ItemsSource = parcelTos;


            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ParcelListView.ItemsSource);
            if (view.GroupDescriptions.Count < 1)
            {
                PropertyGroupDescription groupDescription = new PropertyGroupDescription("SenderName");
                view.GroupDescriptions.Add(groupDescription);
            }
        }

        private void TargetGroup_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<IGrouping<string, ParcelToList>> parcelGroup = from par in bl.GetParcelsList() group par by par.TargetName;
            List<ParcelToList> parcelTos = new();

            foreach (var group in parcelGroup)
            {
                foreach (var par in group)
                {
                    parcelTos.Add(par);
                }
            }
            ParcelListView.ItemsSource = parcelTos;


            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ParcelListView.ItemsSource);
            if (view.GroupDescriptions.Count < 1)
            {
                PropertyGroupDescription groupDescription = new PropertyGroupDescription("TargetName");
                view.GroupDescriptions.Add(groupDescription);
            }
        }

        private void ParcelModecmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectorChanges();
        }
        public void SelectorChanges()
        {
            if ((ParcelModecmb.SelectedItem == null || ParcelModecmb.SelectedItem.ToString() == "Clear filter") && (Prioritycmb.SelectedItem == null || Prioritycmb.SelectedItem.ToString() == "Clear filter"))
            {
                //if both fiter aren't set
                ParcelListView.ItemsSource = bl.GetParcelsList().Select(x => x);
            }
            else
            {
                if (ParcelModecmb.SelectedItem == null || ParcelModecmb.SelectedItem.ToString() == "Clear filter")
                    //if there is only priority filter
                    ParcelListView.ItemsSource = bl.GetParcelsList().Where(p => p.Priority == (Priorities)Prioritycmb.SelectedItem);
                else
                {
                    if (Prioritycmb.SelectedItem == null || Prioritycmb.SelectedItem.ToString() == "Clear filter")
                        //if there is only mode filter
                        ParcelListView.ItemsSource = bl.GetParcelsList().Where(p => p.ParcelMode == (ParcelModes)ParcelModecmb.SelectedItem);
                    else
                        //if the two filter were selected
                        ParcelListView.ItemsSource = bl.GetParcelsList().Where(p => (p.Priority == (Priorities)Prioritycmb.SelectedItem) && (p.ParcelMode == (ParcelModes)ParcelModecmb.SelectedItem));
                }
            }
        }

        private void Prioritycmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectorChanges();
        }
        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ParcelListView.ItemsSource);
            view.GroupDescriptions.Clear();
            SelectorChanges();
        }

        private void DatePicker_CalendarClosed(object sender, RoutedEventArgs e)
        {
            timePicked();
        }

        private void DatePicker_CalendarClosed_1(object sender, RoutedEventArgs e)
        {
            timePicked();
        }
        private void timePicked()
        {
            DateTime? f = from.SelectedDate;
            DateTime? t = to.SelectedDate;
            if (f != null && t == null)
                ParcelListView.ItemsSource = bl.GetParcelsList().Where(p => bl.GetParcel(p.Id).RequestedTime >= f.Value);
            else
            {
                t= t.Value.AddDays(1);
                if (f == null && t != null)
                    ParcelListView.ItemsSource = bl.GetParcelsList().Where(p => bl.GetParcel(p.Id).RequestedTime < t.Value);
                else
                    ParcelListView.ItemsSource = bl.GetParcelsList().Where(p => bl.GetParcel(p.Id).RequestedTime >= f.Value && bl.GetParcel(p.Id).RequestedTime < t.Value);
            }
        }
    }
}
