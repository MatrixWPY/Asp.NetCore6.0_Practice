using static Producer.RedisHelper;

RedisConnection.Init("127.0.0.1:6379");
var redis = RedisConnection.Instance.ConnectionMultiplexer;
var db = redis.GetDatabase();

string queueName = "ListQueue";

Console.WriteLine("Produce to ListQueue:");
for (int i = 0; i < 10; i++)
{
    Console.WriteLine($"{i + 1}:Test_{i + 1}");
    db.ListLeftPush(queueName, $"Test_{i + 1}");
}
Console.WriteLine("Produce Finish");
Console.ReadKey();
