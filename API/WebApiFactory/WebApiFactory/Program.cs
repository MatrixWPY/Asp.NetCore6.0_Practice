using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using NLog.Extensions.Logging;
using System.Data;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using WebApiFactory.Commands.Instance;
using WebApiFactory.Factories.Instance;
using WebApiFactory.Factories.Interface;
using WebApiFactory.Middlewares;
using WebApiFactory.Models.Response;
using WebApiFactory.Services.Instance;
using WebApiFactory.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(opt =>
{
    // 名稱忽略大小寫
    opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    // 序列化命名規則
    opt.JsonSerializerOptions.PropertyNamingPolicy = null;
    // 維持原字元編碼
    opt.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
});

builder.Services.Configure<ApiBehaviorOptions>(opt =>
{
    // 停用ModelStateInvalidFilter => 使用自訂ValidFilter:ValidRequest
    opt.SuppressModelStateInvalidFilter = true;
});

#region 註冊Swagger
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

#region 註冊NLog
builder.Logging.AddNLog("nlog.config");
#endregion

#region 讀取appsettings.json設定
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
#endregion

#region 註冊Redis
builder.Services.AddSingleton<IRedisService, RedisService>();
#endregion

#region 註冊DB連線
builder.Services.AddScoped<SqlConnection>(db => new SqlConnection(builder.Configuration["ConnectionStrings:MsSql"]));
builder.Services.AddScoped<MySqlConnection>(db => new MySqlConnection(builder.Configuration["ConnectionStrings:MySql"]));
#endregion

#region 註冊Service
builder.Services.AddScoped<ContactInfoMssqlService>();
builder.Services.AddScoped<ContactInfoMssqlSPService>();
builder.Services.AddScoped<ContactInfoMysqlService>();
builder.Services.AddScoped<ContactInfoMysqlSPService>();
#endregion

#region 註冊Command
builder.Services.AddScoped<ContactInfoCommand>();
builder.Services.AddScoped<ContactInfoRedisStringCommand>();
builder.Services.AddScoped<ContactInfoRedisHashCommand>();
#endregion

#region 註冊Factory
builder.Services.AddScoped<IServiceFactory, ServiceFactory>();
builder.Services.AddScoped<ICommandFactory, CommandFactory>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.

#region 使用SwaggerUI
var isOpenSwagger = (bool)builder.Configuration.GetValue(typeof(bool), "IsOpenSwagger");
if (app.Environment.IsDevelopment() && isOpenSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
#endregion

app.UseHttpsRedirection();

#region 記錄傳出參數
app.UseLogResponseMiddleware();
#endregion

#region 錯誤處理
// 若有註冊 NLog 套件，則會自動記錄異常至 Level=Error 配置目標中
app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (exceptionHandlerFeature != null)
        {
            var res = new ApiResultRP<string>
            {
                Code = context.Response.StatusCode,
                Msg = "An unexpected error occurred.",
                Result = exceptionHandlerFeature.Error.Message
            };

            await context.Response.WriteAsync(JsonConvert.SerializeObject(res));
        }
    });
});
#endregion

#region 記錄傳入參數
app.UseLogRequestMiddleware();
#endregion

app.UseAuthorization();

app.MapControllers();

app.Run();
