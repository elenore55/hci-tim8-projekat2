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

namespace HCI_Project.view
{
    /// <summary>
    /// Interaction logic for ClientsTickets.xaml
    /// </summary>
    public partial class ClientsTickets : Page
    {
        private RepositoryFactory rf;
        private string email = "milica@gmail.com";

        public List<TicketDTO> Rows { get; set; }

        public ClientsTickets(RepositoryFactory rf)
        {
            InitializeComponent();
            this.rf = rf;
            Rows = new List<TicketDTO>();
            DataContext = this;
            DisplayTickets();
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            DisplayTickets();
        }

        private void DisplayTickets()
        {
            List<Ticket> tickets = rf.TicketRepository.GetByClient(email);
            Rows.Clear();
            foreach (Ticket t in tickets)
            {
                Departure departure = rf.DepartureRepository.GetById(t.DepartureId);
                Line line = rf.LineRepository.GetById(departure.LineId);
                Seat seat = rf.SeatRepository.GetById(t.SeatId);
                Wagon wagon = rf.WagonRepository.GetById(seat.WagonId);
                TicketDTO dto = new TicketDTO()
                {
                    DateTimeOfPurchaseStr = $"{t.PurchaseDateTime}",
                    DateTimeOfDepartureStr = $"{t.DepartureDate.ToShortDateString()} {departure.StartTime.ToShortTimeString()}",
                    Destination = $"{line.GetStartStation().Name} - {line.GetEndStation().Name}",
                    Price = line.Price,
                    SeatStr = $"{seat.Row + 1}{Convert.ToChar(65 + seat.Column)} ({wagon.Class} class)",
                };
                Rows.Add(dto);
            }
        }
    }

    public class TicketDTO
    {
        public string DateTimeOfPurchaseStr { get; set; }
        public string DateTimeOfDepartureStr { get; set; }
        public string Destination { get; set; }
        public double Price { get; set; }
        public string SeatStr { get; set; }
        public string WagonStr { get; set; }
    }
}
