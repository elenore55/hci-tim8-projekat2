using HCI_Project.model;
using HCI_Project.repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace HCI_Project.view.Reports
{
    /// <summary>
    /// Interaction logic for MonthlyReport.xaml
    /// </summary>
    public partial class MonthlyReport : Page
    {
        private RepositoryFactory rf;

        public ObservableCollection<TicketDTO> Rows { get; set; }

        public DateTime SelectedMonth;

        public MonthlyReport(RepositoryFactory r)
        {
            InitializeComponent();
            this.rf = r;
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
            int selectedYear = (int) year.Value;
            int selectedMonth = month.SelectedIndex;
            List<Ticket> tickets = rf.TicketRepository.GetByMonth(selectedMonth, selectedYear);
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

  
}
   

