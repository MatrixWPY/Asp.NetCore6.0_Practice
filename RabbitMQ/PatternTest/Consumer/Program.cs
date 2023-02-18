using Consumer;

string input = string.Empty;
bool showReturnMsg = true;

do
{
    showReturnMsg = true;

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

    switch (input = Console.ReadLine().ToLower())
    {
        case "1":
            Console.Clear();
            Console.Write("Consumer Simple Mode:");
            Console.WriteLine("");

            #region Simple 模式
            Receive.Simple();
            #endregion
            break;

        case "2":
            Console.Clear();
            Console.Write("Consumer Worker Mode:");
            Console.WriteLine("");

            #region Worker 模式
            Receive.Worker();
            #endregion
            break;

        case "3":
            Console.Clear();
            Console.Write("Consumer Publish/Subscribe Mode:");
            Console.WriteLine("");

            #region Publish/Subscribe 模式 (ExchangeType: Fanout)
            Receive.Fanout();
            #endregion
            break;

        case "4":
            Console.Clear();
            Console.Write("Consumer Routing Mode:");
            Console.WriteLine("");

            #region Routing 模式 (ExchangeType: Direct)
            Receive.Direct();
            #endregion
            break;

        case "5":
            Console.Clear();
            Console.Write("Consumer Topics Mode:");
            Console.WriteLine("");

            #region Topics 模式 (ExchangeType: Topic)
            Receive.Topic();
            #endregion
            break;

        case "6":
            Console.Clear();
            Console.Write("Consumer RPC Mode:");
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
            break;

        default:
            Console.Clear();
            showReturnMsg = false;
            break;
    }

    if (showReturnMsg)
    {
        Console.WriteLine("");
        Console.Write("Return to menu? (Y/N):");
        input = Console.ReadLine().ToLower();
        if (input == "y")
        {
            Console.Clear();
        }
        else
        {
            input = "exit";
        }
    }
} while (input != "exit");