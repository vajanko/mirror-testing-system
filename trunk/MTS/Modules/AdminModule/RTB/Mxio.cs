using System;
using System.Runtime.InteropServices;

namespace MTS.Modules.TesterModule
{
    static class Mxio
    {
        /**********************************************/
        /*                                            */
        /*     Ethernet Module Connect Command        */
        /*                                            */
        /**********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int MXEIO_Init();

        [DllImport("MXIO_NET.dll")]
        public static extern void MXEIO_Exit();

        [DllImport("MXIO_NET.dll")]
        public static extern int MXEIO_Connect(byte[] szIP, UInt16 port, UInt32 timeOut, ref Int32 connection);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXEIO_Disconnect(Int32 connection);

        [DllImport("MXIO_NET.dll")]
        public static extern int MXEIO_CheckConnection(Int32 connection, UInt32 timeOut, ref byte status);

        /***********************************************/
        /*                                             */
        /*           Digital Input Command             */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int DI_Reads(Int32 connection, byte slot, byte startChannel, byte count, ref UInt32 value);

        [DllImport("MXIO_NET.dll")]
        public static extern int DI_Read(Int32 connection, byte slot, byte channel, ref byte value);

        /***********************************************/
        /*                                             */
        /*          Digital Output Command             */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int DO_Reads(Int32 connection, byte slot, byte startChannel, byte count, ref UInt32 value);

        [DllImport("MXIO_NET.dll")]
        public static extern int DO_Writes(Int32 connection, byte slot, byte startChannel, byte bytCount, UInt32 value);

        [DllImport("MXIO_NET.dll")]
        public static extern int DO_Read(Int32 connection, byte slot, byte channel, ref byte value);

        [DllImport("MXIO_NET.dll")]
        public static extern int DO_Write(Int32 connection, byte slot, byte channel, byte value);

        //[DllImport("MXIO_NET.dll")]
        //public static extern int DO_GetSafeValues(Int32 connection, byte slot, byte startChannel, byte count, ref UInt32 value);

        //[DllImport("MXIO_NET.dll")]
        //public static extern int DO_SetSafeValues(Int32 connection, byte slot, byte startChannel, byte count, UInt32 value);

        //[DllImport("MXIO_NET.dll")]
        //public static extern int DO_GetSafeValue(Int32 connection, byte slot, byte channel, ref byte value);

        //[DllImport("MXIO_NET.dll")]
        //public static extern int DO_SetSafeValue(Int32 connection, byte slot, byte channel, byte value);

        //[DllImport("MXIO_NET.dll")]
        //public static extern int DO_GetSafeValues_W(Int32 connection, byte slot, byte startChannel, byte count, UInt16[] values);

        //[DllImport("MXIO_NET.dll")]
        //public static extern int DO_SetSafeValues_W(Int32 connection, byte slot, byte startChannel, byte count, UInt16[] values);

        /***********************************************/
        /*                                             */
        /*           Analog Input Command              */
        /*                                             */
        /***********************************************/
        [DllImport("MXIO_NET.dll")]
        public static extern int AI_Reads(Int32 connection, byte slot, byte startChannel, byte count, double[] value);

        [DllImport("MXIO_NET.dll")]
        public static extern int AI_Read(Int32 connection, byte slot, byte channel, ref double value);

        [DllImport("MXIO_NET.dll")]
        public static extern int AI_ReadRaws(Int32 connection, byte slot, byte startChannel, byte count, UInt16[] value);

        [DllImport("MXIO_NET.dll")]
        public static extern int AI_ReadRaw(Int32 connection, byte slot, byte channel, ref UInt16 value);



        /*************************************************/
        /*                                               */
        /*                  Error Code                   */
        /*                                               */
        /*************************************************/
        public const int MXIO_OK = 0;

        public const int ILLEGAL_FUNCTION = 1001;
        public const int ILLEGAL_DATA_ADDRESS = 1002;
        public const int ILLEGAL_DATA_VALUE = 1003;
        public const int SLAVE_DEVICE_FAILURE = 1004;
        public const int SLAVE_DEVICE_BUSY = 1006;

        public const int EIO_TIME_OUT = 2001;
        public const int EIO_INIT_SOCKETS_FAIL = 2002;
        public const int EIO_CREATING_SOCKET_ERROR = 2003;
        public const int EIO_RESPONSE_BAD = 2004;
        public const int EIO_SOCKET_DISCONNECT = 2005;
        public const int PROTOCOL_TYPE_ERROR = 2006;
        public const int EIO_PASSWORD_INCORRECT = 2007;

        public const int SIO_OPEN_FAIL = 3001;
        public const int SIO_TIME_OUT = 3002;
        public const int SIO_CLOSE_FAIL = 3003;
        public const int SIO_PURGE_COMM_FAIL = 3004;
        public const int SIO_FLUSH_FILE_BUFFERS_FAIL = 3005;
        public const int SIO_GET_COMM_STATE_FAIL = 3006;
        public const int SIO_SET_COMM_STATE_FAIL = 3007;
        public const int SIO_SETUP_COMM_FAIL = 3008;
        public const int SIO_SET_COMM_TIME_OUT_FAIL = 3009;
        public const int SIO_CLEAR_COMM_FAIL = 3010;
        public const int SIO_RESPONSE_BAD = 3011;
        public const int SIO_TRANSMISSION_MODE_ERROR = 3012;
        public const int SIO_BAUDRATE_NOT_SUPPORT = 3013;

