using HCI_Project.model;
using HCI_Project.repository;
using HCI_Project.view;
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

namespace HCI_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RepositoryFactory rf;

        public MainWindow()
        {
            InitializeComponent();
            rf = new RepositoryFactory();
            Populate(rf.StationRepository, rf.DepartureRepository, rf.LineRepository, rf.TrainRepository, rf.WagonRepository, rf.SeatRepository);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Populate(StationRepository sr, DepartureRepository dr, LineRepository lr, TrainRepository tr, WagonRepository wr, SeatRepository sr1)
        {
            sr.ClearAll();
            dr.ClearAll();
            lr.ClearAll();
            tr.ClearAll();
            wr.ClearAll();
            sr1.ClearAll();
            Station s1 = new Station()
            {
                Id = sr.GetNextId(),
                Name = "Stanica 1",
                Coords = new model.Point() { X = 3, Y = 5 },
                Type = StationType.Start,
                LineId = 1
            };
            Station s2 = new Station()
            {
                Id = sr.GetNextId(),
                Name = "Stanica 2",
                Coords = new model.Point() { X = 5, Y = 5 },
                Type = StationType.Middle,
                LineId = 1
            };
            Station s3 = new Station()
            {
                Id = sr.GetNextId(),
                Name = "Stanica 3",
                Coords = new model.Point() { X = 10, Y = 10 },
                Type = StationType.End,
                LineId = 1
            };
            Station s4 = new Station()
            {
                Id = sr.GetNextId(),
                Name = "Stanica 4",
                Coords = new model.Point() { X = 6, Y = 5 },
                Type = StationType.Start,
                LineId = 2
            };
            Station s5 = new Station()
            {
                Id = sr.GetNextId(),
                Name = "Stanica 5",
                Coords = new model.Point() { X = 6, Y = 5 },
                Type = StationType.Middle,
                LineId = 2
            };
            Station s7 = new Station()
            {
                Id = sr.GetNextId(),
                Name = "Stanica 7",
                Coords = new model.Point() { X = 6, Y = 55 },
                Type = StationType.Middle,
                LineId = 2
            };
            Station s6 = new Station()
            {
                Id = sr.GetNextId(),
                Name = "Stanica 6",
                Coords = new model.Point() { X = 15, Y = 10 },
                Type = StationType.End,
                LineId = 2
            };
            sr.Add(s1);
            sr.Add(s2);
            sr.Add(s3);
            sr.Add(s4);
            sr.Add(s5);
            sr.Add(s6);
            sr.Add(s7);

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    long id = sr1.GetNextId();
                    int wagonId = 1;
                    if (id > 20) wagonId = 2;
                    Seat seat = new Seat()
                    {
                        Id = id,
                        Row = i % 5,
                        Column = j,
                        WagonId = wagonId
                    };
                    sr1.Add(seat);
                }
            }

            Wagon wagon = new Wagon()
            {
                Id = wr.GetNextId(),
                Ordinal = 0,
                Class = WagonClass.First,
                Rows = 5,
                SeatsPerRow = 4,
                Seats = sr1.GetAll().Where(x => x.Id <= 20).ToList()
            };
            Wagon wagon2 = new Wagon()
            {
                Id = wr.GetNextId(),
                Ordinal = 1,
                Class = WagonClass.Second,
                Rows = 5,
                SeatsPerRow = 4,
                Seats = sr1.GetAll().Where(x => x.Id > 20).ToList()
            };
            wr.Add(wagon);
            wr.Add(wagon2);

            Train train = new Train()
            {
                Id = tr.GetNextId(),
                Wagons = new List<Wagon>() { wagon, wagon2 }
            };
            tr.Add(train);

            Departure d1 = new Departure()
            {
                Id = dr.GetNextId(),
                StartTime = new DateTime(2022, 6, 1, 13, 0, 0),
                LineId = 1,
                Train = train
            };
            Departure d2 = new Departure()
            {
                Id = dr.GetNextId(),
                StartTime = new DateTime(2022, 6, 1, 14, 30, 0),
                LineId = 1,
                Train = train
            };
            Departure d3 = new Departure()
            {
                Id = dr.GetNextId(),
                StartTime = new DateTime(2022, 6, 1, 18, 0, 0),
                LineId = 1,
                Train = train
            };
            Departure d4 = new Departure()
            {
                Id = dr.GetNextId(),
                StartTime = new DateTime(2022, 6, 1, 11, 0, 0),
                LineId = 2,
                Train = train
            };
            Departure d5 = new Departure()
            {
                Id = dr.GetNextId(),
                StartTime = new DateTime(2022, 6, 1, 15, 15, 0),
                LineId = 2,
                Train = train
            };
            Departure d6 = new Departure()
            {
                Id = dr.GetNextId(),
                StartTime = new DateTime(2022, 6, 1, 20, 20, 0),
                LineId = 2,
                Train = train
            };
            dr.Add(d1);
            dr.Add(d2);
            dr.Add(d3);
            dr.Add(d4);
            dr.Add(d5);
            dr.Add(d6);

            Line l1 = new model.Line()
            {
                Id = lr.GetNextId(),
                Departures = new List<Departure>() { d1, d2, d3 },
                Stations = new List<Station>() { s1, s2, s3 },
                Price = 50,
                OffsetsInMinutes = new List<int>() { 0, 45, 60 }
            };
            Line l2 = new model.Line()
            {
                Id = lr.GetNextId(),
                Departures = new List<Departure>() { d4, d5, d6 },
                Stations = new List<Station>() { s4, s5, s7, s6 },
                Price = 60,
                OffsetsInMinutes = new List<int>() { 0, 45, 60, 100 }
            };
            lr.Add(l1);
            lr.Add(l2);
        }


        private void purchase_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new TicketPurchase(rf);
        }

        private void tickets_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ClientsTickets(rf);
        }
    }
}
