using static Consumer.RedisHelper;

RedisConnection.Init("127.0.0.1:6379");
var redis = RedisConnection.Instance.ConnectionMultiplexer;
var db = redis.GetDatabase();

string queueName = "ListQueue";

Console.WriteLine("Consume from ListQueue:");
var data = db.ListRightPop(queueName, count: 5);
if (data?.Any() ?? false)
{
    foreach (var item in data)
    {
        Console.WriteLine(item);
    }
}
Console.WriteLine("Consume Finish");
Console.ReadKey();
