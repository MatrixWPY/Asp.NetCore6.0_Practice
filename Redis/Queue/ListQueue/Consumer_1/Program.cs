using static Consumer_1.RedisHelper;

RedisConnection.Init("127.0.0.1:6379");
var redis = RedisConnection.Instance.ConnectionMultiplexer;
var db = redis.GetDatabase();

string queueName = "ListQueue";

Console.WriteLine("Consume from ListQueue:");
while (true)
{
    var data = db.ListRightPop(queueName, count: 5);
    if (data?.Any() ?? false)
    {
        foreach (var item in data)
        {
            Console.WriteLine(item);
        }
        Console.WriteLine();
    }
}
