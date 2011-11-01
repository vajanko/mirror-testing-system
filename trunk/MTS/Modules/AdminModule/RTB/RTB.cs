using System;
using System.Collections.Generic;

namespace MTS.Modules.TesterModule
{
    abstract class RTB
    {
        protected byte slot;
        protected byte channels;

        public abstract int Read(Int32 connection);

        public RTB(byte slot, byte channels)
        {
            this.slot = slot;
            this.channels = channels;
        }
    }

    class DigitalInput : RTB
    {
        protected UInt32 data;
        public UInt32 Data { get { return data; } }
        public bool this[int i]
        {
            get { return (data & (1 << i)) != 0; }
        }

        public override int Read(Int32 connection)
        {
            return Mxio.DI_Reads(connection, slot, 0, channels, ref data);
        }

        public DigitalInput(byte slot, byte channels) : base(slot, channels) { }
    }

    class DigitalOutput : DigitalInput
    {
        public int Write(Int32 connnection, UInt32 value)
        {
            int ret = Mxio.DO_Writes(connnection, slot, 0, channels, value);
            if (ret == Mxio.MXIO_OK)
                data = value;
            return ret;
        }

        public DigitalOutput(byte slot, byte channels) : base(slot, channels) { }
    }

    class AnalogInput : RTB
    {
        protected UInt16[] data;
        public UInt16 this[int i]
        {
            get { return data[i]; }
        }

        public override int Read(Int32 connection)
        {
            return Mxio.AI_ReadRaws(connection, slot, 0, channels, data);
        }

        public AnalogInput(byte slot, byte channels)
            : base(slot, channels)
        {
            data = new UInt16[channels];
        }
    }
}
