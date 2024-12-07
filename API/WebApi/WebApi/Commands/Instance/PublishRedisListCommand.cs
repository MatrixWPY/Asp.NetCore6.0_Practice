using WebApi.Commands.Interface;
using WebApi.DtoModels.Common;
using WebApi.Services.Interface;

namespace WebApi.Commands.Instance
{
    /// <summary>
    /// 
    /// </summary>
    public class PublishRedisListCommand : BaseCommand, IPublishCommand
    {
        private readonly IRedisService _redisService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisService"></param>
        public PublishRedisListCommand(IRedisService redisService)
        {
            _redisService = redisService;
        }

        public ApiResultRP<bool> Publish<T>(T value)
        {
            _redisService.PublishListQueue("Channel_ListQueue", value);
            return SuccessRP(true);
        }
    }
}
