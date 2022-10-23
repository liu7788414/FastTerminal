using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;
using System.IO;
using System.Timers;
using System.Runtime.Serialization.Json;

using TradeStation.Infrastructure.Payloads;

namespace TradeStation.Infrastructure.Services.IOSocket
{
    public class StateObject
    {
        // Client socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 8 * 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        // public StringBuilder sb = new StringBuilder();
    }
    public class AsyncTcpClient : INotifyPropertyChanged
    {
        private Socket theSocket = null;

        private string remoteServerIp;
        private int remoteServerPort;

        private Timer autoReconnectTimer = null;
        const int ReconnectInterval = 3000;

        private bool autoReconnect = false;

        // The response from the remote device.
        private StringBuilder responseStrBuilder = new StringBuilder();
        private string responseString = string.Empty;

        //json left/right braces counters
        private int lbraceCnt = 0;
        private int rbraceCnt = 0;

        public class ResponseRecvEventArgs : EventArgs
        {
            public ResponseType ReplyT { get; private set; }
            public string JsonMsg { get; private set; }

            public ResponseRecvEventArgs(ResponseType t, string msg)
            {
                ReplyT = t;
                JsonMsg = msg;
            }
        }

        public class ConnectionStatusChangedEventArgs : EventArgs
        {
            public bool Connected { get; private set; }
            public string Message { get; private set; }

            public ConnectionStatusChangedEventArgs(bool connected, string msg)
            {
                Connected = connected;
                Message = msg;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event EventHandler JsonResponseRecvHandler;
        public event EventHandler ConnectionStatusChangeHandler;

        private string responseMsg = string.Empty;
        public string ResponseMsg
        {
            get { return responseMsg; }

            set
            {
                if (responseMsg != value)
                {
                    responseMsg = value;
                    NotifyPropertyChanged("ResponseMsg");
                }
            }
        }

        private bool connected = false;
        public bool Connected
        {
            get { return connected; }
            set
            {
                if (connected != value)
                {
                    connected = value;
                    NotifyPropertyChanged("Connected");
                }
            }
        }

        public AsyncTcpClient(string remoteIp, int port, bool autoCon = true)
        {
            remoteServerIp = remoteIp;
            remoteServerPort = port;
            autoReconnect = autoCon;
        }

        ~AsyncTcpClient()
        {
            closeSocket();
        }

        private void closeSocket()
        {
            if (theSocket != null && theSocket.Connected == true)
            {
                // Release the socket.
                theSocket.Shutdown(SocketShutdown.Both);
                theSocket.Close();
            }
            Connected = false;
        }

        public void stopClient()
        {
            Console.WriteLine("Stopping async socket client");
            autoReconnect = false;
            if (autoReconnectTimer != null)
            {
                autoReconnectTimer.Stop();
            }

            closeSocket();
            JsonResponseRecvHandler = null;
            ConnectionStatusChangeHandler = null;
        }

        public void startClient()
        {
            Console.WriteLine("Starting async socket client");
            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                // The name of the 
                // remote device is "host.contoso.com".
                //IPHostEntry ipHostInfo = Dns.GetHostEntry("host.contoso.com");
                //IPAddress ipAddress = ipHostInfo.AddressList[0];;

                //IPEndPoint remoteEP = new IPEndPoint(remoteIp, port);

                // Create a TCP/IP socket.
                theSocket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                theSocket.BeginConnect(remoteServerIp, remoteServerPort,
                    new AsyncCallback(connectCallback), theSocket);
                //connectDone.WaitOne();

                // Send test data to the remote device.

                //sendDone.WaitOne();


                //receiveDone.WaitOne();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                startReconnectServer();
            }
        }

        private void startReconnectServer()
        {
            if (autoReconnect == true)
            {
                if (autoReconnectTimer == null)
                {
                    autoReconnectTimer = new Timer(ReconnectInterval);
                    autoReconnectTimer.Elapsed += new ElapsedEventHandler(autoReconnectTimerTriggred);
                }

                autoReconnectTimer.Start();
            }
        }

        private void autoReconnectTimerTriggred(object sender, ElapsedEventArgs arg)
        {
            autoReconnectTimer.Stop();
            if (theSocket.Connected == false)
            {
                startClient();
            }
        }

