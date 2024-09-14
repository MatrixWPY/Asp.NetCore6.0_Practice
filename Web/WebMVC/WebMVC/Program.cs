using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebMVC.Models.Interface;
using WebMVC.Profiles;
using WebMVC.Repositories.Interface;
using WebMVC.Services.Instance;
using WebMVC.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

#region 讀取appsettings.json設定
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var dbConnectString = builder.Configuration["ConnectionStrings:MsSql"];
var ormType = builder.Configuration["OrmType"];
#endregion

#region 註冊DB連線
switch (ormType)
{
    case "Dapper":
        builder.Services.AddScoped<IDbConnection, SqlConnection>(e => new SqlConnection(dbConnectString));
        break;

    case "DapperSP":
        builder.Services.AddScoped<IDbConnection, SqlConnection>(e => new SqlConnection(dbConnectString));
        break;

    case "DapperSimpleCRUD":
        builder.Services.AddScoped<IDbConnection, SqlConnection>(e => new SqlConnection(dbConnectString));
        break;

    case "EFCore":
        builder.Services.AddDbContext<WebMVC.Models.Instance.EFCore.WebMvcDbContext>(options => options.UseSqlServer(dbConnectString));
        break;
}
#endregion

#region 註冊Model
switch (ormType)
{
    case "Dapper":
        builder.Services.AddTransient<IContactInfoModel, WebMVC.Models.Instance.Dapper.ContactInfoModel>();
        break;

    case "DapperSP":
        builder.Services.AddTransient<IContactInfoModel, WebMVC.Models.Instance.DapperSP.ContactInfoModel>();
        break;

    case "DapperSimpleCRUD":
        builder.Services.AddTransient<IContactInfoModel, WebMVC.Models.Instance.DapperSimpleCRUD.ContactInfoModel>();
        break;

    case "EFCore":
        builder.Services.AddTransient<IContactInfoModel, WebMVC.Models.Instance.EFCore.ContactInfoModel>();
        break;
}
#endregion

#region 註冊Repository
switch (ormType)
{
    case "Dapper":
        builder.Services.AddScoped<IContactInfoRepository, WebMVC.Repositories.Instance.Dapper.ContactInfoRepository>();
        break;

    case "DapperSP":
        builder.Services.AddScoped<IContactInfoRepository, WebMVC.Repositories.Instance.DapperSP.ContactInfoRepository>();
        break;

    case "DapperSimpleCRUD":
        builder.Services.AddScoped<IContactInfoRepository, WebMVC.Repositories.Instance.DapperSimpleCRUD.ContactInfoRepository> ();
        break;

    case "EFCore":
        builder.Services.AddScoped<IContactInfoRepository, WebMVC.Repositories.Instance.EFCore.ContactInfoRepository>();
        break;
}
#endregion

#region 註冊Service
builder.Services.AddScoped<IContactInfoService, ContactInfoService>();
#endregion

#region 設定AutoMapper
builder.Services.AddAutoMapper(typeof(ContactInfoProfile));
#endregion

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ContactInfo}/{action=Index}/{id?}");

app.Run();
