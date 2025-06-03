using WebApi.Commands.Interface;
using WebApi.Models;
using WebApi.Services.Interface;

namespace WebApi.BackServices
{
    /// <summary>
    /// ContactInfo BackgroundService For Subscribe
    /// </summary>
    public class ContactInfoSubscribeBackService : BackgroundService
    {
        private readonly ISubscribeCommand _subscribeCommand;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscribeCommand"></param>
        /// <param name="serviceScopeFactory"></param>
        public ContactInfoSubscribeBackService(
            ISubscribeCommand subscribeCommand,
            IServiceScopeFactory serviceScopeFactory)
        {
            _subscribeCommand = subscribeCommand;
            _serviceScopeFactory = serviceScopeFactory;
        }

        /// <summary>
        /// ContactInfo - 訂閱資料
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _subscribeCommand.Subscribe<ContactInfo>((obj) =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var contactInfoService = scope.ServiceProvider.GetRequiredService<IContactInfoService>();
                    return contactInfoService.Insert(obj);
                }
            });
            return Task.CompletedTask;
        }
    }
}
