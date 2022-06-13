using HCI_Project.model;
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
using HCI_Project.repository;
using WPFCustomMessageBox;
using MaterialDesignThemes.Wpf;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
namespace HCI_Project.view.TrainHandling
{
    /// <summary>
    /// Interaction logic for TrainEditWindow.xaml
    /// </summary>
    public partial class TrainEditWindow : Window
    {
        public int NumberOfRows { get; set; }
        public int NumberOfColumns { get; set; }

        private readonly string FIRST_CLASS = "#d1d19b";
        private readonly string SECOND_CLASS = "#8ba6b3";

        private Button selectedWagon;
        private string selectedWagonClass = "";
        private readonly RepositoryFactory rf;
        public Train Train { get; set; }

        private bool AddWagonMode = false;


        public TrainEditWindow(RepositoryFactory rf, long selectedId)
        {
            this.rf = rf;
            this.Train = rf.TrainRepository.GetById(selectedId);
            DataContext = this;
            InitializeComponent();
            PopulateWagonsGrid();
        }

        private void PopulateWagonsGrid()
        {
            wagonsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            for (int i = 0; i < Train.Wagons.Count; i++)
            {
                wagonsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
            for (int i = 0; i < Train.Wagons.Count; i++)
            {
                AddWagonButton(Train, i);
            }
            AddAddWagon();
        }

        private void AddAddWagon()
        {
            wagonsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Button addWagon = new Button()
            {
                Content = " + Add wagon",
                Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#7edc73"),
                Foreground = Brushes.Black,
                Margin = new Thickness(0, 10, 20, 10),
                MinHeight = 40,
                ToolTip = "Click here to add a wagon"
            };
            Grid.SetColumn(addWagon, 0);
            Grid.SetRow(addWagon, Train.Wagons.Count);
            wagonsGrid.Children.Add(addWagon);
            addWagon.Click += new RoutedEventHandler(addWagon_Click);
        }

        private void addWagon_Click(object sender, RoutedEventArgs e)
        {
            // ovdje treba da se obrise sve sto je bilo ranije iscrtano
            // i na canvasu i u formi
            // i onda treba dati novo znacenje dugmetu za dodavanje
            // ako je mod za dodavanje, usmjeriti ga da radi druge stvari
            AddWagonMode = true;
            form.Visibility = Visibility.Visible;
            Console.WriteLine("Treba da implementiram dodavanje wagona sada");
            clearCanvas();
        }

        private void clearCanvas()
        {
            seatsGrid.ColumnDefinitions.Clear();
            seatsGrid.RowDefinitions.Clear();
            seatsGrid.Children.Clear();

            numerationGrid.ColumnDefinitions.Clear();
            numerationGrid.RowDefinitions.Clear();
            numerationGrid.Children.RemoveRange(1, numerationGrid.Children.Count - 1);

            numOfRows.Text = "";
            numOfSeats.Text = "";
            trainBorder.Visibility = Visibility.Hidden;
        }

        private void AddWagonButton(Train train, int i)
        {
            Button wagonBtn = new Button()
            {
                Background = (SolidColorBrush)new BrushConverter().ConvertFrom(GetWagonButtonColor(train.Wagons[i])),
                Foreground = Brushes.Black,
                Margin = new Thickness(0, 10, 20, 10),
                MinHeight = 40,
                ToolTip = "Click to select the wagon"
            };
            PackIcon icon = CreateStarIcon(train.Wagons[i]);
            TextBlock content = new TextBlock() { Text = $"Wagon {i + 1}" };
            Grid grid = CreateWagonButtonGrid(icon, content);
            // sadrzaj vagona je ikonica + text
            wagonBtn.Content = grid;
            wagonBtn.Click += new RoutedEventHandler(wagonBtn_Click);
            Grid.SetColumn(wagonBtn, 0);
            Grid.SetRow(wagonBtn, i);
            wagonsGrid.Children.Add(wagonBtn);
        }

        private PackIcon CreateStarIcon(Wagon wagon)
        {
            PackIcon icon = new PackIcon()
            {
                Kind = PackIconKind.Star,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(-3, 0, 4, 0)
            };
            if (wagon.Class == WagonClass.Second) icon.Visibility = Visibility.Hidden;
            return icon;
        }

        private Grid CreateWagonButtonGrid(PackIcon icon, TextBlock content)
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            Grid.SetColumn(icon, 0);
            grid.Children.Add(icon);
            Grid.SetColumn(content, 1);
            grid.Children.Add(content);
            return grid;
        }


