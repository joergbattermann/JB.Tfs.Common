// <copyright file="WorkItemStoreExtensions.cs" company="Joerg Battermann">
//     (c) 2012 Joerg Battermann.
//     License: Microsoft Public License (Ms-PL). For details see https://github.com/jbattermann/JB.Tfs.Common/blob/master/LICENSE
// </copyright>
// <author>Joerg Battermann</author>

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JB.Tfs.Common
{
    public static class WorkItemStoreExtensions
    {
        /// <summary>
        /// Gets the global lists.
        /// </summary>
        /// <param name="workItemStore">The workItemStore.</param>
        /// <returns></returns>
        public static GlobalListCollection GetGlobalLists(this WorkItemStore workItemStore)
        {
            if (workItemStore == null) throw new ArgumentNullException("workItemStore");
            return new GlobalListCollection(workItemStore);
        }

        /// <summary>
        /// Gets the global list values.
        /// </summary>
        /// <param name="workItemStore">The work item store.</param>
        /// <param name="globalListName">Name of the global list.</param>
        /// <returns></returns>
        public static IEnumerable<string> GetGlobalListValues(this WorkItemStore workItemStore, string globalListName)
        {
            if (workItemStore == null) throw new ArgumentNullException("workItemStore");
            if (globalListName == null) throw new ArgumentNullException("globalListName");
            if (string.IsNullOrWhiteSpace(globalListName)) throw new ArgumentOutOfRangeException("globalListName");

            var globalList =
                workItemStore.GetGlobalLists().FirstOrDefault(
                    list => list.Name.Equals(globalListName, StringComparison.InvariantCultureIgnoreCase));

            if (globalList == null)
                throw new ArgumentOutOfRangeException("globalListName", "A global list with this name does not exist in WorkItemStore");

            return globalList.Values ?? Enumerable.Empty<string>();
        }
    }
}
