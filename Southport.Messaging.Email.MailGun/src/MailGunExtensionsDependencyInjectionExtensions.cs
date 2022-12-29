using Microsoft.Extensions.Configuration;
using Southport.Messaging.Email.MailGun;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class MailGunExtensionsDependencyInjectionExtensions
{
    public static IServiceCollection AddMailGunServices(this IServiceCollection services, IConfigurationSection section)
    {
        services.Configure<MailGunOptions>(section);
        services.AddHttpClient<IMailGunMessageFactory, MailGunMessageFactory>();
        return services;
    }
    public static IServiceCollection AddExecutionLoggerServices(this IServiceCollection services, MailGunOptions options)
    {
        services.AddSingleton(Options.Options.Create(options));
        services.AddHttpClient<IMailGunMessageFactory, MailGunMessageFactory>();
        return services;
    }
}
