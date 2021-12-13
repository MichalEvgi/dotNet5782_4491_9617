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

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneWindow.xaml
    /// </summary>
    public partial class DroneWindow : Window
    {
        IBL bl;
        public ObservableCollection<DroneToList> droneTos;
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
                    MessageBox.Show("לא כל השדות מלאים");
                else
                {
                    //add the drone
                    bl.AddDrone(new Drone { Id = Convert.ToInt32(IdtxtBox.Text), Model = ModeltxtBox.Text, MaxWeight = (WeightCategories)WeightCmb.SelectedItem }, Convert.ToInt32(SIdtxtBox.Text));
                    //update
                    dr.droneTos.Add(bl.GetDroneTo(Convert.ToInt32(IdtxtBox.Text)));
                    //seccessfully added message
                    MessageBoxResult result = MessageBox.Show("נוסף בהצלחה");
                    if (result == MessageBoxResult.OK)
                    {
                        dr.SelectorChanges();
                        //close when OK pressed
                        this.Close();
                    }
                }
            }
            catch (NotFoundException ex)
            {
                MessageBox.Show(ex.ToString());
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

        public Drone selectedDrone;
        public object s_SystemMenuHandle { get; private set; }
        public object Handle { get; private set; }
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
            //fill the textBox of the drone properties
            Idtxtbox.Text = selectedDrone.Id.ToString();
            Batterytxtbox.Text = selectedDrone.Battery.ToString();
            Weighttxtbox.Text = selectedDrone.MaxWeight.ToString();
            Modeltxtbox.Text = selectedDrone.Model;
            Statustxtbox.Text = selectedDrone.Status.ToString();
            locationtxtbox.Text = selectedDrone.CurrentLocation.ToString();
            if (selectedDrone.Status != DroneStatus.Delivery)
            { //the drone is not in delivery
                //hide the transfered parcel
                Deliverytxtbox.Visibility = Visibility.Hidden;
                Deliverylbl.Visibility = Visibility.Hidden;
                Deliverybt.Content = "שיוך חבילה";
                //enable charging
                Chargingbt.IsEnabled = true;
            }
            else
            { //the drone is in delivery
                //show the transfered parcel
                Deliverytxtbox.Text = selectedDrone.TransferedParcel.ToString();
                Deliverytxtbox.Visibility = Visibility.Visible;
                Deliverylbl.Visibility = Visibility.Visible;
                //disable charging
                Chargingbt.IsEnabled = false;
                if (!selectedDrone.TransferedParcel.OnTheWay)
                    Deliverybt.Content = "איסוף חבילה";
                else
                    Deliverybt.Content = "אספקת חבילה";
            }
            if(selectedDrone.Status== DroneStatus.Maintenance)
            { //the drone in charging
              //disable delivery
                Deliverybt.IsEnabled = false;
                Chargingbt.Content = "שחרור מטעינה";
            }
            else
            {   //the drone is not in charging
                //enable delivery
                Deliverybt.IsEnabled = true;
                Chargingbt.Content = "שליחה לטעינה";
            }
            TimeLbl.Visibility = Visibility.Hidden;
            TimeTxt.Visibility = Visibility.Hidden;
            ReleaseBt.Visibility = Visibility.Hidden;
        }
        //update drone model
        private void Updatebt_Click(object sender, RoutedEventArgs e)
        {
            //change the model
            bl.UpdateDrone(selectedDrone.Id, Modeltxtbox.Text);
            //seccessfully update message
            MessageBox.Show("עודכן בהצלחה");
            //updating
            dr.SelectorChanges();
        }
        // delivery button click event
        private void Deliverybt_Click(object sender, RoutedEventArgs e)
        {
            if (Deliverybt.Content.ToString() == "שיוך חבילה")
                //Assign parcel state
                AssignParcel();
            else
                if (Deliverybt.Content.ToString() == "איסוף חבילה")
                //Pick parcel state
                PickParcel();
            else
                //Deliver parcel state
                DeliverParcel();
        }
        //assign drone to parcel
        private void AssignParcel()
        {
            try
            {
                //assign drone to parcel
                bl.DroneToParcel(selectedDrone.Id);
                //seccessfully asigned message
                MessageBox.Show("שיוך בהצלחה");
                //updating
                dr.SelectorChanges();
                selectedDrone = bl.GetDrone(selectedDrone.Id);
                DataContext = selectedDrone;
                //show the delivered parcel
                Deliverytxtbox.Text = selectedDrone.TransferedParcel.ToString();
                Deliverylbl.Visibility = Visibility.Visible;
                Deliverytxtbox.Visibility = Visibility.Visible;
                //change the delivery button state
                Deliverybt.Content = "איסוף חבילה";
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
        //pick parcel
        private void PickParcel()
        {
            try
            {
                //pick parcel
                bl.PickParcel(selectedDrone.Id);
                //seccessfuly picked up message
                MessageBox.Show("נאסף בהצלחה");
                //updating
                locationtxtbox.Text = selectedDrone.TransferedParcel.SourceLocation.ToString();
                dr.SelectorChanges();
                selectedDrone = bl.GetDrone(selectedDrone.Id);
                DataContext = selectedDrone;
                //change delivery button state
                Deliverybt.Content = "אספקת חבילה";
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
        //deliver parcel
        private void DeliverParcel()
        {
            try
            {
                //deliver parcel
                bl.DeliverParcel(selectedDrone.Id);
                //seccessfully delivered message
                MessageBox.Show("סופק בהצלחה");
                //updating
                locationtxtbox.Text = selectedDrone.TransferedParcel.DestinationLocation.ToString();
                dr.SelectorChanges();
                selectedDrone = bl.GetDrone(selectedDrone.Id);
                DataContext = selectedDrone;
                //hide delivered parcel
                Deliverylbl.Visibility = Visibility.Hidden;
                Deliverytxtbox.Visibility = Visibility.Hidden;
                Deliverybt.Content = "שיוך חבילה";
                //enable Charging
                Chargingbt.IsEnabled = true;
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
        //charging button click event
        private void Chargingbt_Click(object sender, RoutedEventArgs e)
        {
            if (Chargingbt.Content.ToString() == "שליחה לטעינה")
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
                MessageBox.Show("נשלח לטעינה בהצלחה");
                //update drone list
                dr.SelectorChanges();
                selectedDrone = bl.GetDrone(selectedDrone.Id);
                DataContext = selectedDrone;
                locationtxtbox.Text = selectedDrone.CurrentLocation.ToString();
                //disable deliver parcel with the drone
                Deliverybt.IsEnabled = false;
                //change the state of charging button
                Chargingbt.Content = "שחרור מטעינה";
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
        //enable realse drone from charging
        private void SendFromCharge()
        {
            TimeLbl.Visibility = Visibility.Visible;
            TimeTxt.Visibility = Visibility.Visible;
            ReleaseBt.Visibility = Visibility.Visible;
        }
        //realse drone from charging
        private void ReleaseBt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Release Drone
                bl.ReleaseDrone(selectedDrone.Id, Convert.ToDouble(TimeTxt.Text));
                //hide all the buttons of releasing drone
                TimeLbl.Visibility = Visibility.Hidden;
                TimeTxt.Visibility = Visibility.Hidden;
                ReleaseBt.Visibility = Visibility.Hidden;
                TimeTxt.Text = "";
                //seccessfully released
                MessageBox.Show("שוחרר מטעינה בהצלחה");
                //enable deliver parcel with the drone
                Deliverybt.IsEnabled = true;
                //change the state of charging button
                Chargingbt.Content = "שליחה לטעינה";
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
        private void Exitbt_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
        //check if only number is entered to time textBox
        private void TimeTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(TimeTxt.Text, "[^0-9 .]"))
            {
                MessageBox.Show("Please enter only numbers.");
                TimeTxt.Text = TimeTxt.Text.Remove(TimeTxt.Text.Length - 1);
            }
        }        
    }
}