using BasicTest.Jobs;
using NLog.Extensions.Logging;
using Quartz;
using Quartz.Simpl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region 註冊NLog
builder.Logging.AddNLog("nlog.config");
#endregion

#region 註冊 Quartz 並設定排程
builder.Services.AddQuartz(q =>
{
    q.UseJobFactory<MicrosoftDependencyInjectionJobFactory>();

    var jkPerMinuteJob = new JobKey(nameof(WriteLogPerMinuteJob));
    q.AddJob<WriteLogPerMinuteJob>(opts =>
        opts.WithIdentity(jkPerMinuteJob)
    );
    q.AddTrigger(opts => opts
        .ForJob(jkPerMinuteJob)
        .WithIdentity($"{nameof(WriteLogPerMinuteJob)}-trigger")
        .WithCronSchedule("0 0/1 * * * ?", x => x.InTimeZone(TimeZoneInfo.Local))
    );

    var jkOnTimeJob = new JobKey(nameof(WriteLogOnTimeJob));
    q.AddJob<WriteLogOnTimeJob>(opts =>
        opts.WithIdentity(jkOnTimeJob)
    );
    q.AddTrigger(opts => opts
        .ForJob(jkOnTimeJob)
        .WithIdentity($"{nameof(WriteLogOnTimeJob)}-trigger")
        .WithCronSchedule("0 0 6 * * ?", x => x.InTimeZone(TimeZoneInfo.Local))
    );
});
#endregion

#region 啟動背景 Quartz 服務
builder.Services.AddQuartzHostedService(opt =>
{
    opt.WaitForJobsToComplete = true;
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.Run();
