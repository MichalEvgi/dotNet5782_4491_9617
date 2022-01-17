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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneWindow.xaml
    /// </summary>
    public partial class DroneWindow : Window
    {
        IBL bl;
        private DroneListWindow dr;
        //open add drone window
        public DroneWindow(IBL bL, DroneListWindow dlw)
        {
            bl = bL;
            dr = dlw;
            InitializeComponent();
            //set the items of WeightCmb to WeightCategories
            WeightCmb.ItemsSource = Enum.GetValues(typeof(WeightCategories));
            //show addDrone only
            addDrone.Visibility = Visibility.Visible;
            actions.Visibility = Visibility.Hidden;
            this.Width = 300;
            this.Height = 450;
        }
        //close window by cancel button
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //add drone click button
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IdtxtBox.Text == "" || ModeltxtBox.Text == "" || WeightCmb.SelectedItem == null || SIdtxtBox.Text == "")
                    //not all the fields are full
                    MessageBox.Show("Not all fields are full");
                else
                {
                    //add the drone
                    bl.AddDrone(new Drone { Id = Convert.ToInt32(IdtxtBox.Text), Model = ModeltxtBox.Text, MaxWeight = (WeightCategories)WeightCmb.SelectedItem }, Convert.ToInt32(SIdtxtBox.Text));
                    
                    //seccessfully added message
                    MessageBoxResult result = MessageBox.Show("Seccessfuly added");
                    if (result == MessageBoxResult.OK)
                    {
                        dr.SelectorChanges();
                       // dr.DroneListView.Items.Refresh();
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
            catch (NotFoundException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        

        public Drone selectedDrone;
       //open action drone window
        public DroneWindow(IBL bL, DroneToList drone,DroneListWindow dl)
        {
            
            bl = bL;
            dr = dl;
            InitializeComponent();
            selectedDrone = bl.GetDrone(drone.Id);
            DataContext = selectedDrone;
            //show the actions only
            actions.Visibility = Visibility.Visible;
            addDrone.Visibility = Visibility.Hidden;
            this.Height = 450;
            this.Width = 400;
            if (selectedDrone.Status != DroneStatus.Delivery)
            { //the drone is not in delivery
                //hide the transfered parcel
                Parcelbtn.Visibility = Visibility.Collapsed;
                //enable charging
                Chargingbt.IsEnabled = true;
            }
            else
            { //the drone is in delivery
                //show the transfered parcel
                Parcelbtn.Visibility = Visibility.Visible;
                //disable charging
                Chargingbt.IsEnabled = false;
                Chargingbt.IsEnabled = false;
            }
            if(selectedDrone.Status== DroneStatus.Maintenance)
            { //the drone in charging
              //disable delivery
                Deliverybt.IsEnabled = false;
                Chargingbt.Content = "Realease from charging";
            }
            else
            {   //the drone is not in charging
                //enable delivery
                if (selectedDrone.Status == DroneStatus.Available)
                     Deliverybt.IsEnabled = true;
                else
                    Deliverybt.IsEnabled = false;
                Chargingbt.Content = "Send for charging";
            }
        }
        //update drone model
        private void Updatebt_Click(object sender, RoutedEventArgs e)
        {
            //change the model
            bl.UpdateDrone(selectedDrone.Id, Modeltxtbox.Text);
            //seccessfully update message
            MessageBox.Show("successfully updated");
            //updating
            dr.SelectorChanges();
        }
        //assign drone to parcel
        private void Deliverybt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //assign drone to parcel
                bl.DroneToParcel(selectedDrone.Id);
                //seccessfully asigned message
                MessageBox.Show("Associated successfully");
                //updating
                dr.SelectorChanges();
                selectedDrone = bl.GetDrone(selectedDrone.Id);
                DataContext = selectedDrone;
                //show the delivered parcel
                Parcelbtn.Visibility = Visibility.Visible;
                //change the delivery button state
                Deliverybt.IsEnabled = false;
                //disable charging
                Chargingbt.IsEnabled = false;
            }
            catch (NotFoundException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (DroneStatusException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (EmptyListException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
               
        //charging button click event
        private void Chargingbt_Click(object sender, RoutedEventArgs e)
        {
            if (Chargingbt.Content.ToString() == "Send for charging")
                //send to charge
                SendToChargeb();
            else
                //release from charging
                SendFromCharge();
        }
        // send drone for charging
        private void SendToChargeb()
        {
            try
            {
                //send to charge
                bl.SendToCharge(selectedDrone.Id);
                //charged seccessfully message
                MessageBox.Show("Send for charging successfully");
                //update drone list
                dr.SelectorChanges();
                selectedDrone = bl.GetDrone(selectedDrone.Id);
                DataContext = selectedDrone;
                //disable deliver parcel with the drone
                Deliverybt.IsEnabled = false;
                //change the state of charging button
                Chargingbt.Content = "Realease from charging";
            }
            catch (NotFoundException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (DroneStatusException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (BatteryException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (EmptyListException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        // realse drone from charging
        private void SendFromCharge()
        {
            try
            {
                //Release Drone
                bl.ReleaseDrone(selectedDrone.Id);
                //seccessfully released
                MessageBox.Show("Realse from charging successfully");
                //enable deliver parcel with the drone
                Deliverybt.IsEnabled = true;
                //change the state of charging button
                Chargingbt.Content = "Send for charging";
                //update the list
                dr.SelectorChanges();
                selectedDrone = bl.GetDrone(selectedDrone.Id);
                DataContext = selectedDrone;

            }
            catch (NotFoundException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (DroneStatusException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        //close action drone window
        private bool closeWindow = false;
        private void Exitbt_Click(object sender, RoutedEventArgs e)
        {
            if(worker!=null)
            {
                Cursor = Cursors.Wait;
                worker.CancelAsync();
                closeWindow = true;
                Exitbt.IsEnabled = false;
                Simulatorbt.IsEnabled = false;
            }
            else
               Close();
        }
        //check if only number is entered to id textBox
        private void IdtxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (System.Text.RegularExpressions.Regex.IsMatch(IdtxtBox.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                IdtxtBox.Text = IdtxtBox.Text.Remove(IdtxtBox.Text.Length - 1);
            }
        }
        //check if only number is entered to station id textBox
        private void SIdtxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(SIdtxtBox.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                SIdtxtBox.Text = SIdtxtBox.Text.Remove(SIdtxtBox.Text.Length - 1);
            }
        }

        //show the drone in charging details window
        public DroneWindow(IBL bL, DroneInCharging drone)
        {
            bl = bL;
            InitializeComponent();
            selectedDrone = bl.GetDrone(drone.Id);
            DataContext = selectedDrone;
            this.Height = 450;
            this.Width = 400;
            Updatelbl.Visibility = Visibility.Collapsed;
            Updatebt.Visibility = Visibility.Collapsed;
            Chargingbt.Visibility = Visibility.Collapsed;
            Deliverybt.Visibility = Visibility.Collapsed;
            deliverylbl.Visibility = Visibility.Collapsed;
            Chargelbl.Visibility = Visibility.Collapsed;
            Simulatorbt.Visibility = Visibility.Collapsed;
            //show the actions only
            actions.Visibility = Visibility.Visible;
            addDrone.Visibility = Visibility.Hidden;
            //the drone is not in delivery
            //hide the transfered parcel
            Parcelbtn.Visibility = Visibility.Collapsed;
            Modeltxtbox.IsReadOnly = true;
        }

        //show the drone in parcel details window
        public DroneWindow(IBL bL, DroneInParcel drone)
        {
            bl = bL;
            InitializeComponent();
            selectedDrone = bl.GetDrone(drone.Id);
            DataContext = selectedDrone;
            Updatelbl.Visibility = Visibility.Collapsed;
            Updatebt.Visibility = Visibility.Collapsed;
            Chargingbt.Visibility = Visibility.Collapsed;
            Deliverybt.Visibility = Visibility.Collapsed;
            deliverylbl.Visibility = Visibility.Collapsed;
            Chargelbl.Visibility = Visibility.Collapsed;
            Simulatorbt.Visibility = Visibility.Collapsed;
            //show the actions only
            actions.Visibility = Visibility.Visible;
            addDrone.Visibility = Visibility.Hidden;
            this.Height = 450;
            this.Width = 400;
            //the drone is not in delivery
            //hide the transfered parcel
            Parcelbtn.Visibility = Visibility.Collapsed;
            Modeltxtbox.IsReadOnly = true;
        }

        //open the parcel in transfer window
        private void Parcelbtn_Click(object sender, RoutedEventArgs e)
        {
            prcl= new ParcelInDeliveryWindow(bl, selectedDrone.TransferedParcel, selectedDrone.Id);
            prcl.Show();
        }

        BackgroundWorker worker;

        private void Simulatorbt_Click(object sender, RoutedEventArgs e)
        {
            if (Simulatorbt.Content.ToString() == "Automatic")
            {
                Simulatorbt.Content = "Manual";
                //cover the action buttons
                Updatebt.Visibility = Visibility.Collapsed;
                Deliverybt.Visibility = Visibility.Collapsed;
                Chargingbt.Visibility = Visibility.Collapsed;
                deliverylbl.Visibility = Visibility.Collapsed;
                Chargelbl.Visibility = Visibility.Collapsed;
                Updatelbl.Visibility = Visibility.Collapsed;
                Parcelbtn.Visibility = Visibility.Collapsed;
                playSimulator();
                worker.RunWorkerAsync();
                ReportProgress();
            }
            else
            {
                Cursor = Cursors.Wait;
                worker.CancelAsync();
                Exitbt.IsEnabled = false;
                Simulatorbt.IsEnabled = false;
            }
        }

        //playing the simulator
        private void playSimulator()
        {
            worker = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            bl.playSimulator(selectedDrone.Id, ReportProgress, IsStop);
        }
        public void ReportProgress()
        {
            worker.ReportProgress(0);
        }

        public bool IsStop()
        {
            return worker.CancellationPending;
        }
        private ParcelInDeliveryWindow prcl;
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            selectedDrone = bl.GetDrone(selectedDrone.Id);
            DataContext = selectedDrone;
            if(selectedDrone.TransferedParcel!=null)
            {
                if (prcl != null)
                     prcl.ParcelChange();
                else
                  {
                    prcl = new ParcelInDeliveryWindow(bl, selectedDrone.TransferedParcel, selectedDrone.Id);
                    prcl.Show();
                  }
            }
            else
            {
                if (prcl != null)
                {
                    prcl.Close();
                    prcl = null;
                }
            } 
            dr.SelectorChanges();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (prcl != null)
                prcl.Close();
            if (closeWindow)
                Close();
            else
            {
                //close
                Cursor = Cursors.Arrow;
                Simulatorbt.Content = "Automatic";
                Simulatorbt.IsEnabled = true;
                Exitbt.IsEnabled = true;
                Updatelbl.Visibility = Visibility.Visible;
                Updatebt.Visibility = Visibility.Visible;
                Deliverybt.Visibility = Visibility.Visible;
                Chargingbt.Visibility = Visibility.Visible;
                deliverylbl.Visibility = Visibility.Visible;
                Chargelbl.Visibility = Visibility.Visible;
                if (selectedDrone.Status != DroneStatus.Delivery)
                { //the drone is not in delivery
                  //hide the transfered parcel
                    Parcelbtn.Visibility = Visibility.Collapsed;
                    //enable charging
                    Chargingbt.IsEnabled = true;
                }
                else
                { //the drone is in delivery
                  //show the transfered parcel
                    Parcelbtn.Visibility = Visibility.Visible;
                    //disable charging
                    Chargingbt.IsEnabled = false;
                }
                if (selectedDrone.Status == DroneStatus.Maintenance)
                { //the drone in charging
                  //disable delivery
                    Deliverybt.IsEnabled = false;
                    Chargingbt.Content = "Realease from charging";
                }
                else
                {   //the drone is not in charging
                    //enable delivery
                    if (selectedDrone.Status == DroneStatus.Available)
                        Deliverybt.IsEnabled = true;
                    else
                        Deliverybt.IsEnabled = false;
                    Chargingbt.Content = "Send for charging";
                }
            }
        }

    }
}