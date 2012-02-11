// <copyright file="IGlobalList.cs" company="Joerg Battermann">
//     (c) 2012 Joerg Battermann.
//     License: Microsoft Public License (Ms-PL). For details see https://github.com/jbattermann/JB.Tfs.Common/blob/master/LICENSE
// </copyright>
// <author>Joerg Battermann</author>

namespace JB.Tfs.Common
{
    using System.Collections.Generic;
    using System.Xml;

    public interface IGlobalList
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the values.
        /// </summary>
        IEnumerable<string> Values { get; }

        /// <summary>
        /// Gets the global list item values.
        /// </summary>
        /// <param name="globalListXmlElement">The global list XML element.</param>
        /// <returns></returns>
        IEnumerable<string> GetGlobalListItemValues(XmlElement globalListXmlElement);
    }
}