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
    /// A 
    /// </summary>
    public class WorkItemStoreConnectionPool : IDisposable
    {
        private readonly ConcurrentDictionary<WorkItemStore, bool> _workItemStores;
        private readonly TfsTeamProjectCollection _tfsTeamProjectCollection;
        private static readonly object LockerObject = new object();
        private volatile bool _disposing;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemStoreConnectionPool"/> class.
        /// </summary>
        /// <param name="tfsTeamProjectCollection">The TFS team project collection.</param>
        /// <param name="amountOfPooledConnections">The amount of pooled connections.</param>
        public WorkItemStoreConnectionPool(TfsTeamProjectCollection tfsTeamProjectCollection, int amountOfPooledConnections = 1)
        {
            if (tfsTeamProjectCollection == null) throw new ArgumentNullException("tfsTeamProjectCollection");

            if (amountOfPooledConnections < 1)
                throw new ArgumentOutOfRangeException("amountOfPooledConnections", "The minimum amount of concurrent connections must be 1 or higher");

            lock (LockerObject)
            {
                _tfsTeamProjectCollection = tfsTeamProjectCollection;
                _workItemStores = new ConcurrentDictionary<WorkItemStore, bool>();

                for (var i = 0; i < amountOfPooledConnections; i++)
                {
                    _workItemStores.TryAdd(new WorkItemStore(tfsTeamProjectCollection), false);
                }

                Instance = this;                
            }
        }

        /// <summary>
        /// Increases the size of the pool.
        /// </summary>
        /// <param name="increaseBy">The amount of stores to increase the pool by.</param>
        public void IncreasePoolSize(int increaseBy = 1)
        {
            if (increaseBy < 1)
                throw new ArgumentOutOfRangeException("increaseBy", "The minimum amount of new concurrent connections must be 1 or higher");

            lock (LockerObject)
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

            lock (LockerObject)
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
        /// Gets the singleton instance.
        /// </summary>
        public static WorkItemStoreConnectionPool Instance { get; private set; }

        /// <summary>
        /// Tries to get and reserve a pooled <see cref="T:Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemStore"/>.
        /// </summary>
        /// <returns></returns>
        public WorkItemStore TryGetWorkItemStore()
        {
            lock (LockerObject)
            {
                foreach (var workItemStore in _workItemStores.Where(workItemStore => workItemStore.Value != true))
                {
                    _workItemStores.TryUpdate(workItemStore.Key, true, false);
                    return workItemStore.Key;
                }

                return null;
            }
        }

        /// <summary>
        /// Tries the release.
        /// </summary>
        /// <param name="workItemStore">The <see cref="T:Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemStore"/>.</param>
        /// <returns></returns>
        internal bool TryRelease(WorkItemStore workItemStore)
        {
            if (workItemStore == null) throw new ArgumentNullException("workItemStore");

            lock (LockerObject)
            {
                return _workItemStores.ContainsKey(workItemStore) &&
                       _workItemStores.TryUpdate(workItemStore, false, true);
            }
        }

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(1000);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose(int timeToWaitForReservedWorkItemStores)
        {
            if (_disposing)
                return;

            _disposing = true;

            SpinWait.SpinUntil(() => _workItemStores.All(store => store.Value == false), timeToWaitForReservedWorkItemStores);

            _workItemStores.Clear();
        }

        #endregion
    }
}
