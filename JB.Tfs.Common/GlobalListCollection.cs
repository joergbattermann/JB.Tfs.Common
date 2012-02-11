// <copyright file="GlobalListCollection.cs" company="Joerg Battermann">
//     (c) 2012 Joerg Battermann.
//     License: Microsoft Public License (Ms-PL). For details see https://github.com/jbattermann/JB.Tfs.Common/blob/master/LICENSE
// </copyright>
// <author>Joerg Battermann</author>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JB.Tfs.Common
{
    public class GlobalListCollection : IGlobalListCollection
    {
        private readonly List<GlobalList> _globalListCollection = new List<GlobalList>();

        private const string ProcessingInstructionData = "version='1.0' encoding='utf-8'";
        private const string ProcessingInstructionTarget = "xml";

        private const string GlobalListsPrefix = "gl";
        private const string GlobalListsIdentifier = "GLOBALLISTS";
        private const string GlobalListsNamespace = "http://schemas.microsoft.com/VisualStudio/2005/workitemtracking/globallists";
        private const string GlobalListIdentifier = "GLOBALLIST";

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalListCollection"/> class.
        /// </summary>
        /// <param name="workItemStore">The WorkItemStore to query.</param>
        public GlobalListCollection(WorkItemStore workItemStore)
        {
            if (workItemStore == null) throw new ArgumentNullException("workItemStore");
            FetchDataFromTeamFoundationServer(workItemStore);
        }

        /// <summary>
        /// Fetches the data from team foundation server.
        /// </summary>
        private void FetchDataFromTeamFoundationServer(WorkItemStore workItemStore)
        {
            if (workItemStore == null) throw new ArgumentNullException("workItemStore");

            // remove previously fetched entries
            _globalListCollection.Clear();

            foreach (var globalListXmlElement in GetGlobalListXmlElements(GetOrCreateGlobalListsXmlDocument(workItemStore)))
            {
                _globalListCollection.Add(new GlobalList(globalListXmlElement));
            }
        }

        /// <summary>
        /// Gets the global list XML elements.
        /// </summary>
        /// <param name="xmlDocument">The XML document.</param>
        /// <returns></returns>
        private IEnumerable<XmlElement> GetGlobalListXmlElements(XmlDocument xmlDocument)
        {
            var globalListsElement = GetGlobalListsXmlElement(xmlDocument);

            if (globalListsElement == null)
                throw new XmlException("GlobalList contains no proper root element.");
            return (from XmlNode element in globalListsElement.ChildNodes
                    where element.LocalName == GlobalListIdentifier
                    select element as XmlElement);
        }

        /// <summary>
        /// Gets the global lists XML element.
        /// </summary>
        /// <param name="xmlDocument">The XML document.</param>
        /// <returns></returns>
        private XmlElement GetGlobalListsXmlElement(XmlDocument xmlDocument)
        {
            if (xmlDocument == null) throw new ArgumentNullException("xmlDocument");

            return (from XmlNode element in xmlDocument.ChildNodes
                    where
                        (element.Prefix == GlobalListsPrefix && element.LocalName == GlobalListsIdentifier &&
                         element.NamespaceURI == GlobalListsNamespace)
                    select element as XmlElement).FirstOrDefault();
        }

        /// <summary>
        /// Gets or creates the global lists document.
        /// </summary>
        /// <returns></returns>
        private XmlDocument GetOrCreateGlobalListsXmlDocument(WorkItemStore workItemStore)
        {
            if (workItemStore == null) throw new ArgumentNullException("workItemStore");

            XmlDocument xmlDocument = workItemStore.ExportGlobalLists();

            if (xmlDocument != null)
                return xmlDocument;

            xmlDocument = new XmlDocument();
            //Define encoding for non english languages
            XmlProcessingInstruction xmlProcessingInstruction = xmlDocument.CreateProcessingInstruction(ProcessingInstructionTarget, ProcessingInstructionData);

            xmlDocument.AppendChild(xmlProcessingInstruction);
            var globalListsRoot = xmlDocument.CreateElement(GlobalListsPrefix, GlobalListsIdentifier, GlobalListsNamespace);
            xmlDocument.AppendChild(globalListsRoot);

            return xmlDocument;
        }

        /// <summary>
        /// Saves the data to the team foundation server.
        /// </summary>
        public void Save(WorkItemStore workItemStore)
        {
            if (workItemStore == null) throw new ArgumentNullException("workItemStore");

            throw new NotImplementedException();
            // workItemStore.ImportGlobalLists(_xmlDocument.InnerXml);
        }

        /// <summary>
        /// Refreshes the data from the team foundation server.
        /// </summary>
        public void Refresh(WorkItemStore workItemStore)
        {
            if (workItemStore == null) throw new ArgumentNullException("workItemStore");
            FetchDataFromTeamFoundationServer(workItemStore);
        }

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<GlobalList> GetEnumerator()
        {
            return _globalListCollection.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
