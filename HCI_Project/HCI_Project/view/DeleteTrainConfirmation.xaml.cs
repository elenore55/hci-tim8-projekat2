using HCI_Project.view.dialogs;
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
using System.Windows.Shapes;

namespace HCI_Project.view
{
    /// <summary>
    /// Interaction logic for DeleteTrainConfirmation.xaml
    /// </summary>
    public partial class DeleteTrainConfirmation : Window
    {
        public event EventHandler OnConfirm;

        public DeleteTrainConfirmation()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            OnConfirm?.Invoke(this, EventArgs.Empty);
            this.Close();
            TrainDeleted td = new TrainDeleted();
            td.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            td.ShowDialog();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
