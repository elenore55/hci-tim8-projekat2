﻿using HCI_Project.model;
using HCI_Project.repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for TicketPurchase.xaml
    /// </summary>
    public partial class TicketPurchase : Page
    {
        public List<string> StationNames { get; set; }
        private StationRepository stationRepository;
        private LineRepository lineRepository;
        private ReservationRepository reservationRepository;
        private TicketRepository ticketRepository;
        public ObservableCollection<DepartureDTO> MyRows { get; set; }
        public List<Departure> Departures { get; set; }

        public TicketPurchase(StationRepository stationRepository, LineRepository lineRepository, TicketRepository ticketRepository, ReservationRepository reservationRepository)
        {
            InitializeComponent();
            this.stationRepository = stationRepository;
            this.lineRepository = lineRepository;
            this.reservationRepository = reservationRepository;
            this.ticketRepository = ticketRepository;

            List<Station> stations = stationRepository.GetAll();
            StationNames = (from s in stations select s.Name).ToList();
            tbFrom.ItemsSource = StationNames;
            tbTo.ItemsSource = StationNames;
            MyRows = new ObservableCollection<DepartureDTO>();
            Departures = new List<Departure>();
            DataContext = this;
        }

        private void btnShow_Click(object sender, RoutedEventArgs e)
        {
            string from = tbFrom.Text;
            string to = tbTo.Text;
            if (from == "" || to == "" || DepartureDate.SelectedDate == null)
            {
                MessageBox.Show("You did not fill in the required information!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                MarkRed();
            } 
            else
            {
                List<Line> lines = lineRepository.FilterLines(from, to);
                MyRows.Clear();
                Departures.Clear();
                foreach (Line line in lines)
                {
                    int startIndex = 0;
                    int endIndex = line.Stations.Count - 1;
                    for (int i = 0; i < line.Stations.Count; i++)
                    {
                        if (line.Stations[i].Name == from)
                        {
                            startIndex = i;
                        }
                        else if (line.Stations[i].Name == to)
                        {
                            endIndex = i;
                            break;
                        }
                    }

                    foreach (Departure dpt in line.Departures)
                    {
                        DepartureDTO dto = new DepartureDTO()
                        {
                            StartIndex = startIndex,
                            EndIndex = endIndex,
                            DepartureTime = dpt.StartTime,
                            Line = line
                        };
                        MyRows.Add(dto);
                        Departures.Add(dpt);
                    }
                }
            }
        }

        private void MarkRed()
        {
            MarkTbFromRed();
            MarkTbToRed();
            MarkDpRed();
        }

        private void MarkTbFromRed()
        {
            if (tbFrom.Text == "")
            {
                tbFromBorder.BorderThickness = new Thickness(0, 0, 0, 2);
                tbFromBorder.BorderBrush = Brushes.Red;
            }
        }

        private void MarkTbToRed()
        {
            if (tbTo.Text == "")
            {
                tbToBorder.BorderThickness = new Thickness(0, 0, 0, 2);
                tbToBorder.BorderBrush = Brushes.Red;
            }
        }

        private void MarkDpRed()
        {
            if (DepartureDate.SelectedDate == null)
            { 
                DepartureDate.BorderThickness = new Thickness(0, 0, 0, 2);
                DepartureDate.BorderBrush = Brushes.Red;
            }
        }

        private void tbTo_GotFocus(object sender, RoutedEventArgs e)
        {
            tbToBorder.BorderThickness = new Thickness(0, 0, 0, 0);
        }

        private void tbTo_LostFocus(object sender, RoutedEventArgs e)
        {
            MarkTbToRed();
        }

        private void tbFrom_GotFocus(object sender, RoutedEventArgs e)
        {
            tbFromBorder.BorderThickness = new Thickness(0, 0, 0, 0);
        }

        private void tbFrom_LostFocus(object sender, RoutedEventArgs e)
        {
            MarkTbFromRed();
        }

        private void DepartureDate_GotFocus(object sender, RoutedEventArgs e)
        {
            DepartureDate.BorderThickness = new Thickness(0, 0, 0, 0);
            DepartureDate.BorderBrush = Brushes.Black;
        }

        private void DepartureDate_LostFocus(object sender, RoutedEventArgs e)
        {
            MarkDpRed();
        }

        private void btnChoose_Click(object sender, RoutedEventArgs e)
        {
            int rowIndex = dataGrid.SelectedIndex;
            if (rowIndex == -1)
            {
                MessageBox.Show("Departure not selected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            NavigationService.Navigate(new SeatChoice(Departures[rowIndex], DepartureDate.SelectedDate.Value, ticketRepository, reservationRepository));
        }
    }
}

public class DepartureDTO
{
    public int StartIndex { get; set; }
    public int EndIndex { get; set; }
    public DateTime DepartureTime { get; set; }
    public Line Line { get; set; }
    public string DepartureTimeStr
    {
        get
        {
            DateTime start = DepartureTime;
            for (int i = 0; i <= StartIndex; i++)
            {
                start = start.AddMinutes(Line.OffsetsInMinutes[i]);
            }
            return start.ToShortTimeString();
        }
    }
    public string ArrivalTimeStr { 
        get 
        {
            DateTime start = DepartureTime;
            for (int i = 0; i <= EndIndex; i++)
            {
                start = start.AddMinutes(Line.OffsetsInMinutes[i]);
            }
            return start.ToShortTimeString();
        } 
    }
    public double Price { get { return Line.Price; } }
    public string Details
    {
        get
        {
            List<Station> stations = Line.Stations;
            StringBuilder sb = new StringBuilder($"{"STATIONS",-50}{"DEPARTURE",-50}\n\n");
            DateTime prev = DepartureTime;
            for (int i = 0; i < stations.Count; i++)
            {
                int offset = Line.OffsetsInMinutes[i];
                prev = prev.AddMinutes(offset);
                sb.Append($"{stations[i].Name,-55}{prev.ToShortTimeString(),-50}\n");
            }
            return sb.ToString();
        }
    }
}
