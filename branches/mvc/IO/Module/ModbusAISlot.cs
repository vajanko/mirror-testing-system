using System;
using System.Collections.Generic;
using MTS.IO.Channel;

namespace MTS.IO.Module
{
    class ModbusAISlot<TAddress> : ModbusSlot<TAddress> where TAddress:IAddress
    {
        /// <summary>
        /// Array of integer values for reading analog inputs. Item with zero index is value of
        /// StartChannel and so on ...
        /// </summary>
        ushort[] inputs;
        double[] dInputs;

        /// <summary>
        /// Read values of all channels
        /// <param name="hConnnection">Handle of connection from which to read</param>
        /// </summary>
        public override void Read(int hConnection)
        {
            AnalogInput<TAddress> channel;
            // read values from hardware channels to integer array

            // maximum 4 values can read
            // !!! NOT VERY EFFECTIVE !!!

            //Mxio.AI_ReadRaws(hConnection, Slot, StartChannel, 4, inputs);            
            for (int i = 0; i < ChannelsCount; i++)
                Mxio.AI_ReadRaw(hConnection, Slot, (byte)(i + StartChannel), ref inputs[i]);

            // copy values to channels
            for (int i = 0; i < ChannelsCount; i++)
            {   // some of channels may be unused
                channel = Channels[i] as AnalogInput<TAddress>;
                if (channel != null)
                    channel.SetValue(inputs[i]);    // for unused channel value of inputs[i] is unspecified
            }
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of Modbus digital input slot
        /// </summary>
        /// <param name="slot">Address of this slot inside Modbus module</param>
        /// <param name="startChannel">Address of first channel inside this slot</param>
        /// <param name="channelsCount">Number of channels inside this slot</param>
        public ModbusAISlot(byte slot, byte startChannel, byte channelsCount)
            : base(slot, startChannel, channelsCount)
        {
            // allocate as much memory as necessary
            Channels = new AnalogInput<TAddress>[channelsCount];
            inputs = new ushort[channelsCount];
            dInputs = new double[channelsCount];
        }

        #endregion
    }
}
