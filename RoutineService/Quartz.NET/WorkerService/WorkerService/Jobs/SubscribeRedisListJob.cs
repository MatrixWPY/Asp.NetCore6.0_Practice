using Newtonsoft.Json;
using Quartz;
using WorkerService.Services;

namespace WorkerService.Jobs
{
    public class SubscribeRedisListJob : IJob
    {
        private readonly ILogger<SubscribeRedisListJob> _logger;
        private readonly SubscribeRedisListService _subscribeRedisListService;

        public SubscribeRedisListJob(
            ILogger<SubscribeRedisListJob> logger,
            SubscribeRedisListService subscribeRedisListService
        )
        {
            _logger = logger;
            _subscribeRedisListService = subscribeRedisListService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _subscribeRedisListService.SubscribeAsync<ContactInfo>((obj) =>
            {
                _logger.LogInformation(JsonConvert.SerializeObject(obj));
                return Task.FromResult(true);
            });
        }

        #region Models
        public class ContactInfo
        {
            public long ContactInfoID { get; set; }
            public string Name { get; set; }
            public string Nickname { get; set; }
            public EnumGender? Gender { get; set; }
            public short? Age { get; set; }
            public string PhoneNo { get; set; }
            public string Address { get; set; }
            public bool IsEnable { get; set; }
            public DateTime CreateTime { get; set; }
            public DateTime? UpdateTime { get; set; }
            public enum EnumGender
            {
                Female = 0,
                Male = 1
            }
        }
        #endregion
    }
}
