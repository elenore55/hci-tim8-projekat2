﻿using HCI_Project.model;
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
        private string from;
        private string to;
        private double price;

        private readonly ReservationRepository reservationRepository;
        private readonly TicketRepository ticketRepository;

        public SeatChoice(string from, string to, double price, Departure departure, DateTime date, TicketRepository ticketRepository, ReservationRepository reservationRepository)
        {
            InitializeComponent();
            DataContext = this;

            this.reservationRepository = reservationRepository;
            this.ticketRepository = ticketRepository;
            this.from = from;
            this.to = to;
            this.price = price;
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
                    VerticalAlignment = VerticalAlignment.Bottom
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
            TicketData td = new TicketData
            {
                From = from,
                To = to,
                DepartureDateTime = $"{DepartureDate.ToShortDateString()} {Departure.StartTime.ToShortTimeString()}",
                ArrivalDateTime = "11/11/2011 23:20",
                Wagon = $"Number {Grid.GetRow(selectedWagon) + 1}, {selectedWagonClass} class",
                Seat = $"{Grid.GetColumn(selectedSeat)}{Convert.ToChar(65 + Grid.GetRow(selectedSeat))}",
                Price = $"{price} EUR"
            };
            PurchaseConfirmation confirmation = new PurchaseConfirmation(td);
            confirmation.OnConfirm += SavePurchase;
            confirmation.ShowDialog();
        }

        public void SavePurchase(object sender, EventArgs e)
        {
            MessageBox.Show("Ticket successfully purchased!");
            selectedSeat.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(TAKEN);
            selectedSeat.IsEnabled = false;
            btnReserve.IsEnabled = false;
            btnPurchase.IsEnabled = false;
        }
    }
}
