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
    /// Interaction logic for CustomerListWindow.xaml
    /// </summary>
    public partial class CustomerListWindow : Window
    {
        IBL bl;
        public CustomerListWindow(IBL bL)
        {
            bl = bL;
            InitializeComponent();
            CustomerListView.ItemsSource = bl.GetCustomersList();
        }


        private void AddCustomer_Click_1(object sender, RoutedEventArgs e)
        {
            new CustomerWindow(bl,this).Show();
        }

        private void ExitButton_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
