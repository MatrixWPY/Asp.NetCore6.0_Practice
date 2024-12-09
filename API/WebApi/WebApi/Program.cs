using AspectCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using NLog.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using WebApi.BackServices;
using WebApi.Commands.Instance;
using WebApi.Commands.Interface;
using WebApi.DtoModels.Common;
using WebApi.Interceptors;
using WebApi.Middlewares;
using WebApi.Profiles;
using WebApi.Services.Instance;
using WebApi.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

#region Add services to the container.

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

#region 設定Swagger
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        // name: 攸關 SwaggerDocument 的 URL 位置。
        name: "v1",
        // info: 是用於 SwaggerDocument 版本資訊的顯示(內容非必填)。
        info: new OpenApiInfo
        {
            Title = "WebApi",
            Version = "1.0.0",
            Description = "This is ASP.NET Core 6 API Sample.",
            Contact = new OpenApiContact
            {
                Name = "Matrix",
                Email = string.Empty
            }
        }
    );

    // XML 檔案: 文件註解標籤
    var xmlPath = Path.Combine(AppContext.BaseDirectory, "WebApi.xml");
    c.IncludeXmlComments(xmlPath);
});
#endregion

#region 設定NLog
builder.Logging.AddNLog("nlog.config");
#endregion

#region 設定AspectCore
// 使用AspectCore取代預設IoC容器
builder.Host.UseServiceProviderFactory(new DynamicProxyServiceProviderFactory());
builder.Services.ConfigureDynamicProxy();
#endregion

#region 註冊Interceptor
builder.Services.AddScoped<LogInterceptor>();
#endregion

#region 讀取appsettings.json設定
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var isOpenSwagger = (bool)builder.Configuration.GetValue(typeof(bool), "IsOpenSwagger");
var isUseRedis = (bool)builder.Configuration.GetValue(typeof(bool), "IsUseRedis");
var isUseRabbitMQ = (bool)builder.Configuration.GetValue(typeof(bool), "IsUseRabbitMQ");
var isUseBackService = (bool)builder.Configuration.GetValue(typeof(bool), "IsUseBackService");

var dbType = builder.Configuration["DbType"];
var redisType = builder.Configuration["RedisType"];
var queueType = builder.Configuration["QueueType"];
var pubsubType = builder.Configuration["PubSubType"];

var dbConnectString = string.Empty;
switch (dbType)
{
    case "MsSql":
        dbConnectString = builder.Configuration["ConnectionStrings:MsSql"];
        break;

    case "MsSqlSP":
        dbConnectString = builder.Configuration["ConnectionStrings:MsSqlSP"];
        break;

    case "MySql":
        dbConnectString = builder.Configuration["ConnectionStrings:MySql"];
        break;

    case "MySqlSP":
        dbConnectString = builder.Configuration["ConnectionStrings:MySqlSP"];
        break;

    case "Oracle":
        dbConnectString = builder.Configuration["ConnectionStrings:Oracle"];
        break;

    case "OracleSP":
        dbConnectString = builder.Configuration["ConnectionStrings:OracleSP"];
        break;
}
#endregion

#region 註冊Redis
if (isUseRedis)
{
    builder.Services.AddSingleton<IRedisService, RedisService>();
}
#endregion

#region 註冊RabbitMQ
if (isUseRabbitMQ)
{
    builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();
}
#endregion

#region 註冊DB連線
switch (dbType)
{
    case "MsSql":
        builder.Services.AddScoped<IDbConnection, SqlConnection>(db => new SqlConnection(dbConnectString));
        break;

    case "MsSqlSP":
        builder.Services.AddScoped<IDbConnection, SqlConnection>(db => new SqlConnection(dbConnectString));
        break;

    case "MySql":
        builder.Services.AddScoped<IDbConnection, MySqlConnection>(db => new MySqlConnection(dbConnectString));
        break;

    case "MySqlSP":
        builder.Services.AddScoped<IDbConnection, MySqlConnection>(db => new MySqlConnection(dbConnectString));
        break;

    case "Oracle":
        builder.Services.AddScoped<IDbConnection, OracleConnection>(db => new OracleConnection(dbConnectString));
        break;

    case "OracleSP":
        builder.Services.AddScoped<IDbConnection, OracleConnection>(db => new OracleConnection(dbConnectString));
        break;
}
#endregion

#region 註冊Service
switch (dbType)
{
    case "MsSql":
        builder.Services.AddScoped<IContactInfoService, ContactInfoMssqlService>();
        break;

    case "MsSqlSP":
        builder.Services.AddScoped<IContactInfoService, ContactInfoMssqlSPService>();
        break;

    case "MySql":
        builder.Services.AddScoped<IContactInfoService, ContactInfoMysqlService>();
        break;

    case "MySqlSP":
        builder.Services.AddScoped<IContactInfoService, ContactInfoMysqlSPService>();
        break;

    case "Oracle":
        builder.Services.AddScoped<IContactInfoService, ContactInfoOracleService>();
        break;

    case "OracleSP":
        builder.Services.AddScoped<IContactInfoService, ContactInfoOracleSPService>();
        break;
}
#endregion

#region 註冊Command
if (isUseRedis)
{
    switch (redisType)
    {
        case "String":
            builder.Services.AddScoped<IContactInfoCommand, ContactInfoRedisStringCommand>();
            break;

        case "Hash":
            builder.Services.AddScoped<IContactInfoCommand, ContactInfoRedisHashCommand>();
            break;
    }
}
else
{
    builder.Services.AddScoped<IContactInfoCommand, ContactInfoCommand>();
}

switch (queueType)
{
    case "RabbitMQ" when isUseRabbitMQ:
        builder.Services.AddScoped<IQueueCommand, QueueRabbitMQCommand>();
        break;

    case "RedisList" when isUseRedis:
        builder.Services.AddScoped<IQueueCommand, QueueRedisListCommand>();
        break;

    case "RedisStream" when isUseRedis:
        builder.Services.AddScoped<IQueueCommand, QueueRedisStreamCommand>();
        break;
}

switch (pubsubType)
{
    case "RabbitMQ" when isUseRabbitMQ:
        builder.Services.AddScoped<IPublishCommand, PublishRabbitMQCommand>();
        builder.Services.AddScoped<ISubscribeCommand, SubscribeRabbitMQCommand>();
        break;

    case "RedisList" when isUseRedis:
        builder.Services.AddScoped<IPublishCommand, PublishRedisListCommand>();
        builder.Services.AddScoped<ISubscribeCommand, SubscribeRedisListCommand>();
        break;
}
#endregion

#region 註冊BackService
if (isUseBackService)
{
    builder.Services.AddHostedService<ContactInfoSubscribeBackService>();
}
#endregion

#region 設定AutoMapper
builder.Services.AddAutoMapper(typeof(ContactInfoProfile));
#endregion

#endregion

var app = builder.Build();

#region Configure the HTTP request pipeline.

#region 使用SwaggerUI
if (app.Environment.IsDevelopment() && isOpenSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint(
            // url: 需配合 SwaggerDoc 的 name。 "/swagger/{SwaggerDoc name}/swagger.json"
            url: "/swagger/v1/swagger.json",
            // name: 用於 Swagger UI 右上角選擇不同版本的 SwaggerDocument 顯示名稱使用。
            name: "API Document V1"
        );
    });
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

#endregion

app.Run();
