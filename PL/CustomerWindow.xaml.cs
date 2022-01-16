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

namespace PL
{
    /// <summary>
    /// Interaction logic for CustomerWindow.xaml
    /// </summary>
    public partial class CustomerWindow : Window
    {
        IBL bl;
        private CustomerListWindow cl;
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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

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
                        cl.CustomerListView.ItemsSource = bl.GetCustomersList();
                        //close when OK pressed
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

        private void IdtxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(IdtxtBox.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                IdtxtBox.Text = IdtxtBox.Text.Remove(IdtxtBox.Text.Length - 1);
            }
        }

        private void PhonetxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(PhonetxtBox.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                PhonetxtBox.Text = PhonetxtBox.Text.Remove(PhonetxtBox.Text.Length - 1);
            }
        }

        private void LongitudetxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(LongitudetxtBox.Text, "[^0-9 .]"))
            {
                MessageBox.Show("Please enter only numbers.");
                LongitudetxtBox.Text = LongitudetxtBox.Text.Remove(LongitudetxtBox.Text.Length - 1);
            }
        }

        private void LattitudetxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(LattitudetxtBox.Text, "[^0-9 .]"))
            {
                MessageBox.Show("Please enter only numbers.");
                LattitudetxtBox.Text = LattitudetxtBox.Text.Remove(LattitudetxtBox.Text.Length - 1);
            }
        }

        public Customer selectedCustomer;
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

        private void Exitbtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FromCustomerbtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCustomer.FromCustomer.Count() == 0)
                MessageBox.Show("There are no suitable packages");
            else
                new CustomerParcelsListWindow(bl, selectedCustomer).Show();
        }

        private void Tocustomerbtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCustomer.ToCustomer.Count() == 0)
                MessageBox.Show("There are no suitable packages");
            else
                new CustomerParcelsListWindow(bl, selectedCustomer, 1).Show();
        }

        private void Phonetxtbox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(Phonetxtbox1.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                Phonetxtbox1.Text = Phonetxtbox1.Text.Remove(Phonetxtbox1.Text.Length - 1);
            }
        }

        ParcelWindow pr;
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
    }
}
