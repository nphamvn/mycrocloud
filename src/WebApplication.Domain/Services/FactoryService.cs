using Microsoft.Extensions.DependencyInjection;

namespace WebApplication.Domain.Services;
// todo なんでもCreateできちゃうが・・
public interface IFactoryService
{
    T Create<T>(params object[] parameters);
}

public class FactoryService : IFactoryService
{
    private readonly IServiceProvider _provider;

    public FactoryService(IServiceProvider provider)
    {
        _provider = provider;
    }

    public T Create<T>(params object[] parameters)
    {
        return ActivatorUtilities.CreateInstance<T>(_provider, parameters);
    }
}