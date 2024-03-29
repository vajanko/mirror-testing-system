﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MTS.IO;
using MTS.IO.Address;
using MTS.IO.Module;
using System.IO;


namespace MTS.Simulator
{
    public partial class Simulator : Form
    {
        private Channels channels;

        #region Constructors

        public Simulator()
        {
            InitializeComponent();

            channels = new Channels(new DummyModule(), HWSettings.Default.ChannelSettings);
            string currentDir = Path.GetDirectoryName(Application.ExecutablePath);
            channels.LoadConfiguration(Path.Combine(currentDir, "config\\simulatorConfig.csv"));
            channels.Initialize();

            foldTimer.Interval = (double)foldingTime.Value;
            unfoldTimer.Interval = (double)unfoldingTime.Value;
            lockTimer.Interval = (double)lockTime.Value;
            unlockTimer.Interval = (double)unlockTime.Value;
            // let the mirror be inserted
            insertMirrorButton_Click(insertMirrorButton, EventArgs.Empty);
            tester1.IsDirectionLightOn = true;
        }

        #endregion

        Slave slave;
        Random gen = new Random();
        private void listenButton_Click(object sender, EventArgs e)
        {
            listenButton.Enabled = false;

            int port;
            if (!int.TryParse(portBox.Text, out port))
                port = 1234;

            slave = new Slave(channels);
            slave.Update += new Action(slave_Update);

            // set default values to channels
            initializeChannels();

            // initialize tester values
            updateTester();

            slave.Listen(System.Net.IPAddress.Loopback, port);

            disconnectButton.Enabled = true;
        }
        private void disconnectButton_Click(object sender, EventArgs e)
        {
            disconnectButton.Enabled = false;

            if (slave != null)
                slave.Disconnect();

            listenButton.Enabled = true;
        }
        private void Simulator_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (slave != null)
                slave.Disconnect();
            Properties.Settings.Default.Save();
        }

        private void initializeChannels()
        {
            if (channels != null)
            {
                lock (channels)
                {
                    channels.AllowPowerSupply.SetValue(false);
                    channels.AllowMirrorMovement.SetValue(false);
                    channels.IsOldMirror.SetValue(isOldMirror.Checked);
                    channels.IsLeftMirror.SetValue(isLeftMirror.Checked);
                    channels.IsLocked.SetValue(false);
                    channels.IsOldLocked.SetValue(false);
                    channels.IsLeftRubberPresent.SetValue(leftRubber.Checked);
                    channels.IsRightRubberPresent.SetValue(rightRubber.Checked);
                    channels.IsPowerSupplyOff.SetValue(powerOff.Checked);
                    channels.DistanceX.SetValue(2200);
                    channels.DistanceY.SetValue(2000);
                    channels.DistanceZ.SetValue(2000);
                    channels.IsSuckerUp.SetValue(false);
                    channels.IsSuckerDown.SetValue(true);
                    channels.IsDistanceSensorDown.SetValue(true);
                    channels.IsDistanceSensorUp.SetValue(false);
                    channels.DirectionLightOn.SetValue(false);
                }
            }
        }

        void updateTester()
        {
            tester1.Dispatcher.BeginInvoke(new Action(updateTesterChannels));
        }
        void updateTesterChannels()
        {   // this method is executed on GUI thread
            if (channels != null)
            {
                lock (channels)
                {
                    tester1.IsGreenLightOn = channels.GreenLightOn.Value;
                    tester1.IsRedLightOn = channels.RedLightOn.Value;
                    tester1.IsPowerSupplyOn = !channels.IsPowerSupplyOff.Value;
                    tester1.IsStartPressed = channels.IsStartPressed.Value;
                    tester1.IsSuckerUp = channels.IsSuckerUp.Value;
                    if (lockTimer.Running)
                        tester1.IsDeviceOpened = false;
                    else if (unlockTimer.Running)
                        tester1.IsDeviceOpened = true;
                    calibratorX.Text = channels.DistanceX.RealValue.ToString();
                    calibratorY.Text = channels.DistanceY.RealValue.ToString();
                    calibratorZ.Text = channels.DistanceZ.RealValue.ToString();
                    tester1.IsCalibratorUp = channels.IsDistanceSensorUp.Value;
                    tester1.IsDirectionLightOn = channels.DirectionLightOn.Value;
                }
            }
        }

        private Timer foldTimer = new Timer();
        private Timer unfoldTimer = new Timer();
        private Timer lockTimer = new Timer();
        private Timer unlockTimer = new Timer();
        private Timer suckerUpTimer = new Timer(1000);
        private Timer suckerDownTimer = new Timer(1000);
        private Timer suckingTimer = new Timer(1000);
        private Timer blowingTimer = new Timer(1000);

        void slave_Update()
        {
            lock (channels)
            {
                // "switch off" power supply if it is not allowed
                //if (!channels.AllowPowerSupply.Value)
                //    channels.IsPowerSupplyOff.SetValue(true);

                simulateLockUnlock();

                simulatePowerFoldCurrent();

                simulateHeatingFoilCurrent();

                simulateDirectionLightCurrent();
                
                simulateMirrorMovement();

                simulatePowerSupplyVoltage();

                simulatePulloff();

                // update user interface
                updateTester();
            }
        }

