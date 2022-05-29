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
    /// Interaction logic for TicketPurchase.xaml
    /// </summary>
    public partial class TicketPurchase : Page
    {
        public List<string> StationNames { get; set; }
        private StationRepository stationRepository;
        private LineRepository lineRepository;

        public TicketPurchase(StationRepository stationRepository, LineRepository lineRepository)
        {
            InitializeComponent();
            this.stationRepository = stationRepository;
            this.lineRepository = lineRepository;
            List<Station> stations = stationRepository.GetAll();
            StationNames = (from s in stations select s.Name).ToList();
            tbFrom.ItemsSource = StationNames;
            tbTo.ItemsSource = StationNames;
            
        }

        private void btnShow_Click(object sender, RoutedEventArgs e)
        {
            string from = tbFrom.Text;
            string to = tbTo.Text;
            if (from == "" || to == "" || DepartureDate.SelectedDate == null)
            {
                MessageBox.Show("You did not fill in the required information!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                MarkRed();
            } else
            {
                DateTime departureDate = DepartureDate.SelectedDate.GetValueOrDefault();
                List<Line> lines = lineRepository.FilterLines(from, to);
                List<Departure> departures = new List<Departure>();
                foreach (Line line in lines)
                {
                    foreach (Departure dpt in line.Departures)
                    {
                        if (dpt.StartDateTime.Date == departureDate.Date)
                        {
                            departures.Add(dpt);
                        }
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
    }
}
