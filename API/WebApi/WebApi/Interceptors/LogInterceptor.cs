using AspectCore.DynamicProxy;
using Newtonsoft.Json;

namespace WebApi.Interceptors
{
    /// <summary>
    /// Log攔截器
    /// </summary>
    public class LogInterceptor : AbstractInterceptorAttribute
    {
        private readonly ILogger<LogInterceptor> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public LogInterceptor(ILogger<LogInterceptor> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var logId = Guid.NewGuid().ToString("N");
            try
            {
                var className = context.ImplementationMethod.DeclaringType?.FullName;
                var methodName = context.ImplementationMethod.Name;
                var arguments = context.Parameters;
                _logger.LogInformation($"LogId={logId} , Intercept={className}.{methodName}");
                _logger.LogInformation($"LogId={logId} , Arguments={JsonConvert.SerializeObject(arguments)}");

                await next(context);

                var returnValue = context.IsAsync() && context.ImplementationMethod.ReturnType.IsGenericType
                                ? await (dynamic)context.ReturnValue
                                : context.ReturnValue;
                _logger.LogInformation($"LogId={logId} , Return={JsonConvert.SerializeObject(returnValue)}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"LogId={logId} , Interceptor Exception : {ex}");
                throw;
            }
        }
    }
}
