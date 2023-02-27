using Microsoft.IO;

namespace WebApi.Middlewares
{
    /// <summary>
    /// API傳出參數記錄
    /// </summary>
    public class LogResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private readonly ILogger<LogResponseMiddleware> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        public LogResponseMiddleware(RequestDelegate next, ILogger<LogResponseMiddleware> logger)
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
            var originalBodyStream = context.Response.Body;
            await using var responseBody = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBody;

            // 流入 pipeline
            await _next(context);
            // 流出 pipeline

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBodyTxt = await new StreamReader(context.Response.Body).ReadToEndAsync();

            // 排除紀錄SwaggerUI資訊
            if (context.Request.Path.Value.Contains("swagger") == false)
            {
                // 保存傳出參數資訊
                _logger.LogInformation(
                        $"LogId={(string)context.Items["ApiLogId"]} , " +
                        $"ResponseStatus={context.Response.StatusCode} , " +
                        $"ResponseHeader={{{GetHeaders(context.Response.Headers)}}} , " +
                        $"ResponseBody={responseBodyTxt}");
            }

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }

        private static string GetHeaders(IHeaderDictionary headers)
        {
            return string.Join(' ', headers.Select(e => $"{e.Key}:{e.Value}"));
        }
    }

    /// <summary>
    /// 建立 Extension 將此 LogResponseMiddleware 加入 HTTP pipeline
    /// </summary>
    public static class LogResponseMiddlewareExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseLogResponseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LogResponseMiddleware>();
        }
    }
}
