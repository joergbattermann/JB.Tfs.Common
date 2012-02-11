// <copyright file="WorkItemStoreConnectionPool.cs" company="Joerg Battermann">
//     (c) 2012 Joerg Battermann.
//     License: Microsoft Public License (Ms-PL). For details see https://github.com/jbattermann/JB.Tfs.Common/blob/master/LICENSE
// </copyright>
// <author>Joerg Battermann</author>

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace JB.Tfs.Common
{
    /// <summary>
    /// A pool of WorkItemStore connections
    /// </summary>
    public sealed class WorkItemStoreConnectionPool : IWorkItemStoreConnectionPool
    {
        private ConcurrentDictionary<WorkItemStore, bool> _workItemStores;
        private TfsTeamProjectCollection _tfsTeamProjectCollection;
        private readonly object _poolLockerObject = new object();
        private int _isDisposing;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemStoreConnectionPool"/> class.
        /// </summary>
        /// <param name="tfsTeamProjectCollection">The TFS team project collection.</param>
        /// <param name="amountOfPooledConnections">The amount of pooled connections.</param>
        public WorkItemStoreConnectionPool(TfsTeamProjectCollection tfsTeamProjectCollection, int amountOfPooledConnections = 2)
        {
            if (tfsTeamProjectCollection == null) throw new ArgumentNullException("tfsTeamProjectCollection");

            if (amountOfPooledConnections < 1)
                throw new ArgumentOutOfRangeException("amountOfPooledConnections", "The minimum amount of concurrent connections must be 1 or higher");

            lock (_poolLockerObject)
            {
                _tfsTeamProjectCollection = tfsTeamProjectCollection;
                _workItemStores = new ConcurrentDictionary<WorkItemStore,bool>();
                for (var i = 0; i < amountOfPooledConnections; i++)
                {
                    _workItemStores.TryAdd(new WorkItemStore(tfsTeamProjectCollection),false);
                }
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
        /// Increases the size of the pool.
        /// </summary>
        /// <param name="increaseBy">The amount of stores to increase the pool by.</param>
        public void IncreasePoolSize(int increaseBy = 1)
        {
            if (increaseBy < 1)
                throw new ArgumentOutOfRangeException("increaseBy", "The minimum amount of new concurrent connections must be 1 or higher");

            lock (_poolLockerObject)
            {
                _workItemStores.TryAdd(new WorkItemStore(_tfsTeamProjectCollection), false);
            }
        }

        /// <summary>
        /// Decreases the size of the pool.
        /// </summary>
        /// <param name="decreaseBy">The amount of stores to decrease the pool by.</param>
        public void DecreasePoolSize(int decreaseBy = 1)
        {
            if (decreaseBy < 1)
                throw new ArgumentOutOfRangeException("decreaseBy", "The minimum amount of new concurrent connections must be 1 or higher");

            lock (_poolLockerObject)
            {
                if (_workItemStores.Count - decreaseBy < 1)
                    throw new ArgumentOutOfRangeException("decreaseBy", "The amount of concurrent connections would be less than 1 after reduction.");

                var targetSize = _workItemStores.Count - decreaseBy;

                while (_workItemStores.Count > targetSize)
                {
                    foreach (var workItemStore in _workItemStores.Where(workItemStore => !workItemStore.Value))
                    {
                        bool value;
                        _workItemStores.TryRemove(workItemStore.Key, out value);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Tries to get and reserve an avalable, pooled <see cref="T:Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemStore"/>.
        /// Important: Use the using(..) construct or call .Dispose() when done to release the work item store back to the pool.
        /// </summary>
        /// <returns></returns>
        public PooledWorkItemStore TryGetWorkItemStore()
        {
            lock (_poolLockerObject)
            {
                foreach (var workItemStore in _workItemStores.Where(workItemStore => workItemStore.Value != true))
                {
                    _workItemStores.TryUpdate(workItemStore.Key, true, false);
                    return new PooledWorkItemStore(this, workItemStore.Key);
                }

                return null;
            }
        }

        /// <summary>
        /// Tries the release.
        /// </summary>
        /// <param name="workItemStore">The <see cref="T:Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemStore"/>.</param>
        /// <param name="retryAttempts">if set to <c>true</c> [retry attempts].</param>
        /// <returns></returns>
        internal bool TryReleaseWorkItemStore(WorkItemStore workItemStore, int retryAttempts = 0)
        {
            if (workItemStore == null) throw new ArgumentNullException("workItemStore");

            int retryAttempt = 0;
            bool result = false;

            while (result == false && retryAttempt <= retryAttempts)
            {
                lock (_poolLockerObject)
                {
                    result = _workItemStores.ContainsKey(workItemStore) && _workItemStores.TryUpdate(workItemStore, false, true);
                }
                retryAttempt++;
            }

            return result;
        }

        /// <summary>
        /// Gets the used work item stores count.
        /// </summary>
        /// <returns></returns>
        public int GetUsedWorkItemStoresCount()
        {
            lock (_poolLockerObject)
            {
                return _workItemStores.Count(workItemStore => workItemStore.Value == true);
            }
        }

        /// <summary>
        /// Gets the available work item stores count.
        /// </summary>
        /// <returns></returns>
        public int GetAvailableWorkItemStoresCount()
        {
            lock (_poolLockerObject)
            {
                return _workItemStores.Count(workItemStore => workItemStore.Value == false);
            }
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

            if (_workItemStores != null)
            {
                _workItemStores.Clear();
                Interlocked.Exchange(ref _workItemStores, null);
            }

            if (_tfsTeamProjectCollection != null)
                Interlocked.Exchange(ref _tfsTeamProjectCollection, null);

        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="PooledWorkItemStore"/> is reclaimed by garbage collection.
        /// </summary>
        ~WorkItemStoreConnectionPool()
        {
            Dispose();
        }
        #endregion
    }
}
