using System;
using System.Collections.Generic;
using System.Xml;

namespace Tfs.Common
{
    public class GlobalList
    {
        private const string GlobalListsListItemIdentifier = "LISTITEM";

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the values.
        /// </summary>
        public IList<string> Values
        {
            get { return _values.AsReadOnly(); }
        }

        /// <summary>
        /// Internal list of values
        /// </summary>
        private readonly List<string> _values = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalList"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="values">The values.</param>
        internal GlobalList(string name, IEnumerable<string> values = null)
        {
            Name = name;

            if (values != null)
                _values.AddRange(values);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalList"/> class.
        /// </summary>
        /// <param name="globalListXmlElement">The XML element.</param>
        internal GlobalList(XmlElement globalListXmlElement)
        {
            if (globalListXmlElement == null) throw new ArgumentNullException("globalListXmlElement");

            Name = globalListXmlElement.Attributes["name"].Value;
            foreach (var listItemValue in GetGlobalListItemValues(globalListXmlElement))
            {
                _values.Add(listItemValue);
            }
        }

        /// <summary>
        /// Gets the global list item values.
        /// </summary>
        /// <param name="globalListXmlElement">The global list XML element.</param>
        /// <returns></returns>
        private IEnumerable<string> GetGlobalListItemValues(XmlElement globalListXmlElement)
        {
            if (globalListXmlElement == null) throw new ArgumentNullException("globalListXmlElement");

            // this can probably be done a tad more elegantly
            foreach (XmlNode element in globalListXmlElement.ChildNodes)
            {
                XmlAttributeCollection xmlAttributeCollection;
                if (element == null || (xmlAttributeCollection = element.Attributes) == null)
                    continue;

                if ((element.LocalName == GlobalListsListItemIdentifier && xmlAttributeCollection["value"] != null))
                    yield return xmlAttributeCollection["value"].Value;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
