using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MTS.IO
{
    /// <summary>
    /// Allows to transform object to xml format and back similar to xml serialization
    /// </summary>
    public interface IXmlFormatable
    {
        /// <summary>
        /// Save object to xml format
        /// </summary>
        /// <returns>Instance of element in xml format</returns>
        XElement ToXml();

        /// <summary>
        /// Initialize object from given xml format
        /// </summary>
        /// <param name="data">Xml data to be initialized from</param>
        void FromXml(XElement data); 
    }
}
