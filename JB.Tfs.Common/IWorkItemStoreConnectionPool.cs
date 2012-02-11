// <copyright file="IWorkItemStoreConnectionPool.cs" company="Joerg Battermann">
//     (c) 2012 Joerg Battermann.
//     License: Microsoft Public License (Ms-PL). For details see https://github.com/jbattermann/JB.Tfs.Common/blob/master/LICENSE
// </copyright>
// <author>Joerg Battermann</author>

namespace JB.Tfs.Common
{
    using System;

    public interface IWorkItemStoreConnectionPool : IDisposable
    {
        /// <summary>
        /// Increases the size of the pool.
        /// </summary>
        /// <param name="increaseBy">The amount of stores to increase the pool by.</param>
        void IncreasePoolSize(int increaseBy = 1);

        /// <summary>
        /// Decreases the size of the pool.
        /// </summary>
        /// <param name="decreaseBy">The amount of stores to decrease the pool by.</param>
        void DecreasePoolSize(int decreaseBy = 1);

        /// <summary>
        /// Tries to get and reserve an avalable, pooled <see cref="T:Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemStore"/>.
        /// Important: Use the using(..) construct or call .Dispose() when done to release the work item store back to the pool.
        /// </summary>
        /// <returns></returns>
        PooledWorkItemStore TryGetWorkItemStore();

        /// <summary>
        /// Gets the used work item stores count.
        /// </summary>
        /// <returns></returns>
        int GetUsedWorkItemStoresCount();

        /// <summary>
        /// Gets the available work item stores count.
        /// </summary>
        /// <returns></returns>
        int GetAvailableWorkItemStoresCount();
    }
}