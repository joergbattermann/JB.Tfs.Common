// <copyright file="WorkItemStoreExtensions.cs" company="Joerg Battermann">
//     (c) 2012 Joerg Battermann.
//     License: Microsoft Public License (Ms-PL). For details see https://github.com/jbattermann/JB.Tfs.Common/blob/master/LICENSE
// </copyright>
// <author>Joerg Battermann</author>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JB.Tfs.Common
{
    /// <summary>
    /// Extension Methods for <see cref="T:Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemStore"/>
    /// </summary>
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
        /// <param name="workItemStore">The WorkItemStore to query.</param>
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

        /// <summary>
        /// Performs the Query asynchronously.
        /// </summary>
        /// <param name="workItemStore">The WorkItemStore to query.</param>
        /// <param name="wiql">The query to execute.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static Task<WorkItemCollection> QueryAsync(this WorkItemStore workItemStore, string wiql, CancellationToken cancellationToken = new CancellationToken())
        {
            if (workItemStore == null) throw new ArgumentNullException("workItemStore");
            return new Query(workItemStore, wiql).RunQueryAsync(cancellationToken);
        }

        /// <summary>
        /// Performs the Query asynchronously.
        /// </summary>
        /// <param name="workItemStore">The WorkItemStore to query.</param>
        /// <param name="wiql">The query string to execute.</param>
        /// <param name="batchReadParameters">A collection of WorkItem ID and revision pairs.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static Task<WorkItemCollection> QueryAsync(this WorkItemStore workItemStore, string wiql, BatchReadParameterCollection batchReadParameters, CancellationToken cancellationToken = new CancellationToken())
        {
            if (workItemStore == null) throw new ArgumentNullException("workItemStore");
            return new Query(workItemStore, wiql, batchReadParameters).RunQueryAsync(cancellationToken);
        }

        /// <summary>
        /// Performs the Query asynchronously.
        /// </summary>
        /// <param name="workItemStore">The WorkItemStore to query.</param>
        /// <param name="wiql">The query string to execute.</param>
        /// <param name="ids">An array of WorkItem IDs.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static Task<WorkItemCollection> QueryAsync(this WorkItemStore workItemStore, string wiql, int[] ids, CancellationToken cancellationToken = new CancellationToken())
        {
            if (workItemStore == null) throw new ArgumentNullException("workItemStore");
            return new Query(workItemStore, wiql, ids).RunQueryAsync(cancellationToken);
        }

        /// <summary>
        /// Performs the Query asynchronously.
        /// </summary>
        /// <param name="workItemStore">The WorkItemStore to query.</param>
        /// <param name="wiql">The query string to execute.</param>
        /// <param name="ids">An array of WorkItem IDs.</param>
        /// <param name="revs">An array of WorkItem revisions.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static Task<WorkItemCollection> QueryAsync(this WorkItemStore workItemStore, string wiql, int[] ids, int[] revs, CancellationToken cancellationToken = new CancellationToken())
        {
            if (workItemStore == null) throw new ArgumentNullException("workItemStore");
            return new Query(workItemStore, wiql, ids, revs).RunQueryAsync(cancellationToken);
        }

        /// <summary>
        /// Performs the Query asynchronously.
        /// </summary>
        /// <param name="workItemStore">The WorkItemStore to query.</param>
        /// <param name="wiql">The query to execute.</param>
        /// <param name="context">A dictionary of macros and values.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static Task<WorkItemCollection> QueryAsync(this WorkItemStore workItemStore, string wiql, IDictionary context, CancellationToken cancellationToken = new CancellationToken())
        {
            if (workItemStore == null) throw new ArgumentNullException("workItemStore");
            return new Query(workItemStore, wiql, context).RunQueryAsync(cancellationToken);
        }

        /// <summary>
        /// Performs the Query asynchronously.
        /// </summary>
        /// <param name="workItemStore">The WorkItemStore to query.</param>
        /// <param name="wiql">The query to execute.</param>
        /// <param name="context">A dictionary of macros and values.</param>
        /// <param name="dayPrevision">True to ignore time values so that DateTime objects are treated as dates; otherwise, false.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static Task<WorkItemCollection> QueryAsync(this WorkItemStore workItemStore, string wiql, IDictionary context, bool dayPrevision, CancellationToken cancellationToken = new CancellationToken())
        {
            if (workItemStore == null) throw new ArgumentNullException("workItemStore");
            return new Query(workItemStore, wiql, context, dayPrevision).RunQueryAsync(cancellationToken);
        }

        /// <summary>
        /// Performs the Count Query asynchronously.
        /// </summary>
        /// <param name="workItemStore">The WorkItemStore to query.</param>
        /// <param name="wiql">The query to execute.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static Task<int> QueryCountAsync(this WorkItemStore workItemStore, string wiql, CancellationToken cancellationToken = new CancellationToken())
        {
            if (workItemStore == null) throw new ArgumentNullException("workItemStore");
            return new Query(workItemStore, wiql).RunQueryCountAsync(cancellationToken);
        }

        /// <summary>
        /// Performs the Count Query asynchronously.
        /// </summary>
        /// <param name="workItemStore">The WorkItemStore to query.</param>
        /// <param name="wiql">The query to execute.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static Task<int> QueryCountAsync(this WorkItemStore workItemStore, string wiql, IDictionary context, CancellationToken cancellationToken = new CancellationToken())
        {
            if (workItemStore == null) throw new ArgumentNullException("workItemStore");
            return new Query(workItemStore, wiql, context).RunQueryCountAsync(cancellationToken);
        }
    }
}
