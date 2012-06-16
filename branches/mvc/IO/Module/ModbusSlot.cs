using System;
using System.Collections.Generic;
using MTS.IO.Channel;
using MTS.IO.Address;

namespace MTS.IO.Module
{
    /// <summary>
    /// Base class for all Modbus slots
    /// </summary>
    abstract class ModbusSlot<TAddress> where TAddress : IAddress
    {
        #region Properties

        /// <summary>
        /// (Get) Address of this slot inside Modbus module
        /// </summary>
        public byte Slot { get; protected set; }
        /// <summary>
        /// (Get) Address of first channel inside this slot. (Default zero is usually enaught)
        /// </summary>
        public byte StartChannel { get; protected set; }
        /// <summary>
        /// (Get) Number of channels inside this slot
        /// </summary>
        public byte ChannelsCount { get; protected set; }

        #endregion

        public ChannelBase<TAddress>[] Channels { get; protected set; }

        /// <summary>
        /// Read values of all channels
        /// <param name="hConnnection">Handle of connection from which to read</param>
        /// </summary>
        public abstract void Read(int hConnection);

        /// <summary>
        /// Insert a channel to this slot
        /// </summary>
        /// <param name="channel">Modbus channel to insert</param>
        public void AddChannel(ChannelBase<TAddress> channel)
        {
            // this is the only place where modbus address is used
            ModbusAddress addr = channel.Address as ModbusAddress;
            if (addr != null)
                Channels[addr.Channel - StartChannel] = channel;
        }
        /// <summary>
        /// Get an instance of particular channel identified by its name. Return null if there is no such a channel
        /// </summary>
        /// <param name="name">Unique name (identifier) of required channel</param>
        public ChannelBase<TAddress> GetChannelByName(string name)
        {
            for (int i = 0; i < Channels.Length; i++)
                if (Channels[i] != null && Channels[i].Name == name)       
                    return Channels[i];     // find channel with particular name
            // if this method did not returned in the cycle - there is no such a channel
            return null;
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of Modbus slot
        /// </summary>
        /// <param name="slot">Address of this slot inside Modbus module</param>
        /// <param name="startChannel">Address of first channel inside this slot</param>
        /// <param name="channelsCount">Number of channels inside this slot</param>
        public ModbusSlot(byte slot, byte startChannel, byte channelsCount)
        {
            Slot = slot;
            StartChannel = startChannel;
            ChannelsCount = channelsCount;
        }

        #endregion
    }
}
