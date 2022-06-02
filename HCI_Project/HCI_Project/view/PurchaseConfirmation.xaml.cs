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

namespace HCI_Project.view
{
    /// <summary>
    /// Interaction logic for PurchaseConfirmation.xaml
    /// </summary>
    public partial class PurchaseConfirmation : Window
    {
        public event EventHandler OnConfirm;
        public TicketData Data { get; set; }

        public PurchaseConfirmation(TicketData data)
        {
            InitializeComponent();
            Data = data;
            DataContext = this;

            if (Data.IsReservation) lblConfirm.Content = "Please confirm the reservation";
            else lblConfirm.Content = "Please confirm the purchase";
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            OnConfirm?.Invoke(this, EventArgs.Empty);
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }   
}

public class TicketData
{
    public string From { get; set; }
    public string To { get; set; }
    public string DepartureDateTime { get; set; }
    public string ArrivalDateTime { get; set; }
    public string Wagon { get; set; }
    public string Seat { get; set; }
    public string Price { get; set; }
    public bool IsReservation { get; set; }
}
