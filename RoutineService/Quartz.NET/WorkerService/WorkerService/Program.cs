using NLog.Extensions.Logging;
using Quartz;
using Quartz.Simpl;
using WorkerService.Helpers;
using WorkerService.Jobs;
using WorkerService.Services;

var builder = Host.CreateDefaultBuilder(args);

#region 設定 Windows Service 模式執行
builder.UseWindowsService();
#endregion

#region 註冊 NLog
builder.ConfigureLogging(logging =>
{
    logging.AddNLog("nlog.config");
});
#endregion

builder.ConfigureServices((hostContext, services) =>
{
    #region 註冊 Redis
    services.AddSingleton<RedisHelper>();
    #endregion

    #region 註冊 RedisQueue 監聽服務
    services.AddSingleton<SubscribeRedisListService>();
    #endregion

    #region 註冊 Quartz 並設定排程
    services.AddQuartz(q =>
    {
        q.UseJobFactory<MicrosoftDependencyInjectionJobFactory>();

        var jkPerMinuteJob = new JobKey(nameof(WriteLogPerMinuteJob));
        q.AddJob<WriteLogPerMinuteJob>(opts => opts.WithIdentity(jkPerMinuteJob));
        q.AddTrigger(opts => opts.ForJob(jkPerMinuteJob)
                                 .WithIdentity($"{nameof(WriteLogPerMinuteJob)}-trigger")
                                 .WithCronSchedule("0 0/1 * * * ?", x => x.InTimeZone(TimeZoneInfo.Local)));

        var jkOnTimeJob = new JobKey(nameof(WriteLogOnTimeJob));
        q.AddJob<WriteLogOnTimeJob>(opts => opts.WithIdentity(jkOnTimeJob));
        q.AddTrigger(opts => opts.ForJob(jkOnTimeJob)
                                 .WithIdentity($"{nameof(WriteLogOnTimeJob)}-trigger")
                                 .WithCronSchedule("0 0 6 * * ?", x => x.InTimeZone(TimeZoneInfo.Local)));

        var jkRedisListJob = new JobKey(nameof(SubscribeRedisListJob));
        q.AddJob<SubscribeRedisListJob>(opts => opts.WithIdentity(jkRedisListJob));
        q.AddTrigger(opts => opts.ForJob(jkRedisListJob)
                                 .WithIdentity($"{nameof(SubscribeRedisListJob)}-trigger")
                                 .StartNow());
    });
    #endregion

    #region 啟用 Quartz HostedService
    services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    #endregion
});

builder.Build().Run();