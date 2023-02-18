using Producer;

string input = string.Empty;
bool showReturnMsg = true;

do
{
    showReturnMsg = true;

    Console.WriteLine("RabbitMQ Pattern Test");
    Console.WriteLine("");
    Console.WriteLine("Producer Mode");
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
            Console.Write("Producer Simple Mode:");
            Console.WriteLine("");

            #region Simple 模式
            Send.Simple();
            #endregion
            break;

        case "2":
            Console.Clear();
            Console.Write("Producer Worker Mode:");
            Console.WriteLine("");

            #region Worker 模式
            Send.Worker();
            #endregion
            break;

        case "3":
            Console.Clear();
            Console.Write("Producer Publish/Subscribe Mode:");
            Console.WriteLine("");

            #region Publish/Subscribe 模式 (ExchangeType: Fanout)
            Send.Fanout();
            #endregion
            break;

        case "4":
            Console.Clear();
            Console.Write("Producer Routing Mode:");
            Console.WriteLine("");

            #region Routing 模式 (ExchangeType: Direct)
            Send.Direct();
            #endregion
            break;

        case "5":
            Console.Clear();
            Console.Write("Producer Topics Mode:");
            Console.WriteLine("");

            #region Topics 模式 (ExchangeType: Topic)
            Send.Topic();
            #endregion
            break;

        case "6":
            Console.Clear();
            Console.Write("Producer RPC Mode:");
            Console.WriteLine("");

            #region RPC 模式
            //啟動服務端
            RPCServer.RpcHandle();
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
} while(input != "exit");