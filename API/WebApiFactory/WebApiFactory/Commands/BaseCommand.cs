using WebApiFactory.DtoModels.Common;

namespace WebApiFactory.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseCommand
    {
        /// <summary>
        /// 成功回傳
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public ApiResultRP<T> SuccessRP<T>(T result)
        {
            return new ApiResultRP<T>()
            {
                Code = 0,
                Msg = "Success",
                Result = result
            };
        }

        /// <summary>
        /// 失敗回傳
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="errorCode"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public ApiResultRP<T> FailRP<T>(int errorCode, string errorMsg)
        {
            return new ApiResultRP<T>()
            {
                Code = errorCode,
                Msg = errorMsg
            };
        }
    }
}
