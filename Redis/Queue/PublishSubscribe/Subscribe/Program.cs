using static Subscribe.RedisHelper;

RedisConnection.Init("127.0.0.1:6379");
var redis = RedisConnection.Instance.ConnectionMultiplexer;
var db = redis.GetDatabase();

string channel = "topic.test";

var sub = redis.GetSubscriber();
sub.Subscribe(channel, (chl, msg) =>
{
    Console.WriteLine($"{DateTime.Now : yyyy/MM/dd HH:mm:ss.fff} - Received message : {msg}");
});
Console.WriteLine($"{DateTime.Now : yyyy/MM/dd HH:mm:ss.fff} - Subscribed from channel : {channel}");

Console.ReadKey();
