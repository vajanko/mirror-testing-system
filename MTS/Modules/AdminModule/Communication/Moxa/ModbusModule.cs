using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.AdminModule
{
    class ModbusModule : IModule
    {
        public ushort Port { get; set; }
        public string IpAddress { get; set; }
        private uint timeout = 5000;

        private int hConnection;

        private List<ModbusChannel> inputs = new List<ModbusChannel>();

        private List<ModbusChannel> outputs = new List<ModbusChannel>();

        #region IModule Members

        public void LoadConfiguration(string filename)
        {
            throw new NotImplementedException();
        }

        public void Connect()
        {
            Mxio.MXEIO_Init();

            Mxio.MXEIO_Connect(Encoding.UTF8.GetBytes(IpAddress), Port, timeout, ref hConnection);
        }

        public void Initialize()
        {
            // do nothing
        }

        public void Update(TimeSpan time)
        {   // for debug pupose only
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public void UpdateInputs()
        {
            throw new NotImplementedException();
        }

        public void UpdateOutputs()
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            Mxio.MXEIO_Disconnect(hConnection);
            Mxio.MXEIO_Exit();
        }

        public IChannel GetChannelByName(string name)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
