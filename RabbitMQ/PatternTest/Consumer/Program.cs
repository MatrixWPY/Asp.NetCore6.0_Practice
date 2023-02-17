using Consumer;

#region Simple 模式
//Receive.Simple();
#endregion

#region Worker 模式
//Receive.Worker();
#endregion

#region Publish/Subscribe 模式 (ExchangeType: Fanout)
//Receive.Fanout();
#endregion

#region Routing 模式 (ExchangeType: Direct)
//Receive.Direct();
#endregion

#region Topics 模式 (ExchangeType: Topic)
//Receive.Topic();
#endregion

#region RPC 模式
//實例化客戶端
var rpcClient = new RPCClient();
string message = $"消息id={new Random().Next(1, 1000)}";
Console.WriteLine($"【客戶端】RPC請求中:{message}");

//向服務端發送消息，等待回覆
var response = rpcClient.Call(message);
Console.WriteLine($"【客戶端】收到回覆響應:{response}");

rpcClient.Close();
#endregion

Console.ReadKey();