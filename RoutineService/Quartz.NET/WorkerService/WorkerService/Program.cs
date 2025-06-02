using Quartz;
using Quartz.Simpl;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
{
    #region 註冊 Quartz
    services.AddQuartz(q =>
    {
        q.UseJobFactory<MicrosoftDependencyInjectionJobFactory>();
    });
    #endregion

    #region 啟用 Quartz HostedService
    services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    #endregion
});

builder.Build().Run();