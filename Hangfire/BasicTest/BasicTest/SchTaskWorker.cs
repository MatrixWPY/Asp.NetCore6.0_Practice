using BasicTest.Jobs;
using Hangfire;
using System.Linq.Expressions;

namespace BasicTest
{
    public class SchTaskWorker
    {
        private readonly IServiceProvider _services;

        // 取得 IServiceProvider 稍後建立 Scoped 範圍的 Job
        // https://blog.darkthread.net/blog/aspnetcore-use-scoped-in-singleton/
        public SchTaskWorker(IServiceProvider services)
        {
            _services = services;
        }

        // 設定定期排程工作
        public void SetSchTasks()
        {
            using (var scope = _services.CreateScope())
            {
                var job = scope.ServiceProvider.GetRequiredService<WriteLogPerMinute>();
                SetSchTask("WriteLogPerMinute", () => job.Execute(), "0 0/1 * * * ?");
            }
        }

        // 先刪再設，避免錯過時間排程在伺服器啟動時執行
        // https://blog.darkthread.net/blog/missed-recurring-job-in-hangfire/
        void SetSchTask(string id, Expression<Action> job, string cron)
        {
            RecurringJob.RemoveIfExists(id);
            RecurringJob.AddOrUpdate(id, job, cron, TimeZoneInfo.Local);
        }
    }

    // 擴充方法，註冊排程工作元件以及設定排程
    public static class SchTaskWorkerExtensions
    {
        public static WebApplicationBuilder AddSchTaskWorker(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<SchTaskWorker>();
            builder.Services.AddTransient<WriteLogPerMinute>();
            return builder;
        }

        public static void SetSchTasks(this WebApplication app)
        {
            var worker = app.Services.GetRequiredService<SchTaskWorker>();
            worker.SetSchTasks();
        }
    }
}
