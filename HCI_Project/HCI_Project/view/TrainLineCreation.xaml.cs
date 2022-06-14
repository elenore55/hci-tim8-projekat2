using BingMapsRESTToolkit;
using HCI_Project.model;
using HCI_Project.repository;
using HCI_Project.utils;
using MaterialDesignThemes.Wpf;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
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
    public partial class TrainLineCreation : Window
    {
        private const uint WS_EX_CONTEXTHELP = 0x00000400;
        private const uint WS_MINIMIZEBOX = 0x00020000;
        private const uint WS_MAXIMIZEBOX = 0x00010000;
        private const int GWL_STYLE = -16;
        private const int GWL_EXSTYLE = -20;
        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_FRAMECHANGED = 0x0020;
        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_CONTEXTHELP = 0xF180;


        [DllImport("user32.dll")]
        private static extern uint GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, uint newStyle);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);


        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            uint styles = GetWindowLong(hwnd, GWL_STYLE);
            styles &= 0xFFFFFFFF ^ (WS_MINIMIZEBOX | WS_MAXIMIZEBOX);
            SetWindowLong(hwnd, GWL_STYLE, styles);
            styles = GetWindowLong(hwnd, GWL_EXSTYLE);
            styles |= WS_EX_CONTEXTHELP;
            SetWindowLong(hwnd, GWL_EXSTYLE, styles);
            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
            ((HwndSource)PresentationSource.FromVisual(this)).AddHook(HelpHook);
        }

        private IntPtr HelpHook(IntPtr hwnd,
                int msg,
                IntPtr wParam,
                IntPtr lParam,
                ref bool handled)
        {
            if (msg == WM_SYSCOMMAND &&
                    ((int)wParam & 0xFFF0) == SC_CONTEXTHELP)
            {
                HelpProvider.ShowHelp("TrainLineCreation", null);
                handled = true;
            }
            return IntPtr.Zero;
        }

        private readonly string BingMapsKey = "AinQ9hRJn7QhWLbnmUvC6OJ9RvqMuOWGDRkvSqOf5MUgrvbkmFHxHNg6aIjno0CM";
        public delegate void DataChangedEventHandler(object sender, EventArgs e, model.Line line);
        public event DataChangedEventHandler DataChangedEvent;
        private System.Windows.Point StartPoint;
        private List<Pushpin> StopPins = new List<Pushpin>();
        private List<Station> StopStations = new List<Station>();
        private bool IsFirstStation = true;
        private bool newPoint = true;
        private List<Pushpin> StationPins = new List<Pushpin>();
        private List<Station> Stations = new List<Station>();
        private RepositoryFactory rf;
        private bool isEdit = false;
        private model.Line editedLine;
        public TrainLineCreation(RepositoryFactory rf, model.Line l = null)
        {
            this.rf = rf;
            Title = "Line Creation";
            if (l != null)
            {
                Title = "Line Edit";
                editedLine = l;
                isEdit = true;
                StopStations = l.Stations;
                IsFirstStation = false;
                foreach (Station s in StopStations)
                {
                    Location pinLocation = new Location();
                    pinLocation.Latitude = s.Coords.X;
                    pinLocation.Longitude = s.Coords.Y;
                    Pushpin pin = new Pushpin
                    {
                        Location = pinLocation
                    };
                    pin.MouseDoubleClick += new MouseButtonEventHandler(Pin_Click);
                    StopPins.Add(pin);
                }
            }
            InitializeComponent();
            if(isEdit)
            {
                Create_Save.Content = "Save";
                priceTxt.Text = editedLine.Price.ToString();
                percentageTxt.Text = editedLine.FirstClassPercentage.ToString();
            }
            Stations = rf.StationRepository.GetAll();
            MyMap.CredentialsProvider = new ApplicationIdCredentialsProvider(BingMapsKey);
            MyMap.MouseDoubleClick += MyMap_MouseDoubleClick1;
            LBStations.ItemsSource = StopStations;
            addStations();
            UpdateMap();
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            HelpProvider.ShowHelp("TrainLineCreation", this);
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
            foreach(Station stat in StopStations)
            {
                if (stat.Coords.X == s.Coords.X && stat.Coords.Y == s.Coords.Y)
                {
                    NoStops.MessageQueue.Enqueue($"This '{stat.Name}' stop is already in line!", null, null, null, false, true, TimeSpan.FromSeconds(3));
                    UpdateMap();
                    return;
                }
            }
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
            List<SimpleWaypoint> waypoints = new List<SimpleWaypoint>();
            foreach (Station s in StopStations)
            {
                waypoints.Add(new SimpleWaypoint(s.Coords.X, s.Coords.Y));
            }
            BingMapRESTServices.SendRequest(MyMap, waypoints);
            for (int i = 0; i < StopPins.Count; i++)
            {
                StopPins[i].Content = i + 1;
            }
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
        private void price_GotFocus(object sender, RoutedEventArgs e)
        {
            priceTxt.BorderBrush = Brushes.Gray;
        }

        private void price_LostFocus(object sender, RoutedEventArgs e)
        {
            MarkPriceTxtToRed();
        }

        private void percentage_GotFocus(object sender, RoutedEventArgs e)
        {
            percentageTxt.BorderBrush = Brushes.Gray;
        }

        private void percentage_LostFocus(object sender, RoutedEventArgs e)
        {
            MarkPercentageTxtToRed();
        }

        private void MarkPercentageTxtToRed()
        {
            if (percentageTxt.Text.Trim() == "")
            {
                percentageTxt.BorderBrush = Brushes.Red;
            }
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
                        NoPrice.MessageQueue.Enqueue($"Price must be numbers!", null, null, null, false, true, TimeSpan.FromSeconds(3));
                        priceTxt.BorderBrush = Brushes.Red;
                        return;
                    } else if (price < 0)
                    {
                        NoPrice.MessageQueue.Enqueue($"Price must be positive!", null, null, null, false, true, TimeSpan.FromSeconds(3));
                        priceTxt.BorderBrush = Brushes.Red;
                        return;
                    }
                    if (percentageTxt.Text.Trim().Length != 0) {
                        int percentage;
                        if (!Int32.TryParse(percentageTxt.Text, out percentage))
                        {
                            NoPercentage.MessageQueue.Enqueue($"Percentage must be numbers!", null, null, null, false, true, TimeSpan.FromSeconds(3));
                            percentageTxt.BorderBrush = Brushes.Red;
                            return;
                        } else if (percentage < 0 || percentage > 100)
                        {
                            NoPercentage.MessageQueue.Enqueue($"Percentage must be in range 0-100!", null, null, null, false, true, TimeSpan.FromSeconds(3));
                            percentageTxt.BorderBrush = Brushes.Red;
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
                        Offsets.Add(0);
                        foreach (RouteLeg rl in route.RouteLegs)
                        {
                            Offsets.Add((int)rl.TravelDuration / 60);
                        }
                        model.Line l;
                        if (isEdit)
                        {
                            l = editedLine;
                            rf.LineRepository.Delete(l.Id);
                            l.Departures = editedLine.Departures;
                        }
                        else 
                        {
                            l = new model.Line();
                            l.Id = rf.LineRepository.GetNextId();
                            l.Departures = new List<Departure>();
                        }
                        l.Stations = StopStations;
                        l.OffsetsInMinutes = Offsets.GetRange(0, Offsets.Count - 1);
                        l.Price = price;
                        l.LastStation = StopStations[StopStations.Count - 1].Name;
                        l.FirstClassPercentage = percentage;
                        rf.LineRepository.Add(l);
                        DataChangedEventHandler handler = DataChangedEvent;
                        if (handler != null)
                        {
                            handler(this, new EventArgs(), l);
                        }
                        if (isEdit) LineAdded.MessageQueue.Enqueue($"Line '{l.Id}' edited!", null, null, null, false, true, TimeSpan.FromSeconds(3));
                        else LineAdded.MessageQueue.Enqueue($"Line '{l.Id}' succesfuly added!", null, null, null, false, true, TimeSpan.FromSeconds(3));
                    } 
                    else
                    {
                        NoPercentage.MessageQueue.Enqueue($"Percentage must be entered!", null, null, null, false, true, TimeSpan.FromSeconds(3));
                        percentageTxt.BorderBrush = Brushes.Red;
                    }
                } else
                {
                    NoPrice.MessageQueue.Enqueue($"Price must be entered!", null, null, null, false, true, TimeSpan.FromSeconds(3));
                    MarkPriceTxtToRed();
                }
            } else
            {
                NoStops.MessageQueue.Enqueue($"Add 2 or more stops!", null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }
    }
}
