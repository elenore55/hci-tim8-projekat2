using HCI_Project.model;
using HCI_Project.repository;
using HCI_Project.utils;
using HCI_Project.view;
using HCI_Project.view.LinesHandling;
using HCI_Project.view.Reports;
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
            DeactivateOldReservations();
            MainFrame.Content = new Login(rf);
        }

        
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var p = MainFrame.Content as Page;
            HelpProvider.ShowHelp(p.Title, this);
            StationRepository stationRepository = new StationRepository();
            LineRepository lineRepository = new LineRepository();
            DepartureRepository departureRepository = new DepartureRepository();
            MainFrame.Content = new TrainLinesCRUD(rf);
        }
        private void DeactivateOldReservations()
        {
            int days = 3;
            List<Reservation> toDeactivate = new List<Reservation>();
            foreach (Reservation r in rf.ReservationRepository.GetAll())
            {
                if (r.IsActive && (r.DepartureDate - DateTime.Now).TotalDays < days)
                {
                    r.IsActive = false;
                    toDeactivate.Add(r);
                }
            }
            foreach (Reservation r in toDeactivate)
            {
                rf.ReservationRepository.Update(r);
            }
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
            sr.Add(new Station(new System.Windows.Point(45.7737908329366, 19.1170693111155), "Sombor") {Id = sr.GetNextId() });
            sr.Add(new Station(new System.Windows.Point(43.3168743676544, 21.8930174489589), "Nis") { Id = sr.GetNextId() });
            sr.Add(new Station(new System.Windows.Point(44.1845546497502, 21.1054707255333), "Lapovo") { Id = sr.GetNextId() });
            sr.Add(new Station(new System.Windows.Point(44.2737853394934, 19.8837362826773), "Valjevo") { Id = sr.GetNextId() });
            sr.Add(new Station(new System.Windows.Point(43.8468213035184, 20.0354259952504), "Pozega") { Id = sr.GetNextId() });
            sr.Add(new Station(new System.Windows.Point(43.8560751522863, 19.8432587298032), "Uzice") { Id = sr.GetNextId() });
            sr.Add(new Station(new System.Windows.Point(45.1224246748143, 21.2969962172208), "Vrsac") { Id = sr.GetNextId() });
            sr.Add(new Station(new System.Windows.Point(45.126348777821, 19.2292180982901), "Sid") { Id = sr.GetNextId() });
            sr.Add(new Station(new System.Windows.Point(44.0132043068434, 20.9241081767304), "Kragujevac") { Id = sr.GetNextId() });
            sr.Add(new Station(new System.Windows.Point(44.4401983117903, 20.6913074485451), "Mladenovac") { Id = sr.GetNextId() });
            sr.Add(new Station(new System.Windows.Point(44.8068530443879, 20.4176244378858), "Beograd") { Id = sr.GetNextId() });
            sr.Add(new Station(new System.Windows.Point(45.2653991901631, 19.8295025367481), "Novi Sad") { Id = sr.GetNextId() });
            sr.Add(new Station(new System.Windows.Point(43.9167290500169, 21.3734995473952), "Cuprija") { Id = sr.GetNextId() });
            sr.Add(new Station(new System.Windows.Point(43.1423009222924, 22.5993454464567), "Pirot") { Id = sr.GetNextId() });
            sr.Add(new Station(new System.Windows.Point(43.9825635232647, 21.2643563898127), "Jagodina") { Id = sr.GetNextId() });
            sr.Add(new Station(new System.Windows.Point(43.8898154372637, 20.3559119017471), "Cacak") { Id = sr.GetNextId() });
            sr.Add(new Station(new System.Windows.Point(43.5638272233543, 19.5416579208959), "Priboj") { Id = sr.GetNextId() });
            sr.Add(new Station(new System.Windows.Point(43.1335608534931, 21.2681129272782), "Kursumlija") { Id = sr.GetNextId() });
            sr.Add(new Station(new System.Windows.Point(42.6578408443893, 21.150085751261), "Pristina") { Id = sr.GetNextId() });

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Seat seat = new Seat()
                    {
                        Id = sr1.GetNextId(),
                        Row = i,
                        Column = j,
                        WagonId = 1
                    };
                    sr1.Add(seat);
                }
            }

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Seat seat = new Seat()
                    {
                        Id = sr1.GetNextId(),
                        Row = i,
                        Column = j,
                        WagonId = 2
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
                Rows = 6,
                SeatsPerRow = 5,
                Seats = sr1.GetAll().Where(x => x.Id > 20).ToList()
            };
            wr.Add(wagon);
            wr.Add(wagon2);

            Train train = new Train()
            {
                Id = tr.GetNextId(),
                Name = "Moj voz",
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
                Stations = new List<Station>() { sr.GetById(1), sr.GetById(2), sr.GetById(3) },
                Price = 50,
                OffsetsInMinutes = new List<int>() { 0, 45, 60 },
                LastStation = sr.GetById(3).Name,
                FirstClassPercentage = 10
            };
            Line l2 = new model.Line()
            {
                Id = lr.GetNextId(),
                Departures = new List<Departure>() { d4, d5, d6 },
                Stations = new List<Station>() { sr.GetById(4), sr.GetById(5), sr.GetById(6), sr.GetById(7) },
                Price = 60,
                OffsetsInMinutes = new List<int>() { 0, 45, 60, 100 },
                LastStation = sr.GetById(7).Name,
                FirstClassPercentage = 10
            };
            lr.Add(l1);
            lr.Add(l2);
        }


        private void help_Click(object sender, RoutedEventArgs e)
        {
            var p = MainFrame.Content as Page;
            HelpProvider.ShowHelp(p.Title, this);
        }

        
    }
}