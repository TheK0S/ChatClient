using ChatClient.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace ChatClient.ViewModel
{
    class Connections
    {
        string serverIp = "127.0.0.1";
        int serverPort = 8080;
        EndPoint serverEndPoint;
        public ObservableCollection<Message> Messeges { get; set; }

        public Socket CurrentSocket { get; set; }

        public Connections()
        {            
            serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
            Messeges = new ObservableCollection<Message>();
        }

        public async Task Connect()
        {
            CurrentSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await CurrentSocket.ConnectAsync(serverIp, serverPort);
        }

        public void Disconnect()
        {
            CurrentSocket.Shutdown(SocketShutdown.Both);
            CurrentSocket.Close();
        }

        public void SendName(string ownerName)
        {
            Message messageObj = new Message { Date = DateTime.Now.ToLongTimeString(), Ouner = ownerName, Text = "подключился к чату" };
            string jsonMessage = JsonConvert.SerializeObject(messageObj);
            byte[] messageBytes = Encoding.UTF8.GetBytes(jsonMessage);

            CurrentSocket.SendTo(messageBytes, SocketFlags.None, serverEndPoint);
        }

        public async Task SendMessage(string message, string ownerName)
        {
            Message messageObj = new Message { Date = DateTime.Now.ToLongTimeString(), Ouner = ownerName, Text = message };
            string jsonMessage = JsonConvert.SerializeObject(messageObj);
            byte[] messageBytes = Encoding.UTF8.GetBytes(jsonMessage);
            
            await CurrentSocket.SendToAsync(messageBytes, SocketFlags.None, serverEndPoint);
        }

        public async Task ReceiveMessagesAsync()
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            try
            {
                while ((bytesRead = await CurrentSocket.ReceiveAsync(buffer, SocketFlags.None)) > 0)
                {
                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    Message message = JsonConvert.DeserializeObject<Message>(response);

                    if (message != null)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Messeges.Add(message);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении сообщений от сервера: {ex.Message}", "Client error");
            }
        }
    }
}
