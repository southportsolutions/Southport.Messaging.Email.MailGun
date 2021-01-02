using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Southport.Messaging.Email.Core.EmailAttachments;
using Southport.Messaging.Email.Core.Recipient;
using Xunit;
using Xunit.Abstractions;

namespace Southport.Messaging.Email.MailGun.Test
{
    public class MailGunMessageTest : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly IMailGunOptions _options;
        
        private readonly  ITestOutputHelper _output;

        public MailGunMessageTest(ITestOutputHelper output)
        {
            _output = output;
            _httpClient = new HttpClient();
            _options = Startup.GetOptions();
        }
        [Fact]
        public async Task Send_Simple_Message()
        {
            var emailAddress = "test1@southport.solutions";
            var message = new MailGunMessage(_httpClient, _options);
            var responses = await message.AddFromAddress("test2@southport.solutions")
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
        public async Task Send_Simple_Attachment_Message()
        {
            var emailAddress = "test1@southport.solutions";
            var message = new MailGunMessage(_httpClient, _options);
            var responses = await message.AddFromAddress("test2@southport.solutions")
                .AddToAddress(emailAddress)
                .SetSubject("Test Email")
                .AddAttachments(new EmailAttachment()
                {
                    AttachmentFilename = "test.txt",
                    AttachmentType = "text/plain", 
                    Content = "Test attachment content."
                })
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
            var recipient = new EmailRecipient("test1@southport.solutions", new Dictionary<string, object>() {{"name", "John Doe"}, {"states", new List<string> {"CA", "CT", "TN"}}});
            var message = new MailGunMessage(_httpClient, _options);
            var responses = await message.AddFromAddress("test2@southport.solutions")
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

        [Fact]
        public async Task Send_Template_TestEmailAddress_Message()
        {
            var testAddresses = new List<string>() {"test2@southport.solutions", "test3@southport.solutions"};

            var options = new MailGunOptions()
            {
                ApiKey = _options.ApiKey,
                Domain = _options.Domain,
                TestEmailAddresses = string.Join(",", testAddresses)
            };
            var recipient = new EmailRecipient("rob@southportsolutions.com", new Dictionary<string, object>() {{"name", "John Doe"}, {"states", new List<string> {"CA", "CT", "TN"}}});
            var message = new MailGunMessage(_httpClient, options);
            var responses = await message.AddFromAddress("test1@southport.solutions")
                .AddToAddress(recipient)
                .SetSubject("Test - Test Email Address Parameters")
                .SetTemplate("test_template").Send();

            foreach (var response in responses)
            {
                _output.WriteLine(await response.ResponseMessage.Content.ReadAsStringAsync());
                Assert.True(response.ResponseMessage.IsSuccessStatusCode);
                Assert.Contains(testAddresses, s => s.Equals(response.EmailRecipient.EmailAddress.Address));
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
