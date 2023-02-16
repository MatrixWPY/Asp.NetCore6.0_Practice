using Consumer;

// Simple 模式
//Receive.Simple();

// Worker 模式
//Receive.Worker();

// Publish/Subscribe 模式 (ExchangeType: Fanout)
//Receive.Fanout();

// Routing 模式 (ExchangeType: Direct)
//Receive.Direct();

// Topics 模式 (ExchangeType: Topic)
Receive.Topic();

Console.ReadKey();