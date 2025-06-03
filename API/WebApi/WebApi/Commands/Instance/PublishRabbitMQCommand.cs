using WebApi.Commands.Interface;
using WebApi.DtoModels.Common;
using WebApi.Services.Interface;

namespace WebApi.Commands.Instance
{
    /// <summary>
    /// 
    /// </summary>
    public class PublishRabbitMQCommand : BaseCommand, IPublishCommand
    {
        private readonly IRabbitMQService _rabbitMQService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rabbitMQService"></param>
        public PublishRabbitMQCommand(IRabbitMQService rabbitMQService)
        {
            _rabbitMQService = rabbitMQService;
        }

        public ApiResultRP<bool> Publish<T>(T value)
        {
            _rabbitMQService.SendDirect(nameof(PublishRabbitMQCommand), nameof(Publish), typeof(T).Name, value);
            return SuccessRP(true);
        }
    }
}
