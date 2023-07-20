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
        Task reciveMessegeTask;
        Connections connection;
        ClientConnections clientConnections;
        MainViewModel viewModel;
        CancellationTokenSource cancellationTokenSource;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new MainViewModel();

            DataContext = viewModel;
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
            if (!isConnected) await ConnectUser();
            else DisconnectUser();
        }

        private async Task ConnectUser()
        {
            if (isConnected) return;
            else if (name.Text?.Length < 1) return;

            UserName = name.Text;

            connection = new Connections();
            await connection.Connect();

            if (connection != null)
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await connection.SendMessage("", $"{name.Text} подключился к чату");
                });

                tbMessage.Text = string.Empty;
            }

            reciveMessegeTask = StartReciveMessegeTask();

            name.IsEnabled = false;
            btnConnectDisconnect.Content = "To Disconnect";
            btnConnectDisconnect.Background = new SolidColorBrush(Colors.Lime);
            isConnected = true;
        }

        private void DisconnectUser()
        {
            if (!isConnected) return;

            StopReciveMessegeTask();

            connection.Disconnect();

            clientConnections = null;

            name.IsEnabled = true;
            btnConnectDisconnect.Content = "To Connect";
            btnConnectDisconnect.Background = new SolidColorBrush(Colors.Red);
            isConnected = false;
        }

        private Task StartReciveMessegeTask()
        {
            string ounerName = name.Text;
            cancellationTokenSource = new CancellationTokenSource();
            Task task = new Task(async () => await connection.ReceiveMessagesAsync(viewModel.Messages, cancellationTokenSource.Token));
            task.Start();
            return task;
        }

        private void StopReciveMessegeTask()
        {
            cancellationTokenSource?.Cancel();
            reciveMessegeTask?.Wait();
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

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalOffset + e.ViewportHeight >= e.ExtentHeight - 10) // Небольшая погрешность 10, чтобы учесть округления
            {
                int listCount = list.Items.Count;

                if(listCount > 2)
                    list.ScrollIntoView(list.Items[listCount - 1]);
            }
        }
    }
}
