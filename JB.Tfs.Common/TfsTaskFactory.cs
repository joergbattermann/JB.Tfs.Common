// <copyright file="TfsTaskFactory.cs" company="Joerg Battermann">
//     (c) 2012 Joerg Battermann.
//     License: Microsoft Public License (Ms-PL). For details see https://github.com/jbattermann/JB.Tfs.Common/blob/master/LICENSE
//     Based on: http://stackoverflow.com/questions/7518139/how-can-i-use-the-asyncctp-with-an-tfs-apm-method-query-begin-endquery
// </copyright>
// <author>Joerg Battermann</author>

using System;

namespace JB.Tfs.Common
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.TeamFoundation.Client;

    public static class TfsTaskFactory<TResult>
    {
        #region actual FromAsync implementations
        public static Task<TResult> FromAsync(Func<AsyncCallback, ICancelableAsyncResult> beginMethod,
            Func<ICancelableAsyncResult, TResult> endMethod,
            CancellationToken cancellationToken,
            TaskCreationOptions creationOptions)
        {
            // Represent the asynchronous operation by a manually-controlled task.
            var taskCompletionSource = new TaskCompletionSource<TResult>(creationOptions);
            try
            {
                // Begin the TFS asynchronous operation.
                var asyncResult = beginMethod(Callback(endMethod, taskCompletionSource));

                // If our CancellationToken is signalled, cancel the TFS operation.
                cancellationToken.Register(asyncResult.Cancel, false);
            }
            catch (Exception exception)
            {
                // If there is any error starting the TFS operation, pass it to the task.
                taskCompletionSource.TrySetException(exception);
            }

            // Return the manually-controlled task.
            return taskCompletionSource.Task;
        }

        public static Task<TResult> FromAsync<TArg1>(Func<TArg1, AsyncCallback, ICancelableAsyncResult> beginMethod,
            Func<ICancelableAsyncResult, TResult> endMethod,
            TArg1 arg1,
            CancellationToken cancellationToken,
            TaskCreationOptions creationOptions)
        {
            // Represent the asynchronous operation by a manually-controlled task.
            var taskCompletionSource = new TaskCompletionSource<TResult>(creationOptions);
            try
            {
                // Begin the TFS asynchronous operation.
                var asyncResult = beginMethod(arg1, Callback(endMethod, taskCompletionSource));

                // If our CancellationToken is signalled, cancel the TFS operation.
                cancellationToken.Register(asyncResult.Cancel, false);
            }
            catch (Exception exception)
            {
                // If there is any error starting the TFS operation, pass it to the task.
                taskCompletionSource.TrySetException(exception);
            }

            // Return the manually-controlled task.
            return taskCompletionSource.Task;
        }

        public static Task<TResult> FromAsync<TArg1, TArg2>(Func<TArg1, TArg2, AsyncCallback, ICancelableAsyncResult> beginMethod,
            Func<ICancelableAsyncResult, TResult> endMethod,
            TArg1 arg1,
            TArg2 arg2,
            CancellationToken cancellationToken,
            TaskCreationOptions creationOptions)
        {
            // Represent the asynchronous operation by a manually-controlled task.
            var taskCompletionSource = new TaskCompletionSource<TResult>(creationOptions);
            try
            {
                // Begin the TFS asynchronous operation.
                var asyncResult = beginMethod(arg1, arg2, Callback(endMethod, taskCompletionSource));

                // If our CancellationToken is signalled, cancel the TFS operation.
                cancellationToken.Register(asyncResult.Cancel, false);
            }
            catch (Exception exception)
            {
                // If there is any error starting the TFS operation, pass it to the task.
                taskCompletionSource.TrySetException(exception);
            }

            // Return the manually-controlled task.
            return taskCompletionSource.Task;
        }

        public static Task<TResult> FromAsync<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, AsyncCallback, ICancelableAsyncResult> beginMethod,
            Func<ICancelableAsyncResult, TResult> endMethod,
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            CancellationToken cancellationToken,
            TaskCreationOptions creationOptions)
        {
            // Represent the asynchronous operation by a manually-controlled task.
            var taskCompletionSource = new TaskCompletionSource<TResult>(creationOptions);
            try
            {
                // Begin the TFS asynchronous operation.
                var asyncResult = beginMethod(arg1, arg2, arg3, Callback(endMethod, taskCompletionSource));

                // If our CancellationToken is signalled, cancel the TFS operation.
                cancellationToken.Register(asyncResult.Cancel, false);
            }
            catch (Exception exception)
            {
                // If there is any error starting the TFS operation, pass it to the task.
                taskCompletionSource.TrySetException(exception);
            }

            // Return the manually-controlled task.
            return taskCompletionSource.Task;
        }
        #endregion

        #region overrides
        public static Task<TResult> FromAsync(Func<AsyncCallback, ICancelableAsyncResult> beginMethod,
            Func<ICancelableAsyncResult, TResult> endMethod)
        {
            return FromAsync(beginMethod, endMethod, CancellationToken.None);
        }

        public static Task<TResult> FromAsync(Func<AsyncCallback, ICancelableAsyncResult> beginMethod,
            Func<ICancelableAsyncResult, TResult> endMethod,
            CancellationToken cancellationToken)
        {
            return FromAsync(beginMethod, endMethod, cancellationToken, TaskCreationOptions.None);
        }

        public static Task<TResult> FromAsync<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, AsyncCallback, ICancelableAsyncResult> beginMethod,
            Func<ICancelableAsyncResult, TResult> endMethod,
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3)
        {
            return FromAsync(beginMethod, endMethod, arg1, arg2, arg3, CancellationToken.None);
        }

        public static Task<TResult> FromAsync<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, AsyncCallback, ICancelableAsyncResult> beginMethod,
            Func<ICancelableAsyncResult, TResult> endMethod,
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            CancellationToken cancellationToken)
        {
            return FromAsync(beginMethod, endMethod, arg1, arg2, arg3, cancellationToken, TaskCreationOptions.None);
        }

        public static Task<TResult> FromAsync<TArg1, TArg2>(Func<TArg1, TArg2, AsyncCallback, ICancelableAsyncResult> beginMethod,
            Func<ICancelableAsyncResult, TResult> endMethod,
            TArg1 arg1,
            TArg2 arg2)
        {
            return FromAsync(beginMethod, endMethod, arg1, arg2, CancellationToken.None);
        }

        public static Task<TResult> FromAsync<TArg1, TArg2>(Func<TArg1, TArg2, AsyncCallback, ICancelableAsyncResult> beginMethod,
            Func<ICancelableAsyncResult, TResult> endMethod,
            TArg1 arg1,
            TArg2 arg2,
            CancellationToken cancellationToken)
        {
            return FromAsync(beginMethod, endMethod, arg1, arg2, cancellationToken, TaskCreationOptions.None);
        }

        public static Task<TResult> FromAsync<TArg1>(Func<TArg1, AsyncCallback, ICancelableAsyncResult> beginMethod,
            Func<ICancelableAsyncResult, TResult> endMethod,
            TArg1 arg1)
        {
            return FromAsync(beginMethod, endMethod, arg1, CancellationToken.None);
        }

        public static Task<TResult> FromAsync<TArg1>(Func<TArg1, AsyncCallback, ICancelableAsyncResult> beginMethod,
            Func<ICancelableAsyncResult, TResult> endMethod,
            TArg1 arg1,
            CancellationToken cancellationToken)
        {
            return FromAsync(beginMethod, endMethod, arg1, cancellationToken, TaskCreationOptions.None);
        }
        #endregion

        #region helper method for endMethods
        private static AsyncCallback Callback(Func<ICancelableAsyncResult, TResult> endMethod,
            TaskCompletionSource<TResult> taskCompletionSource)
        {
            // This delegate will be invoked when the TFS operation completes.
            return asyncResult =>
            {
                var cancelableAsyncResult = (ICancelableAsyncResult)asyncResult;

                // First check if we were canceled, and cancel our task if we were.
                if (cancelableAsyncResult.IsCanceled)
                    taskCompletionSource.TrySetCanceled();
                else
                {
                    try
                    {
                        // Call the TFS End* method to get the result, and place it in the task.
                        taskCompletionSource.TrySetResult(endMethod(cancelableAsyncResult));
                    }
                    catch (Exception exception)
                    {
                        // Place the TFS operation error in the task.
                        taskCompletionSource.TrySetException(exception);
                    }
                }
            };
        }
        #endregion
    }
}
