using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Model;

namespace Server
{
    public class Server
    {
        private int _currentPlayerAmount = 0;
        private static readonly IPAddress IpAddress = IPAddress.Parse("127.0.0.1");
        private const int Port = 11000;
        private static ManualResetEvent AcceptDone = new (false);
        private Controller _controller;
        public void Start()
        {
            _controller = new Controller();
            IPEndPoint localEndPoint = new IPEndPoint(IpAddress, Port);
            Socket listener = new(IpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);
                while (_currentPlayerAmount != 2)
                {
                    AcceptDone.Reset();
                    Console.WriteLine("Waiting for connection");
                    listener.BeginAccept(AcceptCallback, listener);
                    _currentPlayerAmount++;
                    AcceptDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void AcceptCallback(IAsyncResult asyncResult)
        {
            AcceptDone.Set();
            var socket = (Socket) asyncResult.AsyncState;
            Socket handler = socket.EndAccept(asyncResult);
            var state = new State {ClientSocket = handler};
            handler.BeginReceive(state.Buffer, 0, State.BufferSize,
                0, ReadCallback, state);
        }

        private void ReadCallback(IAsyncResult asyncResult)
        {
            var state = (State) asyncResult.AsyncState;
            Socket socket = state.ClientSocket;
            int bytesRead = socket.EndReceive(asyncResult);
            if (bytesRead <= 0)
                return;
            state.StrBuilder.Append(
                Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));
            string content = state.StrBuilder.ToString();
            if (content.Contains("<EOF>"))
            {
                content = content.Replace("<EOF>", "");
                string response = _controller.HandleRequest(content);
                Send(response + "<EOF>", socket);
                return;
            }
            socket.BeginReceive(state.Buffer, 0,
                State.BufferSize, 0, ReadCallback, state);
        }

        private void Send(string data, Socket socket)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            socket.BeginSend(byteData, 0, byteData.Length, 0,
                SendCallback, socket);
        }

        private void SendCallback(IAsyncResult asyncResult)
        {
            try
            {
                var socket = (Socket) asyncResult.AsyncState;
                int bytesSent = socket.EndSend(asyncResult);
                Console.WriteLine($"Sent {bytesSent} to client");
                var state = new State {ClientSocket = socket};
                socket.BeginReceive(state.Buffer, 0, State.BufferSize,
                    0, ReadCallback, state);
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
        public readonly byte[] Buffer = new byte[BufferSize];
        public readonly StringBuilder StrBuilder = new();
        public Socket ClientSocket;
    }  
}