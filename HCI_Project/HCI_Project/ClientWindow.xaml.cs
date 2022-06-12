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
    /// Interaction logic for ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private readonly RepositoryFactory rf;
        private readonly string email;

        public ClientWindow(string email)
        {
            InitializeComponent();
            this.email = email;
            rf = new RepositoryFactory();
            MainFrame.Content = new WelcomePage();
            // Populate(rf.StationRepository, rf.DepartureRepository, rf.LineRepository, rf.TrainRepository, rf.WagonRepository, rf.SeatRepository);
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

        private void purchase_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new TicketPurchase(email, rf);

        }

        private void tickets_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ClientsTickets(email, rf);
        }

        private void reservations_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ClientsReservations(email, rf);
        }

        private void help_Click(object sender, RoutedEventArgs e)
        {
            var p = MainFrame.Content as Page;
            HelpProvider.ShowHelp(p.Title, this);
        }

        private void network_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ClintLineNetwork(rf);
        }
    }
}