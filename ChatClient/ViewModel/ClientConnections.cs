using ChatClient.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatClient.ViewModel
{
    class ClientConnections
    {
        string host = "127.0.0.1";
        int port = 8080;
        string name;
        TcpClient? client;
        StreamReader? Reader = null;
        StreamWriter? Writer = null;
        ObservableCollection<Message> Messages;

        public ClientConnections(string userName, ObservableCollection<Message> messages)
        {
            name = userName;
            Messages = messages;

            client = new TcpClient();
            client.Connect(host, port);
            Reader = new StreamReader(client.GetStream());
            Writer = new StreamWriter(client.GetStream());
            if (Writer is null || Reader is null) return;

            Task.Run(() => ReceiveMessageAsync(Messages));
        }

        public async Task SendMessageAsync(string message)
        {
            Message messageObj = new Message { Date = DateTime.Now.ToLongTimeString(), Ouner = name, Text = message };
            string jsonMessage = JsonConvert.SerializeObject(messageObj);
            await Writer.WriteLineAsync(jsonMessage);
            await Writer.FlushAsync();
        }

        async Task ReceiveMessageAsync(ObservableCollection<Message> messages)
        {
            while (true)
            {
                try
                {
                    string? message = await Reader.ReadLineAsync();

                    if (message == null) continue;

                    Message msg = JsonConvert.DeserializeObject<Message>(message);

                    if (msg == null) continue;

                    Messages.Add(msg);
                }
                catch
                {
                    MessageBox.Show("Ошибка приема сообщения от сервера", "ClientConnections");
                }
            }
        }
    }
}
