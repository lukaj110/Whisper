using MaterialDesignThemes.Wpf;
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
using Whisper.Client.Helpers;

namespace Whisper.Client
{
    /// <summary>
    /// Interaction logic for LandingWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public Snackbar Snackbar = new();

        public APIHelper apiHelper;
        public MainWindow()
        {
            InitializeComponent();

            apiHelper = new APIHelper("https://localhost:5001/api");

            Snackbar = MainSnackbar;

            Snackbar.MessageQueue.DiscardDuplicates = true;
        }

        public void ShowSnackbar(string Text)
        {
            Snackbar.MessageQueue.Enqueue(Text, null, null, null, false, false, durationOverride: TimeSpan.FromSeconds(1));
        }

        public void CenterWindow()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height + 100;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }

        private void exitButton_Click(object sender, RoutedEventArgs e) => this.Close();

        private void ColorZone_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();
    }
}
