using HCI_Project;
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
using System.Windows.Navigation;

namespace HCI_Project.view
{
    /// <summary>
    /// Interaction logic for TicketPurchase.xaml
    /// </summary>
    public partial class TicketPurchase : Page
    {
        public List<string> StationNames { get; set; }
        private readonly RepositoryFactory rf;
        public ObservableCollection<DepartureDTO> MyRows { get; set; }
        public List<Departure> Departures { get; set; }
        private string email;

        public TicketPurchase(string email, RepositoryFactory rf)
        {
            InitializeComponent();
            this.rf = rf;
            this.email = email;
            DataContext = this;

            List<Station> stations = rf.StationRepository.GetAll();
            StationNames = (from s in stations select s.Name).ToList();
            tbFrom.ItemsSource = StationNames;
            tbTo.ItemsSource = StationNames;
            MyRows = new ObservableCollection<DepartureDTO>();
            Departures = new List<Departure>();
            blackoutRange.End = DateTime.Now.AddDays(-1);
        }

        private void btnShow_Click(object sender, RoutedEventArgs e)
        {
            Display();
        }

        public void Display()
        {
            ValidateStationInputs();
            string from = tbFrom.Text;
            string to = tbTo.Text;

            if (from == "" || to == "" || DepartureDate.SelectedDate == null)
            {
                MessageBox.Show("You did not fill in the required information!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                MarkRed();
            }
            else
            {
                List<Line> lines = rf.LineRepository.FilterLines(from, to);
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
                            Id = dpt.Id,
                            StartIndex = startIndex,
                            EndIndex = endIndex,
                            DepartureTime = dpt.StartTime,
                            Line = line,
                            Train = dpt.Train
                        };
                        MyRows.Add(dto);
                        Departures.Add(dpt);
                    }
                }
                SetDataGridVisibility();
            }
        }

        private void ValidateStationInputs()
        {
            bool isValidStart = false;
            bool isValidEnd = false;
            foreach (Station s in rf.StationRepository.GetAll())
            {
                if (s.Name == tbFrom.Text) isValidStart = true;
                else if (s.Name == tbTo.Text) isValidEnd = true;
            }
            if (!isValidStart) tbFrom.Text = "";
            if (!isValidEnd) tbTo.Text = "";
        }

        private void SetDataGridVisibility()
        {
            if (MyRows.Count > 0)
            {
                dataGrid.Visibility = Visibility.Visible;
                lblNoResults.Visibility = Visibility.Hidden;
            }
            else
            {
                dataGrid.Visibility = Visibility.Hidden;
                lblNoResults.Visibility = Visibility.Visible;
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
            SeatChoice sc = new SeatChoice(email, MyRows[rowIndex], DepartureDate.SelectedDate.Value, rf);
            NavigationService.Navigate(sc);
        }

        private void dataGrid_Selected(object sender, RoutedEventArgs e)
        {
            btnChoose.IsEnabled = true;
        }
    }
}

public class DepartureDTO
{
    public long Id { get; set; }
    public int StartIndex { get; set; }
    public int EndIndex { get; set; }
    public DateTime DepartureTime { get; set; }
    public Line Line { get; set; }
    public Train Train { get; set; }
    public long LineNum
    {
        get
        {
            return Line.Id;
        }
    }
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
    public string PriceStr
    {
        get { return $"{ Line.Price } \u20AC"; }
    }

    public double Price { get { return Line.Price; } }
     
    public string FirstClassPrice
    {
        get
        {
            return $"{ Line.GetFirstClassPrice() } \u20AC";
        }
    }

    public string Details
    {
        get
        {
            List<Station> stations = Line.Stations;
            StringBuilder sb = new StringBuilder($"{"STATION",-50}DEPARTURE\n\n");
            DateTime prev = DepartureTime;
            for (int i = 0; i < stations.Count; i++)
            {
                int offset = Line.OffsetsInMinutes[i];
                prev = prev.AddMinutes(offset);
                sb.Append($"{stations[i].Name,-56}{prev.ToShortTimeString()}\n");
            }
            return sb.ToString();
        }
    }

    public string StationNames
    {
        get
        {
            List<Station> stations = Line.Stations;
            StringBuilder sb = new StringBuilder("STATION\n\n");
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
            StringBuilder sb = new StringBuilder($"DEPARTURE\n\n");
            DateTime prev = DepartureTime;
            for (int i = 0; i < stations.Count; i++)
            {
                int offset = Line.OffsetsInMinutes[i];
                prev = prev.AddMinutes(offset);
                sb.Append($"     {prev.ToShortTimeString()}\n");
            }
            return sb.ToString();
        }
    }
}
