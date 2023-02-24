using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region HttpGet
//app.MapGet("/Test", () =>
//{
//    return new ResponseModel { Message = "GET" };
//});
app.MapGet("/Test", ([FromQuery] int id) =>
{
    return new ResponseModel { Message = "GET by id", Data = new { id } };
});
app.MapGet("/Test/{id}", ([FromRoute] int id) =>
{
    return new ResponseModel { Message = "GET by id", Data = new { id } };
});
#endregion
#region HttpPost
app.MapPost("/Test", ([FromBody] PostRequestModel request) =>
{
    return new ResponseModel { Message = "POST", Data = request };
});
#endregion
#region HttpPut
app.MapPut("/Test/{id}", ([FromRoute] int id, [FromBody] PutRequestModel request) =>
{
    return new ResponseModel { Message = "PUT", Data = new { id, request } };
});
#endregion
#region HttpPatch
app.MapMethods("/Test/{id}", new string[] { "PATCH" }, ([FromRoute] int id, [FromBody] PatchRequestModel request) =>
{
    return new ResponseModel { Message = "PATCH", Data = new { id, request } };
});
#endregion
#region HttpDelete
app.MapDelete("/Test/{id}", ([FromRoute] int id) =>
{
    return new ResponseModel { Message = "DELETE", Data = new { id } };
});
#endregion

app.Run();

#region Request/Response Model
public class PostRequestModel
{
    public long Id { get; set; }
    public string Describe { get; set; }
}
public class PutRequestModel
{
    public string Describe { get; set; }
}
public class PatchRequestModel
{
    public string Describe { get; set; }
}
public class ResponseModel
{
    public string Message { get; set; }
    public object Data { get; set; }
}
#endregion