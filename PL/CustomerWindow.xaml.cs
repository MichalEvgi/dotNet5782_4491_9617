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
using BO;
using BlApi;
using System.ComponentModel;

namespace PL
{
    /// <summary>
    /// Interaction logic for CustomerWindow.xaml
    /// </summary>
    public partial class CustomerWindow : Window
    {
        IBL bl;
        private CustomerListWindow cl;
        public bool ClosingWindow { get; private set; } = true;

        #region ADD CUSTOMER
        /// <summary>
        /// open add customer window
        /// </summary>
        /// <param name="bL">IBL interface</param>
        /// <param name="clw">customer list window</param>
        public CustomerWindow(IBL bL, CustomerListWindow clw)
        {
            bl = bL;
            cl = clw;
            InitializeComponent();
            AddCustomer.Visibility = Visibility.Visible;
            UpdateCustomer.Visibility = Visibility.Hidden;
            this.Width = 300;
            this.Height = 450;
        }

        /// <summary>
        /// cancel adding customer and close the window
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClosingWindow = false;
            this.Close();
        }

        /// <summary>
        /// add customer tp database according to the fill text boxes
        /// </summary>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IdtxtBox.Text == "" || NametxtBox.Text == "" || PhonetxtBox.Text == "" || LongitudetxtBox.Text == "" || LattitudetxtBox.Text == "")
                    //not all the fields are full
                    MessageBox.Show("not all fields are full");
                else
                {
                    //add the customer
                    bl.AddCustomer(new Customer { Id = Convert.ToInt32(IdtxtBox.Text), Name = NametxtBox.Text, Phone = PhonetxtBox.Text, LocationC = new Location { Longitude = Convert.ToDouble(LongitudetxtBox.Text), Lattitude = Convert.ToDouble(LattitudetxtBox.Text) } });
                    //seccessfully added message
                    MessageBoxResult result = MessageBox.Show("Added successfully");
                    if (result == MessageBoxResult.OK)
                    {
                        if(cl!=null)
                           cl.CustomerListView.ItemsSource = bl.GetCustomersList();
                        //close when OK pressed
                        ClosingWindow = false;
                        this.Close();
                    }
                }
            }
            catch (InvalidInputException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (AlreadyExistsException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        /// <summary>
        /// id validation input
        /// </summary>
        private void IdtxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(IdtxtBox.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                IdtxtBox.Text = IdtxtBox.Text.Remove(IdtxtBox.Text.Length - 1);
            }
        }
        /// <summary>
        /// phone validation input
        /// </summary>
        private void PhonetxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(PhonetxtBox.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                PhonetxtBox.Text = PhonetxtBox.Text.Remove(PhonetxtBox.Text.Length - 1);
            }
        }
        /// <summary>
        /// longitude validation input
        /// </summary>
        private void LongitudetxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(LongitudetxtBox.Text, "[^0-9 .]"))
            {
                MessageBox.Show("Please enter only numbers.");
                LongitudetxtBox.Text = LongitudetxtBox.Text.Remove(LongitudetxtBox.Text.Length - 1);
            }
        }
        /// <summary>
        /// lattitude validation input
        /// </summary>
        private void LattitudetxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(LattitudetxtBox.Text, "[^0-9 .]"))
            {
                MessageBox.Show("Please enter only numbers.");
                LattitudetxtBox.Text = LattitudetxtBox.Text.Remove(LattitudetxtBox.Text.Length - 1);
            }
        }
        #endregion
        #region ACTIONS
        public Customer selectedCustomer;
        /// <summary>
        /// open customer actions window
        /// </summary>
        /// <param name="bL">IBL interface</param>
        /// <param name="customer">selected customer</param>
        /// <param name="clw">customer list window</param>
        public CustomerWindow(IBL bL, CustomerToList customer, CustomerListWindow clw)
        {
            bl = bL;
            cl = clw;
            InitializeComponent();
            selectedCustomer = bl.GetCustomer(customer.Id);
            DataContext = selectedCustomer;
            AddCustomer.Visibility = Visibility.Hidden;
            UpdateCustomer.Visibility = Visibility.Visible;
            this.Width = 350;
            this.Height = 400;        
        }
        /// <summary>
        /// update the phone and/or name of the customer
        /// </summary>
        private void Updatebtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.UpdateCustomer(selectedCustomer.Id, Nametxtbox.Text, Phonetxtbox1.Text);
                cl.CustomerListView.ItemsSource = bl.GetCustomersList();
                MessageBox.Show("Successfully updated");
            }
            catch (NotFoundException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (InvalidInputException ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        /// <summary>
        /// phone validation input
        /// </summary>
        private void Phonetxtbox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(Phonetxtbox1.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                Phonetxtbox1.Text = Phonetxtbox1.Text.Remove(Phonetxtbox1.Text.Length - 1);
            }
        }

        /// <summary>
        /// open list of parcel from customer window
        /// </summary>
        private void FromCustomerbtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCustomer.FromCustomer.Count() == 0)
                MessageBox.Show("There are no suitable packages");
            else
                new CustomerParcelsListWindow(bl, selectedCustomer).Show();
        }
        /// <summary>
        /// open list of parcel to customer window
        /// </summary>
        private void Tocustomerbtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCustomer.ToCustomer.Count() == 0)
                MessageBox.Show("There are no suitable packages");
            else
                new CustomerParcelsListWindow(bl, selectedCustomer, 1).Show();
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
        #endregion
        #region CUSTOMER SIGN UP
        /// <summary>
        /// open customer sign up window
        /// </summary>
        /// <param name="bL">IBL interface</param>
        public CustomerWindow(IBL bL)
        {
            bl = bL;
            InitializeComponent();
            AddCustomer.Visibility = Visibility.Visible;
            UpdateCustomer.Visibility = Visibility.Hidden;
            this.Width = 300;
            this.Height = 450;
            
        }
        #endregion
        #region CUSTOMER IN PARCEL
        ParcelWindow pr;
        /// <summary>
        /// open customer in parcel window
        /// </summary>
        /// <param name="bL">IBL interface</param>
        /// <param name="customer">customer in parcel</param>
        /// <param name="prw">parcel window</param>
        public CustomerWindow(IBL bL, CustomerInParcel customer, ParcelWindow prw)
        {
            bl = bL;
            pr = prw;
            InitializeComponent();
            selectedCustomer = bl.GetCustomer(customer.Id);
            DataContext = selectedCustomer;
            AddCustomer.Visibility = Visibility.Hidden;
            UpdateCustomer.Visibility = Visibility.Visible;
            this.Width = 350;
            this.Height = 400;
            FromCustomerbtn.Visibility = Visibility.Collapsed;
            Tocustomerbtn.Visibility = Visibility.Collapsed;
            Updatebtn.Visibility = Visibility.Collapsed;
            Updatelbl.Visibility = Visibility.Collapsed;
            Nametxtbox.IsReadOnly = true;
            Phonetxtbox1.IsReadOnly = true;
        }
        #endregion
    }
}
