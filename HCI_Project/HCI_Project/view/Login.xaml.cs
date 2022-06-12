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
        private RepositoryFactory rf;
        public Login(RepositoryFactory r)
        {
            this.rf = r;
            InitializeComponent();
            FocusManager.SetFocusedElement(this, emailField);
            Keyboard.Focus(emailField);
            //this.WindowHeight = System.Windows.SystemParameters.FullPrimaryScreenHeight / 3 * 2;
            //this.WindowWidth = System.Windows.SystemParameters.FullPrimaryScreenWidth / 2;

        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            foreach (Client c in rf.ClientRepository.GetAll())
            {
                if (c.Email.Equals(emailField.Text) && c.Password.Equals(passwordField.Password.ToString()))
                {
                    Window wnd = Window.GetWindow(this);
                    wnd.Close();
                    ClientWindow cw = new ClientWindow(emailField.Text);
                    cw.Show();
                    return;
                }
            }

            foreach (Manager m in rf.ManagerRepository.GetAll())
            {
                if (m.Email.Equals(emailField.Text) && m.Password.Equals(passwordField.Password.ToString()))
                {
                    ManagerWindow mw = new ManagerWindow();
                    mw.Show();
                    Window wnd = Window.GetWindow(this);
                    wnd.Close();
                    return; 
                }
            }
            error.Visibility = Visibility.Visible;
            loginBtn.Margin = new Thickness(0, 10, 0, 80);
        }


    }
}
