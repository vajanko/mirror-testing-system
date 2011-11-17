﻿using System;
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

            tester1.IsRedLightOn = false;
            tester1.IsGreenLightOn = false;
            tester1.IsPowerSupplyOn = false;
            tester1.IsStartPressed = false;
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

                if (channels != null)
                {
                    lock (channels)
                    {
                        if (channels.IsPowerSupplyOff != null)
                            channels.IsPowerSupplyOff.SetValue(!isPowerOn);
                    }
                }
            }
        }

        private bool isStartPressed = false;
        public bool IsStartPressed
        {
            get { return isStartPressed; }
            set
            {
                isStartPressed = value;
                if (channels != null)
                {
                    lock (channels)
                    {
                        if (channels.IsStartPressed != null)
                            channels.IsStartPressed.SetValue(isStartPressed);
                    }
                }
            }
        }


        private void initializeDevice()
        {
            IsClosed = false;
            IsPowerOn = false;
        }

        #endregion

        private void startButton_MouseDown(object sender, MouseEventArgs e)
        {
            IsStartPressed = true;
        }
        private void startButton_MouseUp(object sender, MouseEventArgs e)
        {
            IsStartPressed = false;
        }
        private void closeDeviceButton_Click(object sender, EventArgs e)
        {
            if (IsClosed) IsClosed = false;
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

            IsPowerOn = false;      // power supply is off

            // initialize tester values
            updateTester();

            slave.Listen(System.Net.IPAddress.Loopback, 1234);
        }

        void updateTester()
        {
            tester1.Dispatcher.Invoke(new Action(updateTesterChannels));
        }
        void updateTesterChannels()
        {
            tester1.IsGreenLightOn = channels.GreenLightOn.Value;
            tester1.IsRedLightOn = channels.RedLightOn.Value;
            tester1.IsPowerSupplyOn = !channels.IsPowerSupplyOff.Value;
            tester1.IsStartPressed = channels.IsStartPressed.Value;
        }

        void slave_Update()
        {
            lock (channels)
            {
                // "swtich off" power supply if it is not allowed
                if (!channels.AllowPowerSupply.Value)
                    channels.IsPowerSupplyOff.SetValue(true);

                if (channels.FoldPowerfold.Value)
                    channels.PowerfoldCurrent.SetValue((uint)gen.Next((int)powerfoldMin.Value, (int)powerfoldMax.Value));
                else if (channels.UnfoldPowerfold.Value)
                    channels.PowerfoldCurrent.SetValue((uint)gen.Next((int)powerfoldMin.Value, (int)powerfoldMax.Value));

                if (channels.HeatingFoilOn.Value)
                    channels.HeatingFoilCurrent.SetValue((uint)gen.Next((int)spiralCurrentMin.Value, (int)spiralCurrentMax.Value));

                if (channels.DirectionLightOn.Value) ;

                updateTester();
            }
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            slave.Disconnect();
        }
    }
}
