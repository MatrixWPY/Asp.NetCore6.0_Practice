using BasicTest;
using Hangfire;
using NLog.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region ���UNLog
builder.Logging.AddNLog("nlog.config");
#endregion

#region ���UHangfire
var dbConnectionString = (string)builder.Configuration.GetValue(typeof(string), "ConnectionStrings:MsSql");
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(dbConnectionString)
);
builder.Services.AddHangfireServer();
#endregion

#region ���U�Ƶ{�u�@����
builder.AddSchTaskWorker();
#endregion
#region ���U�Ƶ{
builder.AddSchTasks();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

#region �ϥ�HangfireDashboard
app.UseHangfireDashboard();
#endregion

#region �]�w�Ƶ{
app.SetSchTasks();
#endregion

app.Run();
