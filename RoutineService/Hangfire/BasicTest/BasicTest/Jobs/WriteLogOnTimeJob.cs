using BasicTest.Jobs.Base;

namespace BasicTest.Jobs
{
    public class WriteLogOnTimeJob : JobBase
    {
        private readonly ILogger<WriteLogOnTimeJob> _logger;

        public WriteLogOnTimeJob(ILogger<WriteLogOnTimeJob> logger)
        {
            _logger = logger;
        }

        public override void Execute()
        {
            _logger.LogInformation($"{nameof(WriteLogOnTimeJob)} Execute");
        }
    }
}
