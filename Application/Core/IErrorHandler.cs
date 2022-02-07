using System;

namespace Core
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}
