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

            // set default values to channels
            initializeChannels();

            // initialize tester values
            updateTester();

            slave.Listen(System.Net.IPAddress.Loopback, 1234);
        }

        private void initializeChannels()
        {
            lock (channels)
            {
                channels.AllowPowerSupply.SetValue(false);
                channels.AllowMirrorMovement.SetValue(false);
                channels.IsOldMirror.SetValue(isOldMirror.Checked);
                channels.IsLeftMirror.SetValue(isLeftMirror.Checked);
                channels.IsLocked.SetValue(false);
                channels.IsOldLocked.SetValue(false);
            }
        }


        void updateTester()
        {
            tester1.Dispatcher.Invoke(new Action(updateTesterChannels));
        }
        void updateTesterChannels()
        {   // this method is executed on GUI thread
            tester1.IsGreenLightOn = channels.GreenLightOn.Value;
            tester1.IsRedLightOn = channels.RedLightOn.Value;
            tester1.IsPowerSupplyOn = !channels.IsPowerSupplyOff.Value;
            tester1.IsStartPressed = channels.IsStartPressed.Value;
            if (channels.IsOldMirror.Value)
                tester1.IsDeviceOpened = !channels.IsOldLocked.Value;
            else
                tester1.IsDeviceOpened = !channels.IsLocked.Value;

            if (measuring)
            {
                timerState.Text = "Measuring";
                timerElapsed.Text = elapsedTime().ToString("{0:2}");
            }
            else
            {
                timerState.Text = "Not measuring";
            }
        }

        private bool measuring = false;
        private DateTime time;
        private void startTimer()
        {
            measuring = true;
            time = DateTime.Now;
        }
        private double elapsedTime() { return (DateTime.Now - time).TotalMilliseconds; }
        private void stopTimer()
        {
            measuring = false;
        }
        void slave_Update()
        {
            lock (channels)
            {
                // "swtich off" power supply if it is not allowed
                if (!channels.AllowPowerSupply.Value)
                    channels.IsPowerSupplyOff.SetValue(true);

                // open device
                if (!channels.LockStrong.Value && !channels.LockWeak.Value &&
                    channels.UnlockStrong.Value && channels.UnlockWeak.Value)
                    unlock();
                // close device
                else if (!channels.LockStrong.Value && !channels.UnlockWeak.Value
                    && !channels.UnlockStrong.Value && channels.LockWeak.Value)
                    lockWeak();

                // simulate powerfold current
                if (channels.FoldPowerfold.Value)
                    channels.PowerfoldCurrent.SetValue((uint)gen.Next((int)powerfoldMin.Value, (int)powerfoldMax.Value));
                else if (channels.UnfoldPowerfold.Value)
                    channels.PowerfoldCurrent.SetValue((uint)gen.Next((int)powerfoldMin.Value, (int)powerfoldMax.Value));

                // simulate heating foil current
                if (channels.HeatingFoilOn.Value)
                    channels.HeatingFoilCurrent.SetValue((uint)gen.Next((int)spiralCurrentMin.Value, (int)spiralCurrentMax.Value));

                // simulate direction light current
                if (channels.DirectionLightOn.Value)
                    channels.DirectionLightCurrent.SetValue((uint)gen.Next((int)directionLightCurrentMin.Value, (int)directionLightCurrentMax.Value));

                // update user interface
                updateTester();
            }
        }

        private void unlock()
        {
            if (!measuring) // time is not being measured
            {
                if (channels.IsOldMirror.Value && !channels.IsOldLocked.Value)
                    return; // already unlocked
                else if (!channels.IsOldMirror.Value && !channels.IsLocked.Value)
                    return; // already unlocked
                startTimer();
            }
            else if (elapsedTime() >= (double)unlockTime.Value)
            {
                if (channels.IsOldMirror.Value)
                    channels.IsOldLocked.SetValue(false);
                else
                    channels.IsLocked.SetValue(false);
                stopTimer();
            }
        }
        private void lockWeak()
        {
            if (!measuring)
            {
                if (channels.IsOldMirror.Value && channels.IsOldLocked.Value)
                    return; // already locked
                else if (!channels.IsOldMirror.Value && channels.IsLocked.Value)
                    return; // already locked
                startTimer();
            }
            else if (elapsedTime() >= (double)unlockTime.Value)
            {
                if (channels.IsOldMirror.Value)
                    channels.IsOldLocked.SetValue(true);
                else
                    channels.IsLocked.SetValue(true);
                stopTimer();
            }
        }

        private void isOldMirror_CheckedChanged(object sender, EventArgs e)
        {
            if (channels != null)
            {
                lock (channels)
                {
                    channels.IsOldMirror.SetValue(isOldMirror.Checked);
                }
            }
        }
        private void isLeftMirror_CheckedChanged(object sender, EventArgs e)
        {
            if (channels != null)
            {
                lock (channels)
                {
                    channels.IsLeftMirror.SetValue(isLeftMirror.Checked);
                }
            }
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            if (slave != null)
                slave.Disconnect();
        }

        private void lockButton_Click(object sender, EventArgs e)
        {
            if (channels != null)
            {
                lock (channels)
                {
                    channels.LockStrong.SetValue(false);
                    channels.LockWeak.SetValue(true);
                    channels.UnlockStrong.SetValue(false);
                    channels.UnlockWeak.SetValue(false);
                }
            }
        }

        private void unlockButton_Click(object sender, EventArgs e)
        {
            if (channels != null)
            {
                lock (channels)
                {
                    channels.LockStrong.SetValue(false);
                    channels.LockWeak.SetValue(false);
                    channels.UnlockStrong.SetValue(true);
                    channels.UnlockWeak.SetValue(true);
                }
            }
        }

        private void insertMirrorButton_Click(object sender, EventArgs e)
        {
            if (tester1.IsDeviceOpened)
                tester1.IsMirrorInserted = true;
        }

        private void removeMirrorButton_Click(object sender, EventArgs e)
        {
            if (tester1.IsDeviceOpened)
                tester1.IsMirrorInserted = false;
        }

        private void leftRubber_CheckedChanged(object sender, EventArgs e)
        {
            if (channels != null)
            {
                lock (channels)
                {
                    if (channels.IsLeftRubberPresent != null)
                        channels.IsLeftRubberPresent.SetValue(leftRubber.Checked);
                }
            }
        }
        private void rightRubber_CheckedChanged(object sender, EventArgs e)
        {
            if (channels != null)
            {
                lock (channels)
                {
                    if (channels.IsRightRubberPresent != null)
                        channels.IsRightRubberPresent.SetValue(rightRubber.Checked);
                }
            }
        }
    }
}
