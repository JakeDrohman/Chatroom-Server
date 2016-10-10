using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace ChatroomServer
{
    class Controller
    {
        
        private IPAddress localAddress;
        private TcpListener listener;
        private Dictionary<string,TcpClient> clientList;
        private Queue<string> messages;
        private int portNumber;
        private ILogger logger;
        private string chatLogLocation;
        public Controller(ILogger logger)
        {
            portNumber = 52262;
            localAddress = IPAddress.Parse("192.168.1.77");
            listener = new TcpListener(localAddress,portNumber);
            clientList = new Dictionary<string, TcpClient>();
            this.logger = logger;
            chatLogLocation = "ChatLog.txt";
        }
        private void StartListening()
        {
            listener.Start();
        }
        private void GetUser()
        {
            while (true)
            {
                TcpClient user = listener.AcceptTcpClient();
                NetworkStream networkStream = user.GetStream();
                SendNamePrompt(networkStream);
                Task.Run(() =>SetUsername(user));                
            }
        }
        private void SetUsername(TcpClient user)
        {
            string name = (ReceiveMessage(user,null));
            if (name != null)
            {
                clientList.Add(name, user);
                BroadcastMessage((name + " has joined the room"), name);
                Task.Run(() => RouteMessage(user, name));
            }
        }
        private void RemoveClient(string name)
        {
            clientList.Remove(name);
        }
        private string ReceiveMessage(TcpClient client,string name)
        {
            try
            {
                NetworkStream networkStream = client.GetStream();
                byte[] messageBuffer = new byte[client.ReceiveBufferSize];
                int bytesRead = networkStream.Read(messageBuffer, 0, client.ReceiveBufferSize);
                string messageReceived = Encoding.ASCII.GetString(messageBuffer, 0, bytesRead);
                return messageReceived;
            }
            catch
            {
                string exitMessage = "has left the room";
                RemoveClient(name);
                return exitMessage;
            }
        }
        private void SendNamePrompt(NetworkStream networkStream)
        {
            string namePrompt = "Please enter your name";
            byte [] promptBuffer = Encoding.ASCII.GetBytes(namePrompt);
            int bytesInBuffer = Encoding.ASCII.GetByteCount(namePrompt);
            networkStream.Write(promptBuffer, 0, bytesInBuffer);
        }
        private void BroadcastMessage(string packagedMessage,string name)
        {
            
            Console.WriteLine(packagedMessage);
            byte[] messageBuffer = Encoding.ASCII.GetBytes(packagedMessage);
            int bytesInBuffer = Encoding.ASCII.GetByteCount(packagedMessage);
            foreach (KeyValuePair<string,TcpClient> client in clientList)
            {
                if (name.Equals(client.Key)) { }
                else
                {
                    NetworkStream networkstream = client.Value.GetStream();
                    networkstream.Write(messageBuffer, 0, bytesInBuffer);
                }
            }
        }
        private void RouteMessage(TcpClient client,string name)
        {
            while (clientList.ContainsValue(client))
            {
                string message = ReceiveMessage(client,name);
                string packagedMessage = (name + ": " + message);
                messages.Enqueue(packagedMessage);
                Task.Run(() =>logger.Log(packagedMessage, chatLogLocation));
                BroadcastMessage(packagedMessage,name);
                messages.Dequeue();
            }
        }
        public void RunServer()
        {
            StartListening();
            Thread AddUsers = new Thread(new ThreadStart(GetUser));
            AddUsers.Start();
        }        
    }
}
