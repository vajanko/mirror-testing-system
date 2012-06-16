using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Editor
{
    /// <summary>
    /// Class that manage known ids of test parameters.
    /// </summary>
    public static class ParamIds
    {
        #region Parameter

        /// <summary>
        /// Id of minimal angle parameter - minimal angle that must be achieved during the test.
        /// </summary>
        public const string MinAngle = "MinAngle";
        /// <summary>
        /// Id of minimal current parameter - minimal current that must be measured during the test.
        /// </summary>
        public const string MinCurrent = "MinCurrent";
        /// <summary>
        /// Id of maximal current parameter - maximal current that must be measured during the test.
        /// </summary>
        public const string MaxCurrent = "MaxCurrent";
        /// <summary>
        /// Id of maximum time of current overloaded parameter - current can exceed <see cref="MaxCurrent"/>
        /// parameter value but only for specified period of time.
        /// </summary>
        public const string MaxOverloadTime = "MaxOverloadTime";
        /// <summary>
        /// Id of maximum time testing parameter - the test can not be running more than time specified.
        /// </summary>
        public const string MaxTestingTime = "MaxTestingTime";
        /// <summary>
        /// If of test presence parameter - presence of particular mirror component is checked.
        /// </summary>
        public const string TestPresence = "Presence";
        /// <summary>
        /// Id of supplier name parameter - mirror supplier is printed on the label.
        /// </summary>
        public const string SupplierName = "SupplierName";
        /// <summary>
        /// Id of supplier code parameter - code of mirror supplier is printed on the label.
        /// </summary>
        public const string SupplierCode = "SupplierCode";
        /// <summary>
        /// Id of mirror part number parameter.
        /// </summary>
        public const string PartNumber = "PartNumber";
        /// <summary>
        /// Id of mirror description parameter.
        /// </summary>
        public const string DescriptionId = "Description";
        /// <summary>
        /// Id of mirror weight parameter.
        /// </summary>
        public const string Weight = "Weight";
        /// <summary>
        /// Id of mirror orientation parameter.
        /// </summary>
        public const string Orientation = "Orientation";

        /// <summary>
        /// Id of direction light lighting time parameter - time of direction light being on during testing.
        /// </summary>
        public const string LighteningTime = "LightingTime";
        /// <summary>
        /// Id of direction light break time parameter - time of direction light being off during testing.
        /// </summary>
        public const string BreakTime = "BreakTime";
        /// <summary>
        /// Id of direction light test cycles parameter - number of direction light being switched on and
        /// off during its testing = number of <see cref="LighteningTime"/> and <see cref="BreakTime"/>
        /// periods.
        /// </summary>
        public const string BlinkCount = "BlinksCount";
        /// <summary>
        /// Id of testing time parameter - exact time of test duration.
        /// </summary>
        public const string TestingTime = "TestingTime";

        #endregion
    }
}
