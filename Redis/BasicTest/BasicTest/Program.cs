using BasicTest;
using System.Text.Json;

var connection = new RedisConnection();
var db = connection.Connect("127.0.0.1:6379");
//db = connection.GetDatabase("127.0.0.1:6379");

var Data = (Member)null;
if (db.IsExist("Data"))
{
    Console.WriteLine("Data exist in redis");

    Data = db.Get<Member>("Data");
    Console.WriteLine("Get Data from redis");
}
else
{
    Console.WriteLine("Data not exist in redis");

    var model = new Member()
    {
        Name = "Matrix",
        Level = 1,
        CreateTime = DateTime.Now
    };

    db.Set("Data", model, TimeSpan.FromSeconds(30));
    Console.WriteLine("Set Data to redis");

    Data = db.Get<Member>("Data");
    Console.WriteLine("Get Data from redis");
}
Console.WriteLine(JsonSerializer.Serialize(Data));
Console.ReadKey();

public class Member
{
    public string Name { get; set; }

    public int Level { get; set; }

    public DateTime CreateTime { get; set; }
}