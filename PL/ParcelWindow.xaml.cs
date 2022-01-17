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
    /// Interaction logic for ParcelWindow.xaml
    /// </summary>
    public partial class ParcelWindow : Window
    {
        IBL bl;
        private ParcelListWindow pr;
        Customer customer;
        private CustomerLoginWindow clg;
        public bool ClosingWindow { get; private set; } = true;
        #region ADD PARCEL
        /// <summary>
        /// open add parcel window
        /// </summary>
        /// <param name="bL">IBL interface</param>
        /// <param name="plw">ParcelListWindow</param>
        public ParcelWindow(IBL bL, ParcelListWindow plw)
        {
            bl = bL;
            pr = plw;
            InitializeComponent();
            WeightCmb.ItemsSource = Enum.GetValues(typeof(WeightCategories));
            PriorityCmb.ItemsSource = Enum.GetValues(typeof(Priorities));
            addParcel.Visibility = Visibility.Visible;
            actions.Visibility = Visibility.Hidden;
            Height = 450;
            Width = 300;
        }

        /// <summary>
        /// add parcel from customer interface
        /// </summary>
        /// <param name="bL">IBL interface</param>
        /// <param name="c">customer</param>
        /// <param name="cl">Customer Login Window</param>
        public ParcelWindow(IBL bL,Customer c,CustomerLoginWindow cl)
        {
            bl = bL;
            customer = c;
            clg = cl;
            InitializeComponent();
            SenderIdtxtBox.Text = Convert.ToString(customer.Id);
            SenderIdtxtBox.IsReadOnly = true;
            addParcel.Visibility = Visibility.Visible;
            actions.Visibility = Visibility.Hidden;
            this.Height = 450;
            this.Width = 300;
            WeightCmb.ItemsSource = Enum.GetValues(typeof(WeightCategories));
            PriorityCmb.ItemsSource = Enum.GetValues(typeof(Priorities));
        }
        /// <summary>
        /// close window event. close window if  ClosingWindow=false
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = ClosingWindow;
        }
        /// <summary>
        /// click on cancel button event
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClosingWindow = false;
            this.Close();
        }
        /// <summary>
        /// add parcel
        /// </summary>
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
                    bl.AddParcel(new Parcel
                    {
                        Sender = new CustomerInParcel { Id = Convert.ToInt32(SenderIdtxtBox.Text) },
                        Target = new CustomerInParcel { Id = Convert.ToInt32(TargetIdtxtBox.Text) },
                        Weight = (WeightCategories)WeightCmb.SelectedItem,
                        Priority = (Priorities)PriorityCmb.SelectedItem
                    });
                    if (pr != null)
                        pr.ParcelListView.ItemsSource = bl.GetParcelsList();
                    if (clg != null)
                    {
                        customer = bl.GetCustomer(customer.Id);
                        clg.FromcustomerList.ItemsSource = customer.FromCustomer;
                        clg.Pickupcmb.Items.Clear();
                        foreach (ParcelInCustomer p in customer.FromCustomer)
                        {
                            if (p.ParcelMode == ParcelModes.Associated)
                                clg.Pickupcmb.Items.Add(p.Id);
                        }
                    }
                    //seccessfully added message
                    MessageBoxResult result = MessageBox.Show("Seccussfuly added");
                    if (result == MessageBoxResult.OK)
                    {
                        //close when OK pressed
                        ClosingWindow = false;
                        this.Close();
                    }
                }
            }
            catch (NotFoundException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        /// <summary>
        /// check if only number is entered to sender id textBox
        /// </summary>
        private void SenderIdtxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(SenderIdtxtBox.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                SenderIdtxtBox.Text = SenderIdtxtBox.Text.Remove(SenderIdtxtBox.Text.Length - 1);
            }
        }
        /// <summary>
        /// check if only number is entered to target id textBox
        /// </summary>
        private void TargetIdtxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(TargetIdtxtBox.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                TargetIdtxtBox.Text = TargetIdtxtBox.Text.Remove(TargetIdtxtBox.Text.Length - 1);
            }
        }
        #endregion
        #region PARCEL ACTIONS
        public Parcel selectedParcel;
        /// <summary>
        /// open parcel actions window
        /// </summary>
        /// <param name="bL">IBL interface</param>
        /// <param name="parcel">parcel to list</param>
        /// <param name="plw">Parcel List Window</param>
        public ParcelWindow(IBL bL, ParcelToList parcel, ParcelListWindow plw)
        {
            bl = bL;
            pr = plw;
            selectedParcel = bl.GetParcel(parcel.Id);
            DataContext = selectedParcel;
            InitializeComponent();
            addParcel.Visibility = Visibility.Hidden;
            actions.Visibility = Visibility.Visible;
            this.Height = 360;
            this.Width = 560;
            if (selectedParcel.DeliveredTime == null)
            {
                Deliveredtxtbox.Visibility = Visibility.Collapsed;
                Deliverybt.Content = "Deliver parcel";
            }
            else
            {
                Deliverybt.IsEnabled = false;
                Dronebt.Visibility = Visibility.Collapsed;
            }
            if (selectedParcel.PickedUpTime == null)
            {
                PickedUptxtbox.Visibility = Visibility.Collapsed;
                Deliverybt.Content = "Pick up parcel";
            }
            if (selectedParcel.ScheduledTime == null)
            {
                Scheduledtxtbox.Visibility = Visibility.Collapsed;
                Dronebt.Visibility = Visibility.Collapsed;
                Deliverybt.IsEnabled = false;
                Deletebt.IsEnabled = true;
            }
            else
            {
                Deletebt.IsEnabled = false;
            }
        }
        /// <summary>
        /// open the sender customer in parcel
        /// </summary>
        private void Senderbt_Click(object sender, RoutedEventArgs e)
        {
            new CustomerWindow(bl, selectedParcel.Sender, this).Show();
        }
        /// <summary>
        /// open the target customer in parcel
        /// </summary>
        private void Targetbt_Click(object sender, RoutedEventArgs e)
        {
            new CustomerWindow(bl, selectedParcel.Target, this).Show();
        }
        /// <summary>
        /// open drone in parcel window
        /// </summary>
        private void Dronebt_Click(object sender, RoutedEventArgs e)
        {
            new DroneWindow(bl, selectedParcel.DroneP).Show();
        }
        /// <summary>
        /// delete parcel
        /// </summary>
        private void Deletebt_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete?", "Delete parcel", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                bl.DeleteParcel(selectedParcel.Id);
                pr.ParcelListView.ItemsSource = bl.GetParcelsList();
                MessageBoxResult result2 = MessageBox.Show("Deleted successfuly");
                if (result2 == MessageBoxResult.OK)
                {
                    ClosingWindow = false;
                    Close();
                }
            }

        }

        /// <summary>
        /// deliver/pick up a parcel
        /// </summary>
        private void Deliverybt_Click(object sender, RoutedEventArgs e)
        {
            if (selectedParcel.PickedUpTime == null)
                pickedUp();
            else
                deliver();
        }

        /// <summary>
        /// pick up parcel
        /// </summary>
        private void pickedUp()
        {
            try
            {
                //pick parcel
                bl.PickParcel(selectedParcel.DroneP.Id);
                //seccessfuly picked up message
                MessageBox.Show("Picked up seccessfuly");
                //updating
                pr.ParcelListView.ItemsSource = bl.GetParcelsList();
                selectedParcel = bl.GetParcel(selectedParcel.Id);
                DataContext = selectedParcel;
                PickedUptxtbox.Visibility = Visibility.Visible;
                //change delivery button state
                Deliverybt.Content = "Deliver parcel";
            }
            catch (NotFoundException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (DroneStatusException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (ParcelModeException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        /// <summary>
        /// deliver parcel
        /// </summary>
        private void deliver()
        {
            try
            {
                //deliver parcel
                bl.DeliverParcel(selectedParcel.DroneP.Id);
                //seccessfully delivered message
                MessageBox.Show("Delivered seccessfuly");
                //updating
                selectedParcel = bl.GetParcel(selectedParcel.Id);
                DataContext = selectedParcel;
                pr.ParcelListView.ItemsSource = bl.GetParcelsList();
                Deliveredtxtbox.Visibility = Visibility.Visible;
                Dronebt.Visibility = Visibility.Collapsed;
                //change delivery button state
                Deliverybt.IsEnabled = false;
            }
            catch (NotFoundException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (DroneStatusException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (ParcelModeException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        /// <summary>
        /// click on exit button event
        /// </summary>
        private void Exitbt_Click(object sender, RoutedEventArgs e)
        {
            ClosingWindow = false;
            Close();
        }
        #endregion
        #region PARCEL IN CUSTOMER
        /// <summary>
        /// open the parcel send by or to a customer
        /// </summary>
        /// <param name="bL">IBL interface</param>
        /// <param name="parcel">ParcelInCustomer</param>
        /// <param name="senderOrTarget">0=the customer is the target, 1=the customer is athe sender</param>
        public ParcelWindow(IBL bL, ParcelInCustomer parcel, int senderOrTarget)
        {
            bl = bL;
            selectedParcel = bl.GetParcel(parcel.Id);
            DataContext = selectedParcel;
            InitializeComponent();
            addParcel.Visibility = Visibility.Hidden;
            actions.Visibility = Visibility.Visible;
            Senderbt.Visibility = Visibility.Collapsed;
            Targetbt.Visibility = Visibility.Collapsed;
            this.Height = 360;
            this.Width = 560;
            OtherCustomerlbl.Visibility = Visibility.Visible;
            Othertxtbox.Visibility = Visibility.Visible;
            Othertxtbox.Text = parcel.OtherCustomer.ToString();
            if (senderOrTarget==0)
                OtherCustomerlbl.Content = "Sender:";
            else
                OtherCustomerlbl.Content = "Target:";
            if (selectedParcel.DeliveredTime == null)
            {
                Deliveredtxtbox.Visibility = Visibility.Collapsed;
                Dronebt.Visibility = Visibility.Collapsed;
            }
            if (selectedParcel.PickedUpTime == null)
            {
                PickedUptxtbox.Visibility = Visibility.Collapsed;
            }
            if (selectedParcel.ScheduledTime == null)
            {
                Scheduledtxtbox.Visibility = Visibility.Collapsed;
                Dronebt.Visibility = Visibility.Collapsed;
            }
            Deliverylbl.Visibility = Visibility.Collapsed;
            Deliverybt.Visibility = Visibility.Collapsed;
            Deletebt.Visibility = Visibility.Collapsed;
            
        }
        #endregion
    }
}
