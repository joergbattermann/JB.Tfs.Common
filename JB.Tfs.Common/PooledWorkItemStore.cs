// <copyright file="PooledWorkItemStore.cs" company="Joerg Battermann">
//     (c) 2012 Joerg Battermann.
//     License: Microsoft Public License (Ms-PL). For details see https://github.com/jbattermann/JB.Tfs.Common/blob/master/LICENSE
// </copyright>
// <author>Joerg Battermann</author>

using System;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JB.Tfs.Common
{
    /// <summary>
    /// A <see cref="T:Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemStore"/> retrieved from the <see cref="T:JB.Tfs.Common.WorkItemStoreConnectionPool"/>
    /// </summary>
    public class PooledWorkItemStore : IDisposable
    {
        private volatile bool _disposing;
        private volatile WorkItemStore _workItemStore;

        /// <summary>
        /// Gets the work item store.
        /// </summary>
        public WorkItemStore WorkItemStore
        {
            get { return _workItemStore; }

        }

        /// <summary>
        /// Determines whether this instance is disposing.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is disposing; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsDisposing()
        {
            return _disposing;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledWorkItemStore"/> class.
        /// </summary>
        /// <param name="workItemStore">The work item store.</param>
        internal PooledWorkItemStore(WorkItemStore workItemStore)
        {
            if (workItemStore == null) throw new ArgumentNullException("workItemStore");
            _workItemStore = workItemStore;
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
            if (_disposing)
                return;
            
            GC.SuppressFinalize(this);
            _disposing = true;

            if (_workItemStore == null)
                return;

            var released = false;
            while (released == false)
            {
                if (WorkItemStoreConnectionPool.Instance == null || WorkItemStoreConnectionPool.Instance.IsDisposing())
                    break;
                
                released = WorkItemStoreConnectionPool.Instance.TryRelease(_workItemStore);
            }
            
            _workItemStore = null;
        }

        #endregion
    }
}
