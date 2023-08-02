using BasicTest.Jobs.Base;

namespace BasicTest.Jobs
{
    public class WriteLogPerMinuteJob : JobBase
    {
        private readonly ILogger<WriteLogPerMinuteJob> _logger;

        public WriteLogPerMinuteJob(ILogger<WriteLogPerMinuteJob> logger)
        {
            _logger = logger;
        }

        public override void Execute()
        {
            _logger.LogInformation($"{nameof(WriteLogPerMinuteJob)} Execute");
        }
    }
}
