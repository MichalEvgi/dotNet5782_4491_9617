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
    /// Interaction logic for ParcelListWindow.xaml
    /// </summary>
    public partial class ParcelListWindow : Window
    {
        IBL bl;
        public ParcelListWindow(IBL bL)
        {
            bl = bL;
            ParcelListView.ItemsSource = bl.GetParcelsList();
            InitializeComponent();
        }
        private void ParcelListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            new ParcelWindow(bl, (ParcelToList)(this.ParcelListView.SelectedItem), this).Show();
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddParcel_Click(object sender, RoutedEventArgs e)
        {
            new ParcelWindow(bl, this).Show();
        }
    }
}
