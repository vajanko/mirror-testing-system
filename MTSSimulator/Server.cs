using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using MTS.IO.Module;

namespace MTS.Simulator
{
    //class Server
    //{
    //    private Thread listener;
    //    private bool running;
    //    private NetworkStream stream;
    //    private void litening()
    //    {

    //        Master master = new Master("127.0.0.1", Port);

    //        //Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    //        //server.Bind(new IPEndPoint(IPAddress.Loopback, Port));

    //        //server.Listen(1);
    //        //while (running)
    //        //{
    //        //    // we will wait here until any incomming conection aprears
    //        //    Socket client = server.Accept();

    //        //    stream = new NetworkStream(client, System.IO.FileAccess.ReadWrite);

    //        //    // as there is only one connection we do not need to create an instance
    //        //    // of object and run its method
    //        //    Thread thread = new Thread(new ThreadStart(loop));
    //        //    thread.Start();
    //        //}
    //    }

    //    private void loop()
    //    {

    //    }

    //    public ushort Port { get; private set; }

    //    public void Start()
    //    {
    //        running = true;
    //    }
    //    public void Stop()
    //    {
    //        running = false;
    //    }

    //    #region Constructors

    //    public Server()
    //    {
    //        listener = new Thread(new ThreadStart(litening));
    //        Port = 502;
    //        running = false;
    //    }
    //    public Server(ushort port)
    //        : this()
    //    {
    //        Port = port;
    //    }

    //    #endregion
    //}
}
