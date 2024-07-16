using WebApiFactory.Commands.Interface;

namespace WebApiFactory.Factories.Interface
{
    public interface ICommandFactory
    {
        IContactInfoCommand CreateContactInfoCommand(string commandType);
    }
}
