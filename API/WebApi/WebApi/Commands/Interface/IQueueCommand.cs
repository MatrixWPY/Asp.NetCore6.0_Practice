using WebApi.DtoModels.Common;

namespace WebApi.Commands.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface IQueueCommand
    {
        /// <summary>
        /// 接收資料從 Queue
        /// </summary>
        /// <param name="cnt"></param>
        /// <returns></returns>
        ApiResultRP<IEnumerable<T>> Receive<T>(int cnt);

        /// <summary>
        /// 傳送資料至 Queue
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        ApiResultRP<bool> Send<T>(T value);
    }
}
