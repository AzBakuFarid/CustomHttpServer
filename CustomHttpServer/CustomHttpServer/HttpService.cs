using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomHttpServer
{
    class HttpService
    {
        public static readonly string Version = System.Configuration.ConfigurationManager.AppSettings["HttpVersion"];
        public static readonly string ServiceName = System.Configuration.ConfigurationManager.AppSettings["ServiceName"];
        public static readonly string MsgDirectory = System.Configuration.ConfigurationManager.AppSettings["MsgDirectory"];
        public static readonly string WebDirectory = System.Configuration.ConfigurationManager.AppSettings["WebDirectory"];

        private bool IsRunning { get; set; }
        private TcpListener Listener { get; set; }

        public HttpService(int _port)
        {
            Listener = new TcpListener(IPAddress.Any, _port);
        }


        public void Start()
        {
            Thread ServerThread = new Thread(new ThreadStart(Run));
            ServerThread.Start();
        }

        private void Run()
        {
            IsRunning = true;

            Listener.Start();

            while (IsRunning)
            {

                Console.WriteLine("Waiting for connection...");

                TcpClient client = Listener.AcceptTcpClient();

                Console.WriteLine("Client connected!");

                HandleClient(client);

                client.Close();
            }

            IsRunning = false;

            Listener.Stop();
        }

        private void HandleClient(TcpClient _client)
        {

            StreamReader reader = new StreamReader(_client.GetStream());

            string msg = "";
            try
            {
                while (reader.Peek() != -1)
                {
                    msg += reader.ReadLine() + " ";

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


            //Console.WriteLine($"Request : {Environment.NewLine} {msg}" );

            Request _request = Request.GetRequest(msg);

            Responce _responce = Responce.From(_request);
            _responce.Post(_client.GetStream());
        }
    }
}
