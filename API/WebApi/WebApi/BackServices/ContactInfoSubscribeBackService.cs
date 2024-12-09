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
        private readonly IContactInfoService _contactInfoService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscribeCommand"></param>
        /// <param name="contactInfoService"></param>
        public ContactInfoSubscribeBackService(
            ISubscribeCommand subscribeCommand,
            IContactInfoService contactInfoService)
        {
            _subscribeCommand = subscribeCommand;
            _contactInfoService = contactInfoService;
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
                return _contactInfoService.Insert(obj);
            });
            return Task.CompletedTask;
        }
    }
}
