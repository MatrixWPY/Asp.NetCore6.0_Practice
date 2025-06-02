using Quartz;

namespace WorkerService.Jobs
{
    public class WriteLogOnTimeJob : IJob
    {
        private readonly ILogger<WriteLogOnTimeJob> _logger;

        public WriteLogOnTimeJob(ILogger<WriteLogOnTimeJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"{nameof(WriteLogOnTimeJob)} Execute");
            return Task.CompletedTask;
        }
    }
}
