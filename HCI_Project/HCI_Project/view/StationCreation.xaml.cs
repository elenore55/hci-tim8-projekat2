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
    public partial class StationCreation : Page
    {
        private System.Windows.Point StartPoint;
        private bool stationPicked = false;
        private bool newPoint = true;
        private Pushpin pinSelected;
        private RepositoryFactory rf;
        public StationCreation(RepositoryFactory rf)
        {
            this.rf = rf;
            InitializeComponent();
            MyMap.MouseDoubleClick += MyMap_MouseDoubleClick1;
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
                            PinMissing.MessageQueue.Enqueue($"Station '{s.Name}' has same coordinates!", null, null, null, false, true, TimeSpan.FromSeconds(3));
                            return;
                        }
                        if (s.Name == stationName.Text.Trim())
                        {
                            NameMissing.MessageQueue.Enqueue($"Station with same name exists!", null, null, null, false, true, TimeSpan.FromSeconds(3));
                            return;
                        }
                    }
                    Station station = new Station(new System.Windows.Point(pinSelected.Location.Latitude, pinSelected.Location.Longitude), stationName.Text.Trim());
                    rf.StationRepository.Add(station);
                    AddingInfo.MessageQueue.Enqueue($"Station '{stationName.Text.Trim()}' succesfuly added!", null, null, null, false, true, TimeSpan.FromSeconds(3));
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
