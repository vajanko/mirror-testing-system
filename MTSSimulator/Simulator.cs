using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MTS.IO.Module;


namespace MTS.Simulator
{
    public partial class Simulator : Form
    {
        private MTS.IO.IModule module;

        public Simulator()
        {
            InitializeComponent();
            initializeDevice();
            module = new DummyModule();
            module.LoadConfiguration("dummyConfig.csv");
            module.Initialize();
        }

        #region Device status

        private bool isClosed = false;
        public bool IsClosed
        {
            get { return isClosed; }
            private set
            {
                isClosed = value;
                if (value) closeDeviceButton.Text = "Open device";
                else closeDeviceButton.Text = "Close device";
            }
        }
        public bool IsOpened
        {
            get { return !IsClosed; }
            set { IsClosed = !value; }
        }
        
        private bool isPowerOn = false;
        public bool IsPowerOn
        {
            get { return isPowerOn; }
            private set
            {
                isPowerOn = value;
                if (value) powerButton.Text = "Off";
                else powerButton.Text = "On";
            }
        }


        private void initializeDevice()
        {
            IsClosed = false;
            IsPowerOn = false;
        }

        #endregion

        private void closeDeviceButton_Click(object sender, EventArgs e)
        {
            if (IsClosed)
                IsClosed = false;
            else IsClosed = true;
        }
        private void powerButton_Click(object sender, EventArgs e)
        {
            if (IsPowerOn)
                IsPowerOn = false;
            else IsPowerOn = true;
        }
        Slave slave;
        private void listenButton_Click(object sender, EventArgs e)
        {
            slave = new Slave(module);
            slave.Listen(System.Net.IPAddress.Loopback, 1234);
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            slave.Disconnect();
        }
    }
}
