using HCI_Project.model;
using HCI_Project.repository;
using HCI_Project.view.DepartureHandling;
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

namespace HCI_Project.view.LinesHandling
{
    /// <summary>
    /// Interaction logic for LinesView.xaml
    /// </summary>
    public partial class LinesView : Page
    {
        public List<string> StationNames { get; set; }
        private readonly RepositoryFactory rf;
        public ObservableCollection<LineDTO> MyRows { get; set; }
        //public List<Departure> Departures { get; set; }

        public LinesView(RepositoryFactory rf)
        {
            this.rf = rf;
            MyRows = findAllRows();
            InitializeComponent();
            
            DataContext = this;

            List<Station> stations = rf.StationRepository.GetAll();
            StationNames = (from s in stations select s.Name).ToList();
            tbFrom.ItemsSource = StationNames;
            tbTo.ItemsSource = StationNames;
            SetDataGridVisibility();
            //Departures = new List<Departure>();
        }

        private ObservableCollection<LineDTO> findAllRows()
        {
            ObservableCollection<LineDTO> retVal = new ObservableCollection<LineDTO>();
            
            foreach (Line line in rf.LineRepository.GetAll())
            {
                LineDTO l = new LineDTO(line);
                retVal.Add(l);
            }
            Console.WriteLine("Ukupna duzina svih redova je " + retVal.Count);
            return retVal;
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

            if (from == "" || to == "")
            {
                MessageBox.Show("You did not fill in the required information!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                MarkRed();
            }
            else
            {
                List<Line> lines = rf.LineRepository.FilterLines(from, to);
                MyRows.Clear();
                //Departures.Clear();
                foreach (Line line in lines)
                {
                    LineDTO l = new LineDTO(line);
                    MyRows.Add(l);
                    // za svaku pronadjenu liniju treba da dodam LineDTO

                    /*int startIndex = 0;
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
                    }*/
                    /*foreach (Departure dpt in line.Departures)
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
                    */
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

     
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            
            LineDTO selected = dataGrid.SelectedItem as LineDTO;
            long lineId = selected.Id;
            Console.WriteLine("Id odabrane linije je " + lineId);

            DeparturesEdit de = new DeparturesEdit(rf, lineId);
            //NavigationService.Navigate(de);
            Window wnd = Window.GetWindow(this);
            wnd.Content = de;
        }
    }
}

public class LineDTO
{
    public long Id { get { return Line.Id; } set { Id = value; } }
    public Line Line { get; set; }

    public String FirstStation { get { return Line.Stations[0].Name; } set { FirstStation = value; } }
    public String LastStation { get { return Line.Stations[Line.Stations.Count - 1].Name; } set { LastStation = value; } }

    public double Price { get { return Line.Price; } set { Price = value; } }

    public LineDTO(Line line)
    {
        Line = line;
    }
    public string Details
    {
        get
        {
            List<Station> stations = Line.Stations;
            StringBuilder sb = new StringBuilder($"{"STATIONS"}\n\n");
            //DateTime prev = DepartureTime;
            for (int i = 0; i < stations.Count; i++)
            {
                //int offset = Line.OffsetsInMinutes[i];
                //prev = prev.AddMinutes(offset);
                sb.Append($"{stations[i].Name}\n");
            }
            return sb.ToString();
        }
    }

}

