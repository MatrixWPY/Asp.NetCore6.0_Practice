namespace BasicTest.Jobs
{
    public class WriteLogPerMinute
    {
        private readonly ILogger<WriteLogPerMinute> _logger;

        public WriteLogPerMinute(ILogger<WriteLogPerMinute> logger)
        {
            _logger = logger;
        }

        public void Execute()
        {
            _logger.LogInformation($"{nameof(WriteLogPerMinute)} Execute");
        }
    }
}
