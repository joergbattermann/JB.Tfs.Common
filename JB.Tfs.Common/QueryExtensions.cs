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
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static Task<WorkItemCollection> RunQueryAsync(this Query query, CancellationToken cancellationToken = new CancellationToken())
        {
            if (query == null) throw new ArgumentNullException("query");
            return Nito.AsyncEx.TeamFoundationClientAsyncFactory<WorkItemCollection>.FromApm(query.BeginQuery, query.EndQuery, cancellationToken);
        }

        /// <summary>
        /// Runs the count query asynchronously.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static Task<int> RunQueryCountAsync(this Query query, CancellationToken cancellationToken = new CancellationToken())
        {
            if (query == null) throw new ArgumentNullException("query");
            return Nito.AsyncEx.TeamFoundationClientAsyncFactory<int>.FromApm(query.BeginCountOnlyQuery, query.EndCountOnlyQuery, cancellationToken);
        }

        /// <summary>
        /// Runs the link query asynchronously.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static Task<WorkItemLinkInfo[]> RunLinkQueryAsync(this Query query, CancellationToken cancellationToken = new CancellationToken())
        {
            if (query == null) throw new ArgumentNullException("query");
            return Nito.AsyncEx.TeamFoundationClientAsyncFactory<WorkItemLinkInfo[]>.FromApm(query.BeginLinkQuery, query.EndLinkQuery, cancellationToken);
        }
    }
}
