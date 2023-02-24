using Microsoft.AspNetCore.Mvc;
using MiniValidation;
using System.ComponentModel.DataAnnotations;

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
    if (MiniValidator.TryValidate(request, out var errors))
    {
        return new ResponseModel { Message = "POST", Data = request };
    }
    return new ResponseModel { Message = "POST", Data = (errors.SelectMany(e => e.Value)) };
});
#endregion
#region HttpPut
app.MapPut("/Test/{id}", ([FromRoute] int id, [FromBody] PutRequestModel request) =>
{
    if (MiniValidator.TryValidate(request, out var errors))
    {
        return new ResponseModel { Message = "PUT", Data = new { id, request } };
    }
    return new ResponseModel { Message = "PUT", Data = (errors.SelectMany(e => e.Value)) };
});
#endregion
#region HttpPatch
app.MapMethods("/Test/{id}", new string[] { "PATCH" }, ([FromRoute] int id, [FromBody] PatchRequestModel request) =>
{
    if (MiniValidator.TryValidate(request, out var errors))
    {
        return new ResponseModel { Message = "PATCH", Data = new { id, request } };
    }
    return new ResponseModel { Message = "PATCH", Data = (errors.SelectMany(e => e.Value)) };
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
    [Required]
    public long? Id { get; set; }
    [Required]
    public string Describe { get; set; }
}
public class PutRequestModel
{
    [Required]
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