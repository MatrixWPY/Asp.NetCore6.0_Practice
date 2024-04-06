using StackExchange.Redis;
using static Producer.RedisHelper;

RedisConnection.Init("127.0.0.1:6379");
var redis = RedisConnection.Instance.ConnectionMultiplexer;
var db = redis.GetDatabase();

string queueName = "StreamQueue";
string groupName = "QueueGroup";

#region 前置設定 Consumer Group
// 因需先 StreamCreateConsumerGroup 後再 StreamAdd，後續 StreamReadGroup 才讀的到資料
// 故在 Producer 時先行設置 Consumer 會使用的 GroupName

var hasKey = db.KeyExists(queueName, CommandFlags.None);
if (hasKey == false)
{
    // 此時尚未執行 StreamCreateConsumerGroup，故此筆資料不會被 StreamReadGroup 讀出
    // 此處執行 StreamAdd 目的為使之後的 StreamGroupInfo 可作用
    db.StreamAdd(queueName, "-1", "QueueRoot");
}

var groups = db.StreamGroupInfo(queueName, CommandFlags.None);
if (groups == null || groups?.Any(x => x.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase)) == false)
{
    try
    {
        // 設置 Consumer 會使用的 GroupName
        // 之後執行 StreamAdd 的資料已可被 StreamReadGroup 讀出
        db.StreamCreateConsumerGroup(queueName, groupName, StreamPosition.NewMessages);
    }
    catch
    {
        Console.WriteLine($"Create Consumer Group Fail.");
        Console.ReadKey();
        throw;
    }
}
#endregion

Console.WriteLine("Produce to StreamQueue:");
for (int i = 0; i < 10; i++)
{
    Console.WriteLine($"{i + 1}:Test_{i + 1}");
    db.StreamAdd(queueName, $"{i + 1}", $"Test_{i + 1}");
}

Console.WriteLine("Produce Finish");
Console.ReadKey();