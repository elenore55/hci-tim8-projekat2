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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
            this.WindowHeight = System.Windows.SystemParameters.FullPrimaryScreenHeight / 3 * 2;
            this.WindowWidth = System.Windows.SystemParameters.FullPrimaryScreenWidth / 2;

        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            RepositoryFactory rf = new RepositoryFactory();
            Console.WriteLine("Uneseni mejl je " + usernameField.Text);
            foreach (Client c in rf.ClientRepository.GetAll())
            {
                Console.WriteLine("Ime " + c.Name);
            }
        }


    }
}
