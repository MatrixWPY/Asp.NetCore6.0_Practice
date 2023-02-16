using Producer;

// Simple 模式
//Send.Simple();

// Worker 模式
//Send.Worker();

// Publish/Subscribe 模式 (ExchangeType: Fanout)
//Send.Fanout();

// Routing 模式 (ExchangeType: Direct)
//Send.Direct();

// Topics 模式 (ExchangeType: Topic)
Send.Topic();

Console.ReadKey();