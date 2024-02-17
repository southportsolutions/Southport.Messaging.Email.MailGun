using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HandlebarsDotNet;
using Southport.Messaging.Email.Core;
using Southport.Messaging.Email.Core.EmailAttachments;
using Southport.Messaging.Email.Core.Recipient;
using Southport.Messaging.Email.Core.Result;

namespace Southport.Messaging.Email.MailGun
{
    public class MailGunMessage : IMailGunMessage
    {
        private readonly HttpClient _httpClient;
        private readonly IMailGunOptions _options;
        private readonly List<Stream> _streams = new();

        #region FromAddress

        public IEmailAddress FromAddress { get; set; }

        public string From => FromAddress.ToString();

        public IMailGunMessage SetFromAddress(IEmailAddress emailAddress)
        {
            FromAddress = emailAddress;
            return this;
        }

        public IMailGunMessage SetFromAddress(string emailAddress, string name = null)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                throw new ArgumentNullException(nameof(emailAddress), "The email address is required");
            }
            return SetFromAddress(new EmailAddress(emailAddress, name));
        }

        #endregion

        #region ToAddresses
  
        public IEnumerable<IEmailRecipient> ToAddresses { get; set; }

        public IEnumerable<IEmailRecipient> ToAddressesValid => ToAddresses.Where(e => e.EmailAddress.IsValid);
        public IEnumerable<IEmailRecipient> ToAddressesInvalid => ToAddresses.Where(e => !e.EmailAddress.IsValid);

        public IMailGunMessage AddToAddress(IEmailRecipient recipient)
        {
            ((List<IEmailRecipient>)ToAddresses).Add(recipient);
            return this;
        }

        public IMailGunMessage AddToAddress(string emailAddress, string name = null)
        {
            return AddToAddress(new EmailRecipient(emailAddress, name));
        }

        public IMailGunMessage AddToAddresses(List<IEmailRecipient> recipients)
        {
            ((List<IEmailRecipient>)ToAddresses).AddRange(recipients);
            return this;
        }

        #endregion

        #region CcAddresses
        
        public IEnumerable<IEmailAddress> CcAddresses { get; set; }

        public IEnumerable<IEmailAddress> CcAddressesValid => CcAddresses.Where(e => e.IsValid);
        public IEnumerable<IEmailAddress> CcAddressesInvalid =>  CcAddresses.Where(e => !e.IsValid);

        public IMailGunMessage AddCcAddress(IEmailAddress address)
        {
            ((List<IEmailAddress>) CcAddresses).Add(address);
            return this;
        }

        public IMailGunMessage AddCcAddress(string address, string name = null)
        {
            return AddCcAddress(new EmailAddress(address, name));
        }

        public IMailGunMessage AddCcAddresses(List<IEmailAddress> addresses)
        {
            ((List<IEmailAddress>)CcAddresses).AddRange(addresses);
            return this;
        }

        #endregion

        #region BccAddresses
        
        public IEnumerable<IEmailAddress> BccAddresses { get; set; }

        public IEnumerable<IEmailAddress> BccAddressesValid => BccAddresses.Where(e => e.IsValid);
        public IEnumerable<IEmailAddress> BccAddressesInvalid => BccAddresses.Where(e => !e.IsValid);
        
        public IMailGunMessage AddBccAddress(IEmailAddress address)
        {
            ((List<IEmailAddress>) BccAddresses).Add(address);
            return this;
        }

        public IMailGunMessage AddBccAddress(string address, string name = null)
        {
            return AddBccAddress(new EmailAddress(address, name));
        }

        public IMailGunMessage AddBccAddresses(List<IEmailAddress> addresses)
        {
            ((List<IEmailAddress>)BccAddresses).AddRange(addresses);
            return this;
        }

        #endregion

        #region Subject

        public string Subject { get; private set; }
        
        public IMailGunMessage SetSubject(string subject)
        {
            Subject = subject;
            return this;
        }

        #endregion

        #region Text

        public string Text { get; set; }
        
        public IMailGunMessage SetText(string text)
        {
            Text = text?.Trim() ?? "";
            return this;
        }

        #endregion

        #region HTML

        public string Html { get; set; }
        
        public IMailGunMessage SetHtml(string html)
        {
            Html = html?.Trim() ?? "";
            return this;
        }

        #endregion

        #region AmpHtml

        public string AmpHtml { get; set; }
        
        public IMailGunMessage SetAmpHtml(string ampHtml)
        {
            AmpHtml = ampHtml?.Trim() ?? "";
            return this;
        }

        #endregion

        #region Attachments

        public List<IEmailAttachment> Attachments { get; set; }
        
        public IMailGunMessage AddAttachments(IEmailAttachment attachment)
        {
            Attachments.Add(attachment);
            return this;
        }

        public IMailGunMessage AddAttachments(List<IEmailAttachment> attachments)
        {
            Attachments = attachments;
            return this;
        }

        #endregion

        #region Template

        public string TemplateId { get; set; }
        
        public IMailGunMessage SetTemplate(string template)
        {
            TemplateId = template;
            return this;
        }

        #endregion

        #region TemplateVersion
        
        public string TemplateVersion { get; set; }
        
        public IMailGunMessage SetTemplateVersion(string templateVersion)
        {
            TemplateVersion = templateVersion;
            return this;
        }

        #endregion

        #region TemplateText
        
        public string TemplateText { get; set; }
        
        public IMailGunMessage SetTemplateText(string templateText)
        {
            TemplateText = templateText;
            return this;
        }

        #endregion

        #region Tags

        public List<string> Tags { get; set; }
        
        public IMailGunMessage SetTag(string tag)
        {
            Tags.Add(tag);
            return this;
        }
        
        public IMailGunMessage SetTags(List<string> tags)
        {
            Tags = tags;
            return this;
        }

        #endregion

        #region Dkim
        
        public bool? Dkim { get; set; }
        
        public IMailGunMessage SetDkim(bool dkim)
        {
            Dkim = dkim;
            return this;
        }

        #endregion

        #region DeliveryTime
        
        public DateTime? DeliveryTime { get; set; }
        
        public IMailGunMessage SetDeliveryTime(DateTime deliveryTime)
        {
            DeliveryTime = deliveryTime;
            return this;
        }

        #endregion

        #region TestMode
        
        public bool? TestMode { get; set; }

        public IMailGunMessage SetTestMode(bool testMode)
        {
            TestMode = testMode;
            return this;
        }

        #endregion

        #region Tracking
        
        public bool Tracking { get; set; }

        public IMailGunMessage SetTracking(bool tracking)
        {
            Tracking = tracking;
            return this;
        }

        #endregion

        #region TrackingClicks
        
        public bool TrackingClicks { get; set; }
        
        public IMailGunMessage SetTrackingClicks(bool tracking)
        {
            TrackingClicks = tracking;
            return this;
        }

        #endregion

        #region TrackingOpens
        
        public bool TrackingOpens { get; set; }

        public IMailGunMessage SetTrackingOpens(bool tracking)
        {
            TrackingOpens = tracking;
            return this;
        }

        #endregion

        #region RequireTls
        
        public bool RequireTls { get; set; }
        
        public IMailGunMessage SetRequireTls(bool requireTls)
        {
            RequireTls = requireTls;
            return this;
        }

        #endregion

        #region SkipVerification
        
        public bool SkipVerification { get; set; }
        
        public IMailGunMessage SetSkipVerification(bool verification)
        {
            SkipVerification = verification;
            return this;
        }

        #endregion

        #region Custom Variables
        
        public Dictionary<string, string> CustomArguments { get; }

        public MailGunMessage AddCustomVariable(string name, string value)
        {
            CustomArguments.Add(name, value);
            return this;
        }

        #endregion

        #region Substitutions

        public Dictionary<string, object> Substitutions { get; } = new();

        public IMailGunMessage AddSubstitution(string key, object value)
        {
            Substitutions[key] = value;
            return this;
        }

        public IMailGunMessage AddSubstitutions(Dictionary<string, object> substitutions)
        {
            foreach (var substitution in substitutions)
            {
                Substitutions[substitution.Key] = substitution.Value;
            }

            return this;
        }

        #endregion

        #region Custom Headers
        
        public Dictionary<string, string> CustomHeaders { get; set; }
        
        public IMailGunMessage AddHeader(string key, string header)
        {
            CustomHeaders.Add(key, header);
            return this;
        }

        public MailGunMessage AddRecipientVariable(string emailAddress, string key, string value)
        {
            throw new NotImplementedException();
        }

        public IMailGunMessage SetReplyTo(string emailAddress)
        {
            CustomHeaders.Add("Reply-To", emailAddress);
            return this;
        }

        public IMailGunMessage AddCustomArgument(string key, string value)
        {
            throw new NotImplementedException();
        }

        public IMailGunMessage AddCustomArguments(Dictionary<string, string> customArguments)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Core Methods

        IEmailMessageCore IEmailMessageCore.SetFromAddress(string emailAddress, string name)
        {
            return SetFromAddress(emailAddress, name);
        }

        IEmailMessageCore IEmailMessageCore.SetFromAddress(IEmailAddress emailAddress)
        {
            return SetFromAddress(emailAddress);
        }

        IEmailMessageCore IEmailMessageCore.AddToAddress(IEmailRecipient recipient)
        {
            return AddToAddress(recipient);
        }

        IEmailMessageCore IEmailMessageCore.AddToAddress(string emailAddress, string name)
        {
            return AddToAddress(emailAddress, name);
        }

        IEmailMessageCore IEmailMessageCore.AddToAddresses(List<IEmailRecipient> recipients)
        {
            return AddToAddresses(recipients);
        }

        IEmailMessageCore IEmailMessageCore.AddCcAddress(IEmailAddress emailAddress)
        {
            return AddCcAddress(emailAddress);
        }

        IEmailMessageCore IEmailMessageCore.AddCcAddress(string emailAddress, string name)
        {
            return AddCcAddress(emailAddress, name);
        }

        IEmailMessageCore IEmailMessageCore.AddCcAddresses(List<IEmailAddress> emailAddresses)
        {
            return AddCcAddresses(emailAddresses);
        }

        IEmailMessageCore IEmailMessageCore.AddBccAddress(IEmailAddress emailAddress)
        {
            return AddBccAddress(emailAddress);
        }

        IEmailMessageCore IEmailMessageCore.AddBccAddress(string emailAddress, string name)
        {
            return AddBccAddress(emailAddress, name);
        }

        IEmailMessageCore IEmailMessageCore.AddBccAddresses(List<IEmailAddress> emailAddresses)
        {
            return AddBccAddresses(emailAddresses);
        }

        IEmailMessageCore IEmailMessageCore.SetSubject(string subject)
        {
            return SetSubject(subject);
        }

        IEmailMessageCore IEmailMessageCore.SetText(string text)
        {
            return SetText(text);
        }

        IEmailMessageCore IEmailMessageCore.SetHtml(string html)
        {
            return SetHtml(html);
        }

        IEmailMessageCore IEmailMessageCore.AddAttachments(IEmailAttachment attachment)
        {
            return AddAttachments(attachment);
        }

        IEmailMessageCore IEmailMessageCore.AddAttachments(List<IEmailAttachment> attachments)
        {
            return AddAttachments(attachments);
        }

        IEmailMessageCore IEmailMessageCore.SetTemplate(string template)
        {
            return SetTemplate(template);
        }

        IEmailMessageCore IEmailMessageCore.SetDeliveryTime(DateTime deliveryTime)
        {
            return SetDeliveryTime(deliveryTime);
        }

        IEmailMessageCore IEmailMessageCore.SetTestMode(bool testMode)
        {
            return SetTestMode(testMode);
        }

        IEmailMessageCore IEmailMessageCore.SetTracking(bool tracking)
        {
            return SetTracking(tracking);
        }

        IEmailMessageCore IEmailMessageCore.SetTrackingClicks(bool tracking)
        {
            return SetTrackingClicks(tracking);
        }

        IEmailMessageCore IEmailMessageCore.SetTrackingOpens(bool tracking)
        {
            return SetTrackingOpens(tracking);
        }

        IEmailMessageCore IEmailMessageCore.SetReplyTo(string emailAddress)
        {
            return SetReplyTo(emailAddress);
        }

        IEmailMessageCore IEmailMessageCore.AddCustomArguments(Dictionary<string, string> customArguments)
        {
            return AddCustomArguments(customArguments);
        }

        IEmailMessageCore IEmailMessageCore.AddCustomArgument(string key, string value)
        {
            return AddCustomArgument(key, value);
        }

        IEmailMessageCore IEmailMessageCore.AddSubstitution(string key, object value)
        {
            return AddSubstitution(key, value);
        }

        IEmailMessageCore IEmailMessageCore.AddSubstitutions(Dictionary<string, object> substitutions)
        {
            return AddSubstitutions(substitutions);
        }

        #endregion

        public MailGunMessage(HttpClient httpClient, IMailGunOptions options, bool tracking = true, bool trackingClicks = true, bool trackingOpens = true)
        {
            _httpClient = httpClient;
            _options = options;
            ToAddresses = new List<IEmailRecipient>();
            CcAddresses = new List<IEmailAddress>();
            BccAddresses = new List<IEmailAddress>();
            Attachments = new List<IEmailAttachment>();
            CustomHeaders = new Dictionary<string, string>();
            CustomArguments = new Dictionary<string, string>();
            Tags = new List<string>();

            Tracking = tracking;
            TrackingClicks = trackingClicks;
            TrackingOpens = trackingOpens;
        }

        #region Send

        public async Task<IEnumerable<IEmailResult>> Send(CancellationToken cancellationToken = default)
        {
            return await Send(_options.Domain, cancellationToken);
        }

        public async Task<IEnumerable<IEmailResult>> Send(bool substitute = true, CancellationToken cancellationToken = default)
        {
            return await Send(_options.Domain, substitute, cancellationToken);
        }

        public async Task<IEnumerable<IEmailResult>> Send(string domain, CancellationToken cancellationToken = default)
        {
            return await Send(domain, true, cancellationToken);
        }

        private async Task<IEnumerable<IEmailResult>> Send(string domain, bool substitute = true, CancellationToken cancellationToken = default)
        {
            if (FromAddress == null)
            {
                throw new SouthportMessagingException("The from address is required.");
            }

            if (!ToAddressesValid.Any() && !CcAddressesValid.Any() && !BccAddressesValid.Any())
            {
                throw new SouthportMessagingException("There must be at least 1 recipient.");
            }

            if (string.IsNullOrWhiteSpace(Html) && string.IsNullOrWhiteSpace(Text) && string.IsNullOrWhiteSpace(AmpHtml) && string.IsNullOrWhiteSpace(TemplateId))
            {
                throw new SouthportMessagingException("The message must have a message or reference a template.");
            }

            var formContents = GetMultipartFormDataContent(substitute);

            var results = new List<IEmailResult>();
            try
            {
                foreach (var formContent in formContents)
                {
                    var message = new HttpRequestMessage(HttpMethod.Post, $"https://api.mailgun.net/v3/{domain}/messages") { Content = formContent.Value };
                    message.Headers.Authorization = new BasicAuthenticationHeaderValue("api", _options.ApiKey);
                    var responseMessage = await _httpClient.SendAsync(message, cancellationToken);
                    var result = new EmailResult(formContent.Key, responseMessage.IsSuccessStatusCode, await responseMessage.Content.ReadAsStringAsync(cancellationToken));
                    results.Add(result);

                    formContent.Value.Dispose();
                }
            }
            finally
            {
                foreach (var stream in _streams)
                {
                    await stream.DisposeAsync();
                }
            }

            return results;
        }

        #endregion

        #region GetMultipartFormDataContent

        private Dictionary<IEmailRecipient, MultipartFormDataContent> GetMultipartFormDataContent(bool substitute = false)
        {
            var contents = new Dictionary<IEmailRecipient, MultipartFormDataContent>();
            var toAddresses = GetTestAddresses(ToAddressesValid.ToList());

            foreach (var emailRecipient in toAddresses)
            {
                contents[emailRecipient] = GetMultipartFormDataContent(emailRecipient, substitute);
            }

            return contents;
        }

        private MultipartFormDataContent GetMultipartFormDataContent(IEmailRecipient emailRecipient, bool substitute = false)
        {
            var substitutions = emailRecipient.Substitutions ?? new Dictionary<string, object>();
            foreach (var substitution in Substitutions.Where(s => !substitutions.ContainsKey(s.Key)))
            {
                substitutions[substitution.Key] = substitution.Value;
            }

            // ReSharper disable once UseObjectOrCollectionInitializer
            var content = new MultipartFormDataContent($"----------{Guid.NewGuid():N}");

            #region Addresses

            content.Add(new StringContent(From), "from");
            AddAddressToMultipartForm(emailRecipient, "to", ref content);
            AddAddressesToMultipartForm(CcAddressesValid, "cc", ref content);
            AddAddressesToMultipartForm(BccAddressesValid, "bcc", ref content);

            #endregion

            #region Subject

            Substitute(Subject, "subject", substitute ? emailRecipient.Substitutions : null, ref content);

            #endregion

            #region Text/HTML

            if (string.IsNullOrWhiteSpace(TemplateId))
            {
                Substitute(Text, "text", substitute ? substitutions : null, ref content);
                Substitute(Html, "html", substitute ? substitutions : null, ref content);
                Substitute(AmpHtml, "amp-html", substitute ? substitutions : null, ref content);
            }

            #endregion

            #region Attachments

            //global attachments
            foreach (var attachment in Attachments)
            {
                var streamContent = new StreamContent(GetStream(attachment.Content));
                streamContent.Headers.Add("Content-Type", attachment.AttachmentType);
                content.Add(streamContent, "attachment", attachment.AttachmentFilename);
            }

            //recipient specific attachments
            foreach (var attachment in emailRecipient.Attachments)
            {
                var streamContent = new StreamContent(GetStream(attachment.Content));
                streamContent.Headers.Add("Content-Type", attachment.AttachmentType);
                content.Add(streamContent, "attachment", attachment.AttachmentFilename);
            }

            #endregion

            #region Template

            AddStringContent(TemplateId, "template", ref content);
            AddStringContent(TemplateVersion, "t:version", ref content);
            AddStringContent(TemplateText, "t:text", ref content);

            #endregion

            #region Tags

            foreach (var tag in Tags)
            {
                AddStringContent(tag, "o:tag", ref content);
            }

            #endregion

            #region Dkim

            if (Dkim != null)
            {
                var value = Dkim == true ? "yes" : "no";
                AddStringContent(value, "o:dkim", ref content);
            }

            #endregion

            #region DeliveryTime

            if (DeliveryTime != null)
            {
                var value = DeliveryTime.Value.ToString("R");
                AddStringContent(value, "o:deliverytime", ref content);
            }

            #endregion

            #region TestMode

            if (TestMode == true)
            {
                AddStringContent("yes", "o:testmode", ref content);
            }

            #endregion

            #region Tracking

            AddStringContent(Tracking ? "yes" : "no", "o:tracking", ref content);
            AddStringContent(TrackingClicks ? "yes" : "no", "o:tracking-clicks", ref content);
            AddStringContent(TrackingOpens ? "yes" : "no", "o:tracking-opens", ref content);

            #endregion

            #region Security (TLS/Cert Verification)

            AddStringContent(RequireTls ? "yes" : "no", "o:require-tls", ref content);
            AddStringContent(SkipVerification ? "yes" : "no", "o:skip-verification", ref content);

            #endregion

            #region Custom Arguments

            foreach (var argument in emailRecipient.CustomArguments)
            {
                AddStringContent(argument.Value, $"v:{argument.Key}", ref content);
            }

            foreach (var argument in CustomArguments.Where(c=>!emailRecipient.CustomArguments.ContainsKey(c.Key)))
            {
                AddStringContent(argument.Value, $"v:{argument.Key}", ref content);
            }

            #endregion

            #region Custom Headers

            foreach (var customHeader in CustomHeaders)
            {
                AddStringContent(customHeader.Value, $"h:{customHeader.Key}", ref content);
            }

            #endregion

            #region Recipient Variables
            if (!substitutions.Any())
            {
                return content;
            }


            if (!string.IsNullOrWhiteSpace(TemplateId))
            {
                var json = JsonSerializer.Serialize(substitutions);
                var stringContent = new StringContent(json , Encoding.UTF8, "application/json");
                content.Add(stringContent, "h:X-Mailgun-Variables");
            }
            else
            {
                var dictionary = new Dictionary<string, object> {[emailRecipient.EmailAddress.Address] = substitutions};
                var json = JsonSerializer.Serialize(dictionary);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                content.Add(stringContent, "recipient-variables");
            }


            #endregion

            return content;
        }

        #endregion

        #region Helpers

        private void Substitute(string text, string key, Dictionary<string, object> substitutions, ref MultipartFormDataContent content)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            if (substitutions != null && substitutions.Any())
            {
                var compileFunc = Handlebars.Compile(text);
                text = compileFunc(substitutions);
            }
            AddStringContent(text, key, ref content);
        }

        private IEnumerable<IEmailRecipient> GetTestAddresses(IEnumerable<IEmailRecipient> toAddresses)
        {
            if (string.IsNullOrWhiteSpace(_options.TestEmailAddresses))
            {
                return toAddresses;
            }

            var testEmailAddresses = _options.TestEmailAddresses.Split(',');
            var toAddressesTemp = new List<IEmailRecipient>();
            foreach (var toAddress in toAddresses)
            {
                var customArgs = toAddress.CustomArguments;
                customArgs["IsTest"] = "true";

                toAddressesTemp.AddRange(testEmailAddresses.Select(emailAddress => new EmailRecipient(emailAddress.Trim(), substitutions: toAddress.Substitutions,  customArguments: toAddress.CustomArguments, attachments: toAddress.Attachments)));
            }

            if (CcAddresses.Any())
            {
                CcAddresses = testEmailAddresses.Select(emailAddress => new EmailAddress(emailAddress.Trim()));
            }

            if (BccAddresses.Any())
            {
                BccAddresses = testEmailAddresses.Select(emailAddress => new EmailAddress(emailAddress.Trim()));
            }

            toAddresses = toAddressesTemp;

            return toAddresses;
        }

        private Stream GetStream(string content)
        {
            var stream = new MemoryStream();
            var sw = new StreamWriter(stream, Encoding.UTF8);
            sw.Write(content);
            sw.Flush();//otherwise you are risking empty stream
            stream.Seek(0, SeekOrigin.Begin);
            _streams.Add(stream);
            return stream;
        }

        #endregion

        #region MultipartForm Helper Methods

        private void AddAddressesToMultipartForm(IEnumerable<IEmailAddress> emailAddresses, string key, ref MultipartFormDataContent content)
        {
            foreach (var emailAddress in emailAddresses)
            {
                AddAddressToMultipartForm(emailAddress, key, ref content);
            }
        }

        private void AddAddressToMultipartForm(IEmailAddress emailAddress, string key, ref MultipartFormDataContent content)
        {
            AddStringContent(emailAddress.ToString(), key, ref content);
        }

        private void AddAddressToMultipartForm(IEmailRecipient emailRecipient, string key, ref MultipartFormDataContent content)
        {
            AddAddressToMultipartForm(emailRecipient.EmailAddress, key, ref content);
        }

        private void AddStringContent(string stringContent, string key, ref MultipartFormDataContent content)
        {
            if (string.IsNullOrWhiteSpace(stringContent))
            {
                return;
            }

            content.Add(new StringContent(stringContent), key);
        }

        #endregion
    }
}