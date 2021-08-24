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

namespace Client.WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private NamePClient clent;
        public MainWindow()
        {
            InitializeComponent();

        }

        private void BtnConnect_OnClick(object sender, RoutedEventArgs e)
        {
            clent = new NamePClient("test");
            //clent.SendOnly = true;
            clent.Connect();
            clent.EventConnect += (result, msg) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    txtContent.Text += result + msg + Environment.NewLine;
                }));
            };
            clent.EventReceivedMessage += content =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    txtContent.Text += content + Environment.NewLine;
                }));
            };
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            clent.Send(txtMsg.Text);
        }
    }
}
