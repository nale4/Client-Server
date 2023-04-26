using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Klient_Server
{
    public struct Str
    {
        public string str { get; set; }
    }
    public partial class MainWindow : Window
    {
        public Thread listenThread;
        public Thread clientThread;

        object locker = new object();
        public static List<string> list;
        int port = 8888;
        static TcpListener listener;
        public static string mes;
        public MainWindow()
        {
            InitializeComponent();

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                listener.Start();
                log.Text += ("Server is active\n");

                listenThread = new Thread(() => listen());
                listenThread.Start();
            }
            catch
            {
                Stop();
            }
            

        }

        void listen()
        {
            while (true)
            {
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    lock (locker)
                    {
                        Dispatcher.BeginInvoke(new Action(() => log.Text += ("Новый клиент подключен.\n")));
                    }
                    clientThread = new Thread(() => Process(client));
                    clientThread.Start();
                }
                catch
                {
                    if(clientThread!= null)
                    {
                        clientThread.Abort();
                    }
                }
            }
        }

        public void Process(TcpClient tcpClient)
        {
            TcpClient client = tcpClient;
            NetworkStream stream = null;

            try
            {
                string logl;
                stream = client.GetStream();
                byte[] data = new byte[64];
                while (true)
                {
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);
                    string message = builder.ToString();
                    lock(locker) 
                    {
                        Dispatcher.Invoke(() => { log.Text += message + "\n"; });
                    }
                    message = parse(message);
                    data = Encoding.Unicode.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Stop();
                log.Text += (ex.ToString());
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }


        }

        public static string parse(string mes)
        {
            string[] parse;
            parse = mes.Split(':');
            string str = parse[1];
            str = ReverseStringBuilder(str);
            parse[1] = str;
            mes = parse[0] + ":" + parse[1];
            return mes;
        }

        static string ReverseStringBuilder(string str)
        {
            StringBuilder sb = new StringBuilder(str.Length);
            for (int i = str.Length; i-- != 0;)
                sb.Append(str[i]);
            return sb.ToString();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            Stop();
        }

        public void Stop()
        {
            //clientThread.Abort();
            //listenThread.Abort();
            listener.Stop();

            lock (locker)
            {
                log.Text += ("Server is stop\n");
            }
        }
    }
}
