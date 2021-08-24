using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Kv.NamePipe;

namespace Server.WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private NamePServer server;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            server = new NamePServer("test");
            //server.ReadOnly = true;
            server.Start();
            server.EventClientDisconnect += () =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    txtContent.Text += "客户端断开连接" + Environment.NewLine;
                }));
            };
            server.EventReceivedMessage += content =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    txtContent.Text += content + Environment.NewLine;
                }));
            };
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            server.Send(txtMsg.Text);
        }
    }
}
