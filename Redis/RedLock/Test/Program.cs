using System.Net;
using System.Text;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Task[] tasks = new Task[20];
            for(int n=0; n<tasks.Length; n++)
            {
                tasks[n] = Task.Run(async () =>
                {
                    for (int i = 0; i < 100; i++)
                    {
                        HttpResponseMessage objHttpResponseMessage = await RequestApi("GET", "https://localhost:44348/api/Order", 30, "", "application/json");
                        string strResJson = await objHttpResponseMessage.Content.ReadAsStringAsync();
                        Console.WriteLine(strResJson);
                    }
                });
            }
            Task.WaitAll(tasks);

            Console.ReadKey();
        }

        static async Task<HttpResponseMessage> RequestApi(string strAction, string strUrl, int iTimeout, string strParam, string strParamType)
        {
            using (var client = new HttpClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                client.Timeout = TimeSpan.FromSeconds(iTimeout);

                HttpResponseMessage response = null;
                switch (strAction)
                {
                    case "HEAD":
                        response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, new Uri(strUrl)));
                        break;
                    case "GET":
                        response = await client.GetAsync(strUrl);
                        break;
                    case "POST":
                        response = await client.PostAsync(strUrl, new StringContent(strParam, Encoding.UTF8, strParamType));
                        break;
                }

                return response;
            }
        }
    }
}