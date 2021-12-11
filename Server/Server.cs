using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    public class Server
    {
        private int _currentPlayerAmount = 0;
        private static readonly IPAddress IpAddress = IPAddress.Parse("127.0.0.1");
        private const int Port = 11000;
        private static ManualResetEvent AllDone = new (false);
        public void Start()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IpAddress, Port);
            Socket listener = new(IpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);
                while (true)
                {
                    if (_currentPlayerAmount == 2)
                        break;
                    AllDone.Reset();
                    Console.WriteLine("Waiting for connection");
                    listener.BeginAccept(AcceptCallback, listener);
                    _currentPlayerAmount++;
                    AllDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void AcceptCallback(IAsyncResult asyncResult)
        {
            AllDone.Set();
            var listener = (Socket) asyncResult.AsyncState;
            Socket handler = listener.EndAccept(asyncResult);
            var state = new State {ClientSocket = handler};
            handler.BeginReceive(state.Buffer, 0, State.BufferSize,
                0, ReadCallback, state);
        }

        private static void ReadCallback(IAsyncResult asyncResult)
        {
            var content = String.Empty;
            var state = (State) asyncResult.AsyncState;
            Socket socket = state.ClientSocket;
            int bytesRead = socket.EndReceive(asyncResult);
            if (bytesRead <= 0)
                return;
            state.StrBuilder.Append(
                Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));
            content = state.StrBuilder.ToString();
            if (content.Contains("<EOF>"))
            {
                HandleReceivedData(content, socket);
                return;
            }
            socket.BeginReceive(state.Buffer, 0,
                State.BufferSize, 0, ReadCallback, state);
        }

        private static void HandleReceivedData(string data, Socket socket)
        {
            Console.WriteLine($"Read {data.Length} bytes from socket\n Data: {data}");
            Send(data, socket);
        }

        private static void Send(string data, Socket socket)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            socket.BeginSend(byteData, 0, byteData.Length, 0,
                SendCallback, socket);
        }

        private static void SendCallback(IAsyncResult asyncResult)
        {
            try
            {
                var handler = (Socket) asyncResult.AsyncState;
                int bytesSent = handler.EndSend(asyncResult);
                Console.WriteLine($"Sent {bytesSent} to client");
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        } 
    }
    
    public class State
    {
        public const int BufferSize = 1024;
        public byte[] Buffer = new byte[BufferSize];
        public StringBuilder StrBuilder = new();
        public Socket ClientSocket;
    }  
}