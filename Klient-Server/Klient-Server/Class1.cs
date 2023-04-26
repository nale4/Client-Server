using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace Klient_Server
{
    class Server
    {
        const int port = 8888;
        static TcpListener listener;
        public static void connection() 
        {
            try
            {
                listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                listener.Start();
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Thread clientThread = new Thread(() => Process(client));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (listener != null)
                    listener.Stop();
            }
        }
        public static void Process(TcpClient tcpClient)
        {
            TcpClient client = tcpClient;
            NetworkStream stream = null;
            try
            {
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
                    Console.WriteLine(message);
                    data = Encoding.Unicode.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }

    }
}
