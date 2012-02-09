// <copyright file="QueryExtensions.cs" company="Joerg Battermann">
//     (c) 2012 Joerg Battermann.
//     License: Microsoft Public License (Ms-PL). For details see https://github.com/jbattermann/JB.Tfs.Common/blob/master/LICENSE
// </copyright>
// <author>Joerg Battermann</author>

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JB.Tfs.Common
{
    /// <summary>
    /// Extension Methods for <see cref="T:Microsoft.TeamFoundation.WorkItemTracking.Client.Query"/>
    /// </summary>
    public static class QueryExtensions
    {
        /// <summary>
        /// Runs the query asynchronously.
        /// </summary>
        /// <param name="query">The <see cref="T:Microsoft.TeamFoundation.WorkItemTracking.Client.Query"/> to execute.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static Task<WorkItemCollection> RunQueryAsync(this Query query, CancellationToken cancellationToken = new CancellationToken())
        {
            if (query == null) throw new ArgumentNullException("query");
            return TfsTaskFactory<WorkItemCollection>.FromAsync(query.BeginQuery, query.EndQuery, cancellationToken);
        }

        /// <summary>
        /// Runs the count query asynchronously.
        /// </summary>
        /// <param name="query">The <see cref="T:Microsoft.TeamFoundation.WorkItemTracking.Client.Query"/> to execute.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static Task<int> RunQueryCountAsync(this Query query, CancellationToken cancellationToken = new CancellationToken())
        {
            if (query == null) throw new ArgumentNullException("query");
            return TfsTaskFactory<int>.FromAsync(query.BeginCountOnlyQuery, query.EndCountOnlyQuery, cancellationToken);
        }

        /// <summary>
        /// Runs the link query asynchronously.
        /// </summary>
        /// <param name="query">The <see cref="T:Microsoft.TeamFoundation.WorkItemTracking.Client.Query"/> to execute.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static Task<WorkItemLinkInfo[]> RunLinkQueryAsync(this Query query, CancellationToken cancellationToken = new CancellationToken())
        {
            if (query == null) throw new ArgumentNullException("query");
            return TfsTaskFactory<WorkItemLinkInfo[]>.FromAsync(query.BeginLinkQuery, query.EndLinkQuery, cancellationToken);
        }
    }
}
