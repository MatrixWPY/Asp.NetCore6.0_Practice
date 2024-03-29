﻿using BasicTest.Jobs;
using BasicTest.Jobs.Base;
using Hangfire;

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
                SetSchTask(nameof(WriteLogPerMinuteJob),
                    scope.ServiceProvider.GetRequiredService<WriteLogPerMinuteJob>(),
                    "0 0/1 * * * ?");

                SetSchTask(nameof(WriteLogOnTimeJob),
                    scope.ServiceProvider.GetRequiredService<WriteLogOnTimeJob>(),
                    "0 0 6 * * ?");
            }
        }

        private void SetSchTask(string id, JobBase job, string cron)
        {
            // 先刪再設，避免錯過時間排程在伺服器啟動時執行
            // https://blog.darkthread.net/blog/missed-recurring-job-in-hangfire/
            RecurringJob.RemoveIfExists(id);
            RecurringJob.AddOrUpdate(id, () => job.Execute(), cron, TimeZoneInfo.Local);
        }
    }

    // 擴充方法:註冊排程工作元件/註冊排程/設定排程
    public static class SchTaskWorkerExtensions
    {
        public static WebApplicationBuilder AddSchTaskWorker(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<SchTaskWorker>();
            return builder;
        }

        public static WebApplicationBuilder AddSchTasks(this WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<WriteLogPerMinuteJob>();
            builder.Services.AddTransient<WriteLogOnTimeJob>();
            return builder;
        }

        public static void SetSchTasks(this WebApplication app)
        {
            var worker = app.Services.GetRequiredService<SchTaskWorker>();
            worker.SetSchTasks();
        }
    }
}
