using WebApiFactory.Commands.Instance;
using WebApiFactory.Commands.Interface;
using WebApiFactory.Factories.Interface;

namespace WebApiFactory.Factories.Instance
{
    public class CommandFactory : ICommandFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IContactInfoCommand CreateContactInfoCommand(string commandType)
        {
            return commandType switch
            {
                "Normal" => _serviceProvider.GetRequiredService<ContactInfoCommand>(),
                "RedisString" => _serviceProvider.GetRequiredService<ContactInfoRedisStringCommand>(),
                "RedisHash" => _serviceProvider.GetRequiredService<ContactInfoRedisHashCommand>(),
                _ => throw new ArgumentException("Invalid CommandType")
            };
        }
    }
}
