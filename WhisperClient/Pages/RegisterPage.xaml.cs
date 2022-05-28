using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Whisper.Client.Models;

namespace Whisper.Client.Pages
{
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        private MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void registerButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text) ||
               string.IsNullOrWhiteSpace(txtUsername.Text) ||
               string.IsNullOrWhiteSpace(txtPassword.Password) ||
               string.IsNullOrWhiteSpace(txtPasswordConfirm.Password))
            {
                mainWindow.ShowSnackbar("All fields must be filled out.");
                return;
            }

            if (txtPassword.Password != txtPasswordConfirm.Password)
            {
                mainWindow.ShowSnackbar("Passwords do not match.");
                return;
            }

            this.IsEnabled = false;

            progressBar.Visibility = Visibility.Visible;

            var registerStatus = await mainWindow.apiHelper.Register(txtEmail.Text, txtUsername.Text, txtPassword.Password, mainWindow.apiHelper.dh.PublicKey);

            switch (registerStatus)
            {
                case StatusCode.ConnectionRefused:
                    mainWindow.ShowSnackbar("Failed to connect.");
                    break;
                case StatusCode.Forbidden:
                    mainWindow.ShowSnackbar("User already registered.");
                    break;
                case StatusCode.BadRequest:
                    mainWindow.ShowSnackbar("Email/Username/Password cannot be empty.");
                    break;
                case StatusCode.OK:
                    mainWindow.ShowSnackbar("Registered.");
                    this.NavigationService.Navigate(new Uri("Pages/MainPage.xaml", UriKind.Relative));
                    break;
                default:
                    mainWindow.ShowSnackbar("Error while registering.");
                    break;
            }

            progressBar.Visibility = Visibility.Collapsed;

            this.IsEnabled = true;
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("Pages/LoginPage.xaml", UriKind.Relative));
        }
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && this.IsEnabled) registerButton_Click(sender, e);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) => txtUsername.Focus();
    }
}
