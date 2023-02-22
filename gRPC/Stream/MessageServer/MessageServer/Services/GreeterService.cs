using Bogus;
using Grpc.Core;

namespace MessageServer.Services
{
    public class GreeterService : CandidateService.CandidateServiceBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override async Task<CreateCvResponse> CreateCv(IAsyncStreamReader<Candidate> requestStream, ServerCallContext context)
        {
            var result = new CreateCvResponse
            {
                IsSuccess = false
            };

            // stream 讀取
            while (await requestStream.MoveNext())
            {
                var candidate = requestStream.Current;
                // 實際處理
                Console.WriteLine(candidate.Name);
            }

            result.IsSuccess = true;
            return result;
        }

        public override async Task DownloadCv(DownloadByName request, IServerStreamWriter<Candidate> responseStream, ServerCallContext context)
        {
            var fakeJobs = new Faker<Job>()
                .RuleFor(a => a.Title, (f, u) => f.Company.Bs())
                .RuleFor(a => a.Salary, (f, u) => f.Commerce.Random.Int(1000, 2000))
                .RuleFor(a => a.JobDescription, (f, u) => f.Lorem.Text());
            var createRequests = new Faker<Candidate>()
                .RuleFor(a => a.Name, (f, u) => f.Name.FullName())
                .RuleFor(a => a.Jobs, (f, u) =>
                {
                    u.Jobs.AddRange(fakeJobs.GenerateBetween(3, 5));
                    return u.Jobs;
                }).Generate();

            createRequests.Name = request.Name;

            // 將每筆資料逐一透過 WriteAsync 輸出
            await responseStream.WriteAsync(createRequests);
        }

        public override async Task CreateDownloadCv(IAsyncStreamReader<Candidate> requestStream, IServerStreamWriter<Candidates> responseStream, ServerCallContext context)
        {
            var candidates = new Candidates();

            // 將收到的資料逐一取出
            while (await requestStream.MoveNext())
            {
                var candidate = requestStream.Current;

                candidate.Name += " Server";

                candidates.Candidates_.Add(candidate);
            }

            // 將處理後的資料回傳
            await responseStream.WriteAsync(candidates);
        }
    }
}