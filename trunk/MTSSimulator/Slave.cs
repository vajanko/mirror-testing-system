using System;
using System.Collections.Generic;
using System.Text;
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
        private Thread listenerThread;
        private TcpListener slave;
        private bool running = false;
        private NetworkStream stream;
        private Channels channels;
        private List<Thread> threads = new List<Thread>();
        private System.Timers.Timer timer;

        private event Action update;
        public event Action Update
        {
            add { update += value; }
            remove { update -= value; }
        }

        public void Listen(IPAddress ip, int port)
        {
            slave = new TcpListener(port);
            timer = new System.Timers.Timer(100);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Start();
            running = true;

            //Socket slave = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //slave.Bind(new IPEndPoint(ip, port));

            // create a new thread for listeninng incomming connections
            listenerThread = new Thread(new ParameterizedThreadStart(loop));
            listenerThread.Start(slave);
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (update != null)
                update();
        }

        public void Disconnect()
        {
            timer.Stop();
            slave.Stop();
            foreach (Thread t in threads)
            {
                t.Abort();
            }
            listenerThread.Abort();
        }

        /// <summary>
        /// Listening loop for incomming connections
        /// </summary>
        /// <param name="slave"></param>
        private void loop(object slaveListener)
        {
            TcpListener slave = slaveListener as TcpListener;
            slave.Start(10);

            if (slave == null)
                running = false;

            while (running)
            {
                try
                {
                    TcpClient master = slave.AcceptTcpClient();

                    Responder responder = new Responder(master, channels);
                    Thread mt = new Thread(new ThreadStart(responder.respond));
                    threads.Add(mt);
                    mt.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    running = false;
                }
            }
        }

        public Slave(Channels channels)
        {
            this.channels = channels;
        }
    }

    class Responder
    {
        private TcpClient master;
        IModule module;

        public void respond()
        {
            if (master != null)
            {

                NetworkStream stream = master.GetStream();
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream);
                while (true)
                {
                    try
                    {
                        string command = reader.ReadLine();

                        if (command == "read")
                        {
                            // lock channels
                            lock (module)
                            {
                                foreach (IChannel channel in module)
                                {
                                    writer.Write("{0}:{1}\n", channel.Name, System.Text.ASCIIEncoding.ASCII.GetString(channel.ValueBytes));
                                }
                                writer.WriteLine("end");
                                //stream.Flush();
                                writer.Flush();
                            }
                        }
                    }
                    catch
                    {
                        break;
                    }
                }
            }
        }

        public Responder(TcpClient master, IModule module)
        {
            this.master = master;
            this.module = module;
        }
    }
}
