using System;

namespace Utilities
{
    public class SynchronousThreadRunner : IThreadRunnerService
    {
        public void QueueExecution(Action work)
        {
            work();
        }
        public void QueueExecution(Action work, Action completionCallback)
        {
            work();
            completionCallback();
        }
        public void QueueExecution(Action work, Action completionCallback, Action<Exception> errorCallback)
        {
            try
            {
                work();
                completionCallback();
            }
            catch (Exception ex)
            {
                errorCallback(ex);
            }
        }
    }
}