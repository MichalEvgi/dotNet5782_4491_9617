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
        public DroneWindow(IBL.IBL bL, ObservableCollection<DroneToList> droneTo)
        {
            bl = bL;
            InitializeComponent();
            WeightCmb.ItemsSource = Enum.GetValues(typeof(WeightCategories));
            addDrone.Visibility = Visibility.Visible;
            actions.Visibility = Visibility.Hidden;
            droneTos = droneTo;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.AddDrone(new Drone { Id = Convert.ToInt32(IdtxtBox.Text), Model = ModeltxtBox.Text, MaxWeight = (WeightCategories)WeightCmb.SelectedItem }, Convert.ToInt32(SIdtxtBox.Text));
                droneTos.Add(bl.GetDroneTo(Convert.ToInt32(IdtxtBox.Text)));
                MessageBoxResult result = MessageBox.Show("נוסף בהצלחה");
                if(result== MessageBoxResult.OK)
                {
                    this.Close();
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
        public DroneWindow(IBL.IBL bL,DroneToList drone)
        {
            bl = bL;
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
            if (selectedDrone.Status == IBL.BO.DroneStatus.Delivery)
            {
                Deliverytxtbox.Text = selectedDrone.TransferedParcel.ToString();
                Deliverytxtbox.Visibility = Visibility.Visible;
                Deliverylbl.Visibility = Visibility.Visible;
            }
            else
            {
                Deliverytxtbox.Visibility = Visibility.Hidden;
                Deliverylbl.Visibility = Visibility.Hidden;
            }
            locationtxtbox.Text = selectedDrone.CurrentLocation.ToString();
        }

        private void Updatebt_Click(object sender, RoutedEventArgs e)
        {
            bl.UpdateDrone(selectedDrone.Id, ModeltxtBox.Text);
            MessageBox.Show("עודכן בהצלחה");
        }

        private void AssignParcelbt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.DroneToParcel(selectedDrone.Id);
                MessageBox.Show("שיוך בהצלחה");
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

        private void PickParcelbt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.PickParcel(selectedDrone.Id);
                MessageBox.Show("נאסף בהצלחה");
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

        private void DeliverParcelbt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.DeliverParcel(selectedDrone.Id);
                MessageBox.Show("סופק בהצלחה");
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

        private void SendToChargebt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.SendToCharge(selectedDrone.Id);
                MessageBox.Show("נשלח לטעינה בהצלחה");
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

        private void SendFromChargebt_Click(object sender, RoutedEventArgs e)
        {
            TimeLbl.Visibility = Visibility.Visible;
            TimeTxt.Visibility = Visibility.Visible;
            ReleaseBt.Visibility = Visibility.Visible;
        }

        private void ReleaseBt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.ReleaseDrone(selectedDrone.Id, Convert.ToInt32(TimeTxt.Text));
                TimeLbl.Visibility = Visibility.Hidden;
                TimeTxt.Visibility = Visibility.Hidden;
                ReleaseBt.Visibility = Visibility.Hidden;
                MessageBox.Show("שוחרר מטעינה בהצלחה");
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
    }
}
