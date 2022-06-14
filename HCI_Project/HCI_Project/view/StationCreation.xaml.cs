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
    /// Interaction logic for StationCreation.xaml
    /// </summary>
    public partial class StationCreation : Window
    {
        private string BingMapsKey = "AinQ9hRJn7QhWLbnmUvC6OJ9RvqMuOWGDRkvSqOf5MUgrvbkmFHxHNg6aIjno0CM";
        public delegate void DataChangedEventHandler(object sender, EventArgs e);
        public event DataChangedEventHandler DataChangedEvent;
        private System.Windows.Point StartPoint;
        private bool stationPicked = false;
        private bool newPoint = true;
        private Pushpin pinSelected;
        private RepositoryFactory rf;
        private bool isEdit = false;
        private Station editedStation;
        public StationCreation(RepositoryFactory rf, Station s = null)
        {
            this.rf = rf;
            Title = "Station Creation";
            if (s != null)
            {
                Title = "Station Edit";
                isEdit = true;
                editedStation = s;
                Location pinLocation = new Location();
                pinLocation.Latitude = s.Coords.X;
                pinLocation.Longitude = s.Coords.Y;
                Pushpin pin = new Pushpin
                {
                    Location = pinLocation
                };
                pin.MouseDoubleClick += new MouseButtonEventHandler(Pin_Click);
                pinSelected = pin;
                stationPicked = true;
            }
            InitializeComponent();
            MyMap.CredentialsProvider = new ApplicationIdCredentialsProvider(BingMapsKey);
            MyMap.MouseDoubleClick += MyMap_MouseDoubleClick1;
            if (isEdit)
            {
                stationName.Text = editedStation.Name;
                MyMap.Children.Add(pinSelected);
                MyMap.Center = pinSelected.Location;
                stationPin.Cursor = Cursors.No;
                Create_save.Content = "Save";
                MyMap.UpdateLayout();
            }
        }
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            HelpProvider.ShowHelp("StationCreation", this);
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
            e.Handled = true;
            System.Windows.Point pinPoint = e.GetPosition(MyMap);
            Location pinLocation = MyMap.ViewportPointToLocation(pinPoint);
            Pushpin pin = new Pushpin
            {
                Location = pinLocation,
                ToolTip = pinLocation.ToString()
            };
            pin.MouseDoubleClick += new MouseButtonEventHandler(Pin_Click);
            MyMap.Children.Clear();
            MyMap.Children.Add(pin);
            pinSelected = pin;
            stationPicked = true;
            stationPin.Cursor = Cursors.No;
            stationPin.MouseMove -= new MouseEventHandler(Pushpin_MouseMove);
        }
        private void Pin_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Pushpin pin = (Pushpin)sender;
            MyMap.Children.Remove(pin);
            newPoint = true;
            stationPicked = false;
            stationPin.MouseMove += new MouseEventHandler(Pushpin_MouseMove);
            stationPin.Cursor = Cursors.Hand;
            pinSelected = null;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (stationPicked)
            {
                if (stationName.Text.Trim().Length != 0)
                {
                    foreach(Station s in rf.StationRepository.GetAll())
                    {
                        if (s.Coords.X == pinSelected.Location.Latitude && s.Coords.Y == pinSelected.Location.Longitude)
                        {
                            if (isEdit) 
                            {
                                if (editedStation.Coords.X == pinSelected.Location.Latitude && editedStation.Coords.Y == pinSelected.Location.Longitude)
                                {
                                    break;
                                }
                            }
                            PinMissing.MessageQueue.Enqueue($"Station '{s.Name}' has same coordinates!", null, null, null, false, true, TimeSpan.FromSeconds(3));
                            return;
                            
                        }
                        if (s.Name == stationName.Text.Trim())
                        {
                            if (isEdit)
                            {
                                if (editedStation.Name == stationName.Text.Trim())
                                {
                                    break;
                                }
                            }
                            NameMissing.MessageQueue.Enqueue($"Station with same name exists!", null, null, null, false, true, TimeSpan.FromSeconds(3));
                            return;
                        }
                    }
                    Station station;
                    if (isEdit)
                    {
                        rf.StationRepository.Delete(editedStation.Id);
                        station = editedStation;
                        station.Name = stationName.Text.Trim();
                        station.Coords = new System.Windows.Point(pinSelected.Location.Latitude, pinSelected.Location.Longitude);
                    } else
                    {
                        station = new Station(new System.Windows.Point(pinSelected.Location.Latitude, pinSelected.Location.Longitude), stationName.Text.Trim());
                        station.Id = rf.StationRepository.GetNextId();
                    }
                    rf.StationRepository.Add(station);
                    DataChangedEventHandler handler = DataChangedEvent;
                    if (handler != null)
                    {
                        handler(this, new EventArgs());
                    }
                    if (isEdit) AddingInfo.MessageQueue.Enqueue($"Station '{stationName.Text.Trim()}' succesfuly updated!", null, null, null, false, true, TimeSpan.FromSeconds(3));
                    else AddingInfo.MessageQueue.Enqueue($"Station '{stationName.Text.Trim()}' succesfuly added!", null, null, null, false, true, TimeSpan.FromSeconds(3));
                }
                else
                {
                    MakeStationTxtRed();
                    NameMissing.MessageQueue.Enqueue($"Enter station name!", null, null, null, false, true, TimeSpan.FromSeconds(3));
                }
            }
            else
            {
                PinMissing.MessageQueue.Enqueue($"Place pin on the map!", null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        private void tbFrom_GotFocus(object sender, RoutedEventArgs e)
        {
            stationName.BorderBrush = Brushes.Gray;
        }

        private void tbFrom_LostFocus(object sender, RoutedEventArgs e)
        {
            MakeStationTxtRed();
        }

        private void MakeStationTxtRed()
        {
            if (stationName.Text.Trim() == "")
            {
                stationName.BorderBrush = Brushes.Red;
            }
        }
    }
}
