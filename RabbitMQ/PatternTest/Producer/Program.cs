using Producer;

#region Simple 模式
//Send.Simple();
#endregion

#region Worker 模式
//Send.Worker();
#endregion

#region Publish/Subscribe 模式 (ExchangeType: Fanout)
//Send.Fanout();
#endregion

#region Routing 模式 (ExchangeType: Direct)
//Send.Direct();
#endregion

#region Topics 模式 (ExchangeType: Topic)
//Send.Topic();
#endregion

#region RPC 模式
//啟動服務端
RPCServer.RpcHandle();
#endregion

Console.ReadKey();