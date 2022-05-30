using HCI_Project.model;
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
using HCI_Project.repository;

namespace HCI_Project.view
{
    /// <summary>
    /// Interaction logic for SeatChoice.xaml
    /// </summary>
    /// 

    public partial class SeatChoice : Page
    {
        public int NumberOfRows { get; set; }
        public int NumberOfColumns { get; set; }
        public DateTime DepartureDate { get; set; }
        public Departure Departure { get; set; }

        private ReservationRepository reservationRepository;
        private TicketRepository ticketRepository;

        public SeatChoice(Departure departure, DateTime date, TicketRepository ticketRepository, ReservationRepository reservationRepository)
        {
            InitializeComponent();
            DataContext = this;

            this.reservationRepository = reservationRepository;
            this.ticketRepository = ticketRepository;

            Train train = departure.Train;
            Wagon wagon = train.Wagons[0];
            NumberOfRows = wagon.Rows;
            NumberOfColumns = wagon.SeatsPerRow;
            List<Seat> seats = wagon.Seats;
            DepartureDate = date;
            Departure = departure;

            for (int i = 0; i < NumberOfRows; i++)
            {
                var rowDef = new RowDefinition
                {
                    Height = GridLength.Auto
                };
                seatsGrid.RowDefinitions.Add(rowDef);
            }
            for (int i = 0; i < NumberOfColumns; i++)
            {
                var colDef = new ColumnDefinition
                {
                    Width = GridLength.Auto
                };
                seatsGrid.ColumnDefinitions.Add(colDef);
            }
            int count = 0;
            foreach (Seat seat in seats)
            {
                Button seatBtn = new Button()
                {
                    Content = $"Seat {++count}",
                    Background = (SolidColorBrush)new BrushConverter().ConvertFrom(GetButtonColor(seat)),
                    Foreground = Brushes.Black,
                    Margin = GetMargin(seat.Column)
                };
                Grid.SetColumn(seatBtn, seat.Column);
                Grid.SetRow(seatBtn, seat.Row);
                seatsGrid.Children.Add(seatBtn);
            }
        }

        private Thickness GetMargin(int j)
        {
            if (j == NumberOfColumns / 2 - 1) return new Thickness(5, 5, 20, 5);
            if (j == NumberOfColumns / 2) return new Thickness(20, 5, 5, 5);
            return new Thickness(5, 5, 5, 5);
        }

        private bool IsSeatPurchased(Seat seat)
        {
            List<Ticket> tickets = ticketRepository.GetAll();
            foreach (Ticket ticket in tickets)
            {
                if (ticket.SeatId == seat.Id && ticket.DepartureId == Departure.Id && ticket.DepartureDate == DepartureDate)
                    return true;
            }
            return false;
        }

        private bool IsSeatReserved(Seat seat)
        {
            List<Reservation> reservations = reservationRepository.GetAll();
            foreach (Reservation res in reservations)
            {
                if (res.IsActive && res.SeatId == seat.Id && res.DepartureId == Departure.Id && res.DepartureDate == DepartureDate)
                {
                    return true;
                }
            }
            return false;
        }

        private string GetButtonColor(Seat seat)
        {
            if (IsSeatPurchased(seat)) return "#e38d8d";
            if (IsSeatReserved(seat)) return "#f5e189";
            return "#c7e8b7";
        }
    }
}
