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

namespace HCI_Project.view.Reports
{
    
    public partial class DepartureTickets : Page
    {
        private readonly RepositoryFactory rf;
        private DepartureDTO dep;

        public ObservableCollection<TicketDTO> Rows { get; set; }
        private DateTime selectedDate;
        public DepartureTickets(RepositoryFactory rf, DepartureDTO selected, DateTime selectedD)
        {
            InitializeComponent();
#if DEBUG
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
#endif
            this.rf = rf;
            this.dep = selected;
            this.selectedDate = selectedD;
            Rows = new ObservableCollection<TicketDTO>();
            DataContext = this;
            setTitle();
            DisplayTickets();
        }

        private void setTitle()
        {
            // Ovdje dodati labelu sa adekvatnim naslovom 
            String title = "Departures on " + selectedDate.ToString("dddd, dd MMMM yyyy") + "; at " + dep.DepartureTime.ToString("HH:mm") + "h";
            lblTitle.Content = title;
        }

        private void DisplayTickets()
        {
            
            List<Ticket> tickets = rf.TicketRepository.GetByDepartureId(dep.Id, selectedDate);
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

