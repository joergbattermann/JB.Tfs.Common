// <copyright file="PooledWorkItemStore.cs" company="Joerg Battermann">
//     (c) 2012 Joerg Battermann.
//     License: Microsoft Public License (Ms-PL). For details see https://github.com/jbattermann/JB.Tfs.Common/blob/master/LICENSE
// </copyright>
// <author>Joerg Battermann</author>

using System;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JB.Tfs.Common
{
    using System.Threading;

    /// <summary>
    /// A <see cref="T:Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemStore"/> retrieved from the <see cref="T:JB.Tfs.Common.WorkItemStoreConnectionPool"/>
    /// </summary>
    public sealed class PooledWorkItemStore : IPooledWorkItemStore
    {
        private WeakReference _workItemStoreReference;
        private WeakReference _workItemStoreConnectionPoolReference;
        private int _isDisposing;

        /// <summary>
        /// Gets the work item store.
        /// </summary>
        public WorkItemStore WorkItemStore
        {
            get {
                return _workItemStoreReference == null
                           ? null
                           : (_workItemStoreReference.IsAlive ? _workItemStoreReference.Target as WorkItemStore : null);
            }
        }

        /// <summary>
        /// Gets the parent work item store connection pool.
        /// </summary>
        public WorkItemStoreConnectionPool WorkItemStoreConnectionPool
        {
            get
            {
                return _workItemStoreConnectionPoolReference == null
                           ? null
                           : (_workItemStoreConnectionPoolReference.IsAlive ? _workItemStoreConnectionPoolReference.Target as WorkItemStoreConnectionPool : null);
            }
        }

        /// <summary>
        /// Determines whether this instance is disposing.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is disposing; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsDisposing()
        {
            return _isDisposing == 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledWorkItemStore"/> class.
        /// </summary>
        /// <param name="workItemStoreConnectionPool">The work item store connection pool.</param>
        /// <param name="workItemStore">The work item store.</param>
        internal PooledWorkItemStore(WorkItemStoreConnectionPool workItemStoreConnectionPool, WorkItemStore workItemStore)
        {
            if (workItemStoreConnectionPool == null) throw new ArgumentNullException("workItemStoreConnectionPool");
            if (workItemStore == null) throw new ArgumentNullException("workItemStore");

            _workItemStoreConnectionPoolReference = new WeakReference(workItemStoreConnectionPool);
            _workItemStoreReference = new WeakReference(workItemStore);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="PooledWorkItemStore"/> is reclaimed by garbage collection.
        /// </summary>
        ~PooledWorkItemStore()
        {
            Dispose();
        }

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (IsDisposing())
                return;

            Interlocked.Exchange(ref _isDisposing, 1);

            try
            {
                TryRelease();
            }
            finally
            {
                Interlocked.Exchange(ref _workItemStoreReference, null);
                Interlocked.Exchange(ref _workItemStoreConnectionPoolReference, null);
            }
        }

        /// <summary>
        /// Tries to release the underlying WorkItemStore back to pool. Cannot be used safely anymore afterwards.
        /// </summary>
        /// <param name="retryAttempts">The retry attempts.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool TryRelease(int retryAttempts = 0)
        {

            int retryAttempt = 0;
            bool result = false;

            while (result == false && retryAttempt <= retryAttempts)
            {
                if (WorkItemStoreConnectionPool == null || WorkItemStoreConnectionPool.IsDisposing())
                    break;

                result = WorkItemStoreConnectionPool.TryReleaseWorkItemStore(WorkItemStore, retryAttempts);

                retryAttempt++;
            }

            return result;
        }

        #endregion
    }
}
