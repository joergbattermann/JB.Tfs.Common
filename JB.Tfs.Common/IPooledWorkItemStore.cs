// <copyright file="IPooledWorkItemStore.cs" company="Joerg Battermann">
//     (c) 2012 Joerg Battermann.
//     License: Microsoft Public License (Ms-PL). For details see https://github.com/jbattermann/JB.Tfs.Common/blob/master/LICENSE
// </copyright>
// <author>Joerg Battermann</author>

namespace JB.Tfs.Common
{
    using System;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    public interface IPooledWorkItemStore : IDisposable
    {
        /// <summary>
        /// Gets the work item store.
        /// </summary>
        WorkItemStore WorkItemStore { get; }

        /// <summary>
        /// Gets the parent work item store connection pool.
        /// </summary>
        WorkItemStoreConnectionPool WorkItemStoreConnectionPool { get; }

        /// <summary>
        /// Tries to release the underlying WorkItemStore back to pool. Cannot be used safely anymore afterwards.
        /// </summary>
        /// <param name="retryAttempts">The retry attempts.</param>
        /// <returns>True if successful, otherwise false.</returns>
        bool TryRelease(int retryAttempts = 0);
    }
}