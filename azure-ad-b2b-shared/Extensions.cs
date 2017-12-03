using System;

namespace azure_ad_b2b_shared
{
    public static class Extensions
    {
        public static ServiceResult<T> ToErrorResult<T>(this ServiceResult<T> result, string error)
        {
            if (result == null) { throw new ArgumentNullException(nameof(result)); }
            return new ServiceResult<T>() { Message = error, Success = false };
        }
    }
}