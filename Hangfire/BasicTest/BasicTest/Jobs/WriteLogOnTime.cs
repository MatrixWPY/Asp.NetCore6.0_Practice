namespace BasicTest.Jobs
{
    public class WriteLogOnTime
    {
        private readonly ILogger<WriteLogOnTime> _logger;

        public WriteLogOnTime(ILogger<WriteLogOnTime> logger)
        {
            _logger = logger;
        }

        public void Execute()
        {
            _logger.LogInformation($"{nameof(WriteLogOnTime)} Execute");
        }
    }
}
