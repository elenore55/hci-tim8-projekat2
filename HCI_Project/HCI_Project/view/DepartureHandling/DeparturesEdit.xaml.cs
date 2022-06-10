using HCI_Project.model;
using HCI_Project.repository;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HCI_Project.view.DepartureHandling
{
    /// <summary>
    /// Interaction logic for DeparturesEdit.xaml
    /// </summary>
    public partial class DeparturesEdit : Page
    {
        public RepositoryFactory rf;
        public long LineId;
        public int currentDepNum;
        public DeparturesEdit(RepositoryFactory rep, long id)
        {
            
            rf = rep;
            LineId = id;
            InitializeComponent();
            PopulateStationsGrid();
            PopulateDepartureGrid();
            
        }

        private void PopulateDepartureGrid()
        {
            List<String> times = findDepartureTimes();
            deleteGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            watchGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            for (int i = 0; i < times.Count; i++)
            {                
                watchGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                deleteGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
            for (int i = 0; i < times.Count; i++)
            {
                AddTimeTextBox(times[i], i);
                AddDeleteButton(i);
            }
            currentDepNum = times.Count;
            addAddButton(times.Count);
        }

        private void AddDeleteButton(int i)
        {
            Button b = new Button();
            b.Width = 80;
            b.Height = 40;
            b.Margin = new Thickness(20, 5, 5, 5);
            b.FontSize = 14;
            b.Content = "Delete";
            b.Name = "s" + i;       // dodala sam s napocetku jer sam broj ne moze da bude ime
            b.Click += new RoutedEventHandler(deleteDepartureBtn_Click);


            Grid.SetColumn(b, 0);
            Grid.SetRow(b, i);
            deleteGrid.Children.Add(b);
        }

        private void deleteDepartureBtn_Click(object sender, RoutedEventArgs e)
        {
            // sad treba zatamniti picker i na dugmetu napisati da je obrisan
            Button itself = (Button)sender;
            itself.Content = "Deleted";
            itself.IsEnabled = false;
            int idx = Int32.Parse(itself.Name.Substring(1)); // izvukla sam id dugmeta
            List<TimePicker> allPickers = AllPickers(watchGrid);
            TimePicker myPicker = allPickers[idx];
            myPicker.IsEnabled = false;

        }

        private void AddTimeTextBox(string v, int i)
        {
            TimePicker tp = new TimePicker();
            tp.Width = 130;
            tp.Height = 40;
            tp.Margin = new Thickness(5, 5, 5, 5);
            tp.FontSize = 20;
            setDefaultValue(tp, i);     //ova funkcija ce da stavi vrijednost na originalnu

            Grid.SetColumn(tp, 0);
            Grid.SetRow(tp, i);
            watchGrid.Children.Add(tp);
        }

       
        private void addAddButton(int n)
        {
            Button b = new Button();
            b.Content = "+ Add departure";
            b.Margin = new Thickness(0, 30, 0, 30);
            // ovo je dodavanje reda
            watchGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            //watchGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            // ovo je dodavanje dugmeta
            Grid.SetColumn(b, 0);
            Grid.SetRow(b, n);
            watchGrid.Children.Add(b);
            b.Click += new RoutedEventHandler(addDepartureBtn_Click);

            //watchGrid.Children.Add(b);
        }

        private void addDepartureBtn_Click(object sender, RoutedEventArgs e)
        {
            // ovdje treba prvo programski dodati novi red, pa onda upisati vrijednost u njega
            currentDepNum++;
            watchGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            TimePicker tp = new TimePicker();
            tp.Width = 130;
            tp.Height = 40;
            tp.Margin = new Thickness(5, 5, 5, 5);
            tp.FontSize = 20;

            Grid.SetColumn(tp, 0);
            Grid.SetRow(tp, currentDepNum);
            watchGrid.Children.Add(tp);
        }

        private void setDefaultValue(TimePicker tp, int i)
        {
            tp.SelectedTime = rf.LineRepository.GetById(LineId).Departures[i].StartTime;
        }

        private List<string> findDepartureTimes()
        {
            List<String> retVal = new List<string>();
            foreach (Departure d in rf.LineRepository.GetById(LineId).Departures)
            {
                retVal.Add(d.StartTime.ToString());
            }
            return retVal;
        }

        private void PopulateStationsGrid()
        {
            List<String> stations = findStations();
            stationsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            for (int i = 0; i < stations.Count; i++)
            {
                stationsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                stationsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                // dodala sam dva reda da bih mogla da pravim izmedju
            }
            for (int i = 0; i < stations.Count; i++)
            {
                AddStationLabel(stations[i], i * 2);
                if (i < stations.Count - 1)
                {
                    // zadnji put ne treba dodati ovo polje
                    AddIcon(i * 2 + 1);
                    AddStationTextBox(i * 2 + 1, getPreValue(i));
                    AddMinutesLabel(i * 2 + 1);
                }
            }
        }

        private void AddMinutesLabel(int v)
        {
            Label l = new Label();
            l.Content = "min";
            l.FontSize = 20;
            //l.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(l, 2);
            Grid.SetRow(l, v);
            stationsGrid.Children.Add(l);
        }

        private void AddIcon(int v)
        {
            PackIcon icon = new PackIcon()
            {
                Kind = PackIconKind.TimerSand,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                //Margin = new Thickness(5, 5, 5, 5),
                Width = 30,
                Height = 30
            };
            Grid.SetColumn(icon, 0);
            Grid.SetRow(icon, v);
            stationsGrid.Children.Add(icon);
        }

        private void AddStationTextBox(int i, int m)
        {
            TextBox t = new TextBox();
            t.Width = 40;
            t.BorderThickness = new Thickness(1);
            t.Text = m.ToString();
            t.FontSize = 20;
            t.PreviewTextInput += PreviewTextInput;
            t.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(t, 1);
            Grid.SetRow(t, i);
            stationsGrid.Children.Add(t);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // ovdje smo sacuvali promjene iz prve polovine prozora
            saveOffsets();
            // sad cemo da cuvamo promjene iz druge polovine
            saveDepartures();
            // i na kraju opet jos samo osvjezimo stranicu da se sacuvaju promejne
            // a mozda to i ne mora 
            DeparturesEdit de = new DeparturesEdit(rf, LineId);
            Window wnd = Window.GetWindow(this);
            wnd.Content = de;
        }

        private void saveDepartures()
        {
            //Console.WriteLine("Duzina liste departura je " + rf.LineRepository.GetById(LineId).Departures.Count());
            List<TimePicker> allPickers= AllPickers(watchGrid);
            List<Departure> deps = rf.LineRepository.GetById(LineId).Departures;
            for (int i = 0; i < deps.Count; i++)
            {
                deps[i].StartTime = allPickers[i].SelectedTime.Value;
            }
            // sad jos jednom proci kroz sve pickere i ako bude neki disableovan, njega obrisati
            // vjerovatno treba ici odzada
            
            // ovo su ovi koji su novododati
            for (int i = deps.Count; i < allPickers.Count; i++)
            {
                if (allPickers[i].SelectedTime!=null)
                {
                    Departure newD = new Departure();
                    newD.StartTime = allPickers[i].SelectedTime.Value;
                    newD.LineId = LineId;
                    newD.Train = rf.TrainRepository.GetById(1);
                    deps.Add(newD);
                }
                
            }
            for (int i = allPickers.Count - 1; i >= 0; i--)
            {
                Console.WriteLine("Kad sam isla unazad indeks je bio " + i);
                if (allPickers[i].IsEnabled == false)
                {
                    Console.WriteLine("Bio je disableovan");
                    deps.RemoveAt(i);
                }
            }
            Console.WriteLine("Duzina departura je " + deps.Count);
            rf.LineRepository.SaveAll();
        }


        private void saveOffsets()
        {
            List<TextBox> allBoxes = AllTextBoxes(stationsGrid);
            for (int i = 0; i < allBoxes.Count; i++)
            {
                rf.LineRepository.GetById((int)LineId).OffsetsInMinutes[i + 1] = Int32.Parse(allBoxes[i].Text);
            }
            rf.LineRepository.SaveAll();
        }

        private void btnDiscard_Click(object sender, RoutedEventArgs e)
        {
            DeparturesEdit de = new DeparturesEdit(rf, LineId);
            Window wnd = Window.GetWindow(this);
            wnd.Content = de;
        }

        List<TextBox> AllTextBoxes(DependencyObject parent)
        {
            var list = new List<TextBox>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is TextBox)
                    list.Add(child as TextBox);
                list.AddRange(AllTextBoxes(child));
            }
            return list;
        }

        private List<TimePicker> AllPickers(DependencyObject parent)
        {
            var list = new List<TimePicker>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is TimePicker)
                    list.Add(child as TimePicker);
                list.AddRange(AllPickers(child));
            }
            return list;
        }

        private void AddStationLabel(string v, int i)
        {
            Label l = new Label();
            l.Content = v;
            l.FontSize = 20;
            l.Margin = new Thickness(0,20,0,20);
            l.HorizontalAlignment = HorizontalAlignment.Center;
            l.FontWeight = FontWeights.Bold;
            Grid.SetColumn(l, 0);
            Grid.SetRow(l, i);
            stationsGrid.Children.Add(l);
        }

        private int getPreValue(int i)
        {
            // ovdje treba napisati koja je inicijalna duzina
            return rf.LineRepository.GetById(LineId).OffsetsInMinutes[i + 1];
        }

        private List<string> findStations()
        {
            List<String> retVal = new List<string>();
            foreach (Station s in rf.LineRepository.GetById(LineId).Stations)
            {
                retVal.Add(s.Name);
            }
            return retVal;
        }
        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

    }
}
