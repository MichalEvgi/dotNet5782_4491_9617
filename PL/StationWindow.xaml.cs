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
    /// Interaction logic for StationWindow.xaml
    /// </summary>
    public partial class StationWindow : Window
    {
        IBL bl;
        private StationListWindow st;
        public StationWindow(IBL bL, StationListWindow slw)
        {
            bl = bL;
            st = slw;
            InitializeComponent();
            addStaion.Visibility = Visibility.Visible;
            actions.Visibility = Visibility.Hidden;
        }
        //close window by cancel button
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IdtxtBox.Text == "" || NametxtBox.Text == "" || LngtxtBox.Text == "" || LattxtBox.Text == "" || SlotstxtBox.Text=="")
                    //not all the fields are full
                    MessageBox.Show("לא כל השדות מלאים");
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
                    //seccessfully added message
                    MessageBoxResult result = MessageBox.Show("Seccessfuly added");
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
            catch (InvalidInputException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (AlreadyExistsException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public Station selectedStation;
        public StationWindow(IBL bL, StationToList station, StationListWindow dlw)
        {
            bl = bL;
            InitializeComponent();
            selectedStation = bl.GetStation(station.Id);
            DataContext = selectedStation;
            addStaion.Visibility = Visibility.Hidden;
            actions.Visibility = Visibility.Visible;
        }

        private void Exitbt_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Updatebt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //change the model
                bl.UpdateStation(selectedStation.Id, Convert.ToInt32(Nametxtbox.Text), Convert.ToInt32(AllSlotstxtbox.Text));
                //seccessfully update message
                MessageBox.Show("successfully updated");
            }
            catch(InvalidInputException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
