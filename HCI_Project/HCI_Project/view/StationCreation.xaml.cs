using HCI_Project.model;
using Microsoft.Maps.MapControl.WPF;
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
    /// Interaction logic for StationCreation.xaml
    /// </summary>
    public partial class StationCreation : Page
    {
        private string BingMapsKey = "AinQ9hRJn7QhWLbnmUvC6OJ9RvqMuOWGDRkvSqOf5MUgrvbkmFHxHNg6aIjno0CM";
        private System.Windows.Point StartPoint;
        private bool stationPicked = false;
        private bool newPoint = true;
        public StationCreation()
        {
            InitializeComponent();
            MyMap.MouseDoubleClick += MyMap_MouseDoubleClick1;
        }

        private void MyMap_MouseDoubleClick1(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void Pushpin_MouseMove(object sender, MouseEventArgs e)
        {
            System.Windows.Point point = e.GetPosition(null);
            Vector diff = StartPoint - point;
            if (e.LeftButton == MouseButtonState.Pressed && (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                Pushpin p = e.Source as Pushpin;
                DataObject data = new DataObject(p);
                DragDrop.DoDragDrop(p, data, DragDropEffects.Move);
            }
        }
        private void MyMap_DragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
            System.Windows.Point pinLocaiton = e.GetPosition(MyMap);
            Location click = MyMap.ViewportPointToLocation(pinLocaiton);
            Pushpin newP = new Pushpin
            {
                Location = click
            };
            if (newPoint)
            {
                MyMap.Children.Add(newP);
                newPoint = false;
            }
            MyMap.Children.Remove(MyMap.Children[MyMap.Children.Count - 1]);
            MyMap.Children.Add(newP);
        }

        private void Pushpin_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartPoint = e.GetPosition(null);
        }

        private void MyMap_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("myFormat") || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void MyMap_Drop(object sender, DragEventArgs e)
        {
            e.Handled = true;
            System.Windows.Point pinPoint = e.GetPosition(MyMap);
            Location pinLocation = MyMap.ViewportPointToLocation(pinPoint);
            Pushpin pin = new Pushpin
            {
                Location = pinLocation,
                ToolTip = pinLocation.ToString()
            };
            pin.MouseDoubleClick += new MouseButtonEventHandler(Pin_Click);
            MyMap.Children.Clear();
            MyMap.Children.Add(pin);
            stationPicked = true;
            stationPin.Cursor = Cursors.No;
            stationPin.MouseMove -= new MouseEventHandler(Pushpin_MouseMove);
        }
        private void Pin_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Pushpin pin = (Pushpin)sender;
            MyMap.Children.Remove(pin);
            newPoint = true;
            stationPicked = false;
            stationPin.MouseMove += new MouseEventHandler(Pushpin_MouseMove);
            stationPin.Cursor = Cursors.Hand;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