        private void wagonBtn_Click(object sender, RoutedEventArgs e)
        {
            AddWagonMode = false;
            form.Visibility = Visibility.Visible;
            trainBorder.Visibility = Visibility.Visible;

            Button btn = sender as Button;
            //if (btn == selectedWagon) return;
            Wagon wagon = Train.Wagons[Grid.GetRow(btn)];
            MarkWagonSelected(wagon, btn);
            UnselectPreviousWagon();

            selectedWagon = btn;
            trainBorder.BorderThickness = new Thickness(2, 2, 2, 2);
            NumberOfRows = wagon.Rows;
            NumberOfColumns = wagon.SeatsPerRow;

            setDefaultFormParameters();
            UpdateSeatsGrid();
            UpdateNumerationsGrid();
            AddSeatButtons(wagon);
            AddNumerations();


            Grid.SetColumnSpan(trainBorder, NumberOfRows);
            Grid.SetRowSpan(trainBorder, NumberOfColumns);
        }

        private void MarkWagonSelected(Wagon wagon, Button btn)
        {
            //lblWagonName.Content = $"Wagon {wagon.Ordinal + 1} - {wagon.Class} class";
            //lblWagonName.Visibility = Visibility.Visible;
            //lblSeatChoice.Visibility = Visibility.Visible;
            selectedWagonClass = wagon.Class.ToString();
            if (wagon.Class == WagonClass.First)
            {
                btn.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(FIRST_CLASS);
            }
            else
            {
                btn.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(SECOND_CLASS);
            }
        }

        private void UnselectPreviousWagon()
        {
            if (selectedWagon != null)
            {
                Wagon prev = Train.Wagons[Grid.GetRow(selectedWagon)];
                if (prev.Class == WagonClass.First)
                {
                    selectedWagon.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(FIRST_CLASS);
                }
                else
                {
                    selectedWagon.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(SECOND_CLASS);
                }
            }
        }

        private void UpdateSeatsGrid()
        {
            seatsGrid.ColumnDefinitions.Clear();
            seatsGrid.RowDefinitions.Clear();
            seatsGrid.Children.Clear();
            for (int i = 0; i < NumberOfRows; i++)
            {
                var colDef = new ColumnDefinition
                {
                    Width = GridLength.Auto
                };
                seatsGrid.ColumnDefinitions.Add(colDef);
            }
            for (int i = 0; i < NumberOfColumns; i++)
            {
                var rowDef = new RowDefinition
                {
                    Height = GridLength.Auto
                };
                seatsGrid.RowDefinitions.Add(rowDef);
            }
        }

        private void UpdateNumerationsGrid()
        {
            numerationGrid.ColumnDefinitions.Clear();
            numerationGrid.RowDefinitions.Clear();
            numerationGrid.Children.RemoveRange(1, numerationGrid.Children.Count - 1);
            for (int i = 0; i <= NumberOfRows; i++)
            {
                var colDef = new ColumnDefinition
                {
                    Width = GridLength.Auto
                };
                numerationGrid.ColumnDefinitions.Add(colDef);
            }
            for (int i = 0; i <= NumberOfColumns; i++)
            {
                var rowDef = new RowDefinition
                {
                    Height = GridLength.Auto
                };
                numerationGrid.RowDefinitions.Add(rowDef);
            }
        }

        private void AddSeatButtons(Wagon wagon)
        {
            foreach (Seat seat in wagon.Seats)
            {
                Label seatLbl = new Label()
                {
                    Content = $" {seat.Row + 1}{Convert.ToChar(65 + seat.Column)}",
                    //Background = (SolidColorBrush)new BrushConverter().ConvertFrom(FIRST_CLASS),
                    Background = findSeatColor(wagon.Class),
                    Foreground = Brushes.Black,
                    Margin = GetMargin(seat.Column),
                    //IsEnabled = IsSeatFree(seat),
                    FontSize = 22,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Name = $"btn_{seat.Id}",
                    ToolTip = "Click to select the seat"
                };
                //seatBtn.Click += new RoutedEventHandler(seatBtn_Click);
                Grid.SetColumn(seatLbl, seat.Row);
                Grid.SetRow(seatLbl, seat.Column);
                seatsGrid.Children.Add(seatLbl);
            }
        }

        private Brush findSeatColor(WagonClass c)
        {
            String s;
            if (c == WagonClass.First) s = FIRST_CLASS;
            else s = SECOND_CLASS;
            return (SolidColorBrush)new BrushConverter().ConvertFrom(s);
        }

        private void AddNumerations()
        {
            AddVerticalNumerations();
            AddHorizontalNumerations();
        }

        private void AddVerticalNumerations()
        {
            for (int i = 0; i < NumberOfColumns; i++)
            {
                Label lbl = new Label()
                {
                    Content = Convert.ToChar(65 + i).ToString(),
                    FontSize = 20,
                    Margin = GetLabelMargin(i),
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Padding = new Thickness(0, 0, 0, 0)
                };
                Grid.SetRow(lbl, i + 1);
                Grid.SetColumn(lbl, 0);
                numerationGrid.Children.Add(lbl);
            }
        }

