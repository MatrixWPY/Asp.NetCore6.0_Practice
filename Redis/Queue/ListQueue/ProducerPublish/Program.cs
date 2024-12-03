using static ProducerPublish.RedisHelper;

RedisConnection.Init("127.0.0.1:6379");
var redis = RedisConnection.Instance.ConnectionMultiplexer;
var db = redis.GetDatabase();

string queueName = "ListQueue";
string channelName = "ListChannel";

Console.WriteLine($"Produce to {queueName}:");
for (int i = 0; i < 10; i++)
{
    Console.WriteLine($"{i + 1}:Test_{i + 1}");
    db.ListLeftPush(queueName, $"Test_{i + 1}");
}
Console.WriteLine("Produce Finish");

Console.WriteLine($"Publish to {channelName}");
db.Publish(channelName, queueName);

Console.ReadKey();