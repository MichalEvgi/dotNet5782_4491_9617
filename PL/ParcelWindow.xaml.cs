﻿using System;
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
using System.ComponentModel;

namespace PL
{
    /// <summary>
    /// Interaction logic for ParcelWindow.xaml
    /// </summary>
    public partial class ParcelWindow : Window
    {
        IBL bl;
        private ParcelListWindow pr;
        Customer customer;
        private CustomerLoginWindow clg;
        public bool ClosingWindow { get; private set; } = true;
        public ParcelWindow(IBL bL, ParcelListWindow plw)
        {
            bl = bL;
            pr = plw;
            InitializeComponent();
            WeightCmb.ItemsSource = Enum.GetValues(typeof(WeightCategories));
            PriorityCmb.ItemsSource = Enum.GetValues(typeof(Priorities));
            addParcel.Visibility = Visibility.Visible;
            actions.Visibility = Visibility.Hidden;
            this.Height = 450;
            this.Width = 300;
        }

        public ParcelWindow(IBL bL,Customer c,CustomerLoginWindow cl)
        {
            bl = bL;
            customer = c;
            clg = cl;
            InitializeComponent();
            SenderIdtxtBox.Text = Convert.ToString(customer.Id);
            SenderIdtxtBox.IsReadOnly = true;
            addParcel.Visibility = Visibility.Visible;
            actions.Visibility = Visibility.Hidden;
            this.Height = 450;
            this.Width = 300;
            WeightCmb.ItemsSource = Enum.GetValues(typeof(WeightCategories));
            PriorityCmb.ItemsSource = Enum.GetValues(typeof(Priorities));
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = ClosingWindow;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClosingWindow = false;
            this.Close();
        }

        public Parcel selectedParcel;
        public ParcelWindow(IBL bL, ParcelToList parcel, ParcelListWindow plw)
        {
            bl = bL;
            pr = plw;
            selectedParcel = bl.GetParcel(parcel.Id);
            DataContext = selectedParcel;
            InitializeComponent();
            addParcel.Visibility = Visibility.Hidden;
            actions.Visibility = Visibility.Visible;
            this.Height = 360;
            this.Width = 560;
            if (selectedParcel.DeliveredTime == null)
            {
                Deliveredtxtbox.Visibility = Visibility.Collapsed;
                Deliverybt.Content = "Deliver parcel";
            }
            else
            {
                Deliverybt.IsEnabled = false;
                Dronebt.Visibility = Visibility.Collapsed;
            }
            if (selectedParcel.PickedUpTime == null)
            {
                PickedUptxtbox.Visibility = Visibility.Collapsed;
                Deliverybt.Content = "Pick up parcel";
            }
            if (selectedParcel.ScheduledTime == null)
            {
                Scheduledtxtbox.Visibility = Visibility.Collapsed;
                Dronebt.Visibility = Visibility.Collapsed;
                Deliverybt.IsEnabled = false;
                Deletebt.IsEnabled = true;
            }
            else
            {
                Deletebt.IsEnabled = false;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SenderIdtxtBox.Text == "" || TargetIdtxtBox.Text == "" || WeightCmb.SelectedItem == null || PriorityCmb.SelectedItem == null)
                    //not all the fields are full
                    MessageBox.Show("Not all fields are full");
                else
                {
                    //add the drone
                    bl.AddParcel(new Parcel { Sender = new CustomerInParcel { Id = Convert.ToInt32( SenderIdtxtBox.Text) },
                        Target = new CustomerInParcel { Id = Convert.ToInt32(TargetIdtxtBox.Text) }, 
                        Weight = (WeightCategories)WeightCmb.SelectedItem,
                        Priority = (Priorities)PriorityCmb.SelectedItem
                    });
                    if(pr!=null)
                       pr.ParcelListView.ItemsSource = bl.GetParcelsList();
                    if (clg != null)
                        clg.FromcustomerList.ItemsSource = bl.GetCustomer(customer.Id).FromCustomer;
                    //seccessfully added message
                    MessageBoxResult result = MessageBox.Show("Seccussfuly added");
                    if (result == MessageBoxResult.OK)
                    {
                        //close when OK pressed
                        ClosingWindow = false;
                        this.Close();
                    }
                }
            }
            catch (NotFoundException ex)
            {
              MessageBox.Show(ex.ToString());
            }
        }

