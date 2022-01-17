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
    /// Interaction logic for EntryWindow.xaml
    /// </summary>
    public partial class EntryWindow : Window
    {
        private IBL bL;
        Customer customer;
        public EntryWindow()
        {
            try
            {
                bL = BlFactory.GetBl();
                InitializeComponent();
            }
            catch (DalConfigException)
            {
            }
            
        }

        private void Enterbtn_Click(object sender, RoutedEventArgs e)
        {
            if (Passwordtxtbox.Text == Convert.ToString(1111))
            {
                new MainWindow(bL).Show();
            }
            else
            {
                try
                {
                   customer=bL.GetCustomer(Convert.ToInt32(Passwordtxtbox.Text));
                    new CustomerLoginWindow(bL, customer).Show();
                }
                catch(NotFoundException ex)
                {
                    MessageBox.Show("This customer does not exist, please log in");
                }
            }
        }

        private void Loginbtn_Click(object sender, RoutedEventArgs e)
        {
            new CustomerWindow(bL).Show();
        }
    }
}