        private void simulateLockUnlock()
        {
            // open device
            if (!channels.LockStrong.Value && !channels.LockWeak.Value &&
                channels.UnlockStrong.Value && channels.UnlockWeak.Value
                && channels.IsClosed())
            {
                unlock();
            }
            // close device
            else if (!channels.LockStrong.Value && !channels.UnlockWeak.Value
                && !channels.UnlockStrong.Value && channels.LockWeak.Value
                && !channels.IsClosed())
            {
                lockWeak();
            }
        }
        private void simulatePowerFoldCurrent()
        {
            // simulate powerfold current
            if (channels.FoldPowerfold.Value)
            {
                unfoldTimer.Initialize();
                if (foldTimer.Finished(DateTime.Now))
                    setIsFolded();

                channels.PowerfoldCurrent.SetValue((uint)gen.Next((int)powerfoldMin.Value, (int)powerfoldMax.Value));
            }
            else if (channels.UnfoldPowerfold.Value)
            {
                foldTimer.Initialize();
                if (unfoldTimer.Finished(DateTime.Now))
                    setIsUnfolded();

                channels.PowerfoldCurrent.SetValue((uint)gen.Next((int)powerfoldMin.Value, (int)powerfoldMax.Value));
            }
            else
            {
                channels.PowerfoldCurrent.SetValue(0);
            }
        }
        private void simulateHeatingFoilCurrent()
        {
            if (channels.HeatingFoilOn.Value)
                channels.HeatingFoilCurrent.SetValue((uint)gen.Next((int)spiralCurrentMin.Value, (int)spiralCurrentMax.Value));
            else
                channels.HeatingFoilCurrent.SetValue(0);
        }
        private void simulateDirectionLightCurrent()
        {
            if (channels.DirectionLightOn.Value)
                channels.DirectionLightCurrent.SetValue((uint)gen.Next((int)directionLightCurrentMin.Value, (int)directionLightCurrentMax.Value));
            else
                channels.DirectionLightCurrent.SetValue(0);
        }
        private void simulateMirrorMovement()
        {
            simulateDistanceSensors();

            simulateActuators();

            if (channels.IsMirrorMoveingUp)
            {
                channels.DistanceX.SetValue(channels.DistanceX.Value - (uint)movingSpeed.Value);
            }
            else if (channels.IsMirrorMoveingDown)
            {
                channels.DistanceX.SetValue(channels.DistanceX.Value + (uint)movingSpeed.Value);
            }
            else if (channels.IsMirrorMoveingLeft)
            {
                channels.DistanceZ.SetValue(channels.DistanceZ.Value + (uint)movingSpeed.Value);
            }
            else if (channels.IsMirrorMoveingRight)
            {
                channels.DistanceZ.SetValue(channels.DistanceZ.Value - (uint)movingSpeed.Value);
            }
        }
        private void simulateDistanceSensors()
        {
            if (channels.MoveDistanceSensorUp.Value)
                channels.IsDistanceSensorUp.SetValue(true);
            else
                channels.IsDistanceSensorUp.SetValue(false);

            if (channels.MoveDistanceSensorDown.Value)
                channels.IsDistanceSensorDown.SetValue(true);
            else
                channels.IsDistanceSensorDown.SetValue(false);
        }
        private void simulateActuators()
        {
            if (channels.MoveMirrorHorizontal.Value)
                channels.HorizontalActuatorCurrent.SetValue((uint)gen.Next(0, 500));
            else
                channels.HorizontalActuatorCurrent.SetValue(0);

            if (channels.MoveMirrorVertical.Value)
                channels.VerticalActuatorCurrent.SetValue((uint)gen.Next(0, 500));
            else
                channels.VerticalActuatorCurrent.SetValue(0);
        }
        private void simulatePowerSupplyVoltage()
        {
            if (!channels.IsPowerSupplyOff.Value)
            {
                channels.PowerSupplyVoltage1.SetValue((uint)gen.Next(0, 4095));
                channels.PowerSupplyVoltage2.SetValue((uint)gen.Next(0, 4095));
            }
            else
            {
                channels.PowerSupplyVoltage1.SetValue(0);
                channels.PowerSupplyVoltage2.SetValue(0);
            }
        }
        private void simulatePulloff()
        {
            if (channels.MoveSuckerUp.Value)
            {
                suckerDownTimer.Initialize();
                if (suckerUpTimer.Finished(DateTime.Now))
                {
                    channels.IsSuckerUp.SetValue(true);
                    channels.IsSuckerDown.SetValue(false);
                }
            }
            else if (channels.MoveSuckerDown.Value)
            {
                suckerUpTimer.Initialize();
                if (suckerDownTimer.Finished(DateTime.Now))
                {
                    channels.IsSuckerUp.SetValue(false);
                    channels.IsSuckerDown.SetValue(true);
                }
            }

            if (channels.IsSuckerUp.Value)
                simulateAir();
        }
        private void simulateAir()
        {
            if (channels.SuckOn.Value)
            {
                blowingTimer.Initialize();
                if (suckingTimer.Finished(DateTime.Now))
                    channels.IsVacuum.SetValue(true);
            }
            else if (channels.BlowOn.Value)
            {
                suckingTimer.Initialize();
                if (blowingTimer.Finished(DateTime.Now))
                    channels.IsVacuum.SetValue(false);
            }
        }

