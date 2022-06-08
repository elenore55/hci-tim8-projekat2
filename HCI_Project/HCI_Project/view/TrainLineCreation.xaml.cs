using BingMapsRESTToolkit;
using HCI_Project.model;
using HCI_Project.repository;
using HCI_Project.utils;
using MaterialDesignThemes.Wpf;
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
using Location = Microsoft.Maps.MapControl.WPF.Location;

namespace HCI_Project.view
{
    /// <summary>
    /// Interaction logic for TrainLineCreation.xaml
    /// </summary>
    public partial class TrainLineCreation : Page
    {
        private string BingMapsKey = "AinQ9hRJn7QhWLbnmUvC6OJ9RvqMuOWGDRkvSqOf5MUgrvbkmFHxHNg6aIjno0CM";
        private System.Windows.Point StartPoint;
        private List<Pushpin> StopPins = new List<Pushpin>();
        private List<Station> StopStations = new List<Station>();
        private bool IsFirstStation = true;
        private bool newPoint = true;
        private List<Pushpin> StationPins = new List<Pushpin>();
        private List<Station> Stations = new List<Station>();
        private RepositoryFactory rf;
        public TrainLineCreation(RepositoryFactory rf)
        {
            this.rf = rf;
            InitializeComponent();
            Stations = rf.StationRepository.GetAll();
            MyMap.CredentialsProvider = new ApplicationIdCredentialsProvider(BingMapsKey);
            MyMap.MouseDoubleClick += MyMap_MouseDoubleClick1;
            LBStations.ItemsSource = StopStations;
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
                StationPins.Add(pin);
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
                NoStops.IsActive = false;
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
                Location = pinLocation
            };
            pin.MouseDoubleClick += new MouseButtonEventHandler(Pin_Click);
            StopStations.Add(s);
            StopPins.Add(pin);
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
            StopPins.Remove(pin);
            foreach (Station s in StopStations)
            {
                if (s.Coords.X == pin.Location.Latitude && s.Coords.Y == pin.Location.Longitude)
                {
                    StopStations.Remove(s);
                    break;
                }
            }
            UpdateMap();
            IsFirstStation = StopPins.Count == 0;
        }

        private void UpdateMap()
        {
            MyMap.Children.Clear();
            List<SimpleWaypoint> waypoints = new List<BingMapsRESTToolkit.SimpleWaypoint>();
            foreach (Station s in StopStations)
            {
                waypoints.Add(new SimpleWaypoint(s.Coords.X, s.Coords.Y));
            }
            BingMapRESTServices.SendRequest(MyMap, waypoints);
            StopPins.ForEach(x => MyMap.Children.Add(x));
            StationPins.ForEach(x => MyMap.Children.Add(x));
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
                StopStations.Remove(item);
                foreach (Pushpin p in StopPins)
                {
                    if (p.Location.Latitude == item.Coords.X && p.Location.Longitude == item.Coords.Y)
                    {
                        StopPins.Remove(p);
                        break;
                    }
                }
            }
            UpdateMap();
            LBStations.Items.Refresh();
        }
        private void tbFrom_GotFocus(object sender, RoutedEventArgs e)
        {
            priceTxt.BorderBrush = Brushes.Gray;
        }

        private void tbFrom_LostFocus(object sender, RoutedEventArgs e)
        {
            MarkPriceTxtToRed();
        }

        private void MarkPriceTxtToRed()
        {
            if (priceTxt.Text.Trim() == "")
            {
                priceTxt.BorderBrush = Brushes.Red;
            }
        }

        private async void Create_Line_Click(object sender, RoutedEventArgs e)
        {
            if (StopStations.Count > 1)
            {
                if (priceTxt.Text.Trim().Length != 0)
                {
                    double price;
                    if (!Double.TryParse(priceTxt.Text, out price))
                    {
                        return;
                    }
                    List<SimpleWaypoint> waypoints = new List<SimpleWaypoint>();
                    foreach (Station s in StopStations)
                    {
                        waypoints.Add(new SimpleWaypoint(s.Coords.X, s.Coords.Y));
                    }
                    var routeRequest = new RouteRequest()
                    {
                        Waypoints = waypoints,
                        WaypointOptimization = BingMapsRESTToolkit.Extensions.TspOptimizationType.TravelDistance,
                        BingMapsKey = this.BingMapsKey
                    };
                    var r = await routeRequest.Execute();
                    var route = r.ResourceSets[0].Resources[0] as Route;
                    List<int> Offsets = new List<int>();
                    int last = 0;
                    foreach (RouteLeg rl in route.RouteLegs)
                    {
                        int current = (int)rl.TravelDuration/60;
                        Offsets.Add(last + current);
                        last = current;
                    }
                    model.Line l = new model.Line();
                    l.Stations = StopStations;
                    l.OffsetsInMinutes = Offsets.GetRange(0, Offsets.Count-1);
                    l.Price = price;
                    l.Id = rf.LineRepository.GetNextId();
                    LineAdded.MessageQueue.Enqueue($"Line '{l.Id}' succesfuly added!", null, null, null, false, true, TimeSpan.FromSeconds(3));
                }
            } else
            {
                NoStops.IsActive = true;
            }
        }
    }
}
