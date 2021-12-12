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
using IBL.BO;
using IBL;
using System.Collections.ObjectModel;


namespace PL
{
    /// <summary>
    /// Interaction logic for DroneWindow.xaml
    /// </summary>
    public partial class DroneWindow : Window
    {
        IBL.IBL bl;
        public ObservableCollection<DroneToList> droneTos;
        private DroneListWindow dr;
        public DroneWindow(IBL.IBL bL, DroneListWindow dlw)
        {
            bl = bL;
            dr = dlw;
            InitializeComponent();
            WeightCmb.ItemsSource = Enum.GetValues(typeof(WeightCategories));
            addDrone.Visibility = Visibility.Visible;
            actions.Visibility = Visibility.Hidden;
            //droneTos = droneTo;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IdtxtBox.Text == "" || ModeltxtBox.Text == "" || WeightCmb.SelectedItem == null || SIdtxtBox.Text == "")
                    MessageBox.Show("לא כל השדות מלאים");
                else
                {
                    bl.AddDrone(new Drone { Id = Convert.ToInt32(IdtxtBox.Text), Model = ModeltxtBox.Text, MaxWeight = (WeightCategories)WeightCmb.SelectedItem }, Convert.ToInt32(SIdtxtBox.Text));
                    dr.droneTos.Add(bl.GetDroneTo(Convert.ToInt32(IdtxtBox.Text)));
                    MessageBoxResult result = MessageBox.Show("נוסף בהצלחה");
                    if (result == MessageBoxResult.OK)
                    {
                        dr.SelectorChanges();
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


        private void IdtxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (System.Text.RegularExpressions.Regex.IsMatch(IdtxtBox.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                IdtxtBox.Text = IdtxtBox.Text.Remove(IdtxtBox.Text.Length - 1);
            }
        }

        private void SIdtxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(SIdtxtBox.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                SIdtxtBox.Text = SIdtxtBox.Text.Remove(SIdtxtBox.Text.Length - 1);
            }
        }

        public Drone selectedDrone;

        public object s_SystemMenuHandle { get; private set; }
        public object Handle { get; private set; }

        public DroneWindow(IBL.IBL bL, DroneToList drone,DroneListWindow dl)
        {
            bl = bL;
            dr = dl;
            InitializeComponent();
            selectedDrone = bl.GetDrone(drone.Id);
            DataContext = selectedDrone;
            actions.Visibility = Visibility.Visible;
            addDrone.Visibility = Visibility.Hidden;
            Idtxtbox.Text = selectedDrone.Id.ToString();
            Batterytxtbox.Text = selectedDrone.Battery.ToString();
            Weighttxtbox.Text = selectedDrone.MaxWeight.ToString();
            Modeltxtbox.Text = selectedDrone.Model;
            Statustxtbox.Text = selectedDrone.Status.ToString();
            locationtxtbox.Text = selectedDrone.CurrentLocation.ToString();
            if (selectedDrone.Status != DroneStatus.Delivery)
            {
                Deliverytxtbox.Visibility = Visibility.Hidden;
                Deliverylbl.Visibility = Visibility.Hidden;
                Deliverybt.Content = "שיוך חבילה";
                Chargingbt.IsEnabled = true;
            }
            else
            {
                Deliverytxtbox.Text = selectedDrone.TransferedParcel.ToString();
                Deliverytxtbox.Visibility = Visibility.Visible;
                Deliverylbl.Visibility = Visibility.Visible;
                Chargingbt.IsEnabled = false;
                if (!selectedDrone.TransferedParcel.OnTheWay)
                    Deliverybt.Content = "איסוף חבילה";
                else
                    Deliverybt.Content = "אספקת חבילה";
            }
            if(selectedDrone.Status== DroneStatus.Maintenance)
            {
                Deliverybt.IsEnabled = false;
                Chargingbt.Content = "שחרור מטעינה";
            }
            else
            {
                Deliverybt.IsEnabled = true;
                Chargingbt.Content = "שליחה לטעינה";
            }
            TimeLbl.Visibility = Visibility.Hidden;
            TimeTxt.Visibility = Visibility.Hidden;
            ReleaseBt.Visibility = Visibility.Hidden;
        }

        private void Updatebt_Click(object sender, RoutedEventArgs e)
        {
            bl.UpdateDrone(selectedDrone.Id, Modeltxtbox.Text);
            MessageBox.Show("עודכן בהצלחה");
            dr.SelectorChanges();
        }

        private void Deliverybt_Click(object sender, RoutedEventArgs e)
        {
            if (Deliverybt.Content.ToString() == "שיוך חבילה")
                AssignParcel();
            else
                if (Deliverybt.Content.ToString() == "איסוף חבילה")
                PickParcel();
            else
                DeliverParcel();
        }

        private void AssignParcel()
        {
            try
            {
                bl.DroneToParcel(selectedDrone.Id);
                MessageBox.Show("שיוך בהצלחה");
                dr.SelectorChanges();
                selectedDrone = bl.GetDrone(selectedDrone.Id);
                DataContext = selectedDrone;
                Deliverytxtbox.Text = selectedDrone.TransferedParcel.ToString();
                Deliverylbl.Visibility = Visibility.Visible;
                Deliverytxtbox.Visibility = Visibility.Visible;
                Deliverybt.Content = "איסוף חבילה";
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

        private void PickParcel()
        {
            try
            {
                bl.PickParcel(selectedDrone.Id);
                MessageBox.Show("נאסף בהצלחה");
                locationtxtbox.Text = selectedDrone.TransferedParcel.SourceLocation.ToString();
                dr.SelectorChanges();
                selectedDrone = bl.GetDrone(selectedDrone.Id);
                DataContext = selectedDrone;
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

        private void DeliverParcel()
        {
            try
            {
                bl.DeliverParcel(selectedDrone.Id);
                MessageBox.Show("סופק בהצלחה");
                locationtxtbox.Text = selectedDrone.TransferedParcel.DestinationLocation.ToString();
                dr.SelectorChanges();
                selectedDrone = bl.GetDrone(selectedDrone.Id);
                DataContext = selectedDrone;
                Deliverylbl.Visibility = Visibility.Hidden;
                Deliverytxtbox.Visibility = Visibility.Hidden;
                Deliverybt.Content = "שיוך חבילה";
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
        private void Chargingbt_Click(object sender, RoutedEventArgs e)
        {
            if (Chargingbt.Content.ToString() == "שליחה לטעינה")
                SendToChargeb();
            else
                SendFromCharge();
        }


        private void SendToChargeb()
        {
            try
            {
                bl.SendToCharge(selectedDrone.Id);
                MessageBox.Show("נשלח לטעינה בהצלחה");
                dr.SelectorChanges();
                selectedDrone = bl.GetDrone(selectedDrone.Id);
                DataContext = selectedDrone;
                locationtxtbox.Text = selectedDrone.CurrentLocation.ToString();
                Deliverybt.IsEnabled = false;
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

        private void SendFromCharge()
        {
            TimeLbl.Visibility = Visibility.Visible;
            TimeTxt.Visibility = Visibility.Visible;
            ReleaseBt.Visibility = Visibility.Visible;
        }

        private void ReleaseBt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.ReleaseDrone(selectedDrone.Id, Convert.ToDouble(TimeTxt.Text));
                TimeLbl.Visibility = Visibility.Hidden;
                TimeTxt.Visibility = Visibility.Hidden;
                ReleaseBt.Visibility = Visibility.Hidden;
                MessageBox.Show("שוחרר מטעינה בהצלחה");
                Deliverybt.IsEnabled = true;
                Chargingbt.Content = "שליחה לטעינה";
                dr.SelectorChanges();
                selectedDrone = bl.GetDrone(selectedDrone.Id);
                DataContext = selectedDrone;
                TimeTxt.Text = "";
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

        private void Exitbt_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

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
