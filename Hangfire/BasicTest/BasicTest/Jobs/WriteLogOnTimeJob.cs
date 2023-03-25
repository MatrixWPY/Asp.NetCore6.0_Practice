namespace BasicTest.Jobs
{
    public class WriteLogOnTimeJob
    {
        private readonly ILogger<WriteLogOnTimeJob> _logger;

        public WriteLogOnTimeJob(ILogger<WriteLogOnTimeJob> logger)
        {
            _logger = logger;
        }

        public void Execute()
        {
            _logger.LogInformation($"{nameof(WriteLogOnTimeJob)} Execute");
        }
    }
}
