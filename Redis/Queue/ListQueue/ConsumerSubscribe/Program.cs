﻿using static ConsumerSubscribe.RedisHelper;

RedisConnection.Init("127.0.0.1:6379");
var redis = RedisConnection.Instance.ConnectionMultiplexer;
var db = redis.GetDatabase();

string channelName = "ListChannel";
string queueName = "ListQueue";

Console.WriteLine("Subscribe from ListChannel");
var sub = redis.GetSubscriber();
sub.Subscribe(channelName, (chl, msg) =>
{
    Console.WriteLine("Consume from ListQueue:");

    var data = db.ListRightPop(queueName, count: db.ListLength(queueName));
    if (data?.Any() ?? false)
    {
        foreach (var item in data)
        {
            Console.WriteLine(item);
        }
    }

    Console.WriteLine("Consume Finish");
});

Console.ReadKey();
