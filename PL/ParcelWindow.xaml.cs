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
        }
        public Parcel selectedParcel;
        public ParcelWindow(IBL bL, ParcelToList parcel, ParcelListWindow plw)
        {
            bl = bL;
            pr = plw;
            selectedParcel = bl.GetParcel(parcel.Id);
            InitializeComponent();
        }
    }
}
