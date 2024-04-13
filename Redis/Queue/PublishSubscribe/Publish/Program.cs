using static Publish.RedisHelper;

RedisConnection.Init("127.0.0.1:6379");
var redis = RedisConnection.Instance.ConnectionMultiplexer;
var db = redis.GetDatabase();

string channel = "topic.test";
string msg = "Hello World!";

db.Publish(channel, msg);
Console.WriteLine($"{DateTime.Now : yyyy/MM/dd HH:mm:ss.fff} - Published to channel : {channel}");
Console.WriteLine($"{DateTime.Now : yyyy/MM/dd HH:mm:ss.fff} - Sended message : {msg}");

Console.ReadKey();
