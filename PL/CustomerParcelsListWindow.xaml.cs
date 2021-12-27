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

namespace PL
{
    /// <summary>
    /// Interaction logic for CustomerParcelsListWindow.xaml
    /// </summary>
    public partial class CustomerParcelsListWindow : Window
    {
        IBL bl;
        Customer selectedCustomer;
        public CustomerParcelsListWindow(IBL bL,Customer customer)
        {
            bl = bL;
            selectedCustomer = customer;
            InitializeComponent();
            ParcelsFromCustomer.Visibility = Visibility.Visible;
            ParcelsToCustomer.Visibility = Visibility.Hidden;
            FromCustomerList.ItemsSource = customer.FromCustomer;
        }

        public CustomerParcelsListWindow(IBL bL, Customer customer,int i)
        {
            bl = bL;
            selectedCustomer = customer;
            InitializeComponent();
            ParcelsFromCustomer.Visibility = Visibility.Hidden;
            ParcelsToCustomer.Visibility = Visibility.Visible;
            ToCustomerList.ItemsSource = customer.ToCustomer;
        }

        private void Exitbtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
