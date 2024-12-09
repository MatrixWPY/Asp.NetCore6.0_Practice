using WebApi.DtoModels.Common;

namespace WebApi.Commands.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPublishCommand
    {
        /// <summary>
        /// 發佈資料
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        ApiResultRP<bool> Publish<T>(T value);
    }
}
