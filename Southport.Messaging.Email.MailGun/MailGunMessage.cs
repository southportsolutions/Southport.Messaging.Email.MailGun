using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HandlebarsDotNet;
using Newtonsoft.Json;
using Southport.Messaging.Email.Core.EmailAttachments;
using Southport.Messaging.Email.Core.Recipient;
using Southport.Messaging.Email.Core.Result;

namespace Southport.Messaging.Email.MailGun
{
    public class MailGunMessage : IMailGunMessage
    {
        private readonly HttpClient _httpClient;
        private readonly IMailGunOptions _options;
        private List<Stream> _streams = new List<Stream>();

        #region FromAddress
        
        public IEmailAddress FromAddress { get; set; }

        public string From => FromAddress.ToString();

        public IMailGunMessage AddFromAddress(IEmailAddress address)
        {
            FromAddress = address;
            return this;
        }

        public IMailGunMessage AddFromAddress(string address, string name = null)
        {
            FromAddress = new EmailAddress(address, name);
            return this;
        }

        #endregion

        #region ToAddresses
  
        public IEnumerable<IEmailRecipient> ToAddresses { get; set; }

        public IEnumerable<IEmailRecipient> ToAddressesValid => ToAddresses.Where(e => e.EmailAddress.IsValid);
        public IEnumerable<IEmailRecipient> ToAddressesInvalid => ToAddresses.Where(e => e.EmailAddress.IsValid==false);

        //private string To => string.Join(";", ToAddresses);

        public IMailGunMessage AddToAddress(IEmailRecipient address)
        {
            ((List<IEmailRecipient>)ToAddresses).Add(address);
            return this;
        }

        public IMailGunMessage AddToAddress(string address, string name = null)
        {
            return AddToAddress(new EmailRecipient(address, name));
        }

        public IMailGunMessage AddToAddresses(List<IEmailRecipient> addresses)
        {
            ToAddresses = addresses;
            return this;
        }

        #endregion

        #region CcAddresses
        
        public IEnumerable<IEmailRecipient> CcAddresses { get; set; }

        public IEnumerable<IEmailRecipient> CcAddressesValid => CcAddresses.Where(e => e.EmailAddress.IsValid);
        public IEnumerable<IEmailRecipient> CcAddressesInvalid =>  ToAddresses.Where(e => e.EmailAddress.IsValid==false);
        //private string Cc => string.Join(";", CcAddresses);

        public IMailGunMessage AddCcAddress(IEmailRecipient address)
        {
            ((List<IEmailRecipient>) CcAddresses).Add(address);
            return this;
        }

        public IMailGunMessage AddCcAddress(string address, string name = null)
        {
            return AddCcAddress(new EmailRecipient(address, name));
        }

        public IMailGunMessage AddCcAddresses(List<IEmailRecipient> addresses)
        {
            CcAddresses = addresses;
            return this;
        }

        #endregion

        #region BccAddresses
        
        public IEnumerable<IEmailRecipient> BccAddresses { get; set; }

        public IEnumerable<IEmailRecipient> BccAddressesValid => BccAddresses.Where(e => e.EmailAddress.IsValid);
        public IEnumerable<IEmailRecipient> BccAddressesInvalid => BccAddresses.Where(e => e.EmailAddress.IsValid==false);
        //private string Bcc => string.Join(";", BccAddresses);
        
        public IMailGunMessage AddBccAddress(IEmailRecipient address)
        {
            ((List<IEmailRecipient>) BccAddresses).Add(address);
            return this;
        }

        public IMailGunMessage AddBccAddress(string address, string name = null)
        {
            return AddBccAddress(new EmailRecipient(address, name));
        }

        public IMailGunMessage AddBccAddresses(List<IEmailRecipient> addresses)
        {
            BccAddresses = addresses;
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
            Text = text;
            return this;
        }

        #endregion

        #region HTML

        public string Html { get; set; }
        
        public IMailGunMessage SetHtml(string html)
        {
            Html = html;
            return this;
        }

        #endregion

        #region AmpHtml

        public string AmpHtml { get; set; }
        
