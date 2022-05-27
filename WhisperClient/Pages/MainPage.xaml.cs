using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
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
using Whisper.Client.UserControls;

namespace Whisper.Client.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public MainPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            chatBox.Focus();
            mainWindow.CenterWindow();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var view = new AddChatDialog();

            var result = (bool)await DialogHost.Show(view);

            if (!result) return;

            if (view.txtUsername == null || string.IsNullOrWhiteSpace(view.txtUsername.Text)) return;

            var addChatStatus = await mainWindow.apiHelper.AddChat(view.txtUsername.Text);
        }
    }
}
