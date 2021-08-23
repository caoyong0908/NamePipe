using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kv.NamePipe
{
    public class NamePClient : IDisposable
    {
        private NamedPipeClientStream _client;

        private StreamWriter _writer;
        private StreamReader _reader;

        private Task _task;
        private bool _readFlag = false;
        private AutoResetEvent _readEvent;

        public delegate void DelegateMessage(string content);
        public event DelegateMessage EventReceivedMessage;

        public delegate void DelegateConnectResult(bool result, string msg = "");
        public event DelegateConnectResult EventConnect;

        public string NamePipeName { get; }

        public bool SendOnly { get; set; }

        public NamePClient(string pipeName)
        {
            _readEvent = new AutoResetEvent(false);
            NamePipeName = pipeName;
            _client = new NamedPipeClientStream("localhost", NamePipeName, PipeDirection.InOut, PipeOptions.None,
                TokenImpersonationLevel.None);
        }

        /// <summary>
        /// 连接管道
        /// </summary>
        /// <param name="timeout">超时时间</param>
        public void Connect(int timeout = 3000)
        {
            try
            {
                _client.Connect(timeout);
            }
            catch (Exception e)
            {
                EventConnect?.Invoke(false, "连接超时！");
                return;
            }
            _writer = new StreamWriter(_client);
            _reader = new StreamReader(_client);
            _task = Task.Factory.StartNew(Read);
        }


        private void Read()
        {
            if (SendOnly)
            {
                return;
            }
            while (true)
            {
                if (!_readFlag)
                {
                    _readEvent.WaitOne();
                }
                var input = _reader.ReadLine();
                if (_reader.BaseStream is NamedPipeServerStream namedPipeServerStream)
                {
                    if (!namedPipeServerStream.IsConnected)
                    {
                        Console.WriteLine("Server DisConnected!");
                        break;
                    }
                }
                if (string.IsNullOrEmpty(input))
                {
                    continue;
                }

                _readFlag = false;
                EventReceivedMessage?.Invoke(input);
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="msg"></param>
        public void Send(string msg)
        {
            if (_readFlag)
            {
                return;
            }

            _writer.WriteLine(msg);
            _writer.Flush();
            if (SendOnly)
            {
                return;
            }
            _readFlag = true;
            _readEvent.Set();
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void DisConnect()
        {
            Dispose();
        }
        public void Dispose()
        {
            _client.Close();
            _client.Dispose();
        }
    }
}
