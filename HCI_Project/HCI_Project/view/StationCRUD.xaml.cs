using HCI_Project.model;
using HCI_Project.repository;
using Microsoft.Maps.MapControl.WPF;
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
using System.Windows.Shapes;

namespace HCI_Project.view
{
    /// <summary>
    /// Interaction logic for StationCRUD.xaml
    /// </summary>
    public partial class StationCRUD : Page
    {
        private string BingMapsKey = "AinQ9hRJn7QhWLbnmUvC6OJ9RvqMuOWGDRkvSqOf5MUgrvbkmFHxHNg6aIjno0CM";
        private RepositoryFactory rf;
        public List<Station> Stations;
        private List<Pushpin> StationPins = new List<Pushpin>();
        public StationCRUD(RepositoryFactory rf)
        {
            this.rf = rf;
            Stations = rf.StationRepository.GetAll();
            InitializeComponent();
            addStations();
            LBStations.ItemsSource = Stations;
            MyMap.CredentialsProvider = new ApplicationIdCredentialsProvider(BingMapsKey);
        }
        private void addStations()
        {
            StationPins.Clear();
            foreach (Station s in Stations)
            {
                Location pinLocation = new Location();
                pinLocation.Latitude = s.Coords.X;
                pinLocation.Longitude = s.Coords.Y;
                Pushpin pin = new Pushpin
                {
                    Location = pinLocation,
                    ToolTip = s.Name,
                    Height = 18,
                    Width = 18,
                    Template = (ControlTemplate)FindResource("CustomPushpinTemplate")
                };
                StationPins.Add(pin);
            }
            updateMap();
        }

        public void updateMap()
        {
            MyMap.Children.Clear();
            StationPins.ForEach(x => MyMap.Children.Add(x));
        }

        private void Add_Station(object sender, RoutedEventArgs e)
        {
            StationCreation crud = new StationCreation(rf);
            crud.DataChangedEvent += ProductListUpdate_DataChanged;
            crud.Show();
        }

        private void Edit_Station(object sender, RoutedEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as Station;
            StationCreation crud = new StationCreation(rf, item);
            crud.DataChangedEvent += ProductListUpdate_DataChanged;
            crud.Show();
        }

        private void ProductListUpdate_DataChanged(object sender, EventArgs e)
        {
            LBStations.Items.Refresh();
            addStations();
            updateMap();
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as Station;
            foreach (Pushpin p in StationPins)
            {
                if (p.Location.Latitude == item.Coords.X && p.Location.Longitude == item.Coords.Y)
                {
                    StationPins.Remove(p);
                    break;
                }
            };
            foreach(Station s in Stations)
            {
                if(s.Id == item.Id)
                {
                    Stations.Remove(s);
                    break;
                }
            };
            LBStations.Items.Refresh();

        }

        private void Zoom_Station(object sender, MouseButtonEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as Station;
            foreach(Pushpin p in StationPins)
            {
                if(p.Location.Latitude == item.Coords.X && p.Location.Longitude == item.Coords.Y)
                {
                    p.Width = 40;
                    p.Height = 40;
                } else
                {
                    p.Width = 18;
                    p.Height = 18;
                }
            }
            updateMap();
            MyMap.Center = new Location(item.Coords.X, item.Coords.Y);
            MyMap.UpdateLayout();
        }
    }
}
