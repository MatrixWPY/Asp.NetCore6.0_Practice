namespace BasicTest.Jobs
{
    public class WriteLogPerMinuteJob
    {
        private readonly ILogger<WriteLogPerMinuteJob> _logger;

        public WriteLogPerMinuteJob(ILogger<WriteLogPerMinuteJob> logger)
        {
            _logger = logger;
        }

        public void Execute()
        {
            _logger.LogInformation($"{nameof(WriteLogPerMinuteJob)} Execute");
        }
    }
}
