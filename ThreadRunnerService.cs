using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Utilities
{
    public class ThreadRunnerService : IThreadRunnerService
    {
        public void QueueExecution(Action work)
        {
            Task.Factory
                .StartNew(work);
        }
        public void QueueExecution(Action work, Action completionCallback)
        {
            Task.Factory
                .StartNew(work)
                .ContinueWith(_ => completionCallback(), TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void QueueExecution(Action work, Action completionCallback, Action<Exception> errorCallback)
        {
            Task.Factory
                .StartNew(work)
                .ContinueWith(task =>
                {
                    if (task.Status == TaskStatus.Faulted)
                    {
                        Debug.Assert(task.Exception != null, "task.Exception != null");

                        errorCallback((task.Exception).InnerExceptions[0]);
                    }
                    else
                    {
                        completionCallback();
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}