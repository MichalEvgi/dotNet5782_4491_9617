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
    /// Interaction logic for ParcelInDeliveryWindow.xaml
    /// </summary>
    public partial class ParcelInDeliveryWindow : Window
    {
        IBL bl;
        ParcelInTransfer selectedParcel;
        int droneId;
        /// <summary>
        /// open the parcel in delivery with drone window
        /// </summary>
        /// <param name="bL">IBL interface</param>
        /// <param name="parcel">parcel in transfer</param>
        /// <param name="droneid">drone's id that take the parcel</param>
        public ParcelInDeliveryWindow(IBL bL, ParcelInTransfer parcel, int droneid)
        {
            bl = bL;
            InitializeComponent();
            droneId = droneid;
            selectedParcel = parcel;
            DataContext = selectedParcel;
        }
        /// <summary>
        /// refresh this window
        /// </summary>
        public void ParcelChange()
        {
            selectedParcel = bl.GetDrone(droneId).TransferedParcel;
            DataContext = selectedParcel;
        }
        /// <summary>
        /// close the window
        /// </summary>
        private void Exitbt_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
