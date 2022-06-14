using HCI_Project.model;
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

namespace HCI_Project.view
{
    /// <summary>
    /// Interaction logic for ClientsTickets.xaml
    /// </summary>
    public partial class ClientsTickets : Page
    {
        private readonly RepositoryFactory rf;
        private string email;

        public ObservableCollection<TicketDTO> Rows { get; set; }

        public ClientsTickets(string email, RepositoryFactory rf)
        {
            InitializeComponent();
#if DEBUG
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
#endif
            this.rf = rf;
            this.email = email;
            Rows = new ObservableCollection<TicketDTO>();
            DataContext = this;
            DisplayTickets();
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            DisplayTickets();
        }

        private void DisplayTickets()
        {
            DateTime start = DateTime.MinValue;
            DateTime end = DateTime.MaxValue;
            if (startDatePick.SelectedDate != null) start = startDatePick.SelectedDate.Value;
            if (endDatePick.SelectedDate != null) end = endDatePick.SelectedDate.Value;
            List<Ticket> tickets = rf.TicketRepository.GetByClient(email, start, end);
            Rows.Clear();
            foreach (Ticket t in tickets)
            {
                Departure departure = rf.DepartureRepository.GetById(t.DepartureId);
                Line line = rf.LineRepository.GetById(departure.LineId);
                Seat seat = rf.SeatRepository.GetById(t.SeatId);
                Wagon wagon = rf.WagonRepository.GetById(seat.WagonId);
                TicketDTO dto = new TicketDTO()
                {
                    Line = line,
                    DateTimeOfPurchaseStr = $"{t.PurchaseDateTime.ToShortDateString()} {t.PurchaseDateTime.ToShortTimeString()}",
                    DateTimeOfDepartureStr = $"{t.DepartureDate.ToShortDateString()} {departure.StartTime.ToShortTimeString()}",
                    Destination = $"{t.StartStation} - {t.EndStation}",
                    Price = line.Price,
                    SeatStr = $"{seat.Row + 1}{Convert.ToChar(65 + seat.Column)}",
                    WagonStr = $"No. {wagon.Ordinal + 1} ({wagon.Class} class)",
                    TrainName = departure.Train.Name
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
    }

    public class TicketDTO
    {
        public Line Line { get; set; }
        public string DateTimeOfPurchaseStr { get; set; }
        public string DateTimeOfDepartureStr { get; set; }
        public string Destination { get; set; }
        public double Price { get; set; }
        public string PriceStr { get { return $"{Price} \u20AC"; } }
        public string SeatStr { get; set; }
        public string WagonStr { get; set; }
        public string TrainName { get; set; }
        public string SeatDetails 
        { 
            get
            {
                return $"{WagonStr}\n{SeatStr}";
            } 
        }
        public string Details
        {
            get
            {
                List<Station> stations = Line.Stations;
                StringBuilder sb = new StringBuilder($"Date of departure: {DateTimeOfDepartureStr.Substring(0, 10)}\n\n");
                sb.Append($"{"STATION",-40}DEPARTURE\n\n");
                DateTime prev = DateTime.Parse(DateTimeOfDepartureStr);
                for (int i = 0; i < stations.Count; i++)
                {
                    int offset = Line.OffsetsInMinutes[i];
                    prev = prev.AddMinutes(offset);
                    sb.Append($"{stations[i].Name,-46}{prev.ToShortTimeString()}\n");
                }
                return sb.ToString();
            }
        }

        public string StationNames
        {
            get
            {
                List<Station> stations = Line.Stations;
                StringBuilder sb = new StringBuilder();
                sb.Append("STATION\n\n");
                for (int i = 0; i < stations.Count; i++)
                {
                    sb.Append($"{stations[i].Name}\n");
                }
                return sb.ToString();
            }
        }

        public string DepartureTimes
        {
            get
            {
                List<Station> stations = Line.Stations;
                StringBuilder sb = new StringBuilder();
                sb.Append($"DEPARTURE\n\n");
                DateTime prev = DateTime.Parse(DateTimeOfDepartureStr);
                for (int i = 0; i < stations.Count; i++)
                {
                    int offset = Line.OffsetsInMinutes[i];
                    prev = prev.AddMinutes(offset);
                    sb.Append($"     {prev.ToShortTimeString()}\n");
                }
                return sb.ToString();
            }
        }

        public string DepartureDate
        {
            get
            {
                return $"Date of departure: {DateTimeOfDepartureStr.Substring(0, 10)}";
            }
        }
    }
}
