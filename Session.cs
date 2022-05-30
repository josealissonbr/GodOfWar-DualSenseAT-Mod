using Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GodOfWar
{
    internal class Session
    {
        public static UdpClient client;
        public static IPEndPoint endPoint;
        public static DateTime TimeSent;


        public static bool Server_Initialized = false;

        public static bool TouchRGBAnim = true;

        public static bool is_Paused = true;

        public static Mem meme = new Mem();
    }

    internal class GameData
    {
        public struct kratos 
        {
            public static int iHealth;
        }
    }
}
