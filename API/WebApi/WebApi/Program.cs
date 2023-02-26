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
    // �W�٩����j�p�g
    opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    // �ǦC�ƩR�W�W�h
    opt.JsonSerializerOptions.PropertyNamingPolicy = null;
    // ������r���s�X
    opt.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
});

builder.Services.Configure<ApiBehaviorOptions>(opt =>
{
    // ����ModelStateInvalidFilter => �ϥΦۭqValidFilter:ValidRequest
    opt.SuppressModelStateInvalidFilter = true;
});

#region ���USwagger
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        // name: ���� SwaggerDocument �� URL ��m�C
        name: "v1",
        // info: �O�Ω� SwaggerDocument ������T�����(���e�D����)�C
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

    // XML �ɮ�: �����Ѽ���
    var xmlPath = Path.Combine(AppContext.BaseDirectory, "WebApi.xml");
    c.IncludeXmlComments(xmlPath);
});
#endregion

#region ���UNLog
builder.Logging.AddNLog("nlog.config");
#endregion

#region ���UCommand
builder.Services.AddTransient<IContactInfoCommand, ContactInfoCommand>();
#endregion

#region ���UService
builder.Services.AddTransient<IContactInfoService, ContactInfoService>();
#endregion

#endregion

var app = builder.Build();

#region Configure the HTTP request pipeline.

app.UseHttpsRedirection();

#region �O���ǤJ�Ѽ�
app.UseLogRequestMiddleware();
#endregion

#region �O���ǥX�Ѽ�
app.UseLogResponseMiddleware();
#endregion

app.UseAuthorization();

app.MapControllers();

#region �ϥ�SwaggerUI
var isOpenSwagger = (bool)app.Configuration.GetValue(typeof(bool), "IsOpenSwagger");
if (app.Environment.IsDevelopment() && isOpenSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint(
            // url: �ݰt�X SwaggerDoc �� name�C "/swagger/{SwaggerDoc name}/swagger.json"
            url: "/swagger/v1/swagger.json",
            // name: �Ω� Swagger UI �k�W����ܤ��P������ SwaggerDocument ��ܦW�٨ϥΡC
            name: "API Document V1"
        );
    });
}
#endregion

#endregion

app.Run();
