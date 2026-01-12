using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using HandlebarsDotNet;
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
        private readonly MailGunMessageFactory _factory;

        public MailGunMessageTest(ITestOutputHelper output)
        {
            _output = output;
            _httpClient = new HttpClient();
            _options = Startup.GetOptions();

            _factory = new MailGunMessageFactory(_httpClient, Options.Create(_options));
        }

        #region Simple Message

        [Fact]
        public async Task Send_Simple_Message()
        {
            var emailAddress = "test1@southport.solutions";
            await using var message = _factory.Create();
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
        public async Task Send_Simple_AttachmentString_Message()
        {
            var emailAddress = "test1@southport.solutions";
            await using var message = _factory.Create();
            var responses = await message
                .SetFromAddress("test2@southport.solutions")
                .AddToAddress(emailAddress)
                .SetSubject("Test Email String Attachment")
                .AddAttachments(new EmailAttachmentString("Test attachment content.","test.txt"))
                .SetText("This is a test email.").Send();
            

            foreach (var response in responses)
            {
                _output.WriteLine(response.Message);
                Assert.True(response.IsSuccessful);
                Assert.Equal(emailAddress, response.EmailRecipient.EmailAddress.Address);
            }
        }

        [Fact]
        public async Task Send_Simple_AttachmentStream_Message()
        {
            await using var stream = await FileHelpers.OpenFileStreamAsync();
            
            var emailAddress = "test1@southport.solutions";
            await using var message = _factory.Create();
            var responses = await message
                .SetFromAddress("test2@southport.solutions")
                .AddToAddress(emailAddress)
                .SetSubject("Test Email Stream Attachment")
                .AddAttachments(new EmailAttachmentStream(stream,"dummy_stream.pdf", "application/pdf"))
                .SetText("This is a test email.").Send();
            

            foreach (var response in responses)
            {
                _output.WriteLine(response.Message);
                Assert.True(response.IsSuccessful);
                Assert.Equal(emailAddress, response.EmailRecipient.EmailAddress.Address);
            }
        }

        [Fact]
        public async Task Send_Simple_AttachmentBytes_Message()
        {
            var stream = await FileHelpers.OpenFileStreamAsync();
            var bytes = await FileHelpers.StreamToBytesAsync(stream);
            
            var emailAddress = "test1@southport.solutions";
            await using var message = _factory.Create();
            var responses = await message
                .SetFromAddress("test2@southport.solutions")
                .AddToAddress(emailAddress)
                .SetSubject("Test Email Byte Attachment")
                .AddAttachments(new EmailAttachmentBytes(bytes,"dummy_bytes.pdf", "application/pdf"))
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
            await using var message = _factory.Create();
            var responses = await message
                .SetFromAddress("test2@southport.solutions")
                .AddToAddress(emailAddress)
                .SetSubject("Test Email")
                .SetText("Dear {{FirstName}} This is a test email.")
                .Send();


            var substitutedText = "Dear Robert This is a test email.";
            foreach (var response in responses)
            {
                _output.WriteLine(response.Message);
                Assert.True(response.IsSuccessful);
                Assert.Equal(emailAddress.EmailAddress.Address, response.EmailRecipient.EmailAddress.Address);
                Assert.Equal(substitutedText, response.MessageBody);
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

            var substitutions = new Dictionary<string, object>() { ["FirstName"] = "Dont User", ["petName"] = "Test Pet" };

            await using var message = _factory.Create();
            var responses = (await message
                .SetFromAddress("test2@southport.solutions")
                .AddToAddresses(emailRecipients)
                .SetSubject("Test Email")
                .SetHtml(html)
                .AddSubstitutions(substitutions)
                .Send()).ToList();

            for (var i = 0; i < responses.Count(); i++)
            {
                var response = responses.ElementAt(i);
                var recipient = emailRecipients.ElementAt(i);
                _output.WriteLine(response.Message);
                Assert.True(response.IsSuccessful);
                Assert.Equal(recipient.EmailAddress.Address, response.EmailRecipient.EmailAddress.Address);

                var compileFunc = Handlebars.Compile(html.Trim());
                var substitutedText = compileFunc(recipient.Substitutions);
                Assert.Equal(substitutedText, response.MessageBody);
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

            await using var message = _factory.Create();
            var responses = (await message
                .SetFromAddress("test2@southport.solutions")
                .AddToAddresses(emailRecipients)
                .SetSubject("Test Email")
                .SetAmpHtml(ampHtml)
                .Send()).ToList();


            for (var i = 0; i < responses.Count(); i++)
            {
                var response = responses.ElementAt(i);
                var recipient = emailRecipients.ElementAt(i);
                _output.WriteLine(response.Message);
                Assert.True(response.IsSuccessful);
                Assert.Equal(recipient.EmailAddress.Address, response.EmailRecipient.EmailAddress.Address);
                
                var compileFunc = Handlebars.Compile(ampHtml);
                var substitutedText = compileFunc(recipient.Substitutions);
                Assert.Equal(substitutedText, response.MessageBody);

            }
        }

        #endregion

        #region Template

        [Fact]
        public async Task Send_Template_Message()
        {
            var recipient = new EmailRecipient("test1@southport.solutions", substitutions: new Dictionary<string, object>() {{"name", "John Doe"}, {"states", new List<string> {"CA", "CT", "TN"}}});
            await using var message = _factory.Create();
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
            await using var message = new MailGunMessage(_httpClient, options);
            var responses = await message.SetFromAddress("test1@southport.solutions")
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
            recipient.Attachments.Add(new EmailAttachmentString(icalString, "calendar.ics", "text/calendar"));
            await using var message = new MailGunMessage(_httpClient, options);
            var responses = await message.SetFromAddress("test1@southport.solutions")
                .AddToAddress(recipient)
                .AddCcAddress("cc@test.com")
                .AddBccAddress("bcc@test.com")
                .AddCustomArgument("message_id", "1234567890")
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
