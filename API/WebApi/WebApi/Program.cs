using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using WebApi.Commands.Instance;
using WebApi.Commands.Interface;
using WebApi.Middlewares;
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

#region 註冊Swagger
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

#region 註冊NLog
builder.Logging.AddNLog("nlog.config");
#endregion

#region 註冊Command
builder.Services.AddTransient<IContactInfoCommand, ContactInfoCommand>();
#endregion

#region 註冊Service
builder.Services.AddTransient<IContactInfoService, ContactInfoService>();
#endregion

#endregion

var app = builder.Build();

#region Configure the HTTP request pipeline.

app.UseHttpsRedirection();

#region 記錄傳入參數
app.UseLogRequestMiddleware();
#endregion

#region 記錄傳出參數
app.UseLogResponseMiddleware();
#endregion

app.UseAuthorization();

app.MapControllers();

#region 使用SwaggerUI
var isOpenSwagger = (bool)app.Configuration.GetValue(typeof(bool), "IsOpenSwagger");
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

#endregion

app.Run();
