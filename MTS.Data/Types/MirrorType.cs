using System;
using System.Collections.Generic;

namespace MTS.Data.Types
{
    /// <summary>
    /// Type of mirror orientation
    /// </summary>
    public enum MirrorType : byte
    {
        LeftLeftHanded,
        RightLeftHanded,
        LeftRightHanded,
        RightRightHanded
    }

    public class MirrorTypeClass
    {
        #region Properties

        /// <summary>
        /// (Get/Set) Unique id of mirror type. This value is usually saved to database
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// (Get/Set) Name of mirror type (short description). May be used in combo box etc.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// (Get/Set) Long description of mirror type. May be used in a tooltip for complete description
        /// of current mirror type
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Name of mirror type
        /// </summary>
        /// <returns>Mirror type string (could be used in combo box)</returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of <see name="MirrorType"/> class, which describes particular properties
        /// of mirror
        /// </summary>
        /// <param name="id">Unique id of mirror type. This value is usually saved to database</param>
        /// <param name="name">Name of mirror type (short description). May be used in combo box etc.</param>
        /// <param name="description">Long description of mirror type. May be used in a tooltip for complete description
        /// of current mirror type</param>
        public MirrorTypeClass(int id = 0, string name = "", string description = "")
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
        }

        #endregion
    }
}
