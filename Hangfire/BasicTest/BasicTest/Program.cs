using BasicTest;
using Hangfire;
using NLog.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region 註冊NLog
builder.Logging.AddNLog("nlog.config");
#endregion

#region 註冊Hangfire
var dbConnectionString = (string)builder.Configuration.GetValue(typeof(string), "ConnectionStrings:MsSql");
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(dbConnectionString)
);
builder.Services.AddHangfireServer();
#endregion

#region 註冊排程工作元件
// 使用擴充方法註冊排程工作元件
builder.AddSchTaskWorker();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

#region 使用HangfireDashboard
app.UseHangfireDashboard();
#endregion

#region 設定排程工作
// 使用擴充方法設定排程工作
app.SetSchTasks();
#endregion

app.Run();
