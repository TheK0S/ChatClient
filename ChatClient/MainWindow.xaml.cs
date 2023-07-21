using ChatClient.Model;
using ChatClient.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string? UserName; 
        bool isConnected;
        Connections connection;

        public MainWindow()
        {
            InitializeComponent();

            connection = new Connections();

            DataContext = connection;

            isConnected = false;
        }

        private void Messeges_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            DisconnectUser();
            this.Close();
        }

        private void minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void DockPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnConnectDisconnect_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected) ConnectUser();
            else DisconnectUser();
        }

        private void ConnectUser()
        {
            if (isConnected) return;
            else if (string.IsNullOrWhiteSpace(name.Text))
            {
                MessageBox.Show("Введите имя пользователя");
                return;
            }

            UserName = name.Text;
            connection.Connect();            
            StartReciveMessage();
            connection.SendName(UserName?? string.Empty);
            isConnected = true;

            ButtonVisualChanged();
        }

        private void DisconnectUser()
        {
            if (!isConnected) return;

            connection.Disconnect();
            isConnected = false;

            ButtonVisualChanged();
        }

        private void StartReciveMessage()
        {
            Task.Run(async () => { await connection.ReceiveMessagesAsync(); });
        }


        private async void tbMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (connection is null || string.IsNullOrWhiteSpace(tbMessage.Text)) return;

            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await connection.SendMessage(tbMessage.Text, name.Text);
            });

            tbMessage.Text = string.Empty;
        }

        private void ButtonVisualChanged()
        {
            name.IsEnabled = isConnected? false : true;
            btnConnectDisconnect.Content = isConnected ? "To Disconnect" : "To Connect";
            btnConnectDisconnect.Background = isConnected?  new SolidColorBrush(Colors.Lime) : new SolidColorBrush(Colors.Red);          
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange == 0) return;

            if (e.VerticalOffset + e.ViewportHeight + e.ViewportHeight / 5d < e.ExtentHeight) return;

            int listCount = list.Items.Count;

            if(listCount > 1)
                list.ScrollIntoView(list.Items[listCount - 1]);
        }
    }
}