        private void AddHorizontalNumerations()
        {
            for (int i = 0; i < NumberOfRows; i++)
            {
                Label lbl = new Label()
                {
                    Content = (i + 1).ToString(),
                    FontSize = 20,
                    Margin = new Thickness(0, 0, 0, 7),
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Padding = new Thickness(0, 0, 0, 0)
                };
                Grid.SetColumn(lbl, i + 1);
                Grid.SetRow(lbl, 0);
                numerationGrid.Children.Add(lbl);
            }
        }

        private Thickness GetMargin(int j)
        {
            int top = 5;
            int bottom = 5;
            int left = 5;
            int right = 5;
            if (j == NumberOfColumns / 2 - 1) bottom = 20;
            if (j == NumberOfColumns / 2) top = 20;
            return new Thickness(left, top, right, bottom);
        }

        private Thickness GetLabelMargin(int j)
        {
            int top = 2;
            int bottom = 2;
            int left = 5;
            int right = 7;
            if (j == 0) top = 5;
            if (j == NumberOfColumns / 2 - 1) bottom = 35;
            if (j == NumberOfColumns / 2) top = 0;
            if (j == NumberOfColumns - 1) bottom = 17;
            return new Thickness(left, top, right, bottom);
        }


        private string GetWagonButtonColor(Wagon wagon)
        {
            if (wagon.Class == WagonClass.First) return FIRST_CLASS;
            return SECOND_CLASS;
        }

        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (formIsValid())
            {
                if (AddWagonMode == false)
                {
                    trainBorder.Visibility = Visibility.Visible;
                    WagonEdit_Click();
                }
                else AddWagon();
            }
            else
            {
                MessageBox.Show("You did not enter all wagon information!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private bool formIsValid()
        {
            //Console.WriteLine("Odabrana stavka je " + comboWagonClass.SelectedIndex);
            if (numOfRows.Text != "" && numOfSeats.Text != "" && comboWagonClass.SelectedIndex != -1)
                return true;
            return false;
        }

        private void AddWagon()
        {
            int r = Int32.Parse(numOfRows.Text);
            int c = Int32.Parse(numOfSeats.Text);
            long newId = rf.TrainRepository.GetNextId();
            Wagon newWagon = new Wagon()
            {
                Id = newId,
                Ordinal = Train.Wagons.Count,
                Class = getComboClass(),
                Rows = r,
                SeatsPerRow = c,
                //Seats = rf.SeatRepository.GetAll().Where(x => x.Id <= r * c).ToList()
                Seats = findSeats(r, c, newId)
            };
            // sad treba ovaj movi wagon da ubacim u voz i osvjezim promjene
            Train.Wagons.Add(newWagon);
            rf.TrainRepository.SaveAll();
            //selectedWagon.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            PopulateWagonsGrid();
            // restartovati formu
            // obrisati border
            resetForm();
            //trainBorder.Visibility = Visibility.Hidden;
        }

        private void resetForm()
        {
            numOfRows.Text = "";
            numOfSeats.Text = "";
            comboWagonClass.SelectedIndex = 0;
        }

        private void WagonEdit_Click()
        {
            //AddWagonMode = false;
            Wagon prev = Train.Wagons[Grid.GetRow(selectedWagon)];

            int r = Int32.Parse(numOfRows.Text);
            int c = Int32.Parse(numOfSeats.Text);
            Wagon newWagon = new Wagon()
            {
                Id = prev.Id,
                Ordinal = prev.Ordinal,
                Class = getComboClass(),
                Rows = r,
                SeatsPerRow = c,
                //Seats = rf.SeatRepository.GetAll().Where(x => x.Id <= r * c).ToList()
                Seats = findSeats(r, c, prev.Id)
            };
            // sad treba ovaj movi wagon da ubacim u voz i osvjezim promjene
            Train.Wagons[Grid.GetRow(selectedWagon)] = newWagon;
            rf.TrainRepository.SaveAll();
            rf.TrainRepository.SaveAll();
            selectedWagon.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        private List<Seat> findSeats(int r, int c, long wId)
        {
            List<Seat> retVal = new List<Seat>();
            // ovdje treba da iznova napravim sjedista
            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    Seat s = new Seat();
                    s.Row = i;
                    s.Column = j;
                    s.WagonId = wId;
                    s.Id = (r + 1) * (c + 1);
                    retVal.Add(s);
                }
            }
            return retVal;

        }

        private WagonClass getComboClass()
        {
            int index = comboWagonClass.SelectedIndex;
            if (index == 0) return WagonClass.First;
            return WagonClass.Second;
        }

        private void setDefaultFormParameters()
        {
            numOfRows.Text = NumberOfRows.ToString();
            numOfSeats.Text = NumberOfColumns.ToString();
            int selectedComboIndex = 0;
            if (selectedWagonClass == "Second") selectedComboIndex = 1;
            comboWagonClass.SelectedIndex = selectedComboIndex;
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Your changes are saved!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
    }
}
