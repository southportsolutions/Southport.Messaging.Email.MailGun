using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Southport.Messaging.Email.Core.Recipient;
using Xunit;
using Xunit.Abstractions;

namespace Southport.Messaging.Email.MailGun.Tests
{
    public class MailGunMessage_Test : IDisposable
    {
        private HttpClient _httpClient;
        private MailGunOptions _options;
        
        private readonly  ITestOutputHelper _output;

        public MailGunMessage_Test(ITestOutputHelper output)
        {
            _output = output;
            _httpClient = new HttpClient();
            _options = new MailGunOptions()
            {
                ApiKey = "",
                Domain = ""
            };
        }
        [Fact]
        public async Task Send_Simple_Message()
        {
            var emailAddress = "rob@southportsolutions.com";
            var message = new MailGunMessage(_httpClient, _options);
            var responses = await message.AddFromAddress("test@southportsolutions.com")
                .AddToAddress(emailAddress)
                .SetSubject("Test Email")
                .SetText("This is a test email.").Send();
            

            foreach (var response in responses)
            {
                _output.WriteLine(await response.ResponseMessage.Content.ReadAsStringAsync());
                Assert.True(response.ResponseMessage.IsSuccessStatusCode);
                Assert.Equal(emailAddress, response.EmailRecipient.EmailAddress.Address);
            }
        }

        [Fact]
        public async Task Send_Template_Message()
        {
            var recipient = new EmailRecipient("rob@southportsolutions.com", new Dictionary<string, object>() {{"name", "John Doe"}, {"states", new List<string> {"CA", "CT", "TN"}}});
            var message = new MailGunMessage(_httpClient, _options);
            var responses = await message.AddFromAddress("test@southportsolutions.com")
                .AddToAddress(recipient)
                .SetSubject("Test Email")
                .SetTemplate("test_template").Send();

            foreach (var response in responses)
            {
                _output.WriteLine(await response.ResponseMessage.Content.ReadAsStringAsync());
                Assert.True(response.ResponseMessage.IsSuccessStatusCode);
                Assert.Equal(recipient.EmailAddress.Address, response.EmailRecipient.EmailAddress.Address);
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
