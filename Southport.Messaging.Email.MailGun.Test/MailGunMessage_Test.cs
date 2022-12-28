using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Microsoft.Extensions.Options;
using Southport.Messaging.Email.Core.EmailAttachments;
using Southport.Messaging.Email.Core.Recipient;
using Xunit;
using Xunit.Abstractions;

namespace Southport.Messaging.Email.MailGun.Test
{
    public class MailGunMessageTest : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly MailGunOptions _options;
        
        private readonly  ITestOutputHelper _output;
        private readonly MailGunMessageFactory<MailGunOptions> _factory;

        public MailGunMessageTest(ITestOutputHelper output)
        {
            _output = output;
            _httpClient = new HttpClient();
            _options = Startup.GetOptions();

            _factory = new MailGunMessageFactory<MailGunOptions>(_httpClient, Options.Create(_options));
        }

        #region Simple Message

        [Fact]
        public async Task Send_Simple_Message()
        {
            var emailAddress = "test1@southport.solutions";
            var message = _factory.Create();
            var responses = await message
                .SetFromAddress("test2@southport.solutions")
                .AddToAddress(emailAddress)
                .SetSubject("Test Email")
                .SetText("This is a test email.").Send();
            

            foreach (var response in responses)
            {
                _output.WriteLine(response.Message);
                Assert.True(response.IsSuccessful);
                Assert.Equal(emailAddress, response.EmailRecipient.EmailAddress.Address);
            }
        }

