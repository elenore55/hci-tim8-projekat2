using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;

namespace HCI_Project.view
{
    /// <summary>
    /// Interaction logic for DemoPlayer.xaml
    /// </summary>
    public partial class DemoPlayer : Window
    {
        public string LabelText { get; set; }
        public bool VideoExists { get; set; }

        public DemoPlayer(String source)
        {
            InitializeComponent();
            DataContext = this;
            if (File.Exists(source))
            {
                LabelText = "Press Play to start video!";
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += timer_Tick;
                timer.Start();
                Player.Source = new Uri(source, UriKind.Relative);
                Player.LoadedBehavior = MediaState.Manual;
            } 
            else
            {
                LabelText = "No demo for this part";
                btnPlay.Visibility = Visibility.Hidden;
                btnPause.Visibility = Visibility.Hidden;
                btnStop.Visibility = Visibility.Hidden;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (Player.Source != null)
            {
                if (Player.NaturalDuration.HasTimeSpan)
                    lblStatus.Content = String.Format("{0} / {1}", Player.Position.ToString(@"mm\:ss"), Player.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            }
            else
                lblStatus.Content = "No file selected...";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Player.Play();
            info.Visibility = Visibility.Hidden;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Player.Pause();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Player.Stop();
        }
    }
}
