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
using System.Windows.Shapes;

namespace HCI_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
            Main.Content = new TicketPurchase(stationRepository, lineRepository);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AddStations()
        {
            StationRepository sr = new StationRepository();
            Station s1 = new Station()
            {
                Id = sr.GetNextId(),
                Name = "Stanica 1",
                Coords = new model.Point() { X = 3, Y = 5 },
                Type = StationType.Start
            };
            Station s2 = new Station()
            {
                Id = sr.GetNextId(),
                Name = "Stanica 2",
                Coords = new model.Point() { X = 5, Y = 5 },
                Type = StationType.Middle
            };
            Station s3 = new Station()
            {
                Id = sr.GetNextId(),
                Name = "Stanica 3",
                Coords = new model.Point() { X = 10, Y = 10 },
                Type = StationType.End
            };
            sr.Add(s1);
            sr.Add(s2);
            sr.Add(s3);
        }
    }
}
