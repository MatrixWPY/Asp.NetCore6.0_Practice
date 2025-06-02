using Quartz;

namespace WorkerService.Jobs
{
    public class WriteLogPerMinuteJob : IJob
    {
        private readonly ILogger<WriteLogPerMinuteJob> _logger;

        public WriteLogPerMinuteJob(ILogger<WriteLogPerMinuteJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"{nameof(WriteLogPerMinuteJob)} Execute");
            return Task.CompletedTask;
        }
    }
}
