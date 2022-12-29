using System.Net.Http;
using Microsoft.Extensions.Options;
using Southport.Messaging.Email.Core;

namespace Southport.Messaging.Email.MailGun
{
    public class MailGunMessageFactory : IMailGunMessageFactory
    {
        private readonly HttpClient _httpClient;
        private readonly IMailGunOptions _options;

        public MailGunMessageFactory(HttpClient httpClient, IOptions<MailGunOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }


        public IMailGunMessage Create()
        {
            return new MailGunMessage(_httpClient, _options);
        }

        IEmailMessageCore IEmailMessageFactory.Create()
        {
            return Create();
        }
    }
}
