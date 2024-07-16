using WebApiFactory.Factories.Interface;
using WebApiFactory.Services.Instance;
using WebApiFactory.Services.Interface;

namespace WebApiFactory.Factories.Instance
{
    public class ServiceFactory : IServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IContactInfoService CreateContactInfoService(string serviceType)
        {
            return serviceType switch
            {
                "MsSql" => _serviceProvider.GetRequiredService<ContactInfoMssqlService>(),
                "MsSqlSP" => _serviceProvider.GetRequiredService<ContactInfoMssqlSPService>(),
                "MySql" => _serviceProvider.GetRequiredService<ContactInfoMysqlService>(),
                "MySqlSP" => _serviceProvider.GetRequiredService<ContactInfoMysqlSPService>(),
                _ => throw new ArgumentException("Invalid ServiceType")
            };
        }
    }
}
