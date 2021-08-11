using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kv.NamePipe;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            NamePServer server = new NamePServer("test");
            server.EventReceivedMessage += content => { Console.WriteLine(content); };
            server.Start();
            var send = Console.ReadLine();
            server.Send(send);
            Console.ReadKey();
        }
    }
}
