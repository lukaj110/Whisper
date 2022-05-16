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

namespace WhisperClient
{
    /// <summary>
    /// Interaction logic for LandingWindow.xaml
    /// </summary>
    public partial class LandingWindow : Window
    {    
        public LandingWindow()
        {
            InitializeComponent();
        }

        private void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
