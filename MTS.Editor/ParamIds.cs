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

        public const string MinAngle = "MinAngle";
        public const string MinCurrent = "MinCurrent";
        public const string MaxCurrent = "MaxCurrent";
        public const string MaxOverloadTime = "MaxOverloadTime";
        public const string MaxTestingTime = "MaxTestingTime";
        public const string TestPresence = "Presence";

        public const string SupplierName = "SupplierName";
        public const string SupplierCode = "SupplierCode";
        public const string PartNumber = "PartNumber";
        public const string DescriptionId = "Description";
        public const string Weight = "Weight";
        public const string Orientation = "Orientation";

        // direction light
        public const string LighteningTime = "LightingTime";
        public const string BreakTime = "BreakTime";
        public const string BlinkCount = "BlinksCount";

        public const string TestingTime = "TestingTime";

        #endregion
    }
}
