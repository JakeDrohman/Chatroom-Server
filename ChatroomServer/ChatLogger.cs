using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ChatroomServer
{
    class ChatLogger : ILogger
    {
        public void Log(string message, string location)
        {
            File.AppendAllText(location, (Environment.NewLine + DateTime.Now + "  " + message));
        }
    }
}
