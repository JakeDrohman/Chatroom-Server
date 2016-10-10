using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ChatroomServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ChatLogger chatlogger = new ChatLogger();
            Controller controller = new Controller(chatlogger);
            controller.RunServer();
        }
    }
}
