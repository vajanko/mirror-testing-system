using System;
using System.Collections.Generic;

namespace MTS.Data
{
    public class MirrorType
    {
        #region Properties

        /// <summary>
        /// (Get/Set) Unic id of mirror type. This value is usually saved to database
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// (Get/Set) Name of mirror type (short description). May be used in combobox etc.
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
        /// <returns>Mirror type string (could be used in combobox)</returns>
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
        /// <param name="id">Unic id of mirror type. This value is usually saved to database</param>
        /// <param name="name">Name of mirror type (short description). May be used in combobox etc.</param>
        /// <param name="description">Long description of mirror type. May be used in a tooltip for complete description
        /// of current mirror type</param>
        public MirrorType(int id = 0, string name = "", string description = "")
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
        }
        public MirrorType()
        {
        }

        #endregion
    }
}
