using HCI_Project.model;
using HCI_Project.utils;
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
    /// Interaction logic for TrainLineCreation.xaml
    /// </summary>
    public partial class TrainLineCreation : Page
    {
        private string BingMapsKey = "AinQ9hRJn7QhWLbnmUvC6OJ9RvqMuOWGDRkvSqOf5MUgrvbkmFHxHNg6aIjno0CM";
        private System.Windows.Point StartPoint;
        private List<Pushpin> Pins = new List<Pushpin>();
        private List<Pushpin> StationsPins = new List<Pushpin>();
        private List<Station> LineStations = new List<Station>();
        private bool IsFirstStation = true;
        private bool newPoint = true;
        private List<Station> Stations = new List<Station>();
        public TrainLineCreation()
        {
            InitializeComponent();
            Stations.Add(new Station(new System.Windows.Point(45.7737908329366, 19.1170693111155), "Sombor"));
            Stations.Add(new Station(new System.Windows.Point(43.3168743676544, 21.8930174489589), "Nis"));
            Stations.Add(new Station(new System.Windows.Point(44.1845546497502, 21.1054707255333), "Lapovo"));
            Stations.Add(new Station(new System.Windows.Point(44.2737853394934, 19.8837362826773), "Valjevo"));
            Stations.Add(new Station(new System.Windows.Point(43.8468213035184, 20.0354259952504), "Pozega"));
            Stations.Add(new Station(new System.Windows.Point(43.8560751522863, 19.8432587298032), "Uzice"));
            Stations.Add(new Station(new System.Windows.Point(45.1224246748143, 21.2969962172208), "Vrsac"));
            Stations.Add(new Station(new System.Windows.Point(45.126348777821, 19.2292180982901), "Sid"));
            Stations.Add(new Station(new System.Windows.Point(44.0132043068434, 20.9241081767304), "Kragujevac"));
            Stations.Add(new Station(new System.Windows.Point(44.4401983117903, 20.6913074485451), "Mladenovac"));
            Stations.Add(new Station(new System.Windows.Point(44.8068530443879, 20.4176244378858), "Beograd"));
            Stations.Add(new Station(new System.Windows.Point(45.2653991901631, 19.8295025367481), "Novi Sad"));
            Stations.Add(new Station(new System.Windows.Point(43.9167290500169, 21.3734995473952), "Cuprija"));
            Stations.Add(new Station(new System.Windows.Point(43.1423009222924, 22.5993454464567), "Pirot"));
            Stations.Add(new Station(new System.Windows.Point(43.9825635232647, 21.2643563898127), "Jagodina"));
            Stations.Add(new Station(new System.Windows.Point(43.8898154372637, 20.3559119017471), "Cacak"));
            Stations.Add(new Station(new System.Windows.Point(43.5638272233543, 19.5416579208959), "Priboj"));
            Stations.Add(new Station(new System.Windows.Point(43.1335608534931, 21.2681129272782), "Kursumlija"));
            Stations.Add(new Station(new System.Windows.Point(42.6578408443893, 21.150085751261), "Pristina"));
            MyMap.CredentialsProvider = new ApplicationIdCredentialsProvider(BingMapsKey);
            MyMap.MouseDoubleClick += MyMap_MouseDoubleClick1;
            LBStations.ItemsSource = LineStations;
            addStations();
            UpdateMap();
        }
        private void addStations()
        {
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
                StationsPins.Add(pin);
            }
        }

        private void MyMap_MouseDoubleClick1(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void Pushpin_MouseMove(object sender, MouseEventArgs e)
        {
            System.Windows.Point point = e.GetPosition(null);
            Vector diff = StartPoint - point;
            if (e.LeftButton == MouseButtonState.Pressed && (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                Pushpin p = e.Source as Pushpin;
                DataObject data = new DataObject(p);
                DragDrop.DoDragDrop(p, data, DragDropEffects.Move);
            }
        }

        private void Pushpin_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartPoint = e.GetPosition(null);
        }

        private void MyMap_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("myFormat") || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void MyMap_Drop(object sender, DragEventArgs e)
        {
            string[] invaildFields = { "", "", "" };
            e.Handled = true;

            System.Windows.Point pinPoint = e.GetPosition(MyMap);
            Location pinLocation = MyMap.ViewportPointToLocation(pinPoint);
            Station s = findClosest(pinLocation);
            pinLocation.Latitude = s.Coords.X;
            pinLocation.Longitude = s.Coords.Y;
            Pushpin pin = new Pushpin
            {
                Location = pinLocation,
                ToolTip = pinLocation.ToString()
            };
            pin.MouseDoubleClick += new MouseButtonEventHandler(Pin_Click);
            LineStations.Add(s);
            Pins.Add(pin);
            UpdateMap();
            newPoint = true;
            if (IsFirstStation)
            {
                IsFirstStation = false;
            }
        }

        private Station findClosest(Location pinPoint)
        {
            Location closest = pinPoint;
            Station s = new Station();
            double distance = 999999999;
            foreach (Station p in Stations)
            {
                Location pinLocation = new Location();
                pinLocation.Latitude = p.Coords.X;
                pinLocation.Longitude = p.Coords.Y;
                double dist = CalculateManhattanDistance(pinPoint.Latitude, pinLocation.Latitude, pinPoint.Longitude, pinLocation.Longitude);
                if (distance > dist)
                {
                    distance = dist;
                    closest = pinLocation;
                    s = p;
                }
            }
            return s;
        }

        private void Pin_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Pushpin pin = (Pushpin)sender;
            Pins.Remove(pin);
            foreach (Station s in LineStations)
            {
                if (s.Coords.X == pin.Location.Latitude && s.Coords.Y == pin.Location.Longitude)
                {
                    LineStations.Remove(s);
                    break;
                }
            }
            UpdateMap();
            IsFirstStation = Pins.Count == 0;
        }

        private void UpdateMap()
        {
            MyMap.Children.Clear();
            List<BingMapsRESTToolkit.SimpleWaypoint> waypoints = new List<BingMapsRESTToolkit.SimpleWaypoint>();
            foreach (Station s in LineStations)
            {
                waypoints.Add(new BingMapsRESTToolkit.SimpleWaypoint(s.Coords.X, s.Coords.Y));
            }
            BingMapRESTServices.SendRequest(MyMap, waypoints);
            Pins.ForEach(x => MyMap.Children.Add(x));
            StationsPins.ForEach(x => MyMap.Children.Add(x));
            LBStations.Items.Refresh();
        }
        public static double CalculateManhattanDistance(double x1, double x2, double y1, double y2)
        {
            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
        }

        private void MyMap_DragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
            System.Windows.Point pinLocaiton = e.GetPosition(MyMap);
            Location click = MyMap.ViewportPointToLocation(pinLocaiton);
            Pushpin newP = new Pushpin
            {
                Location = click
            };
            if (newPoint)
            {
                MyMap.Children.Add(newP);
                newPoint = false;
            }
            MyMap.Children.Remove(MyMap.Children[MyMap.Children.Count - 1]);
            MyMap.Children.Add(newP);
        }

        private void LBStations_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Station item = ((FrameworkElement)e.OriginalSource).DataContext as Station;
            if (item != null)
            {
                LineStations.Remove(item);
                foreach (Pushpin p in Pins)
                {
                    if (p.Location.Latitude == item.Coords.X && p.Location.Longitude == item.Coords.Y)
                    {
                        Pins.Remove(p);
                        break;
                    }
                }
            }
            UpdateMap();
            LBStations.Items.Refresh();
        }

        private void Create_Line_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
