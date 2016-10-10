using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatroomServer
{
    interface ILogger
    {
        void Log(string message, string location);
    }
}
