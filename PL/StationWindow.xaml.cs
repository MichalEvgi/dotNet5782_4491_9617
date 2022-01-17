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
    /// Interaction logic for StationWindow.xaml
    /// </summary>
    public partial class StationWindow : Window
    {
        IBL bl;
        private StationListWindow st;
        public bool ClosingWindow { get; private set; } = true;
        #region ADD STATION
        /// <summary>
        /// open add station window
        /// </summary>
        /// <param name="bL">IBL interface</param>
        /// <param name="slw">station list window</param>
        public StationWindow(IBL bL, StationListWindow slw)
        {
            bl = bL;
            st = slw;
            InitializeComponent();
            addStaion.Visibility = Visibility.Visible;
            actions.Visibility = Visibility.Hidden;
            this.Height = 450;
            this.Width = 300;
        }
        /// <summary>
        /// close window event. close window if  ClosingWindow=false
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = ClosingWindow;
        }
        //close window by cancel button
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClosingWindow = false;
            this.Close();
        }
        /// <summary>
        /// add station to database according to the fill text boxes
        /// </summary>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IdtxtBox.Text == "" || NametxtBox.Text == "" || LngtxtBox.Text == "" || LattxtBox.Text == "" || SlotstxtBox.Text=="")
                    //not all the fields are full
                    MessageBox.Show("Not all fields are full");
                else
                {
                    //add the drone
                    bl.AddStation(new Station
                    {
                        Id = Convert.ToInt32(IdtxtBox.Text),
                        Name = Convert.ToInt32(NametxtBox.Text),
                        LocationS = new Location { Longitude = Convert.ToDouble(LngtxtBox.Text), Lattitude = Convert.ToDouble(LattxtBox.Text) },
                        AvailableSlots = Convert.ToInt32(SlotstxtBox.Text)
                    });
                    SelectionChange();
                    //seccessfully added message
                    MessageBoxResult result = MessageBox.Show("Seccessfuly added");
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
            catch (InvalidInputException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (AlreadyExistsException ex)
            {
                MessageBox.Show(ex.ToString());
            }
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
        //check if only number is entered to name textBox
        private void NametxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(NametxtBox.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                NametxtBox.Text = NametxtBox.Text.Remove(NametxtBox.Text.Length - 1);
            }
        }
        //check if only number is entered to longitude textBox
        private void LngtxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(LngtxtBox.Text, "[^0-9 .]"))
            {
                MessageBox.Show("Please enter only numbers.");
                LngtxtBox.Text = LngtxtBox.Text.Remove(LngtxtBox.Text.Length - 1);
            }
        }
        //check if only number is entered to lattitude textBox
        private void LattxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(LattxtBox.Text, "[^0-9 .]"))
            {
                MessageBox.Show("Please enter only numbers.");
                LattxtBox.Text = LattxtBox.Text.Remove(LattxtBox.Text.Length - 1);
            }
        }
        //check if only number is entered to slots textBox
        private void SlotstxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(SlotstxtBox.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                SlotstxtBox.Text = SlotstxtBox.Text.Remove(SlotstxtBox.Text.Length - 1);
            }
        }
        #endregion
        #region STATION ACTIONS
        public Station selectedStation;
        /// <summary>
        /// open station actions window
        /// </summary>
        /// <param name="bL">IBL interface</param>
        /// <param name="station">selected station</param>
        /// <param name="slw">station list window</param>
        public StationWindow(IBL bL, StationToList station, StationListWindow slw)
        {
            bl = bL;
            st = slw;
            InitializeComponent();
            selectedStation = bl.GetStation(station.Id);
            DataContext = selectedStation;
            addStaion.Visibility = Visibility.Hidden;
            actions.Visibility = Visibility.Visible;
            this.Height = 380;
            this.Width = 300;
        }

        /// <summary>
        /// refresh the station list view
        /// </summary>
        private void SelectionChange()
        {
            if (st.SlotSelector == null || st.SlotSelector.Text == "Clear filter")
                st.StationListView.ItemsSource = bl.GetStationsList();
            else
                st.StationListView.ItemsSource = bl.AvailableStations();
        }
        
        //check if only number is entered to slots textBox
        private void AllSlotstxtbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(AllSlotstxtbox.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                AllSlotstxtbox.Text = AllSlotstxtbox.Text.Remove(AllSlotstxtbox.Text.Length - 1);
            }
        }
        //check if only number is entered to name textBox
        private void Nametxtbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(Nametxtbox.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                Nametxtbox.Text = NametxtBox.Text.Remove(Nametxtbox.Text.Length - 1);
            }
        }
        
        /// <summary>
        /// click on exit button event
        /// </summary>
        private void Exitbt_Click(object sender, RoutedEventArgs e)
        {
            ClosingWindow = false;
            this.Close();
        }
        /// <summary>
        /// update the name and/or the total slots amount
        /// </summary>
        private void Updatebt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //change the model
                if (AllSlotstxtbox.Text != "")
                    bl.UpdateStation(selectedStation.Id, Convert.ToInt32(Nametxtbox.Text), Convert.ToInt32(AllSlotstxtbox.Text));
                else
                    bl.UpdateStation(selectedStation.Id, Convert.ToInt32(Nametxtbox.Text), -1);
                //seccessfully update message
                SelectionChange();
                MessageBox.Show("successfully updated");
            }
            catch(InvalidInputException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        /// <summary>
        /// open the list of all drones charging in this station
        /// </summary>
        private void ChargeListbt_Click(object sender, RoutedEventArgs e)
        {
            if (selectedStation.DronesInCharging.Count() == 0)
                MessageBox.Show("There are no drones in charging");
            else
            new ChargingListWindow(bl, selectedStation).Show();
        }
        #endregion
    }
}
