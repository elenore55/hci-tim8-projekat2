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
using WPFCustomMessageBox;

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

        private readonly string FREE = "#c7e8b7";
        private readonly string RESERVED = "#f5e189";
        private readonly string TAKEN = "#e38d8d";
        private readonly string SELECTED = "#4bab65";
        private readonly string FIRST_CLASS = "#d1d19b";
        private readonly string SECOND_CLASS = "#8ba6b3";
        private readonly string FIRST_CLASS_SELECTED = "#ebeb50";
        private readonly string SECOND_CLASS_SELECTED = "#63c0eb";

        private Button selectedSeat;
        private Button selectedWagon;

        private readonly ReservationRepository reservationRepository;
        private readonly TicketRepository ticketRepository;

        public SeatChoice(Departure departure, DateTime date, TicketRepository ticketRepository, ReservationRepository reservationRepository)
        {
            InitializeComponent();
            DataContext = this;

            this.reservationRepository = reservationRepository;
            this.ticketRepository = ticketRepository;

            Train train = departure.Train;
            wagonsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            foreach (Wagon w in train.Wagons)
            {
                wagonsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
            for (int i = 0; i < train.Wagons.Count; i++)
            {
                Button wagonBtn = new Button()
                {
                    Content = $"Wagon {i}",
                    Background = (SolidColorBrush)new BrushConverter().ConvertFrom(GetWagonButtonColor(train.Wagons[i])),
                    Foreground = Brushes.Black,
                    RenderTransform = new RotateTransform(90, 0, 0),
                    Margin = new Thickness(0, 35, 0, 35),
                    MinHeight = 40
                };
                wagonBtn.Click += new RoutedEventHandler(wagonBtn_Click);
                Grid.SetColumn(wagonBtn, 0);
                Grid.SetRow(wagonBtn, i);
                wagonsGrid.Children.Add(wagonBtn);
            }
            DepartureDate = date;
            Departure = departure;
        }

        private void seatBtn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != selectedSeat) 
            {
                foreach (Button child in seatsGrid.Children)
                {
                    if (child.IsEnabled)
                    {
                        child.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(FREE);
                    }    
                }
                btn.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(SELECTED);
                btnReserve.IsEnabled = true;
                btnPurchase.IsEnabled = true;
                selectedSeat = btn;
            } 
            else
            {
                btn.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(FREE);
                btnReserve.IsEnabled = false;
                btnPurchase.IsEnabled = false;
                selectedSeat = null;
            }
        }

        private void wagonBtn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == selectedWagon) return;
            Wagon wagon = Departure.Train.Wagons[Grid.GetRow(btn)];
            if (wagon.Class == WagonClass.First)
            {
                btn.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(FIRST_CLASS_SELECTED);
            }
            else
            {
                btn.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(SECOND_CLASS_SELECTED);
            }
            if (selectedWagon != null)
            {
                Wagon prev = Departure.Train.Wagons[Grid.GetRow(selectedWagon)];
                if (prev.Class == WagonClass.First)
                {
                    selectedWagon.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(FIRST_CLASS);
                }
                else
                {
                    selectedWagon.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(SECOND_CLASS);
                }
            }
            selectedWagon = btn;

            NumberOfRows = wagon.Rows;
            NumberOfColumns = wagon.SeatsPerRow;
            List<Seat> seats = wagon.Seats;
            seatsGrid.ColumnDefinitions.Clear();
            seatsGrid.RowDefinitions.Clear();
            seatsGrid.Children.Clear();

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
                    Background = (SolidColorBrush)new BrushConverter().ConvertFrom(GetSeatButtonColor(seat)),
                    Foreground = Brushes.Black,
                    Margin = GetMargin(seat.Column),
                    IsEnabled = IsSeatFree(seat)
                };
                seatBtn.Click += new RoutedEventHandler(seatBtn_Click);
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

        private bool IsSeatFree(Seat seat)
        {
            return !IsSeatPurchased(seat) && !IsSeatReserved(seat);
        }

        private string GetSeatButtonColor(Seat seat)
        {
            if (IsSeatPurchased(seat)) return TAKEN;
            if (IsSeatReserved(seat)) return RESERVED;
            return FREE;
        }

        private string GetWagonButtonColor(Wagon wagon)
        {
            if (wagon.Class == WagonClass.First) return FIRST_CLASS;
            return SECOND_CLASS;
        }

        private void btnReserve_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ticked successfully reserved!");
            selectedSeat.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(RESERVED);
            selectedSeat.IsEnabled = false;
            btnReserve.IsEnabled = false;
            btnPurchase.IsEnabled = false;
        }

        private void btnPurchase_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ticket successfully reserved!");
            selectedSeat.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(TAKEN);
            selectedSeat.IsEnabled = false;
            btnReserve.IsEnabled = false;
            btnPurchase.IsEnabled = false;
        }
    }
}
