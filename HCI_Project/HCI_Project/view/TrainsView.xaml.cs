using HCI_Project.model;
using HCI_Project.repository;
using HCI_Project.view.TrainHandling;
using HCI_Project.view.Trains;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
    /// Interaction logic for Trains.xaml
    /// </summary>
    public partial class TrainsView : Page
    {
        private RepositoryFactory rf;
        public ObservableCollection<TrainDTO> Rows { get; set; }

        public List<string> TrainNames { get; set; }
        private long selectedId { get; set; }
        public TrainsView(RepositoryFactory rf)
        {
            this.rf = rf;
            Rows = new ObservableCollection<TrainDTO>();
            createTrainDTOs();
            InitializeComponent();
            trainName.ItemsSource = TrainNames;
            findTrainNames();
#if DEBUG
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
#endif
        }

        private void findTrainNames()
        {
            TrainNames = new List<string>();
            foreach (Train t in rf.TrainRepository.GetAll())
            {
                TrainNames.Add(t.Name);
            }
        }

        private void createTrainDTOs()
        {
            //ObservableCollection<TrainDTO> retVal = new ObservableCollection<TrainDTO>();
            Rows.Clear();
            List<Train> trains = rf.TrainRepository.GetAll();
            foreach (Train t in trains)
            {
                Rows.Add(new TrainDTO(t.Id, t.Name, t.Wagons.Count));
            }
            //return retVal;

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            TrainDTO selected = trainsGrid.SelectedItem as TrainDTO;
            selectedId = selected.Id;

            DeleteTrainConfirmation dtt = new DeleteTrainConfirmation();
            dtt.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            dtt.OnConfirm += DeleteTrain;
            dtt.ShowDialog();

        }

        public void DeleteTrain(object sender, EventArgs e)
        {

            rf.TrainRepository.Delete(selectedId);
            removeFromRow();

        }

        private void removeFromRow()
        {
            foreach (TrainDTO t in Rows)
            {
                if (t.Id == selectedId)
                {
                    Rows.Remove(t);
                    return;
                }
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService navService = NavigationService.GetNavigationService(this);
            //NavigationService.Navigate(new TrainEdit());
            //this.NavigationService.Navigate(new TrainEdit());
            //this.Content = new TrainEdit();
            TrainDTO selected = trainsGrid.SelectedItem as TrainDTO;
            selectedId = selected.Id;
            /*Window wnd = Window.GetWindow(this);
            wnd.Content = new TrainEdit(rf, selectedId);*/
            TrainEditWindow tew = new TrainEditWindow(rf, selectedId);
            tew.WindowState = WindowState.Maximized; 
            tew.ShowDialog();
            
        }

        private void btnAddTrain_Click(object sender, RoutedEventArgs e)
        {
            /*Window wnd = Window.GetWindow(this);
            wnd.Content = new AddTrain(rf);*/
            AddTrainWindow atw = new AddTrainWindow(rf);
            atw.WindowState = WindowState.Maximized;
            atw.ShowDialog();

        }

        private void btnFilter_Click (object sender, RoutedEventArgs e)
        {
            string enteredName = trainName.Text;
            if (enteredName!="")
            {
                Rows.Clear();
                filterTrains(enteredName);
                //trainsGrid.Items.Refresh();
            }
            else
            {
                createTrainDTOs();
                //trainsGrid.Items.Refresh();
            }
        }

        private void filterTrains(string enteredName)
        {
            //ObservableCollection<TrainDTO> result = new ObservableCollection<TrainDTO>();
            Rows.Clear();
            List<Train> trains = rf.TrainRepository.GetAll();
            foreach (Train t in trains)
            {
                if (t.Name.ToLower().Contains(enteredName.ToLower()))
                {
                    Rows.Add(new TrainDTO(t.Id, t.Name, t.Wagons.Count));
                    //Rows.Remove(t);
                }
            }   
        }

       
        public class TrainDTO
        {
            public long Id { get; set; }
            public String Name { get; set; }
            public int NumOfWagons { get; set; }


            public TrainDTO(long id, String name, int numOfWagons)
            {
                this.Id = id;
                this.Name = name;
                this.NumOfWagons = numOfWagons;
            }


        }


    }
}
