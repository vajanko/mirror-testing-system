﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization;
using MTS.IO;
using MTS.IO.Module;
using MTS.IO.Address;

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

        private event Action update;
        public event Action Update
        {
            add { update += value; }
            remove { update -= value; }
        }

        public void Listen(IPAddress ip, int port)
        {
            if (running) return;

            slave = new TcpListener(ip, port);
            running = true;

            // create a new thread for listening incoming connections
            listenerThread = new Thread(new ParameterizedThreadStart(loop));
            listenerThread.Start(slave);
        }


        public void Disconnect()
        {
            running = false;
            slave.Stop();
            foreach (Thread t in threads)
            {
                t.Abort();
            }
            listenerThread.Abort();
        }

        /// <summary>
        /// Listening loop for incoming connections
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

                    Responder responder = new Responder(master, channels, update);
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
        private Action update;
        private void OnUpdate()
        {
            if (update != null)
                update();
        }

        public void respond()
        {
            if (master == null) return;
 
            NetworkStream stream = master.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);
            ASCIIEncoding enc = new ASCIIEncoding();
            while (true)
            {
                try
                {
                    string command = reader.ReadLine();

                    // read command from master
                    if (command == "read")
                    {
                        // lock channels because multiple thread may access it
                        lock (module)
                        {
                            foreach (IChannel channel in module)
                            {
                                if (channel is IAnalogInput)
                                    writer.Write("{0}:{1}\n", channel.Id, (channel as IAnalogInput).Value);
                                else if (channel is IDigitalInput)
                                    writer.Write("{0}:{1}\n", channel.Id, (channel as IDigitalInput).Value);
                            }
                        }
                        writer.WriteLine("end");
                        writer.Flush();
                    }
                    // write command from master
                    else if (command == "write")
                    {   // lock channels because multiple thread may access it
                        // read channels that should be wrote
                        string line;
                        while ((line = reader.ReadLine()) != "end")
                        {
                            string[] tmp = line.Split(':');
                            if (tmp.Length > 1)
                            {
                                string id = tmp[0];
                                string value = tmp[1];
                                lock (module)
                                {
                                    IChannel channel = module.GetChannel(id);
                                    if (channel != null)
                                    {
                                        if (channel is IAnalogInput)
                                            (channel as IAnalogInput).SetValue(uint.Parse(value));
                                        else if (channel is IDigitalInput)
                                            (channel as IDigitalInput).SetValue(bool.Parse(value));
                                    }
                                }
                            }
                        }
                    }

                    OnUpdate();
                }
                catch (Exception ex)
                {
                    break;
                }
            }
        }

        public Responder(TcpClient master, IModule module, Action onUpdate)
        {
            this.master = master;
            this.module = module;
            this.update = onUpdate;
        }
    }
}
