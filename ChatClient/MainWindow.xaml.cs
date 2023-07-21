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
        public static string UserName; 
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

        private async void btnConnectDisconnect_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected) ConnectUser();
            else DisconnectUser();
        }

        private void ConnectUser()
        {
            if (isConnected) return;
            else if (name.Text?.Length < 1)
            {
                MessageBox.Show("Введите имя");
                return;
            }

            UserName = name.Text;
            connection.Connect();            
            StartReciveMessage();
            connection.SendName(UserName);
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
            if(e.Key == Key.Enter)
            {
                if(connection != null)
                {
                    await Application.Current.Dispatcher.InvokeAsync(async () =>
                    {
                        await connection.SendMessage(tbMessage.Text, name.Text);
                    });

                    tbMessage.Text = string.Empty;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DisconnectUser();
        }

        private void Messages_CollectionChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalOffset + e.ViewportHeight >= e.ExtentHeight - 10) // Небольшая погрешность 10, чтобы учесть округления
            {
                int listCount = list.Items.Count;

                if(listCount > 1)
                    list.ScrollIntoView(list.Items[listCount - 1]);
            }
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

            if (e.VerticalOffset + e.ViewportHeight < e.ExtentHeight) return;

            int listCount = list.Items.Count;

            if(listCount > 1)
                list.ScrollIntoView(list.Items[listCount - 1]);
        }
    }
}
