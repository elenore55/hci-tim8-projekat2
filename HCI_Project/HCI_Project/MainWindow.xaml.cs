﻿using HCI_Project.model;
using HCI_Project.repository;
using HCI_Project.utils;
using HCI_Project.view;
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

namespace HCI_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string BingMapsKey = "AinQ9hRJn7QhWLbnmUvC6OJ9RvqMuOWGDRkvSqOf5MUgrvbkmFHxHNg6aIjno0CM";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*Client c = new Client()
            {
                Name = "Milica"
            };
            ClientRepository cr = new ClientRepository();
            cr.Add(c);
            DepartureRepository dr = new DepartureRepository();
            WagonRepository wr = new WagonRepository();*/

            StationRepository stationRepository = new StationRepository();
            LineRepository lineRepository = new LineRepository();
            DepartureRepository departureRepository = new DepartureRepository();
            // Populate(stationRepository, departureRepository, lineRepository);
            Main.Content = new TrainLineCreation();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Populate(StationRepository sr, DepartureRepository dr, LineRepository lr)
        {
            Station s1 = new Station()
            {
                Id = sr.GetNextId(),
                Name = "Stanica 1",
                Coords = new System.Windows.Point { X = 3, Y = 5 }
            };
            Station s2 = new Station()
            {
                Id = sr.GetNextId(),
                Name = "Stanica 2",
                Coords = new System.Windows.Point { X = 5, Y = 5 }
            };
            Station s3 = new Station()
            {
                Id = sr.GetNextId(),
                Name = "Stanica 3",
                Coords = new System.Windows.Point { X = 10, Y = 10 }
            };
            Station s4 = new Station()
            {
                Id = sr.GetNextId(),
                Name = "Stanica 4",
                Coords = new System.Windows.Point { X = 6, Y = 5 }
            };
            Station s5 = new Station()
            {
                Id = sr.GetNextId(),
                Name = "Stanica 5",
                Coords = new System.Windows.Point { X = 6, Y = 5 }
            };
            Station s7 = new Station()
            {
                Id = sr.GetNextId(),
                Name = "Stanica 7",
                Coords = new System.Windows.Point { X = 6, Y = 55 }
            };
            Station s6 = new Station()
            {
                Id = sr.GetNextId(),
                Name = "Stanica 6",
                Coords = new System.Windows.Point { X = 15, Y = 10 }
            };
            sr.Add(s1);
            sr.Add(s2);
            sr.Add(s3);
            sr.Add(s4);
            sr.Add(s5);
            sr.Add(s6);
            sr.Add(s7);

            Departure d1 = new Departure()
            {
                Id = dr.GetNextId(),
                StartTime = new DateTime(2022, 6, 1, 13, 0, 0),
                LineId = 1
            };
            Departure d2 = new Departure()
            {
                Id = dr.GetNextId(),
                StartTime = new DateTime(2022, 6, 1, 14, 30, 0),
                LineId = 1
            };
            Departure d3 = new Departure()
            {
                Id = dr.GetNextId(),
                StartTime = new DateTime(2022, 6, 1, 18, 0, 0),
                LineId = 1
            };
            Departure d4 = new Departure()
            {
                Id = dr.GetNextId(),
                StartTime = new DateTime(2022, 6, 1, 11, 0, 0),
                LineId = 2
            };
            Departure d5 = new Departure()
            {
                Id = dr.GetNextId(),
                StartTime = new DateTime(2022, 6, 1, 15, 15, 0),
                LineId = 2
            };
            Departure d6 = new Departure()
            {
                Id = dr.GetNextId(),
                StartTime = new DateTime(2022, 6, 1, 20, 20, 0),
                LineId = 2
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
                Price =  50
            };
            Line l2 = new model.Line()
            {
                Id = lr.GetNextId(),
                Departures = new List<Departure>() { d4, d5, d6 },
                Stations = new List<Station>() { s4, s5, s7, s6 },
                Price = 60
            };
            lr.Add(l1);
            lr.Add(l2);
        }
    }
}
