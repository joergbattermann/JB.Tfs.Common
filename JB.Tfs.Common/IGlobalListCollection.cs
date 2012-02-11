// <copyright file="IGlobalListCollection.cs" company="Joerg Battermann">
//     (c) 2012 Joerg Battermann.
//     License: Microsoft Public License (Ms-PL). For details see https://github.com/jbattermann/JB.Tfs.Common/blob/master/LICENSE
// </copyright>
// <author>Joerg Battermann</author>

namespace JB.Tfs.Common
{
    using System.Collections.Generic;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    public interface IGlobalListCollection : IEnumerable<GlobalList>
    {
        /// <summary>
        /// Saves the data to the team foundation server.
        /// </summary>
        void Save(WorkItemStore workItemStore);

        /// <summary>
        /// Refreshes the data from the team foundation server.
        /// </summary>
        void Refresh(WorkItemStore workItemStore);
    }
}