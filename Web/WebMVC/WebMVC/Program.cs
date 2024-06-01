using Microsoft.Data.SqlClient;
using System.Data;
using WebMVC.Models.Interface;
using WebMVC.Repositories.Interface;
using WebMVC.Services.Instance;
using WebMVC.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

#region Ū��appsettings.json�]�w
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var dbConnectString = builder.Configuration["ConnectionStrings:MsSql"];
var ormType = builder.Configuration["OrmType"];
#endregion

#region ���UDB�s�u
builder.Services.AddScoped<IDbConnection, SqlConnection>(e => new SqlConnection(dbConnectString));
#endregion

#region ���UModel
switch (ormType)
{
    case "Dapper":
        builder.Services.AddTransient<IContactInfoModel, WebMVC.Models.Instance.Dapper.ContactInfoModel>();
        break;
}
#endregion

#region ���URepository
switch (ormType)
{
    case "Dapper":
        builder.Services.AddScoped<IContactInfoRepository, WebMVC.Repositories.Instance.Dapper.ContactInfoRepository>();
        break;
}
#endregion

#region ���UService
builder.Services.AddScoped<IContactInfoService, ContactInfoService>();
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
