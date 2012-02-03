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
    public class GlobalListCollection : IList<GlobalList>
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
        /// <param name="workItemStore">The work item store.</param>
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

        #region Implementation of ICollection<GlobalList>

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public void Add(GlobalList item)
        {
            if (item == null) throw new ArgumentNullException("item");
            _globalListCollection.Add(item);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        public void Clear()
        {
            _globalListCollection.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public bool Contains(GlobalList item)
        {
            return _globalListCollection.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.-or-Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
        public void CopyTo(GlobalList[] array, int arrayIndex)
        {
            _globalListCollection.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public bool Remove(GlobalList item)
        {
            return _globalListCollection.Remove(item);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count
        {
            get { return _globalListCollection.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return true; } // Writing / Saving not yet supported
        }

        #endregion

        #region Implementation of IList<GlobalList>

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        public int IndexOf(GlobalList item)
        {
            return _globalListCollection.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param><param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        public void Insert(int index, GlobalList item)
        {
            _globalListCollection.Insert(index, item);
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        public void RemoveAt(int index)
        {
            _globalListCollection.RemoveAt(index);
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <param name="index">The zero-based index of the element to get or set.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        public GlobalList this[int index]
        {
            get
            {
                return _globalListCollection[index];
            }
            set
            {
                _globalListCollection[index] = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="GlobalList">GlobalList</see>
        ///   with the specified name.
        /// </summary>
        public GlobalList this[string name]
        {
            get
            {
                return _globalListCollection.FirstOrDefault(globalList => globalList.Name.Equals(name));
            }
        }

        #endregion
    }
}