        private void unlock()
        {
            //if (!unlockTimer.Running)
            //{
            //    if (channels.IsOldMirror.Value && !channels.IsOldLocked.Value)
            //        return; // already unlocked
            //    else if (!channels.IsOldMirror.Value && !channels.IsLocked.Value)
            //        return; // already unlocked
            //}

            if (unlockTimer.Finished(DateTime.Now))
            {
                if (channels.IsOldMirror.Value)
                    channels.IsOldLocked.SetValue(false);
                else
                    channels.IsLocked.SetValue(false);
                unlockTimer.Initialize();
            }
        }
        private void lockWeak()
        {
            //if (!lockTimer.Running)
            //{
            //    if (channels.IsOldMirror.Value && channels.IsOldLocked.Value)
            //        return; // already locked
            //    else if (!channels.IsOldMirror.Value && channels.IsLocked.Value)
            //        return; // already locked
            //}

            if (lockTimer.Finished(DateTime.Now))
            {
                if (channels.IsOldMirror.Value)
                    channels.IsOldLocked.SetValue(true);
                else
                    channels.IsLocked.SetValue(true);
                lockTimer.Initialize();
            }
        }

        private void setIsFolded()
        {
            channels.IsPowerfoldDown.SetValue(true);
            channels.IsOldPowerfoldDown.SetValue(true);
            channels.IsOldPowerfoldUp.SetValue(false);
            channels.IsPowerfoldUp.SetValue(false);
        }
        private void setIsUnfolded()
        {
            channels.IsPowerfoldDown.SetValue(false);
            channels.IsOldPowerfoldDown.SetValue(false);
            channels.IsOldPowerfoldUp.SetValue(true);
            channels.IsPowerfoldUp.SetValue(true);
        }

        #region Forms Events

        #region Button Clicks

        private void startButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (channels != null)
            {
                lock (channels)
                {
                    channels.IsStartPressed.SetValue(true);
                }
            }
        }
        private void startButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (channels != null)
            {
                lock (channels)
                {
                    channels.IsStartPressed.SetValue(false);
                }
            }
        }

        private void insertMirrorButton_Click(object sender, EventArgs e)
        {
            if (tester1.IsDeviceOpened)
            {
                tester1.IsMirrorInserted = true;
                if (channels == null)
                    return;
                lock (channels)
                {
                    if (channels.IsOldMirror.Value)
                    {
                        channels.IsOldPowerfoldUp.SetValue(true);
                    }
                    else
                    {
                        channels.IsPowerfoldUp.SetValue(true);
                    }
                }
            }
        }
        private void removeMirrorButton_Click(object sender, EventArgs e)
        {
            if (tester1.IsDeviceOpened)
            {
                tester1.IsMirrorInserted = false;
                if (channels == null)
                    return;
                lock (channels)
                {
                    channels.IsPowerfoldUp.SetValue(false);
                }
            }
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
            lockTimer.Initialize();
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
            unlockTimer.Initialize();
        }

        #endregion

        #region Numeric Value Changed

        private void lockTime_ValueChanged(object sender, EventArgs e)
        {
            lockTimer.Interval = (double)lockTime.Value;
        }
        private void unlockTime_ValueChanged(object sender, EventArgs e)
        {
            unlockTimer.Interval = (double)unlockTime.Value;
        }
        private void foldingTime_ValueChanged(object sender, EventArgs e)
        {
            foldTimer.Interval = (double)foldingTime.Value;
        }
        private void unfoldingTime_ValueChanged(object sender, EventArgs e)
        {
            unfoldTimer.Interval = (double)unfoldingTime.Value;
        }

        #endregion

        #region Check/Unceck

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

        private void powerOff_CheckedChanged(object sender, EventArgs e)
        {
            if (channels != null)
            {
                lock (channels)
                {
                    channels.IsPowerSupplyOff.SetValue(powerOff.Checked);
                }
            }
        }

        #endregion

        #endregion


        private void moveMirror_Click(object sender, EventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                lock (channels)
                {
                    string text = b.Text;
                    switch (text)
                    {
                        case "UP":
                            channels.DistanceX.SetValue(channels.DistanceX.Value + (uint)movingSpeed.Value);
                            break;
                        case "DOWN":
                            channels.DistanceX.SetValue(channels.DistanceX.Value - (uint)movingSpeed.Value);
                            break;
                        case "LEFT":
                            channels.DistanceZ.SetValue(channels.DistanceZ.Value + (uint)movingSpeed.Value);
                            break;
                        case "RIGHT":
                            channels.DistanceZ.SetValue(channels.DistanceZ.Value - (uint)movingSpeed.Value);
                            break;
                    }
                }
            }
        }




    }
}
