using static Subscribe.RedisHelper;

RedisConnection.Init("127.0.0.1:6379");
var redis = RedisConnection.Instance.ConnectionMultiplexer;
var db = redis.GetDatabase();

var sub = redis.GetSubscriber();
sub.Subscribe("topic.test", (channel, message) =>
{
    Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff")} - Received message : {message}");
});
Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff")} - Subscribed channel : topic.test ");

Console.ReadKey();