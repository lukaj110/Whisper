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
using Whisper.Client.Models;

namespace Whisper.Client.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
        public LoginPage()
        {
            InitializeComponent();
        }

        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("Pages/RegisterPage.xaml", UriKind.Relative));
        }

        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;

            progressBar.Visibility = Visibility.Visible;

            var loginStatus = await mainWindow.apiHelper.Login(txtUsername.Text, txtPassword.Password);

            switch (loginStatus)
            {
                case StatusCode.ConnectionRefused:
                    mainWindow.ShowSnackbar("Failed to connect.");
                    break;
                case StatusCode.NotFound:
                    mainWindow.ShowSnackbar("User not found.");
                    break;
                case StatusCode.Unauthorized:
                    mainWindow.ShowSnackbar("Invalid password.");
                    break;
                case StatusCode.OK:
                    mainWindow.ShowSnackbar("Logged in.");
                    this.NavigationService.Navigate(new Uri("Pages/MainPage.xaml", UriKind.Relative));
                    break;
                default:
                    mainWindow.ShowSnackbar("Error while logging in.");
                    break;
            }

            progressBar.Visibility = Visibility.Collapsed;

            this.IsEnabled = true;
        }
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter && this.IsEnabled) loginButton_Click(sender, e);
        }
    }
}
