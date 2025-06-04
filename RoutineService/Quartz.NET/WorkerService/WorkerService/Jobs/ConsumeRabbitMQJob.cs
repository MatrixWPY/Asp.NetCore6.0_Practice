using Newtonsoft.Json;
using Quartz;
using WorkerService.Services;

namespace WorkerService.Jobs
{
    public class ConsumeRabbitMQJob : IJob
    {
        private readonly ILogger<ConsumeRabbitMQJob> _logger;
        private readonly ConsumeRabbitMQService _consumeRabbitMQService;

        public ConsumeRabbitMQJob(
            ILogger<ConsumeRabbitMQJob> logger,
            ConsumeRabbitMQService consumeRabbitMQService
        )
        {
            _logger = logger;
            _consumeRabbitMQService = consumeRabbitMQService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _consumeRabbitMQService.Consume<ContactInfo>((obj) =>
            {
                _logger.LogInformation(JsonConvert.SerializeObject(obj));
                return true;
            });
            return Task.CompletedTask;
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
