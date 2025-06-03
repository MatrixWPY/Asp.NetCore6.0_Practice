using WebApiFactory.Services.Interface;

namespace WebApiFactory.Factories.Interface
{
    public interface IServiceFactory
    {
        IContactInfoService CreateContactInfoService(string serviceType);
    }
}
