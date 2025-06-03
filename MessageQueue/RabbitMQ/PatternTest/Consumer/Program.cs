using Consumer;

string input = string.Empty;

do
{
    Console.Clear();

    Console.WriteLine("RabbitMQ Pattern Test");
    Console.WriteLine("");
    Console.WriteLine("Consumer Mode");
    Console.WriteLine("(1)Simple");
    Console.WriteLine("(2)Worker");
    Console.WriteLine("(3)Publish/Subscribe");
    Console.WriteLine("(4)Routing");
    Console.WriteLine("(5)Topics");
    Console.WriteLine("(6)RPC");
    Console.WriteLine("");
    Console.Write("Choise:");

    input = Console.ReadLine();
    Console.Clear();
    Console.WriteLine("[Press any key to menu]");

    switch (input)
    {
        case "1":
            Console.WriteLine("Consumer Simple Mode:");
            Console.WriteLine("");

            #region Simple 模式
            Receive.Simple();
            #endregion

            Console.ReadKey();
            break;

        case "2":
            Console.WriteLine("Consumer Worker Mode:");
            Console.WriteLine("");

            #region Worker 模式
            Receive.Worker();
            #endregion

            Console.ReadKey();
            break;

        case "3":
            Console.WriteLine("Consumer Publish/Subscribe Mode:");
            Console.WriteLine("");

            #region Publish/Subscribe 模式 (ExchangeType: Fanout)
            Receive.Fanout();
            #endregion

            Console.ReadKey();
            break;

        case "4":
            Console.WriteLine("Consumer Routing Mode:");
            Console.WriteLine("");

            #region Routing 模式 (ExchangeType: Direct)
            Receive.Direct();
            #endregion

            Console.ReadKey();
            break;

        case "5":
            Console.WriteLine("Consumer Topics Mode:");
            Console.WriteLine("");

            #region Topics 模式 (ExchangeType: Topic)
            Receive.Topic();
            #endregion

            Console.ReadKey();
            break;

        case "6":
            Console.WriteLine("Consumer RPC Mode:");
            Console.WriteLine("");

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
            break;
    }
} while (input.ToLower() != "exit");