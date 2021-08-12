using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Kv.NamePipe;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            NamePClient client = new NamePClient("test");
            client.Connect();
            var send = Console.ReadLine();
            client.Send(send);
            Console.ReadKey();

            //try
            //{
            //    using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(
            //        "localhost", "test",
            //        PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.None)
            //    )
            //    {
            //        pipeClient.Connect(2000);//连接服务端
            //        StreamWriter sw = new StreamWriter(pipeClient);
            //        StreamReader sr = new StreamReader(pipeClient);


            //        while (true)
            //        {
            //            var msg = Console.ReadLine();
            //            sw.WriteLine(msg);//传递消息到服务端
            //            sw.Flush();//注意一定要有，同服务端一样
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex);
            //}

            Console.ReadKey();

        }
    }
}
