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
using System.Windows.Shapes;

namespace HCI_Project.view
{
    /// <summary>
    /// Interaction logic for SeatChoice.xaml
    /// </summary>
    /// 

    public partial class SeatChoice : Page
    {
        public int NumberOfRows { get; set; }
        public int NumberOfColumns { get; set; }

        public SeatChoice(Departure departure)
        {
            InitializeComponent();
            DataContext = this;
            Train train = departure.Train;
            Wagon wagon = train.Wagons[0];
            NumberOfRows = wagon.Rows;
            NumberOfColumns = wagon.SeatsPerRow;
            for (int i = 0; i < NumberOfRows; i++)
            {
                var rowDef = new RowDefinition
                {
                    Height = GridLength.Auto
                };
                seatsGrid.RowDefinitions.Add(rowDef);
            }
            for (int i = 0; i < NumberOfColumns; i++)
            {
                var colDef = new ColumnDefinition
                {
                    Width = GridLength.Auto
                };
                seatsGrid.ColumnDefinitions.Add(colDef);
            }
            int count = 0;
            for (int i = 0; i < NumberOfRows; i++)
            {
                for (int j = 0; j < NumberOfRows; j++)
                {
                    Button seatBtn = new Button() 
                    { 
                        Content = $"Seat {++count}" 
                    };
                    Grid.SetColumn(seatBtn, j);
                    Grid.SetRow(seatBtn, i);
                    seatsGrid.Children.Add(seatBtn);
                }
            }
        }
    }
}