        public const int PRODUCT_NOT_SUPPORT = 4001;
        public const int HANDLE_ERROR = 4002;
        public const int SLOT_OUT_OF_RANGE = 4003;
        public const int CHANNEL_OUT_OF_RANGE = 4004;
        public const int COIL_TYPE_ERROR = 4005;
        public const int REGISTER_TYPE_ERROR = 4006;
        public const int FUNCTION_NOT_SUPPORT = 4007;
        public const int OUTPUT_VALUE_OUT_OF_RANGE = 4008;
        public const int INPUT_VALUE_OUT_OF_RANGE = 4009;

        /*************************************************/
        /*                                               */
        /*              Data Format Setting              */
        /*                                               */
        /*************************************************/
        /* Data length define	*/
        public const int BIT_5 = 0x00;
        public const int BIT_6 = 0x01;
        public const int BIT_7 = 0x02;
        public const int BIT_8 = 0x03;

        /* Stop bits define	*/
        public const int STOP_1 = 0x00;
        public const int STOP_2 = 0x04;

        /* Parity define	*/
        public const int P_EVEN = 0x18;
        public const int P_ODD = 0x08;
        public const int P_SPC = 0x38;
        public const int P_MRK = 0x28;
        public const int P_NONE = 0x00;

        /*************************************************/
        /*                                               */
        /*        Modbus Transmission Mode Setting       */
        /*                                               */
        /*************************************************/
        public const int MODBUS_RTU = 0x0;
        public const int MODBUS_ASCII = 0x1;

        /*************************************************/
        /*                                               */
        /*            Check Connection Status            */
        /*                                               */
        /*************************************************/
        public const int CHECK_CONNECTION_OK = 0x0;
        public const int CHECK_CONNECTION_FAIL = 0x1;
        public const int CHECK_CONNECTION_TIME_OUT = 0x2;

        /*************************************************/
        /*                                               */
        /*            Modbus Coil Type Setting           */
        /*                                               */
        /*************************************************/
        public const int COIL_INPUT = 0x2;
        public const int COIL_OUTPUT = 0x1;

        /*************************************************/
        /*                                               */
        /*            Modbus Coil Type Setting           */
        /*                                               */
        /*************************************************/
        public const int REGISTER_INPUT = 0x4;
        public const int REGISTER_OUTPUT = 0x3;

        /*************************************************/
        /*                                               */
        /*             ioLogik 4000 Bus Status           */
        /*                                               */
        /*************************************************/
        public const int NORMAL_OPERATION = 0x0;
        public const int BUS_STANDYBY = 0x1;
        public const int BUS_COMMUNICATION_FAULT = 0x2;
        public const int SLOT_CONFIGURATION_FAILED = 0x3;
        public const int NO_EXPANSION_SLOT = 0x4;

        /*************************************************/
        /*                                               */
        /*        ioLogik 4000 Field Power Status        */
        /*                                               */
        /*************************************************/
        public const int FIELD_POWER_ON = 0x0;
        public const int FIELD_POWER_OFF = 0x1;

        /*************************************************/
        /*                                               */
        /*         ioLogik 4000 Watchdog Status          */
        /*                                               */
        /*************************************************/
        public const int WATCHDOG_NO_ERROR = 0x0;
        public const int WATCHDOG_ERROR = 0x1;

        /*************************************************/
        /*                                               */
        /*   ioLogik 4000 Modbus Error Setup Status      */
        /*                                               */
        /*************************************************/
        public const int SETUP_NO_ERROR = 0x0;
        public const int SETUP_ERROR = 0x1;

        /*************************************************/
        /*                                               */
        /*   ioLogik 4000 Modbus Error Check Status      */
        /*                                               */
        /*************************************************/
        public const int CHECKSUM_NO_ERROR = 0x0;
        public const int CHECKSUM_ERROR = 0x1;

        /*************************************************/
        /*                                               */
        /*         ioLogik 4000 AO Safe Action           */
        /*                                               */
        /*************************************************/
        public const int SAFE_VALUE = 0x00;		//Suport AO & DO module
        public const int HOLD_LAST_STATE = 0x01;//Suport AO & DO module
        public const int LOW_LIMIT = 0x02;		//Only suport AO module
        public const int HIGH_LIMIT = 0x03;		//Only suport AO module

        /*************************************************/
        /*                                               */
        /*               Protocol Type                   */
        /*                                               */
        /*************************************************/
        public const int PROTOCOL_TCP = 0x01;
        public const int PROTOCOL_UDP = 0x02;
        /*************************************************/
        /*                                               */
        /*               AI Channel Type                 */
        /*                                               */
        /*************************************************/
        public const int M2K_AI_RANGE_150mv = 0;
        public const int M2K_AI_RANGE_500mv = 1;
        public const int M2K_AI_RANGE_5V = 2;
        public const int M2K_AI_RANGE_10V = 3;
        public const int M2K_AI_RANGE_0_20mA = 4;
        public const int M2K_AI_RANGE_4_20mA = 5;
        public const int M2K_AI_RANGE_0_150mv = 6;
        public const int M2K_AI_RANGE_0_500mv = 7;
        public const int M2K_AI_RANGE_0_5V = 8;
        public const int M2K_AI_RANGE_0_10V = 9;
    }
}
