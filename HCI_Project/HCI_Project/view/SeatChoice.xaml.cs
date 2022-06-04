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

        private readonly string FREE = "#c7e8b7";
        private readonly string RESERVED = "#f2d933";
        private readonly string TAKEN = "#ed4d3b";
        private readonly string SELECTED = "#4bab65";
        private readonly string FIRST_CLASS = "#d1d19b";
        private readonly string SECOND_CLASS = "#8ba6b3";
        private readonly string FIRST_CLASS_SELECTED = "#ebeb50";
        private readonly string SECOND_CLASS_SELECTED = "#63c0eb";

        private Button selectedSeat;
        private Button selectedWagon;
        private string selectedWagonClass = "";
        private DepartureDTO departureDTO;
        private bool isReservationPossible;

        private readonly RepositoryFactory rf;

        public SeatChoice(DepartureDTO departureDTO, DateTime date, RepositoryFactory rf)
        {
            InitializeComponent();
            DataContext = this;

            this.rf = rf;
            this.departureDTO = departureDTO;

            isReservationPossible = (date - DateTime.Now).TotalDays >= 3;

            Train train = departureDTO.Train;
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
                    Margin = new Thickness(0, 10, 20, 10),
                    MinHeight = 40,
                    ToolTip = "Click to select the wagon"
                };
                wagonBtn.Click += new RoutedEventHandler(wagonBtn_Click);
                Grid.SetColumn(wagonBtn, 0);
                Grid.SetRow(wagonBtn, i);
                wagonsGrid.Children.Add(wagonBtn);
            }
            DepartureDate = date;
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
                btnReserve.IsEnabled = isReservationPossible;
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
            Wagon wagon = departureDTO.Train.Wagons[Grid.GetRow(btn)];
            selectedWagonClass = wagon.Class.ToString();
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
                Wagon prev = departureDTO.Train.Wagons[Grid.GetRow(selectedWagon)];
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
            trainBorder.BorderThickness = new Thickness(2, 2, 2, 2);
            NumberOfRows = wagon.Rows;
            NumberOfColumns = wagon.SeatsPerRow;
            List<Seat> seats = wagon.Seats;
            seatsGrid.ColumnDefinitions.Clear();
            seatsGrid.RowDefinitions.Clear();
            seatsGrid.Children.Clear();
            numerationGrid.ColumnDefinitions.Clear();
            numerationGrid.RowDefinitions.Clear();

            for (int i = 0; i < NumberOfRows; i++)
            {
                var colDef = new ColumnDefinition
                {
                    Width = GridLength.Auto
                };
                seatsGrid.ColumnDefinitions.Add(colDef);
            }
            for (int i = 0; i < NumberOfColumns; i++)
            {
                var rowDef = new RowDefinition
                {
                    Height = GridLength.Auto
                };
                seatsGrid.RowDefinitions.Add(rowDef);
            }

            for (int i = 0; i <= NumberOfRows; i++)
            {
                var colDef = new ColumnDefinition
                {
                    Width = GridLength.Auto
                };
                numerationGrid.ColumnDefinitions.Add(colDef);
            }
            for (int i = 0; i <= NumberOfColumns; i++)
            {
                var rowDef = new RowDefinition
                {
                    Height = GridLength.Auto
                };
                numerationGrid.RowDefinitions.Add(rowDef);
            }

            foreach (Seat seat in seats)
            {
                Button seatBtn = new Button()
                {
                    Content = $" {seat.Row + 1}{Convert.ToChar(65 + seat.Column)}",
                    Background = (SolidColorBrush)new BrushConverter().ConvertFrom(GetSeatButtonColor(seat)),
                    Foreground = Brushes.Black,
                    Margin = GetMargin(seat.Column),
                    IsEnabled = IsSeatFree(seat),
                    FontSize = 22,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Name = $"btn_{seat.Id}",
                    ToolTip = "Click to select the seat"
                };
                seatBtn.Click += new RoutedEventHandler(seatBtn_Click);
                Grid.SetColumn(seatBtn, seat.Row);
                Grid.SetRow(seatBtn, seat.Column);
                seatsGrid.Children.Add(seatBtn);
            }

            for (int i = 0; i < NumberOfColumns; i++)
            {
                Label lbl = new Label()
                {
                    Content = Convert.ToChar(65 + i).ToString(),
                    FontSize = 20,
                    Margin = GetMargin(i),
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Padding = new Thickness(0, 0, 0, 0)
                };
                Grid.SetRow(lbl, i + 1);
                Grid.SetColumn(lbl, 0);
                numerationGrid.Children.Add(lbl);
            }
            for (int i = 0; i < NumberOfRows; i++)
            {
                Label lbl = new Label()
                {
                    Content = (i + 1).ToString(),
                    FontSize = 20,
                    Margin = new Thickness(0, 0, 0, 7),
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Padding = new Thickness(0, 0, 0, 0)
                };
                Grid.SetColumn(lbl, i + 1);
                Grid.SetRow(lbl, 0);
                numerationGrid.Children.Add(lbl);
            }
            Grid.SetColumnSpan(trainBorder, 5);
            Grid.SetRowSpan(trainBorder, 4);
        }

        private Thickness GetMargin(int j)
        {
            int top = 5;
            int bottom = 5;
            int left = 5;
            int right = 5;
            if (j == NumberOfRows / 2 - 1) bottom = 20;
            if (j == NumberOfRows / 2) top = 20;
            return new Thickness(left, top, right, bottom);
        }

        private Thickness GetLabelMargin(int j)
        {
            int top = 5;
            int bottom = 0;
            int left = 0;
            int right = 7;
            if (j == 0) top = 12;
            if (j == NumberOfRows / 2) top = 20;
            return new Thickness(left, top, right, bottom);
        }

        private bool IsSeatPurchased(Seat seat)
        {
            List<Ticket> tickets = rf.TicketRepository.GetAll();
            foreach (Ticket ticket in tickets)
            {
                if (ticket.SeatId == seat.Id && ticket.DepartureId == departureDTO.Id && ticket.DepartureDate == DepartureDate)
                    return true;
            }
            return false;
        }

        private bool IsSeatReserved(Seat seat)
        {
            List<Reservation> reservations = rf.ReservationRepository.GetAll();
            foreach (Reservation res in reservations)
            {
                if (res.IsActive && res.SeatId == seat.Id && res.DepartureId == departureDTO.Id && res.DepartureDate == DepartureDate)
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
            TicketData td = new TicketData
            {
                From = departureDTO.Line.Stations[departureDTO.StartIndex].Name,
                To = departureDTO.Line.Stations[departureDTO.EndIndex].Name,
                DepartureDateTime = $"{DepartureDate.ToShortDateString()} {departureDTO.DepartureTimeStr}",
                ArrivalDateTime = $"{DepartureDate.ToShortDateString()} {departureDTO.ArrivalTimeStr}",
                Wagon = $"Number {Grid.GetRow(selectedWagon) + 1}, {selectedWagonClass} class",
                Seat = $"{Grid.GetColumn(selectedSeat) + 1}{Convert.ToChar(65 + Grid.GetRow(selectedSeat))}",
                Price = $"{departureDTO.Price} EUR",
                IsReservation = true
            };
            PurchaseConfirmation confirmation = new PurchaseConfirmation(td);
            confirmation.OnConfirm += SaveReservation;
            confirmation.ShowDialog();
        }

        private void btnPurchase_Click(object sender, RoutedEventArgs e)
        {
            TicketData td = new TicketData
            {
                From = departureDTO.Line.Stations[departureDTO.StartIndex].Name,
                To = departureDTO.Line.Stations[departureDTO.EndIndex].Name,
                DepartureDateTime = $"{DepartureDate.ToShortDateString()} {departureDTO.DepartureTimeStr}",
                ArrivalDateTime = $"{DepartureDate.ToShortDateString()} {departureDTO.ArrivalTimeStr}",
                Wagon = $"Number {Grid.GetRow(selectedWagon) + 1}, {selectedWagonClass} class",
                Seat = $"{Grid.GetColumn(selectedSeat) + 1}{Convert.ToChar(65 + Grid.GetRow(selectedSeat))}",
                Price = $"{departureDTO.Price} EUR",
                IsReservation = false
            };
            PurchaseConfirmation confirmation = new PurchaseConfirmation(td);
            confirmation.OnConfirm += SavePurchase;
            confirmation.ShowDialog();
        }

        public void SavePurchase(object sender, EventArgs e)
        {
            MessageBox.Show("Ticket successfully purchased!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            DisableSelectedSeat();
            selectedSeat.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(TAKEN);
            Departure dpt = rf.DepartureRepository.GetById(departureDTO.Id);
            Line line = rf.LineRepository.GetById(dpt.LineId);
            Ticket ticket = new Ticket()
            {
                Id = rf.TicketRepository.GetNextId(),
                PurchaseDateTime = DateTime.Now,
                DepartureDate = DepartureDate,
                ClientEmail = "milica@gmail.com",
                SeatId = long.Parse(selectedSeat.Name.Substring(4)),
                DepartureId = departureDTO.Id,
                StartStation = line.Stations[departureDTO.StartIndex].Name,
                EndStation = line.Stations[departureDTO.EndIndex].Name
            };
            rf.TicketRepository.Add(ticket);
        }

        public void SaveReservation(object sender, EventArgs e)
        {
            MessageBox.Show("Ticket successfully reserved!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            DisableSelectedSeat();
            selectedSeat.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(RESERVED);
            Departure dpt = rf.DepartureRepository.GetById(departureDTO.Id);
            Line line = rf.LineRepository.GetById(dpt.LineId);
            Reservation reservation = new Reservation()
            {
                Id = rf.ReservationRepository.GetNextId(),
                ReservationDateTime = DateTime.Now,
                DepartureDate = DepartureDate,
                ClientEmail = "milica@gmail.com",
                SeatId = long.Parse(selectedSeat.Name.Substring(4)),
                DepartureId = departureDTO.Id,
                IsActive = true,
                StartStation = line.Stations[departureDTO.StartIndex].Name,
                EndStation = line.Stations[departureDTO.EndIndex].Name
            };
            rf.ReservationRepository.Add(reservation);
        }

        private void DisableSelectedSeat()
        {
            selectedSeat.IsEnabled = false;
            btnReserve.IsEnabled = false;
            btnPurchase.IsEnabled = false;
        }
    }
}
