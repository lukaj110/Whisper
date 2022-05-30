using MaterialDesignThemes.Wpf;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
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

        DispatcherTimer dispatcherTimer = new DispatcherTimer();


        public MainPage()
        {
            InitializeComponent();

            lbChats.ItemsSource = chats;

            messagesControl.ItemsSource = messages;

            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);

            dispatcherTimer.Tick += DispatcherTimer_Tick;
        }

        private async void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            var newChatList = (await mainWindow.apiHelper.GetChats()).Where(e => !chats.Any(c => c.UserId == e.UserId));

            foreach (var chat in newChatList)
            {
                var key = mainWindow.apiHelper.dh.DeriveKey(Convert.FromBase64String(chat.PubKey));

                chat.Aes = new(Encoding.UTF8.GetBytes(SHA512.Hash(Convert.ToBase64String(key))[..32]));

                chats.Add(chat);
            }

            var newMessages = (await mainWindow.apiHelper.GetAllMessages()).Where(e => !chats.SelectMany(c => c.Messages).Any(m => m.MessageId == e.MessageId));

            foreach (var message in newMessages)
            {
                Debug.WriteLine(JsonSerializer.Serialize(message));

                var user = message.ChannelId == me.ChannelId
                    ? chats.Single(e => e.UserId == message.Sender)
                    : chats.Single(e => e.ChannelId == message.ChannelId);

                message.Content = user.Aes.DecryptEcb(message.Content);

                user.Messages.Add(message);

                if (lbChats.SelectedItem != null && ((Chat)lbChats.SelectedItem).UserId == user.UserId)
                    messages.Add(message);
            }

            if (newMessages.Count() > 0)
            {
                SystemSounds.Beep.Play();

                chatScrollViewer.ScrollToEnd();
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            mainWindow.CenterWindow();

            var chatList = await mainWindow.apiHelper.GetChats();

            foreach (var chat in chatList)
            {
                var msgs = await mainWindow.apiHelper.GetMessages(chat.ChannelId);

                var key = mainWindow.apiHelper.dh.DeriveKey(Convert.FromBase64String(chat.PubKey));

                chat.Aes = new(Encoding.UTF8.GetBytes(SHA512.Hash(Convert.ToBase64String(key))[..32]));

                foreach (var msg in msgs)
                {
                    msg.Content = chat.Aes.DecryptEcb(msg.Content);
       
                    chat.Messages.Add(msg);
                }

                chats.Add(chat);
            }

            me = await mainWindow.apiHelper.GetMyInfo();

            mainGrid.DataContext = me;

            dispatcherTimer.Start();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();

            var view = new AddChatDialog();

            var result = (bool)await DialogHost.Show(view);

            if (!result)
            {
                dispatcherTimer.Start();

                return;
            }

            if (view.txtUsername == null || string.IsNullOrWhiteSpace(view.txtUsername.Text))
            {
                dispatcherTimer.Start();

                return;
            }

            var user = await mainWindow.apiHelper.GetUserInfo(view.txtUsername.Text);

            if (user == null)
            {
                mainWindow.ShowSnackbar("Could not add user.");

                dispatcherTimer.Start();

                return;
            }

            var key = mainWindow.apiHelper.dh.DeriveKey(Convert.FromBase64String(user.PubKey));

            user.Aes = new(Encoding.UTF8.GetBytes(SHA512.Hash(Convert.ToBase64String(key))[..32]));

            chats.Add(user);
        }

        private void lbChats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbChats.SelectedItem != null)
            {
                if (!txtMessage.IsEnabled) txtMessage.IsEnabled = true;

                var chat = (Chat)lbChats.SelectedItem;

                messages.Clear();

                chat.Messages.ForEach(m => messages.Add(m));

                chatScrollViewer.ScrollToEnd();
            }
        }

        private async void sendBtn_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();

            if (lbChats.SelectedItem == null || string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                dispatcherTimer.Start();

                return;
            };

            var chat = (Chat)lbChats.SelectedItem;

            var msg = new Message
            {
                ChannelId = chat.ChannelId,
                Content = chat.Aes.EncryptEcb(txtMessage.Text),
                Checksum = SHA512.Hash(txtMessage.Text),
            };

            var message = await mainWindow.apiHelper.SendMessage(msg);

            if (message != null)
            {
                message.Content = chat.Aes.DecryptEcb(message.Content);

                chat.Messages.Add(message);

                messages.Add(message);

                txtMessage.Clear();

                chatScrollViewer.ScrollToEnd();
            }

            dispatcherTimer.Start();
        }

        private void txtMessage_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers != ModifierKeys.Shift)
            {
                sendBtn_Click(sender, e);

                e.Handled = true;
            }
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
