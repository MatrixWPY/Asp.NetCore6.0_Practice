using Microsoft.IO;

namespace WebApi.Middlewares
{
    /// <summary>
    /// API傳入參數記錄
    /// </summary>
    public class LogRequestMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private readonly ILogger<LogRequestMiddleware> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        public LogRequestMiddleware(RequestDelegate next, ILogger<LogRequestMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering();
            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            await context.Request.Body.CopyToAsync(requestStream);

            // 產生唯一的 LogId 串起 Request & Response 兩筆 log 資料
            context.Items["ApiLogId"] = GetLogId();

            // 保存傳入參數資訊
            _logger.LogInformation(
                    $"LogId={(string)context.Items["ApiLogId"]} , " +
                    $"Url={context.Request.Scheme}://{context.Request.Host.ToUriComponent()}{context.Request.Path}{context.Request.QueryString} , " +
                    $"RequestHeader={{{GetHeaders(context.Request.Headers)}}} , " +
                    $"RequestBody={ReadStreamInChunks(requestStream)}");

            context.Request.Body.Position = 0;
            await _next(context);
        }

        private static string GetLogId()
        {
            // LogId = DateTime(yyyyMMddhhmmssfff) + 1-UpperCase + 2-Digits
            var random = new Random();
            return $"{DateTime.Now:yyyyMMddhhmmssfff}{(char)random.Next('A', 'A' + 26)}{random.Next(10, 99)}";
        }

        private static string GetHeaders(IHeaderDictionary headers)
        {
            return string.Join(' ', headers.Select(e => $"{e.Key}:{e.Value}"));
        }

        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;
            stream.Seek(0, SeekOrigin.Begin);
            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);
            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;
            do
            {
                readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);
            return textWriter.ToString();
        }
    }

    /// <summary>
    /// 建立 Extension 將此 LogRequestMiddleware 加入 HTTP pipeline
    /// </summary>
    public static class LogRequestMiddlewareExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseLogRequestMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LogRequestMiddleware>();
        }
    }
}
