using ChatClient.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatClient.ViewModel
{
    public class MainViewModel
    {
        public ObservableCollection<Message> Messages { get; set; }

        public MainViewModel()
        {
            Messages = new ObservableCollection<Message>();
            Messages.Add(new Message { Ouner = "Kos", Date = DateTime.Now, Text = "Hello! My name is Kos  jhdksajhlkashdlsahdlksahdjlaksjhdlakshjdalkjshdlkhsajdlaksjhdlakshdlksajhdashjdakshdhdhdhdhdhdddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" });
            Messages.Add(new Message { Ouner = "Lex", Date = DateTime.Now, Text = "Hello! My name is Lex" });
        }
    }
}
