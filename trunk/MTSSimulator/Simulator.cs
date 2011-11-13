using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MTS.IO;
using MTS.IO.Module;
using MTS.IO.Settings;


namespace MTS.Simulator
{
    public partial class Simulator : Form
    {
        private Channels channels;

        public Simulator()
        {
            InitializeComponent();
            initializeDevice();

            channels = new Channels(new DummyModule(), HWSettings.Default.ChannelSettings);
            channels.LoadConfiguration("dummyConfig.csv");
            channels.Initialize();
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
                if (channels != null && channels.IsPowerSupplyOff != null)
                    channels.IsPowerSupplyOff.SetValue(!isPowerOn);
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
            if (IsPowerOn) IsPowerOn = false;
            else IsPowerOn = true;
        }
        Slave slave;
        Random gen = new Random();
        private void listenButton_Click(object sender, EventArgs e)
        {
            slave = new Slave(channels);
            slave.Update += new Action(slave_Update);
            // debug
            channels.FoldPowerfold.SetValue(true);
            channels.HeatingFoilOn.SetValue(true);
            channels.AllowPowerSupply.SetValue(true);

            IsPowerOn = false;      // power supply is off

            slave.Listen(System.Net.IPAddress.Loopback, 1234);
        }

        void slave_Update()
        {
            if (channels.FoldPowerfold.Value)
                channels.PowerfoldCurrent.SetValue((uint)gen.Next((int)powerfoldMin.Value, (int)powerfoldMax.Value));
            else if (channels.UnfoldPowerfold.Value)
                channels.PowerfoldCurrent.SetValue((uint)gen.Next((int)powerfoldMin.Value, (int)powerfoldMax.Value));

            if (channels.HeatingFoilOn.Value)
                channels.HeatingFoilCurrent.SetValue((uint)gen.Next((int)spiralCurrentMin.Value, (int)spiralCurrentMax.Value));

            if (channels.DirectionLightOn.Value) ;
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            slave.Disconnect();
        }
    }
}
