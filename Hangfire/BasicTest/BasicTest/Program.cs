using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region µù¥UHangfire
var dbConnectionString = (string)builder.Configuration.GetValue(typeof(string), "ConnectionStrings:MsSql");
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(dbConnectionString)
);
builder.Services.AddHangfireServer();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

#region ¨Ï¥ÎHangfireDashboard
app.UseHangfireDashboard();
#endregion

app.Run();
