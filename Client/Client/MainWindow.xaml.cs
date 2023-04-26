using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Interop;


namespace Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Thread listenThread;
        int port = 8888;
        string address = "127.0.0.1";
        TcpClient client = null;
        NetworkStream stream = null;
        string username;
        

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            username = name.Text;
            try 
            {
                client = new TcpClient(address, port);
                stream = client.GetStream();
                listenThread = new Thread(() => listen());
                listenThread.Start();
            }
            catch (Exception ex)
            {
                
                massege.Text = (ex.Message);
            }

            

        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            listenThread.Abort();
            log.Text += "Client Disconect\n";
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string message = massege.Text;
            message = String.Format("{0}: {1}", username, message);
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        void listen()
        {
            try 
            {
                while (true)
                {
                    byte[] data = new byte[64];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);
                    string message = builder.ToString();

                    Dispatcher.BeginInvoke(new Action(() => log.Text += ("Сервер: " + message + "\n")));
                }
            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke(new Action(() => log.Text = (ex.Message)));
            }
            finally
            {
                stream.Close();
                client.Close();
            }
        }

    }
}
