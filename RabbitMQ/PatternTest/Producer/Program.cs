using Producer;

string input = string.Empty;

do
{
    Console.Clear();

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

    input = Console.ReadLine();
    Console.Clear();
    Console.WriteLine("[Press any key to menu]");

    switch (input)
    {
        case "1":
            Console.WriteLine("Producer Simple Mode:");
            Console.WriteLine("");

            #region Simple 模式
            Send.Simple();
            #endregion

            Console.ReadKey();
            break;

        case "2":
            Console.WriteLine("Producer Worker Mode:");
            Console.WriteLine("");

            #region Worker 模式
            Send.Worker();
            #endregion

            Console.ReadKey();
            break;

        case "3":
            Console.WriteLine("Producer Publish/Subscribe Mode:");
            Console.WriteLine("");

            #region Publish/Subscribe 模式 (ExchangeType: Fanout)
            Send.Fanout();
            #endregion

            Console.ReadKey();
            break;

        case "4":
            Console.WriteLine("Producer Routing Mode:");
            Console.WriteLine("");

            #region Routing 模式 (ExchangeType: Direct)
            Send.Direct();
            #endregion

            Console.ReadKey();
            break;

        case "5":
            Console.WriteLine("Producer Topics Mode:");
            Console.WriteLine("");

            #region Topics 模式 (ExchangeType: Topic)
            Send.Topic();
            #endregion

            Console.ReadKey();
            break;

        case "6":
            Console.WriteLine("Producer RPC Mode:");
            Console.WriteLine("");

            #region RPC 模式
            //啟動服務端
            RPCServer.RpcHandle();
            #endregion

            Console.ReadKey();
            break;
    }
} while(input.ToLower() != "exit");