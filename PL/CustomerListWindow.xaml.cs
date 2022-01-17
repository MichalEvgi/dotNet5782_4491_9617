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
    /// Interaction logic for CustomerListWindow.xaml
    /// </summary>
    public partial class CustomerListWindow : Window
    {
        IBL bl;
        public bool ClosingWindow { get; private set; } = true;
        /// <summary>
        /// opening the customers list window
        /// </summary>
        /// <param name="bL">IBL interface</param>
        public CustomerListWindow(IBL bL)
        {
            bl = bL;
            InitializeComponent();
            CustomerListView.ItemsSource = bl.GetCustomersList();
        }

        /// <summary>
        /// open window to add customer to database
        /// </summary>
        private void AddCustomer_Click_1(object sender, RoutedEventArgs e)
        {
            new CustomerWindow(bl, this).Show();
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
        private void ExitButton_Click_1(object sender, RoutedEventArgs e)
        {
            ClosingWindow = false;
            Close();
        }
        /// <summary>
        /// open chosen customer window
        /// </summary>
        private void CustomerListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CustomerListView.SelectedItem != null)
                new CustomerWindow(bl, (CustomerToList)(CustomerListView.SelectedItem), this).Show();
        }

    }
}
