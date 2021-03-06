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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Location = Microsoft.Maps.MapControl.WPF.Location;

namespace HCI_Project.view
{
    /// <summary>
    /// Interaction logic for TrainLinesCRUD.xaml
    /// </summary>
    public partial class TrainLinesCRUD : Page
    {
        private readonly string BingMapsKey = "AinQ9hRJn7QhWLbnmUvC6OJ9RvqMuOWGDRkvSqOf5MUgrvbkmFHxHNg6aIjno0CM";
        public List<HCI_Project.model.Line> Lines { get; set; }
        public model.Line SelectedLine { get; set; }
        public LoadingWindow viewer = new LoadingWindow();
        private RepositoryFactory rf; 
        public TrainLinesCRUD(RepositoryFactory rf)
        {
            this.rf = rf;
            Lines = rf.LineRepository.GetAll();
            InitializeComponent();
            MyMap.CredentialsProvider = new ApplicationIdCredentialsProvider(BingMapsKey);
        }

        private void Add_Line(object sender, RoutedEventArgs e)
        {
            TrainLineCreation crud = new TrainLineCreation(rf);
            crud.DataChangedEvent += ProductListUpdate_DataChanged;
            crud.ShowDialog();
        }

        private void Display_Route(object sender, MouseButtonEventArgs e)
        {
            Thread viewerThread = new Thread(delegate ()
            {
                viewer = new LoadingWindow();
                viewer.Show();
                System.Windows.Threading.Dispatcher.Run();
            });

            viewerThread.SetApartmentState(ApartmentState.STA);
            viewerThread.Start();
            var item = ((FrameworkElement)e.OriginalSource).DataContext as model.Line;
            if (item != null)
            {
                SelectedLine = item;
                Draw_Map(item);
            }
            MyMap.UpdateLayout();
            Thread.Sleep(1000);
            System.Windows.Threading.Dispatcher.FromThread(viewerThread).InvokeShutdown();
        }

        private void Draw_Map(model.Line item)
        {
            List<Pushpin> pushpins = new List<Pushpin>();
            MyMap.Children.Clear();
            List<SimpleWaypoint> waypoints = new List<SimpleWaypoint>();
            foreach (Station s in item.Stations)
            {
                waypoints.Add(new SimpleWaypoint(s.Coords.X, s.Coords.Y));
                Location pinLocation = new Location();
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
            for (int i = 0; i < item.Stations.Count; i++)
            {
                pushpins[i].Content = i + 1;
            }
            pushpins.ForEach(x => MyMap.Children.Add(x));
        }

        private void Edit_Line(object sender, RoutedEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as model.Line;
            TrainLineCreation crud = new TrainLineCreation(rf, item);
            crud.DataChangedEvent += ProductListUpdate_DataChanged;
            crud.ShowDialog();
        }

        private void ProductListUpdate_DataChanged(object sender, EventArgs e, model.Line line)
        {
            Lines = rf.LineRepository.GetAll();
            Lines = Lines.OrderBy(o => o.Id).ToList();
            LBLines.ItemsSource = Lines;
            LBLines.Items.Refresh();
            SelectedLine = line;
            Draw_Map(line);
            MyMap.UpdateLayout();
        }



        private void Delete_Line(object sender, RoutedEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as model.Line;
            MessageBoxButton buttons = MessageBoxButton.YesNo;
            MessageBoxResult result = System.Windows.MessageBox.Show($"Are you sure you want to delete line {item.Id}?", "Delete Confirmation", buttons);
            if (result == MessageBoxResult.Yes)
            {
                  if (canBeDeleted(item))
                {
                    MessageBoxResult result2 = System.Windows.MessageBox.Show($"Line have occupied departures in future. Are you sure?", "Delete Confirmation", buttons);
                    if (result2 == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                if (SelectedLine != null)
                {
                    if (SelectedLine.Id == item.Id)
                    {
                        MyMap.Children.Clear();
                    }
                }
                Lines.Remove(item);
                rf.LineRepository.Delete(item.Id);
                LBLines.Items.Refresh();
                LineAdded.MessageQueue.Enqueue($"Line '{item.Id}' succesfuly deleted!", null, null, null, false, true, TimeSpan.FromSeconds(3));
                
            }
        }

        private bool canBeDeleted(model.Line item)
        {
            if (!item.CanBeDeleted()) return false;
            foreach(Reservation r in rf.ReservationRepository.GetAll())
            {
                if (r.IsActive || r.DepartureDate > DateTime.Now)
                {
                    if (item.Departures.Contains(rf.DepartureRepository.GetById(r.DepartureId))) return false;
                }
            }
            foreach(Ticket t in rf.TicketRepository.GetAll())
            {
                if (t.DepartureDate > DateTime.Now)
                {
                    if (item.Departures.Contains(rf.DepartureRepository.GetById(t.DepartureId))) return false;
                }
            }
            return true;
        }
    }
}
