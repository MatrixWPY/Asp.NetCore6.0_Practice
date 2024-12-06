using StackExchange.Redis;
using static ProducerPublish.RedisHelper;

RedisConnection.Init("127.0.0.1:6379");
var redis = RedisConnection.Instance.ConnectionMultiplexer;
var db = redis.GetDatabase();

string queueName = "ListQueue";
string channelName = "ListChannel";

Console.WriteLine($"Produce to {queueName}:");
string[] datas = { "Test_1", "Test_2", "Test_3", "Test_4", "Test_5",
                   "Test_6", "Test_7", "Test_8", "Test_9", "Test_10"};
for (int i = 0; i < datas.Length; i++)
{
    Console.WriteLine($"{datas[i]}");
}
db.ListLeftPush(queueName, datas.Select(e => new RedisValue(e)).ToArray());
Console.WriteLine("Produce Finish");

Console.WriteLine($"Publish to {channelName}");
db.Publish(channelName, queueName);

Console.ReadKey();