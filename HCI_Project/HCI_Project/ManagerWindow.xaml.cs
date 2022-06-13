using HCI_Project.model;
using HCI_Project.repository;
using HCI_Project.view;
using HCI_Project.view.LinesHandling;
using HCI_Project.view.Reports;
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
    /// Interaction logic for ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        private RepositoryFactory rf;
        public ManagerWindow()
        {
            InitializeComponent();
            rf = new RepositoryFactory();
            MainFrame.Content = new WelcomePage();
            DeactivateOldReservations();
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var p = MainFrame.Content as Page;
            HelpProvider.ShowHelp(p.Title, this);
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

        private void help_Click(object sender, RoutedEventArgs e)
        {
            var p = MainFrame.Content as Page;
            HelpProvider.ShowHelp(p.Title, this);
        }

        private void monthlyReport_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new MonthlyReport(rf);
        }

        private void departureReport_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new DepartureReport(rf);
        }

        private void trains_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new TrainsView(rf);
        }

        private void lines_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new TrainLinesCRUD(rf);
        }

        private void stations_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new StationCRUD(rf);
        }
        private void departures_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new LinesView(rf);
        }

        private void demo_Click(object sender, RoutedEventArgs e)
        {
            DemoPlayer m = new DemoPlayer(@"../../videos/create_schedule.mkv");
            m.ShowDialog();
        }

        private void logout_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Hide();
        }
    }
}
