using AspectCore.DependencyInjection;
using AspectCore.DynamicProxy;
using Newtonsoft.Json;

namespace WebApiFactory.Interceptors
{
    /// <summary>
    /// Log全域攔截器
    /// </summary>
    public class LogGlobalInterceptor : AbstractInterceptorAttribute
    {
        private ILogger<LogGlobalInterceptor> _logger;

        [FromServiceContext]
        public ILogger<LogGlobalInterceptor> logger
        {
            get
            {
                return _logger;
            }
            set
            {
                _logger = value;
            }
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

                var returnValue = context.ImplementationMethod.ReturnType.IsInterface ? context.ReturnValue?.GetType().Name :
                                  context.IsAsync() && context.ImplementationMethod.ReturnType.IsGenericType ? await (dynamic)context.ReturnValue : context.ReturnValue;
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
