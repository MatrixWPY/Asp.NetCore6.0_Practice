using Bogus;
using Google.Protobuf.Collections;
using Grpc.Core;
using Grpc.Net.Client;
using MessageClient;
using System.Text.Json;
using static MessageClient.CandidateService;

using var channel = GrpcChannel.ForAddress("https://localhost:7273");
var client = new CandidateService.CandidateServiceClient(channel);

Console.WriteLine("client-side streaming:");
var resCreate = await TestCreate(client);
Console.WriteLine(resCreate);
Console.WriteLine();

Console.WriteLine("server-side streaming:");
var resDownload = await TestDownload(client);
Console.WriteLine(JsonSerializer.Serialize(resDownload));
Console.WriteLine();

Console.WriteLine("bidirectional streaming:");
var resCreateAndDownload = await TestCreateAndDownload(client);
foreach (var item in resCreateAndDownload)
{
    Console.WriteLine(JsonSerializer.Serialize(item));
}
Console.ReadKey();

async Task<bool> TestCreate(CandidateServiceClient serviceClient)
{
    var result = false;

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
        }).Generate(5);

    try
    {
        var reply = (CreateCvResponse)null;

        using (var creator = serviceClient.CreateCv())
        {
            foreach (var createRequest in createRequests)
            {
                await creator.RequestStream.WriteAsync(createRequest);
            }

            await creator.RequestStream.CompleteAsync();

            reply = await creator.ResponseAsync;
        }

        result = reply.IsSuccess;
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }

    return result;
}

async Task<Candidate> TestDownload(CandidateServiceClient serviceClient)
{
    var result = new Candidate();
    try
    {
        using (var client = serviceClient.DownloadCv(new DownloadByName()
        {
            Name = "test"
        }))
        {
            while (await client.ResponseStream.MoveNext())
            {
                result = client.ResponseStream.Current;
            }
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }

    return result;
}

async Task<RepeatedField<Candidate>> TestCreateAndDownload(CandidateServiceClient serviceClient)
{
    var result = new RepeatedField<Candidate>();

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
        }).Generate(5);

    createRequests.ForEach(e => e.Name += " Client");

    try
    {
        using (var call = serviceClient.CreateDownloadCv())
        {
            foreach (var request in createRequests)
            {
                await call.RequestStream.WriteAsync(request);
            }
            await call.RequestStream.CompleteAsync();

            var responseReaderTask = Task.Run(async () =>
            {
                while (await call.ResponseStream.MoveNext())
                {
                    var candidate = call.ResponseStream.Current;
                    result.AddRange(candidate.Candidates_);
                }
            });
            await responseReaderTask;
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }

    return result;
}