using BingMapsRESTToolkit;
using HCI_Project.model;
using HCI_Project.repository;
using HCI_Project.utils;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for ClintLineNetwork.xaml
    /// </summary>
    public partial class ClintLineNetwork : Page
    {
        public class StationList
        {
            public Station Station { get; set; }
            public int Offset { get; set; }
        }
        public class DepartureSimple
        {
            public string StartTime { get; set; }
            public string EndTime { get; set; }
        }
        private string BingMapsKey = "AinQ9hRJn7QhWLbnmUvC6OJ9RvqMuOWGDRkvSqOf5MUgrvbkmFHxHNg6aIjno0CM";
        public List<model.Line> Lines { get; set; }
        public List<StationList> Stations { get; set; }
        public List<DepartureSimple> Departures { get; set; }
        public LoadingWindow viewer = new LoadingWindow();
        private RepositoryFactory rf;
        public ClintLineNetwork(RepositoryFactory rf)
        {
            this.rf = rf;
            Lines = rf.LineRepository.GetAll();
            InitializeComponent();
            MyMap.CredentialsProvider = new ApplicationIdCredentialsProvider(BingMapsKey);
        }

        private void ComboBox_Selected(object sender, RoutedEventArgs e)
        {
            Thread viewerThread = new Thread(delegate ()
            {
                viewer = new LoadingWindow();
                viewer.Show();
                System.Windows.Threading.Dispatcher.Run();
            });
            viewerThread.SetApartmentState(ApartmentState.STA);
            viewerThread.Start();
            model.Line l = Lines[CBLines.SelectedIndex];
            List<Pushpin> pushpins = new List<Pushpin>();
            MyMap.Children.Clear();
            List<SimpleWaypoint> waypoints = new List<SimpleWaypoint>();
            foreach (Station s in l.Stations)
            {
                waypoints.Add(new SimpleWaypoint(s.Coords.X, s.Coords.Y));
                Microsoft.Maps.MapControl.WPF.Location pinLocation = new Microsoft.Maps.MapControl.WPF.Location();
                pinLocation.Latitude = s.Coords.X;
                pinLocation.Longitude = s.Coords.Y;
                Pushpin pin = new Pushpin
                {
                    Location = pinLocation,
                    ToolTip = s.Name
                };
                pushpins.Add(pin);
            }
            BingMapRESTServices.SendRequest(MyMap, waypoints);
            for (int i = 0; i < l.Stations.Count; i++)
            {
                pushpins[i].Content = i + 1;
            }
            Stations = new List<StationList>();
            Departures = new List<DepartureSimple>();
            for(int i = 0; i < l.Stations.Count; i++)
            {
                int offset = l.OffsetsInMinutes[i];
                if (i > 0)
                {
                    offset += Stations[i - 1].Offset;
                }
                Stations.Add(new StationList() { Station = l.Stations[i], Offset = offset });
            }
            foreach (Departure d in l.Departures)
            {
                DateTime start = d.StartTime;
                DateTime end = start.AddMinutes(Stations[Stations.Count - 1].Offset);
                Departures.Add(new DepartureSimple()
                {
                    StartTime = start.ToShortTimeString(),
                    EndTime = end.ToShortTimeString()
                });
            }
            pushpins.ForEach(x => MyMap.Children.Add(x));
            MyMap.UpdateLayout();
            LBDepartures.ItemsSource = Departures;
            LBDepartures.Items.Refresh();
            LBStations.ItemsSource = Stations;
            LBStations.Items.Refresh();
            Thread.Sleep(1000);
            System.Windows.Threading.Dispatcher.FromThread(viewerThread).InvokeShutdown();
        }
    }
}