        //check if only number is entered to sender id textBox
        private void SenderIdtxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(SenderIdtxtBox.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                SenderIdtxtBox.Text = SenderIdtxtBox.Text.Remove(SenderIdtxtBox.Text.Length - 1);
            }
        }

        //check if only number is entered to target id textBox
        private void TargetIdtxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(TargetIdtxtBox.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                TargetIdtxtBox.Text = TargetIdtxtBox.Text.Remove(TargetIdtxtBox.Text.Length - 1);
            }
        }

        private void Exitbt_Click(object sender, RoutedEventArgs e)
        {
            ClosingWindow = false;
            this.Close();
        }

        private void Senderbt_Click(object sender, RoutedEventArgs e)
        {
            new CustomerWindow(bl, selectedParcel.Sender, this).Show();
        }

        private void Targetbt_Click(object sender, RoutedEventArgs e)
        {
            new CustomerWindow(bl, selectedParcel.Target, this).Show();
        }

        private void Dronebt_Click(object sender, RoutedEventArgs e)
        {
            new DroneWindow(bl, selectedParcel.DroneP).Show();
        }

        private void Deletebt_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete?", "Delete parcel", MessageBoxButton.OKCancel);
            if(result == MessageBoxResult.OK)
            {
                bl.DeleteParcel(selectedParcel.Id);
                pr.ParcelListView.ItemsSource = bl.GetParcelsList();
                MessageBoxResult result2=MessageBox.Show("Deleted successfuly");
                if (result2 == MessageBoxResult.OK)
                {
                    ClosingWindow = false;
                    this.Close();
                }
                    


            }

        }

        private void Deliverybt_Click(object sender, RoutedEventArgs e)
        {
            if (selectedParcel.PickedUpTime == null)
                pickedUp();
            else
                deliver();
        }

        //pick up parcel
        private void pickedUp()
        {
            try
            {
                //pick parcel
                bl.PickParcel(selectedParcel.DroneP.Id);
                //seccessfuly picked up message
                MessageBox.Show("Picked up seccessfuly");
                //updating
                pr.ParcelListView.ItemsSource = bl.GetParcelsList();
                selectedParcel = bl.GetParcel(selectedParcel.Id);
                DataContext = selectedParcel;
                PickedUptxtbox.Visibility = Visibility.Visible;
                //change delivery button state
                Deliverybt.Content = "Deliver parcel";
            }
            catch (NotFoundException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (DroneStatusException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (ParcelModeException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        //deliver parcel
        private void deliver()
        {
            try
            {
                //deliver parcel
                bl.DeliverParcel(selectedParcel.DroneP.Id);
                //seccessfully delivered message
                MessageBox.Show("Delivered seccessfuly");
                //updating
                selectedParcel = bl.GetParcel(selectedParcel.Id);
                DataContext = selectedParcel;
                pr.ParcelListView.ItemsSource = bl.GetParcelsList();
                Deliveredtxtbox.Visibility = Visibility.Visible;
                Dronebt.Visibility = Visibility.Collapsed;
                //change delivery button state
                Deliverybt.IsEnabled = false;
            }
            catch (NotFoundException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (DroneStatusException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (ParcelModeException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public ParcelWindow(IBL bL, ParcelInCustomer parcel, int senderOrTarget)
        {
            bl = bL;
            selectedParcel = bl.GetParcel(parcel.Id);
            DataContext = selectedParcel;
            InitializeComponent();
            addParcel.Visibility = Visibility.Hidden;
            actions.Visibility = Visibility.Visible;
            Senderbt.Visibility = Visibility.Collapsed;
            Targetbt.Visibility = Visibility.Collapsed;
            this.Height = 360;
            this.Width = 560;
            OtherCustomerlbl.Visibility = Visibility.Visible;
            Othertxtbox.Visibility = Visibility.Visible;
            Othertxtbox.Text = parcel.OtherCustomer.ToString();
            if (senderOrTarget==0)
                OtherCustomerlbl.Content = "Sender:";
            else
                OtherCustomerlbl.Content = "Target:";
            if (selectedParcel.DeliveredTime == null)
            {
                Deliveredtxtbox.Visibility = Visibility.Collapsed;
                Dronebt.Visibility = Visibility.Collapsed;
            }
            if (selectedParcel.PickedUpTime == null)
            {
                PickedUptxtbox.Visibility = Visibility.Collapsed;
            }
            if (selectedParcel.ScheduledTime == null)
            {
                Scheduledtxtbox.Visibility = Visibility.Collapsed;
                Dronebt.Visibility = Visibility.Collapsed;
            }
            Deliverylbl.Visibility = Visibility.Collapsed;
            Deliverybt.Visibility = Visibility.Collapsed;
            Deletebt.Visibility = Visibility.Collapsed;
            
        }

    }
}
