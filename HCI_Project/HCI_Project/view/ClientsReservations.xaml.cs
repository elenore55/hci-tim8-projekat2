using HCI_Project.model;
using HCI_Project.repository;
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
using System.Windows.Navigation;

namespace HCI_Project.view
{
    /// <summary>
    /// Interaction logic for ClientsReservations.xaml
    /// </summary>
    public partial class ClientsReservations : Page
    {
        public List<ReservationDTO> Rows { get; set; }
        private readonly RepositoryFactory rf;
        private string email = "milica@gmail.com";

        public ClientsReservations(RepositoryFactory rf)
        {
            InitializeComponent();
            this.rf = rf;
            DataContext = this;
            Rows = new List<ReservationDTO>();
            DisplayReservations();
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            DisplayReservations();
        }

        private void DisplayReservations()
        {
            List<Reservation> reservations = rf.ReservationRepository.GetByClient(email);
            Rows.Clear();
            foreach (Reservation r in reservations)
            {
                if (!r.IsActive) continue;
                Departure departure = rf.DepartureRepository.GetById(r.DepartureId);
                Line line = rf.LineRepository.GetById(departure.LineId);
                Seat seat = rf.SeatRepository.GetById(r.SeatId);
                Wagon wagon = rf.WagonRepository.GetById(seat.WagonId);
                ReservationDTO dto = new ReservationDTO()
                {
                    DateTimeOfPurchaseStr = $"{r.ReservationDateTime}",
                    DateTimeOfDepartureStr = $"{r.DepartureDate.ToShortDateString()} {departure.StartTime.ToShortTimeString()}",
                    Destination = $"{line.GetStartStation().Name} - {line.GetEndStation().Name}",
                    Price = line.Price,
                    SeatStr = $"Seat:        {seat.Row + 1}{Convert.ToChar(65 + seat.Column)}",
                    WagonStr = $"Wagon:   Number {wagon.Ordinal + 1} ({wagon.Class} class)",
                    ActiveUntilStr = $"{r.DepartureDate.AddDays(-3).ToShortDateString()}",
                };
                Rows.Add(dto);
            }
            if (Rows.Count > 0)
            {
                ticketsGrid.Visibility = Visibility.Visible;
                lblNoResults.Visibility = Visibility.Hidden;
            }
            else
            {
                ticketsGrid.Visibility = Visibility.Hidden;
                lblNoResults.Visibility = Visibility.Visible;
            }
        }

        private void btnPurchase_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ticketsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnPurchase.IsEnabled = true;
        }
    }

    public class ReservationDTO
    {
        public string DateTimeOfPurchaseStr { get; set; }
        public string DateTimeOfDepartureStr { get; set; }
        public string Destination { get; set; }
        public double Price { get; set; }
        public string SeatStr { get; set; }
        public string WagonStr { get; set; }
        public string SeatDetails
        {
            get
            {
                return $"{WagonStr}\n{SeatStr}";
            }
        }
        public string ActiveUntilStr { get; set; }
    }
}
