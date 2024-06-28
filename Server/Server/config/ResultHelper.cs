using Model.Util;

namespace Server.config
{
    public class ResultHelper<T>
    {
        public static ApiResult<T> Success(T res, string message = "")
        {
            return new ApiResult<T> { IsSuccess = true, Result = res, Msg = message };
        }
        public static ApiResult<T> Fail(string message)
        {
            return new ApiResult<T> { IsSuccess = false, Msg = message };
        }
    }
}