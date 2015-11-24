using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketBot.Services
{
    public class Log
    {
        public static void Simple(string msg)
        {
            Console.WriteLine("[{0}]: {1}", DateTime.UtcNow, msg);
        }
        public static void Addressed(string msg, string sender)
        {
            Console.WriteLine("[{0} | {2}]: {1}", DateTime.UtcNow, msg, sender);
        }
    }
}
