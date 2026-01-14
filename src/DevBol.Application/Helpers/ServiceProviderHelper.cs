using Microsoft.Extensions.DependencyInjection;
using System;

public static class RankingUpdateQueue
{
    private static IServiceProvider _serviceProvider;

    public static void Configure(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public static object GetService(Type serviceType)
    {
        return _serviceProvider?.GetService(serviceType);
    }

    public static T GetService<T>()
    {
        return _serviceProvider.GetService<T>();
    }
}
