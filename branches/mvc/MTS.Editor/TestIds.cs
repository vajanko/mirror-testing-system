using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Editor
{
    /// <summary>
    /// Class that manages known test ids.
    /// </summary>
    public static class TestIds
    {
        #region Test

        /// <summary>
        /// Id of test that holds basic information about the entire test collection.
        /// </summary>
        public const string Info = "Info";
        /// <summary>
        /// Id of test that moves mirror glass east.
        /// </summary>
        public const string TravelEast = "TravelEast";
        /// <summary>
        /// Id of test that moves mirror glass west.
        /// </summary>
        public const string TravelWest = "TravelWest";
        /// <summary>
        /// Id of test that moves mirror glass south.
        /// </summary>
        public const string TravelSouth = "TravelSouth";
        /// <summary>
        /// Id of test that moves mirror glass north.
        /// </summary>
        public const string TravelNorth = "TravelNorth";
        /// <summary>
        /// Id of test that checks power-fold electrical circuits.
        /// </summary>
        public const string Powerfold = "Powerfold";
        /// <summary>
        /// Id of test that checks direction light electrical circuits.
        /// </summary>
        public const string DirectionLight = "DirectionLight";
        /// <summary>
        /// Id of test that checks heating foil electrical circuits.
        /// </summary>
        public const string Heating = "HeatingFoil";
        /// <summary>
        /// Id of test that tries to pull off the mirror glass and prove its quality.
        /// </summary>
        public const string Pulloff = "Pulloff";
        /// <summary>
        /// Id of test that checks for the presence of rubber on mirror cable.
        /// </summary>
        public const string Rubber = "Rubber";

        #endregion
    }
}
