using System;

namespace MTS.AdminModule
{
    /// <summary>
    /// Abstract layer for accessing particular remote hardware module
    /// Module contains several channels digital or analog, input or output
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Load configuration of channels form file
        /// </summary>
        /// <param name="filename">Path to file where configuration of channels is stored</param>
        void LoadConfiguration(string filename);

         /// <summary>
        /// Create a new connection between local computer and some hardware component. At the beginning of
        /// the communication this method must be called.
        /// </summary>
        void Connect();

        /// <summary>
        /// Prepare (initialize) channels for reading and writing. When this method is called, connection
        /// must be established already.
        /// </summary>
        void Initialize();

        /// <summary>
        /// For debug purpose only
        /// </summary>
        /// <param name="time">Time of calling this method</param>
        void Update(TimeSpan time);

        /// <summary>
        /// Read all input and write all output channels
        /// </summary>
        void Update();

        /// <summary>
        /// Read all inputs and outputs channels
        /// </summary>
        void UpdateInputs();

        /// <summary>
        /// Write all outputs channels
        /// </summary>
        void UpdateOutputs();

        /// <summary>
        /// Close connection between local computer and some hardware component. Call this method at the end of
        /// the communication to release resources.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Get an instance of paricular channel identified by its name. Return null if ther is no such a channel
        /// </summary>
        /// <param name="name">Unic name (identifier) of required channel</param>
        IChannel GetChannelByName(string name);

        //#region Channels

        //#region Digital inputs

        //IDigitalInput PowerFoldUnfoldedPositionSensor1 { get; set; }
        //IDigitalInput PowerFoldUnfoldedPositionSensor2 { get; set; }
        //IDigitalInput PowerFoldFoldedPositionSensor { get; set; }
        //IDigitalInput TestingDeviceOpened { get; set; }
        //IDigitalInput TestingDeviceClosed { get; set; }
        //IDigitalInput StartButton { get; set; }
        //IDigitalInput ErrorAcknButton { get; set; }

        //IDigitalInput ControlCurrentOn { get; set; }

        //IDigitalInput SensorHeadOut { get; set; }
        //IDigitalInput SensorHeadIn { get; set; }
        //IDigitalInput InsCheck1 { get; set; }
        //IDigitalInput InsCheck2 { get; set; }
        //IDigitalInput InsCheck3 { get; set; }
        //IDigitalInput InsCheck4 { get; set; }
        //IDigitalInput InsCheck5 { get; set; }
        //IDigitalInput InsCheck6 { get; set; }
        //IDigitalInput InsCheck7 { get; set; }
        //IDigitalInput InsCheck8 { get; set; }
        //IDigitalInput InsCheck9 { get; set; }
        //IDigitalInput InsCheck10 { get; set; }
        //IDigitalInput InsCheck11 { get; set; }
        //IDigitalInput InsCheck12 { get; set; }
        //IDigitalInput InsCheck13 { get; set; }
        //IDigitalInput InsCheck14 { get; set; }
        //IDigitalInput InsCheck15 { get; set; }
        //IDigitalInput InsCheck16 { get; set; }
       
        //#endregion

        //#region Digital outputs

        //IDigitalOutput ALLOW_MIRROR_MOVEMENT { get; set; }
        //IDigitalOutput CONTROL_CURRENT_ALLOW { get; set; }
        //IDigitalOutput MOVE_HORIZONTAL { get; set; }
        //IDigitalOutput MOVE_VERTICAL { get; set; }
        //IDigitalOutput MOVE_REVERSE { get; set; }

        //IDigitalOutput PF_FOLD { get; set; }
        //IDigitalOutput PF_UNFOLD { get; set; }
        //IDigitalOutput OPEN_TESTING_DEVICE { get; set; }
        //IDigitalOutput CLOSE_TESTING_DEVICE { get; set; }

        //IDigitalOutput HEATING_FOIL_ON { get; set; }
        //IDigitalOutput DIRECTION_LIGHT_ON { get; set; }

        //IDigitalOutput SG_GREEN { get; set; }
        //IDigitalOutput SG_RED { get; set; }
        //IDigitalOutput SG_AUDIO { get; set; }

        //IDigitalOutput MARKER_LEFT { get; set; }
        //IDigitalOutput MARKER_RIGHT { get; set; }
        //IDigitalOutput MOVE_SENSOR_UP { get; set; }
        //IDigitalOutput MOVE_SENSOR_DOWN { get; set; }
        //IDigitalOutput MOVE_SUPPORT_IN { get; set; }
        //IDigitalOutput MOVE_SUPPORT_UP { get; set; }
        //IDigitalOutput MOVE_SUPPORT_DOWN { get; set; }

        //#endregion

        //#region Analog inputs

        //IAnalogInput DX { get; set; }
        //IAnalogInput DY { get; set; }
        //IAnalogInput DZ { get; set; }

        //IAnalogInput VerticalActuatorCurrent { get; set; }
        //IAnalogInput HorizontalActuatorCurrent { get; set; }
        //IAnalogInput HeatingFoilCurrent { get; set; }
        //IAnalogInput PoweFoldCurrent { get; set; }
        //IAnalogInput DirectionLightCurrent { get; set; }
        //IAnalogInput ActuatorPowerSupplyVoltage { get; set; }
        //IAnalogInput OtherPowerSupplyVoltage { get; set; }

        //#endregion

        //#endregion
    }
}
