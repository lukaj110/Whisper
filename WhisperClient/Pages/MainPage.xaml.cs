using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
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
using Whisper.Client.Models;
using Whisper.Client.UserControls;
using Whisper.Crypto.Algorithms;

namespace Whisper.Client.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        private readonly ObservableCollection<Chat> chats = new();

        private ObservableCollection<Message> messages = new();

        private Chat me;

        public MainPage()
        {
            InitializeComponent();

            lbChats.ItemsSource = chats;

            messagesControl.ItemsSource = messages;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            mainWindow.CenterWindow();

            var chatList = await mainWindow.apiHelper.GetChats();

            foreach (var chat in chatList)
            {
                var msgs = await mainWindow.apiHelper.GetMessages(chat.ChannelId);

                var key = mainWindow.apiHelper.dh.DeriveKey(Convert.FromBase64String(chat.PubKey));

                AES256 aes = new(Encoding.UTF8.GetBytes(SHA512.Hash(Convert.ToBase64String(key))[..32]));

                foreach (var msg in msgs)
                {
                    msg.Content = aes.DecryptEcb(msg.Content);
                    chat.Messages.Add(msg);
                }

                chats.Add(chat);
            }

            me = await mainWindow.apiHelper.GetMyInfo();

            mainGrid.DataContext = me;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var view = new AddChatDialog();

            var result = (bool)await DialogHost.Show(view);

            if (!result) return;

            if (view.txtUsername == null || string.IsNullOrWhiteSpace(view.txtUsername.Text)) return;

            var user = await mainWindow.apiHelper.GetUserInfo(view.txtUsername.Text);

            if (user == null)
            {
                mainWindow.ShowSnackbar("Could not add user.");
                return;
            }

            chats.Add(user);
        }

        private void lbChats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbChats.SelectedItem != null)
            {
                var chat = (Chat)lbChats.SelectedItem;

                messages.Clear();

                chat.Messages.ForEach(m => messages.Add(m));
            }
        }

        private async void sendBtn_Click(object sender, RoutedEventArgs e)
        {
            if (lbChats.SelectedItem == null || string.IsNullOrWhiteSpace(txtMessage.Text)) return;
            
            var chat = (Chat)lbChats.SelectedItem;

            var key = mainWindow.apiHelper.dh.DeriveKey(Convert.FromBase64String(chat.PubKey));

            AES256 aes = new(Encoding.UTF8.GetBytes(SHA512.Hash(Convert.ToBase64String(key))[..32]));

            var msg = new Message
            {
                ChannelId = chat.ChannelId,
                Content = aes.EncryptEcb(txtMessage.Text),
                Checksum = SHA512.Hash(txtMessage.Text),
            };

            var message = await mainWindow.apiHelper.SendMessage(msg);

            message.Content = aes.DecryptEcb(message.Content);

            chat.Messages.Add(message);

            messages.Add(message);

            txtMessage.Clear();
        }
    }

    public class EqualityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                return false;

            return values[0].Equals(values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
