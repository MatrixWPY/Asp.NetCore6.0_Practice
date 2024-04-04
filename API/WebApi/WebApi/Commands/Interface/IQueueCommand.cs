using WebApi.Models.Data;
using WebApi.Models.Request;
using WebApi.Models.Response;

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
        ApiResultRP<List<ContactInfo>> Receive(int cnt);

        /// <summary>
        /// 傳送資料至 Queue
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        ApiResultRP<bool> Send(ContactInfoAddRQ value);
    }
}