        public IMailGunMessage SetAmpHtml(string ampHtml)
        {
            AmpHtml = ampHtml;
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

        #region RecipientVariableDictionary

        private Dictionary<string, Dictionary<string, object>> RecipientVariableDictionary { get; set; }

        #endregion


        public MailGunMessage(HttpClient httpClient, IMailGunOptions options, bool tracking = true, bool trackingClicks = true, bool trackingOpens = true)
        {
            _httpClient = httpClient;
            _options = options;
            ToAddresses = new List<IEmailRecipient>();
            CcAddresses = new List<IEmailRecipient>();
            BccAddresses = new List<IEmailRecipient>();
            Attachments = new List<IEmailAttachment>();
            CustomHeaders = new Dictionary<string, string>();
            RecipientVariableDictionary = new Dictionary<string, Dictionary<string, object>>();
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

        public async Task<IEnumerable<IEmailResult>> Send(string domain, CancellationToken cancellationToken = default)
        {
            return await Send(domain, false, cancellationToken);
        }

        private async Task<IEnumerable<IEmailResult>> Send(string domain, bool substitute = false, CancellationToken cancellationToken = default)
        {
            if (FromAddress == null)
            {
                throw new SouthportMessagingException("The from address is required.");
            }

            if (ToAddressesValid.Any()==false && CcAddressesValid.Any()==false && BccAddressesValid.Any()==false)
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
                    var message = new HttpRequestMessage(HttpMethod.Post, $"https://api.mailgun.net/v3/{domain}/messages") {Content = formContent.Value};
                    message.Headers.Authorization = new BasicAuthenticationHeaderValue("api", _options.ApiKey);
                    var responseMessage = await _httpClient.SendAsync(message, cancellationToken);
                    results.Add(new EmailResult(formContent.Key, responseMessage));
                
                    formContent.Value.Dispose();
                }
            }
            finally
            {
                foreach (var stream in _streams)
                {
#if NET5_0 || NETSTANDARD2_1
                    await stream.DisposeAsync();
#else
                    stream.Dispose();
#endif
                }
            }
            
            return results;
        }
        
        public async Task<IEnumerable<IEmailResult>> SubstituteAndSend(CancellationToken cancellationToken = default)
        {
            return await Send(_options.Domain, true, cancellationToken);
        }
        
        public async Task<IEnumerable<IEmailResult>> SubstituteAndSend(string domain, CancellationToken cancellationToken = default)
        {
            return await Send(domain, true, cancellationToken);
        }

        #endregion

        #region GetMultipartFormDataContent

        private Dictionary<IEmailRecipient, MultipartFormDataContent> GetMultipartFormDataContent(bool substitute = false)
        {
            var contents = new Dictionary<IEmailRecipient, MultipartFormDataContent>();
            var toAddresses = GetTestAddresses(ToAddressesValid.ToList());

            if (string.IsNullOrWhiteSpace(_options.TestEmailAddresses)==false)
            {
            }

            foreach (var emailRecipient in toAddresses)
            {
                contents[emailRecipient] = GetMultipartFormDataContent(emailRecipient, substitute);
            }

            return contents;
        }

        private MultipartFormDataContent GetMultipartFormDataContent(IEmailRecipient emailRecipient, bool substitute = false)
        {
            
            // ReSharper disable once UseObjectOrCollectionInitializer
            var content = new MultipartFormDataContent($"----------{Guid.NewGuid():N}");

            #region Addresses

            content.Add(new StringContent(From), "from");
            AddAddressToMultipartForm(emailRecipient, "to", ref content);
            AddAddressesToMultipartForm(CcAddressesValid, "cc", ref content);
            AddAddressesToMultipartForm(BccAddressesValid, "bcc", ref content);

            #endregion

            #region Subject

            content.Add(new StringContent(Subject), "subject");

            #endregion

            #region Text/HTML

            if (string.IsNullOrWhiteSpace(TemplateId))
            {
                Substitute(Text, "text", substitute ? emailRecipient.Substitutions : null, ref content);
                Substitute(Html, "html", substitute ? emailRecipient.Substitutions : null, ref content);
                Substitute(AmpHtml, "amp-html", substitute ? emailRecipient.Substitutions : null, ref content);
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

            foreach (var argument in CustomArguments.Where(c=>emailRecipient.CustomArguments.ContainsKey(c.Key)==false))
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
            if (emailRecipient.Substitutions.Any() == false)
            {
                return content;
            }


            if (string.IsNullOrWhiteSpace(TemplateId) == false)
            {
                var json = JsonConvert.SerializeObject(emailRecipient.Substitutions);
                var stringContent = new StringContent(json , Encoding.UTF8, "application/json");
                content.Add(stringContent, "h:X-Mailgun-Variables");
            }
            else
            {
                var dictionary = new Dictionary<string, object> {[emailRecipient.EmailAddress.Address] = emailRecipient.Substitutions};
                var json = JsonConvert.SerializeObject(dictionary);
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
                CcAddresses = testEmailAddresses.Select(emailAddress => new EmailRecipient(emailAddress.Trim()));
            }

            if (BccAddresses.Any())
            {
                BccAddresses = testEmailAddresses.Select(emailAddress => new EmailRecipient(emailAddress.Trim()));
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

        private void AddAddressesToMultipartForm(IEnumerable<IEmailRecipient> emailAddresses, string key, ref MultipartFormDataContent content)
        {
            
            foreach (var emailAddress in emailAddresses)
            {
                AddAddressToMultipartForm(emailAddress, key, ref content);
            }
        }

        private void AddAddressToMultipartForm(IEmailRecipient emailAddress, string key, ref MultipartFormDataContent content)
        {
            AddStringContent(emailAddress.EmailAddress.ToString(), key, ref content);
            if (emailAddress.Substitutions.Any())
            {
                RecipientVariableDictionary.Add(emailAddress.EmailAddress.Address, emailAddress.Substitutions);
            }
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