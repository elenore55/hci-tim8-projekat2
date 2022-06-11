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

namespace HCI_Project.view.Reports
{
    /// <summary>
    /// Interaction logic for DepartureReport.xaml
    /// </summary>
    public partial class DepartureReport : Page
    {
        public List<string> StationNames { get; set; }
        private readonly RepositoryFactory rf;
        public ObservableCollection<DepartureDTO> MyRows { get; set; }
        public List<Departure> Departures { get; set; }
        public DepartureReport(RepositoryFactory rf)
        {
            InitializeComponent();
            this.rf = rf;
            DataContext = this;

            List<Station> stations = rf.StationRepository.GetAll();
            StationNames = (from s in stations select s.Name).ToList();
            tbFrom.ItemsSource = StationNames;
            tbTo.ItemsSource = StationNames;
            MyRows = new ObservableCollection<DepartureDTO>();
            Departures = new List<Departure>();
            
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
                List<Line> lines = rf.LineRepository.FilterLinesEndpoints(from, to);
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

       
        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            DepartureDTO selected = dataGrid.SelectedItem as DepartureDTO;
            Window wnd = Window.GetWindow(this);
            Console.WriteLine("Ono sto je odabrano je " + DepartureDate.SelectedDate);
            wnd.Content = new DepartureTickets(rf, selected, (DateTime)DepartureDate.SelectedDate);
        }

        private void dataGrid_Selected(object sender, RoutedEventArgs e)
        {
            // nista se ne desi
        }
    }
}