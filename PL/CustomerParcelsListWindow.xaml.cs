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
using BO;
using BlApi;
using System.ComponentModel;


namespace PL
{
    /// <summary>
    /// Interaction logic for CustomerParcelsListWindow.xaml
    /// </summary>
    public partial class CustomerParcelsListWindow : Window
    {
        IBL bl;
        Customer selectedCustomer;
        public bool ClosingWindow { get; private set; } = true;
        /// <summary>
        /// open list of all parcels from a chosen customer
        /// </summary>
        /// <param name="bL">IBL interface</param>
        /// <param name="customer">the cosen customer</param>
        public CustomerParcelsListWindow(IBL bL, Customer customer)
        {
            bl = bL;
            selectedCustomer = customer;
            InitializeComponent();
            ParcelsFromCustomer.Visibility = Visibility.Visible;
            ParcelsToCustomer.Visibility = Visibility.Hidden;
            FromCustomerList.ItemsSource = customer.FromCustomer;
        }
        /// <summary>
        /// open list of all parcels to a chosen customer
        /// </summary>
        /// <param name="bL">IBL interface</param>
        /// <param name="customer">the cosen customer</param>
        /// <param name="i">a sign for "to customer" window</param>
        public CustomerParcelsListWindow(IBL bL, Customer customer, int i)
        {
            bl = bL;
            selectedCustomer = customer;
            InitializeComponent();
            ParcelsFromCustomer.Visibility = Visibility.Hidden;
            ParcelsToCustomer.Visibility = Visibility.Visible;
            ToCustomerList.ItemsSource = customer.ToCustomer;
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
        private void Exitbtn_Click(object sender, RoutedEventArgs e)
        {
            ClosingWindow = false;
            this.Close();
        }
        /// <summary>
        /// open parcel window out of parcels from customer
        /// </summary>
        private void ToCustomerList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ToCustomerList.SelectedItem != null)
                new ParcelWindow(bl, (ParcelInCustomer)ToCustomerList.SelectedItem, 0).Show();
        }
        /// <summary>
        /// open parcel window out of parcels to customer
        /// </summary>
        private void FromCustomerList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (FromCustomerList.SelectedItem != null)
                new ParcelWindow(bl, (ParcelInCustomer)FromCustomerList.SelectedItem, 1).Show();
        }
    }
}
