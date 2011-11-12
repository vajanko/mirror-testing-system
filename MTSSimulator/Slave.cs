using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization;
using MTS.IO;

namespace MTS.Simulator
{
    public class Slave
    {
        private Thread listener;
        private bool running;
        private NetworkStream stream;
        private IModule module;
        private List<Thread> threads = new List<Thread>();

        public void Listen(IPAddress ip, int port)
        {
            Socket slave = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            slave.Bind(new IPEndPoint(ip, port));

            // create a new thread for listeninng incomming connections
            listener = new Thread(new ParameterizedThreadStart(loop));
            listener.Start(slave);
        }
        public void Disconnect()
        {
            foreach (Thread t in threads)
            {
                t.Abort();
            }
            listener.Abort();
        }


        private void loop(object slave)
        {
            Socket soc = slave as Socket;
            soc.Listen(10);
            if (soc != null)
            {
                while (true)
                {
                    Socket master = soc.Accept();
                    Responder responder = new Responder(new NetworkStream(master, FileAccess.ReadWrite, true),
                        module);
                    Thread mt = new Thread(new ThreadStart(responder.respond));
                    threads.Add(mt);
                    mt.Start();
                }
            }
        }

        public Slave(IModule module)
        {
            this.module = module;
        }
    }

    class Responder
    {
        private NetworkStream stream;
        IModule module;

        public void respond()
        {
            if (stream != null)
            {
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream);
                while (!reader.EndOfStream)
                {
                    if (reader.ReadLine() == "read")
                    {
                        // lock module
                        foreach (IChannel channel in module)
                        {
                            writer.Write("{0}:", channel.Name);
                            stream.Write(channel.ValueBytes, 0, channel.ValueBytes.Length);
                            writer.WriteLine();
                        }
                    }
                }
            }
        }

        public Responder(NetworkStream stream, IModule module)
        {
            this.stream = stream;
            this.module = module;
        }
    }    
}
