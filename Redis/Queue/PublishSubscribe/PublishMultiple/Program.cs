using static PublishMultiple.RedisHelper;

RedisConnection.Init("127.0.0.1:6379");
var redis = RedisConnection.Instance.ConnectionMultiplexer;
var db = redis.GetDatabase();

string[] channelNames = { "channel_1", "channel_2", "channel_3" };

foreach (var channelName in channelNames)
{
    string msg = $"{channelName} Hello World!";

    db.Publish(channelName, msg);

    Console.WriteLine($"{DateTime.Now: yyyy/MM/dd HH:mm:ss.fff} - Published to channel : {channelName}");
    Console.WriteLine($"{DateTime.Now: yyyy/MM/dd HH:mm:ss.fff} - Sended message : {msg}");
}

Console.ReadKey();
