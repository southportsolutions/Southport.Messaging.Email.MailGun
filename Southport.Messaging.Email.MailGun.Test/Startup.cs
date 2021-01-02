using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Southport.Messaging.Email.MailGun.Tests
{
    public static class Startup
    {
        public static IMailGunOptions Options { get; private set; }

        public static IMailGunOptions GetOptions()
        {
            if (Options == null)
            {
                var configurationBuilder = new ConfigurationBuilder()
                    
                    .AddJsonFile(Path.Combine((new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent).ToString(), "appsettings.json"), true)
                    .AddEnvironmentVariables();
                var config = configurationBuilder.Build();
                Options = new MailGunOptions();
                var section = config.GetSection("MailGun");
                section.Bind(Options);

                if (string.IsNullOrWhiteSpace(Options.ApiKey))
                {
                    Options.ApiKey = Environment.GetEnvironmentVariable("MailGunApiKey");
                    Options.Domain = Environment.GetEnvironmentVariable("MailGunDomain");
                }
            }

            return Options;

        }
    }
}
