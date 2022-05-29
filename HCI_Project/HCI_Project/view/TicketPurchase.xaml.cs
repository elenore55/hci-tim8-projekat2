using HCI_Project.model;
using HCI_Project.repository;
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
    /// Interaction logic for TicketPurchase.xaml
    /// </summary>
    public partial class TicketPurchase : Page
    {
        public List<string> StationNames { get; set; }
        public TicketPurchase()
        {
            InitializeComponent();
            StationRepository sr = new StationRepository();
            List<Station> stations = sr.GetAll();
            StationNames = (from s in stations select s.Name).ToList();
            tbFrom.ItemsSource = StationNames;
            tbTo.ItemsSource = StationNames;
        }
    }
}
