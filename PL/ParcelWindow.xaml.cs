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
    /// Interaction logic for ParcelWindow.xaml
    /// </summary>
    public partial class ParcelWindow : Window
    {
        IBL bl;
        private ParcelListWindow pr;
        public ParcelWindow(IBL bL, ParcelListWindow plw)
        {
            bl = bL;
            pr = plw;
            InitializeComponent();
            WeightCmb.ItemsSource = Enum.GetValues(typeof(WeightCategories));
            PriorityCmb.ItemsSource = Enum.GetValues(typeof(Priorities));
            addParcel.Visibility = Visibility.Visible;
            actions.Visibility = Visibility.Hidden;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public Parcel selectedParcel;
        public ParcelWindow(IBL bL, ParcelToList parcel, ParcelListWindow plw)
        {
            bl = bL;
            pr = plw;
            selectedParcel = bl.GetParcel(parcel.Id);
            DataContext = selectedParcel;
            InitializeComponent();
            addParcel.Visibility = Visibility.Hidden;
            actions.Visibility = Visibility.Visible;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SenderIdtxtBox.Text == "" || TargetIdtxtBox.Text == "" || WeightCmb.SelectedItem == null || PriorityCmb.SelectedItem == null)
                    //not all the fields are full
                    MessageBox.Show("Not all fields are full");
                else
                {
                    //add the drone
                    bl.AddParcel(new Parcel { Sender = new CustomerInParcel { Id = Convert.ToInt32( SenderIdtxtBox.Text) },
                        Target = new CustomerInParcel { Id = Convert.ToInt32(TargetIdtxtBox.Text) }, 
                        Weight = (WeightCategories)WeightCmb.SelectedItem,
                        Priority = (Priorities)PriorityCmb.SelectedItem
                    });
                    //seccessfully added message
                    MessageBoxResult result = MessageBox.Show("Seccussfuly added");
                    if (result == MessageBoxResult.OK)
                    {
                        //close when OK pressed
                        this.Close();
                    }
                }
            }
            catch (NotFoundException ex)
            {
              MessageBox.Show(ex.ToString());
            }
        }

        //check if only number is entered to sender id textBox
        private void SenderIdtxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(SenderIdtxtBox.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                SenderIdtxtBox.Text = SenderIdtxtBox.Text.Remove(SenderIdtxtBox.Text.Length - 1);
            }
        }

        //check if only number is entered to target id textBox
        private void TargetIdtxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(TargetIdtxtBox.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                TargetIdtxtBox.Text = TargetIdtxtBox.Text.Remove(TargetIdtxtBox.Text.Length - 1);
            }
        }
    }
}