        [Fact]
        public async Task Send_Simple_Attachment_Message()
        {
            var emailAddress = "test1@southport.solutions";
            var message = _factory.Create();
            var responses = await message
                .SetFromAddress("test2@southport.solutions")
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
                _output.WriteLine(response.Message);
                Assert.True(response.IsSuccessful);
                Assert.Equal(emailAddress, response.EmailRecipient.EmailAddress.Address);
            }
        }

        #endregion

        #region Message With Sutstituions

        [Fact]
        public async Task Send_Message_Text_WithSubstitutions()
        {
            var emailAddress = new EmailRecipient("test1@southport.solutions", substitutions: new Dictionary<string, object>() {["FirstName"] = "Robert"});
            var message = _factory.Create();
            var responses = await message
                .SetFromAddress("test2@southport.solutions")
                .AddToAddress(emailAddress)
                .SetSubject("Test Email")
                .SetText("Dear {{FirstName}} This is a test email.")
                .Send(true);


            foreach (var response in responses)
            {
                _output.WriteLine(response.Message);
                Assert.True(response.IsSuccessful);
                Assert.Equal(emailAddress.EmailAddress.Address, response.EmailRecipient.EmailAddress.Address);
            }
        }

        [Fact]
        public async Task Send_Message_Html_WithSubstitutions()
        {
            var html = await File.ReadAllTextAsync(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Templates/Html.html"));
            var emailRecipients = new List<IEmailRecipient>()
            {
                new EmailRecipient("test1@southport.solutions", substitutions: new Dictionary<string, object>() {["FirstName"] = "Robert"}),
                new EmailRecipient("test1@southport.solutions", substitutions: new Dictionary<string, object>() {["FirstName"] = "David"})
            };

            var message = _factory.Create();
            var responses = (await message
                .SetFromAddress("test2@southport.solutions")
                .AddToAddresses(emailRecipients)
                .SetSubject("Test Email")
                .SetHtml(html)
                .AddSubstitutions(new Dictionary<string, object>(){["FirstName"]="Dont User", ["petName"]= "Test Pet"})
                .Send(true)).ToList();


            for (var i = 0; i < responses.Count(); i++)
            {
                var response = responses.ElementAt(i);
                var recipient = emailRecipients.ElementAt(i);
                _output.WriteLine(response.Message);
                Assert.True(response.IsSuccessful);
                Assert.Equal(recipient.EmailAddress.Address, response.EmailRecipient.EmailAddress.Address);
            }
        }

        [Fact]
        public async Task Send_Message_AmpHtml_WithSubstitutions()
        {
            var ampHtml = await File.ReadAllTextAsync(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Templates/AmpHtml.html"));
            var emailRecipients = new List<IEmailRecipient>()
            {
                new EmailRecipient("test1@southport.solutions", substitutions: new Dictionary<string, object>() {["FirstName"] = "Robert"}),
                new EmailRecipient("test2@southport.solutions", substitutions: new Dictionary<string, object>() {["FirstName"] = "David"})
            };

            var message = _factory.Create();
            var responses = (await message
                .SetFromAddress("test2@southport.solutions")
                .AddToAddresses(emailRecipients)
                .SetSubject("Test Email")
                .SetAmpHtml(ampHtml).Send(true)).ToList();


            for (var i = 0; i < responses.Count(); i++)
            {
                var response = responses.ElementAt(i);
                var recipient = emailRecipients.ElementAt(i);
                _output.WriteLine(response.Message);
                Assert.True(response.IsSuccessful);
                Assert.Equal(recipient.EmailAddress.Address, response.EmailRecipient.EmailAddress.Address);
            }
        }

        #endregion

        #region Template

        [Fact]
        public async Task Send_Template_Message()
        {
            var recipient = new EmailRecipient("test1@southport.solutions", substitutions: new Dictionary<string, object>() {{"name", "John Doe"}, {"states", new List<string> {"CA", "CT", "TN"}}});
            var message = _factory.Create();
            var responses = await message
                .SetFromAddress("test2@southport.solutions")
                .AddToAddress(recipient)
                .SetSubject("Test Email")
                .SetTemplate("test_template").Send();

            foreach (var response in responses)
            {
                _output.WriteLine(response.Message);
                Assert.True(response.IsSuccessful);
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
            var recipient = new EmailRecipient("rob@southportsolutions.com", substitutions: new Dictionary<string, object>() {{"name", "John Doe"}, {"states", new List<string> {"CA", "CT", "TN"}}});
            var message = new MailGunMessage(_httpClient, options);
            var responses = await message.AddFromAddress("test1@southport.solutions")
                .AddToAddress(recipient)
                .SetSubject("Test - Test Email Address Parameters")
                .SetTemplate("test_template").Send();

            foreach (var response in responses)
            {
                _output.WriteLine(response.Message);
                Assert.True(response.IsSuccessful);
                Assert.Contains(testAddresses, s => s.Equals(response.EmailRecipient.EmailAddress.Address));
            }
        }

        [Fact]
        public async Task Send_Template_WithAttachment_TestEmailAddress_Message()
        {
            var testAddresses = new List<string>() {"test2@southport.solutions", "test3@southport.solutions"};

            var options = new MailGunOptions()
            {
                ApiKey = _options.ApiKey,
                Domain = _options.Domain,
                TestEmailAddresses = string.Join(",", testAddresses)
            };
            
            var timeZone = new VTimeZone("America/Chicago");
            var calendarEvent = new CalendarEvent
            {
                Start = new CalDateTime(DateTime.UtcNow.AddDays(1), timeZone.Location),
                End = new CalDateTime(DateTime.UtcNow.AddDays(1).AddHours(1), timeZone.Location),
                Description = "Test Event",
                Location = "Test Location",
                Summary = "Test Summary",
            };


            var calendar = new Calendar();

            calendar.Events.Add(calendarEvent);

            var serializer = new CalendarSerializer();
            var icalString = serializer.SerializeToString(calendar);

            var recipient = new EmailRecipient("to@test.com", substitutions: new Dictionary<string, object>() {{"name", "John Doe"}, {"states", new List<string> {"CA", "CT", "TN"}}});
            recipient.Attachments.Add(new EmailAttachment()
            {
                AttachmentFilename = "calendar.ics",
                AttachmentType = "text/calendar",
                Content = icalString
            });

            var message = new MailGunMessage(_httpClient, options);
            var responses = await message.AddFromAddress("test1@southport.solutions")
                .AddToAddress(recipient)
                .AddCcAddress("cc@test.com")
                .AddBccAddress("bcc@test.com")
                .SetSubject("Test - Test Email Address Parameters")
                .SetTemplate("test_template").Send();

            foreach (var response in responses)
            {
                _output.WriteLine(response.Message);
                Assert.True(response.IsSuccessful);
                Assert.Contains(testAddresses, s => s.Equals(response.EmailRecipient.EmailAddress.Address));
            }
        }

        #endregion

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
