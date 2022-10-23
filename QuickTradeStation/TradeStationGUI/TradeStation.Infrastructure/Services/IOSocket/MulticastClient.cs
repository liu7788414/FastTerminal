using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;
using System.Diagnostics;

namespace TradeStation.Infrastructure.Services.IOSocket
{
    public class UdpState
    {
        public Socket socket = null;
        public IPEndPoint ipEndPoint = null;
        public const int BufferSize = 80 * 1024;
        public byte[] buffer = new byte[BufferSize];
        public int counter = 0;
    }
    public class MulticastClient : INotifyPropertyChanged
    {
        public class MulticastReceiveEventArgs : EventArgs
        {
            public byte[] bytesRead {get; set; }

            public MulticastReceiveEventArgs(byte[] bytesRead)
            {
                this.bytesRead = bytesRead;
            }

        }

        private Socket theSocket;

        //private UdpState udpReceivState;
        private IPEndPoint listenEndPoint;

        public string IP { get; set; }
        public int Port { get; set; }

        public event EventHandler MessageReceiveHandler;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private UdpClient udpClient;

        public MulticastClient()
        {

        }

        public void JoinGroup(string host, int port)
        {
            try
            {
                if (udpClient != null)
                {
                    udpClient.Close();
                }

                udpClient = new UdpClient();
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, 80 * 1024);

                listenEndPoint = new IPEndPoint(IPAddress.Any, port);
                IPAddress multiGroupAddress = IPAddress.Parse(host);

                udpClient.Client.Bind(listenEndPoint);

                udpClient.EnableBroadcast = true;
                udpClient.Ttl = 50;

                udpClient.JoinMulticastGroup(multiGroupAddress);
                theSocket = udpClient.Client;

                StartReceive();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to join group " + host + ":" + port);
                Console.WriteLine(ex.Message);
            }

        }

        private void StartReceive()
        {
            UdpState state = new UdpState();
            udpClient.BeginReceive(ReceiveCallback, state);
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            //UdpState state = (UdpState)result.AsyncState;

            try
            {
                byte[] bytesRead = udpClient.EndReceive(result, ref listenEndPoint);

                //string response = Encoding.GetEncoding("GBK").GetString(bytesRead);

                MessageReceiveHandler(this, new MulticastReceiveEventArgs(bytesRead));

                StartReceive();
            }
            catch (Exception ex)
            {
                //当切换组播地址时会触发此Exception
                Console.WriteLine("Fail end receive");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
