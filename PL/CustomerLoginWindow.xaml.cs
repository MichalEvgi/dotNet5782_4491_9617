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
using System.ComponentModel;

namespace PL
{
    /// <summary>
    /// Interaction logic for CustomerLoginWindow.xaml
    /// </summary>
    public partial class CustomerLoginWindow : Window
    {
        private IBL bL;
        Customer customer;
        public bool ClosingWindow { get; private set; } = true;
        /// <summary>
        /// opening the customer interface window
        /// </summary>
        /// <param name="bl">IBL interface</param>
        /// <param name="c">customer</param>
        public CustomerLoginWindow(IBL bl,Customer c)
        {
            bL = bl;
            customer = c;
            InitializeComponent();
            FromcustomerList.ItemsSource = customer.FromCustomer;
            TocustomerList.ItemsSource = customer.ToCustomer;
            foreach(ParcelInCustomer p in customer.FromCustomer)
            {
                if(p.ParcelMode==ParcelModes.Associated)
                   Pickupcmb.Items.Add(p.Id);
            }
            foreach (ParcelInCustomer p in customer.ToCustomer)
            {
                if(p.ParcelMode== ParcelModes.Collected)
                  Deliverycmb.Items.Add(p.Id);
            }
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
        /// add parcel by customer
        /// </summary>
        private void Addparcelbtn_Click(object sender, RoutedEventArgs e)
        {
            new ParcelWindow(bL, customer, this).Show();
        }
        /// <summary>
        /// Confirmation of picking up the parcel
        /// </summary>
        private void PickupCheck_Checked(object sender, RoutedEventArgs e)
        {
            if(PickupCheck.IsChecked==true && Pickupcmb.SelectedItem!= null)
            {
                try 
                {
                    Parcel p = bL.GetParcel(Convert.ToInt32(Pickupcmb.SelectedItem));
                    if(p.DroneP!=null)
                       bL.PickParcel(p.DroneP.Id);
                    MessageBox.Show("The parcel picked up successfully");
                }
                catch(DroneStatusException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                catch(NotFoundException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                catch(ParcelModeException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        /// <summary>
        /// Confirmation of delivering the parcel
        /// </summary>
        private void Deliverycheck_Checked(object sender, RoutedEventArgs e)
        {
            if(Deliverycheck.IsChecked==true && Deliverycmb.SelectedItem!=null)
            {
                try
                {
                    Parcel p = bL.GetParcel(Convert.ToInt32(Deliverycmb.SelectedItem));
                    bL.DeliverParcel(p.DroneP.Id);
                    MessageBox.Show("The parcel delivered successfully");
                }
                catch(DroneStatusException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                catch(NotFoundException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                catch(ParcelModeException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
    }
}
