using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kv.NamePipe
{
    public class NamePServer : IDisposable
    {
        private bool _sendFlag = false;
        private AutoResetEvent _sendEvent;
        private Task _task;
        private NamedPipeServerStream _server;

        private bool _clientConnected = false;

        private StreamWriter _writer;
        private StreamReader _reader;

        public int MaxServerCount { get; }

        public string PipeName { get; }

        public bool ReadOnly { get; set; }

        public delegate void DelegateMessage(string content);
        public event DelegateMessage EventReceivedMessage;

        public delegate void DelegateClientConnection();

        public event DelegateClientConnection EventClientDisconnect;
        public event DelegateClientConnection EventClientConnected;

        public NamePServer(string name, int maxServerCount = 100)
        {
            _sendEvent = new AutoResetEvent(false);
            MaxServerCount = maxServerCount;
            PipeName = name;
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start()
        {
            _task = Task.Factory.StartNew(SeverRun);
        }

        private void SeverRun()
        {
            while (true)
            {
                _server = new NamedPipeServerStream(PipeName, PipeDirection.InOut, MaxServerCount);
                _server.WaitForConnection();
                EventClientConnected?.Invoke();
                _clientConnected = true;
                _writer = new StreamWriter(_server);
                _reader = new StreamReader(_server);

                while (true)
                {
                    if (!_clientConnected)
                    {
                        break;
                    }
                    if (_sendFlag && !ReadOnly)
                    {
                        _sendEvent.WaitOne();
                    }
                    var input = _reader.ReadLine();
                    if (_reader.BaseStream is NamedPipeServerStream namedPipeServerStream)
                    {
                        if (!namedPipeServerStream.IsConnected)
                        {
                            _clientConnected = false;
                            EventClientDisconnect?.Invoke();
                            break;
                        }
                    }
                    if (string.IsNullOrEmpty(input))
                    {
                        continue;
                    }

                    _sendFlag = true;
                    EventReceivedMessage?.Invoke(input);
                }
            }
        }

        public void Send(string msg)
        {
            if (ReadOnly)
            {
                return;
            }
            if (!_sendFlag)
            {
                return;
            }
            _writer.WriteLine(msg);
            try
            {
                _writer.Flush();
            }
            catch (Exception e)
            {
                _clientConnected = false;
                EventClientDisconnect?.Invoke();
            }
            finally
            {
                _sendFlag = false;
                _sendEvent.Set();
            }

        }

        public void Dispose()
        {
            _server.Close();
            _server.Dispose();
        }
    }
}
