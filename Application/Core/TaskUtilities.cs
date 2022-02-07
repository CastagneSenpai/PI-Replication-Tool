using System;
using System.Threading.Tasks;

namespace Core
{
    public static class TaskUtilities
    {
        public static async void StartTask(this Task task, IErrorHandler handler = null)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                handler?.HandleError(ex);
            }
        }
    }
}