        public void sendMessage(String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            //data += "\n";
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            // Begin sending the data to the remote device.
            if (theSocket != null && theSocket.Connected == true)
            {
                try
                {
                    theSocket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(sendCallback), theSocket);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
        }

        private void connectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                theSocket = (Socket)ar.AsyncState;

                // Complete the connection.
                theSocket.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    theSocket.RemoteEndPoint.ToString());

                // Receive the response from the remote device.
                Connected = true;
                NotifyConnectionStatus("交易服务器连接成功！");

                startReceive();

                // Signal that the connection has been made.
                //connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

                closeSocket();

                NotifyConnectionStatus("交易服务器连接失败！");

                startReconnectServer();
            }
        }

        private void onResponseReceived()
        {
            //Console.WriteLine("Message received:" + resp);

            //deserialize json
            try
            {
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(ResponseMsg));

                DataContractJsonSerializer baseSer = new DataContractJsonSerializer(typeof(ServerResponse));
                ServerResponse baseResponse = (ServerResponse)baseSer.ReadObject(stream);

                //check if deseriliazation success
                if (JsonResponseRecvHandler != null)
                {
                    JsonResponseRecvHandler(this, new ResponseRecvEventArgs(baseResponse.Type, ResponseMsg));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to parse request:" + ResponseMsg + ",error message:" + ex.Message);
            }
        }

        private void startReceive()
        {
            try
            {
                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = theSocket;

                // Begin receiving the data from the remote device.
                theSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(receiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                closeSocket();
                NotifyConnectionStatus("交易服务器通信失败！");
            }
        }

        private void receiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                //Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = theSocket.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    // state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    int appendStartIndex = 0;
                    byte[] recvBuff = state.buffer;
                    string recvStr = Encoding.UTF8.GetString(state.buffer, 0, bytesRead);

                    for (int i = 0; i < bytesRead; ++i)
                    {
                        if (recvBuff[i] == '\r' || recvBuff[i] == '\n')
                            continue;

                        if (recvBuff[i] == '{')
                        {
                            if (appendStartIndex < 0 || lbraceCnt <= 0)
                            {
                                appendStartIndex = i;
                            }
                            lbraceCnt++;
                        }
                        else if (recvBuff[i] == '}')
                        {
                            rbraceCnt++;
                            //check matched
                            if (lbraceCnt == rbraceCnt)
                            {
                                string msgGet = Encoding.UTF8.GetString(state.buffer, appendStartIndex, i - appendStartIndex + 1);
                                responseStrBuilder.Append(Encoding.UTF8.GetString(state.buffer, appendStartIndex, i - appendStartIndex + 1));
                                ResponseMsg = responseStrBuilder.ToString();
                                onResponseReceived();
                                responseStrBuilder.Clear();
                                lbraceCnt = 0;
                                rbraceCnt = 0;
                                appendStartIndex = i + 1;
                            }
                            else if (rbraceCnt > lbraceCnt)
                            {
                                appendStartIndex = -1;
                                lbraceCnt = 0;
                                rbraceCnt = 0;
                            }
                        }
                    }

                    if (appendStartIndex >= 0 && appendStartIndex < bytesRead - 1)
                    {
                        string msgGet = Encoding.UTF8.GetString(state.buffer, appendStartIndex, bytesRead - appendStartIndex);
                        responseStrBuilder.Append(Encoding.UTF8.GetString(state.buffer, appendStartIndex, bytesRead - appendStartIndex));
                    }
                }
                else
                {
                    Console.WriteLine(bytesRead + " size message received");
                    closeSocket();
                }
                theSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                         new AsyncCallback(receiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                closeSocket();
                NotifyConnectionStatus("交易服务器通信失败！");
                startReconnectServer();
            }
        }

        private void sendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                //Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = theSocket.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
                //sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                closeSocket();

                NotifyConnectionStatus("发送消息至服务器失败！");
            }
        }

        private void NotifyConnectionStatus(string msg)
        {
            if (ConnectionStatusChangeHandler != null)
            {
                ConnectionStatusChangeHandler(this, new ConnectionStatusChangedEventArgs(Connected, msg));
            }
        }
    }
}
