using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Southport.Messaging.Email.Core;
using Southport.Messaging.Email.Core.EmailAttachments;
using Southport.Messaging.Email.Core.Recipient;
using Southport.Messaging.Email.Core.Result;

namespace Southport.Messaging.MailGun
{
    public class MailGunMessage : IMailGunMessage
    {
        private readonly HttpClient _httpClient;
        private readonly IMailGunOptions _options;

        #region FromAddress

        [JsonIgnore]
        public IEmailAddress FromAddress { get; set; }

        public string From => FromAddress.ToString();

        public IEmailMessage AddFromAddress(IEmailAddress address)
        {
            FromAddress = address;
            return this;
        }

        public IEmailMessage AddFromAddress(string address, string name = null)
        {
            FromAddress = new EmailAddress(address, name);
            return this;
        }

        #endregion

        #region ToAddresses

        [JsonIgnore]    
        public IEnumerable<IEmailRecipient> ToAddresses { get; set; }

        public IEnumerable<IEmailRecipient> ToAddressesValid => ToAddresses.Where(e => e.EmailAddress.Validate());
        public IEnumerable<IEmailRecipient> ToAddressesInvalid => ToAddresses.Where(e => e.EmailAddress.Validate()==false);

        //private string To => string.Join(";", ToAddresses);

        public IEmailMessage AddToAddress(IEmailRecipient address)
        {
            ((List<IEmailRecipient>)ToAddresses).Add(address);
            return this;
        }

        public IEmailMessage AddToAddress(string address, string name = null)
        {
            return AddToAddress(new EmailRecipient(name, address));
        }

        public IEmailMessage AddToAddresses(List<IEmailRecipient> addresses)
        {
            ToAddresses = addresses;
            return this;
        }

        #endregion

        #region CcAddresses

        [JsonIgnore]
        public IEnumerable<IEmailRecipient> CcAddresses { get; set; }

        public IEnumerable<IEmailRecipient> CcAddressesValid => CcAddresses.Where(e => e.EmailAddress.Validate());
        public IEnumerable<IEmailRecipient> CcAddressesInvalid =>  ToAddresses.Where(e => e.EmailAddress.Validate()==false);
        //private string Cc => string.Join(";", CcAddresses);

        public IEmailMessage AddCcAddress(IEmailRecipient address)
        {
            ((List<IEmailRecipient>) CcAddresses).Add(address);
            return this;
        }

        public IEmailMessage AddCcAddress(string address, string name = null)
        {
            return AddCcAddress(new EmailRecipient(name, address));
        }

        public IEmailMessage AddCcAddresses(List<IEmailRecipient> addresses)
        {
            CcAddresses = addresses;
            return this;
        }

        #endregion

        #region BccAddresses

        [JsonIgnore]
        public IEnumerable<IEmailRecipient> BccAddresses { get; set; }

        public IEnumerable<IEmailRecipient> BccAddressesValid => BccAddresses.Where(e => e.EmailAddress.Validate());
        public IEnumerable<IEmailRecipient> BccAddressesInvalid => BccAddresses.Where(e => e.EmailAddress.Validate()==false);
        //private string Bcc => string.Join(";", BccAddresses);
        
        public IEmailMessage AddBccAddress(IEmailRecipient address)
        {
            ((List<IEmailRecipient>) BccAddresses).Add(address);
            return this;
        }

        public IEmailMessage AddBccAddress(string address, string name = null)
        {
            return AddBccAddress(new EmailRecipient(name, address));
        }

        public IEmailMessage AddBccAddresses(List<IEmailRecipient> addresses)
        {
            BccAddresses = addresses;
            return this;
        }

        #endregion

        #region Subject

        public string Subject { get; private set; }
        
        public IEmailMessage SetSubject(string subject)
        {
            Subject = subject;
            return this;
        }

        #endregion

        #region Text

        public string Text { get; set; }
        
        public IEmailMessage SetText(string text)
        {
            Text = text;
            return this;
        }

        #endregion

        #region HTML

        public string Html { get; set; }
        
        public IEmailMessage SetHtml(string html)
        {
            Html = Html;
            return this;
        }

        #endregion

        #region AmpHtml

        public string AmpHtml { get; set; }
        
        public MailGunMessage SetAmpHtml(string ampHtml)
        {
            AmpHtml = ampHtml;
            return this;
        }

        #endregion

        #region Attachments

        public List<IEmailAttachment> Attachments { get; set; }
        
        public IEmailMessage AddAttachments(IEmailAttachment attachment)
        {
            Attachments.Add(attachment);
            return this;
        }

        public IEmailMessage AddAttachments(List<IEmailAttachment> attachments)
        {
            Attachments = attachments;
            return this;
        }

        #endregion

        #region Template

        public string TemplateId { get; set; }
        
        public IEmailMessage SetTemplate(string template)
        {
            TemplateId = template;
            return this;
        }

        #endregion

        #region TemplateVersion

        [JsonProperty("t:version")]
        public string TemplateVersion { get; set; }
        
        public IEmailMessage SetTemplateVersion(string templateVersion)
        {
            TemplateVersion = templateVersion;
            return this;
        }

        #endregion

        #region TemplateText

        [JsonProperty("t:text")]
        public string TemplateText { get; set; }
        
        public IEmailMessage SetTemplateText(string templateText)
        {
            TemplateText = templateText;
            return this;
        }

        #endregion

        #region Tags

        public List<string> Tags { get; set; }
        
        public MailGunMessage SetTag(string tag)
        {
            Tags.Add(tag);
            return this;
        }
        
        public MailGunMessage SetTags(List<string> tags)
        {
            Tags = tags;
            return this;
        }

        #endregion

        #region Dkim

        [JsonProperty("o:dkim")]
        public bool? Dkim { get; set; }
        
        public MailGunMessage SetDkim(bool dkim)
        {
            Dkim = dkim;
            return this;
        }

        #endregion

        #region DeliveryTime

        [JsonProperty("o:deliverytime")]
        public DateTime? DeliveryTime { get; set; }
        
        public IEmailMessage SetDeliveryTime(DateTime deliveryTime)
        {
            DeliveryTime = deliveryTime;
            return this;
        }

        #endregion

        #region TestMode

        [JsonProperty("o:testmode")]
        public bool? TestMode { get; set; }

        public IEmailMessage SetTestMode(bool testMode)
        {
            TestMode = testMode;
            return this;
        }

        #endregion

        #region Tracking

        [JsonProperty("o:tracking")]
        public bool Tracking { get; set; }

        public IEmailMessage SetTracking(bool tracking)
        {
            Tracking = tracking;
            return this;
        }

        #endregion

        #region TrackingClicks

        [JsonProperty("o:tracking-clicks")]
        public bool TrackingClicks { get; set; }
        
        public IEmailMessage SetTrackingClicks(bool tracking)
        {
            TrackingClicks = tracking;
            return this;
        }

        #endregion

        #region TrackingOpens

        [JsonProperty("o:tracking-opens")]
        public bool TrackingOpens { get; set; }

        public IEmailMessage SetTrackingOpens(bool tracking)
        {
            TrackingOpens = tracking;
            return this;
        }

        #endregion

        #region RequireTls

        [JsonProperty("o:require-tls")]
        public bool RequireTls { get; set; }
        
        public MailGunMessage SetRequireTls(bool requireTls)
        {
            RequireTls = requireTls;
            return this;
        }

        #endregion

        #region SkipVerification

        [JsonProperty("o:skip-verification")]
        public bool SkipVerification { get; set; }
        
        public MailGunMessage SetSkipVerification(bool verification)
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

        [JsonProperty("h:X-My-Header")]
        public Dictionary<string, string> CustomHeaders { get; set; }
        
        public MailGunMessage AddHeader(string key, string header)
        {
            CustomHeaders.Add(key, header);
            return this;
        }

        public MailGunMessage AddRecipientVariable(string emailAddress, string key, string value)
        {
            throw new NotImplementedException();
        }

        public IEmailMessage SetReplyTo(string emailAddress)
        {
            CustomHeaders.Add("Reply-To", emailAddress);
            return this;
        }

        public IEmailMessage AddCustomArgument(string key, string value)
        {
            throw new NotImplementedException();
        }

        public IEmailMessage AddCustomArguments(Dictionary<string, string> customArguments)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region RecipientVariableDictionary

        private Dictionary<string, Dictionary<string, object>> RecipientVariableDictionary { get; set; }

        #endregion


        public MailGunMessage(HttpClient httpClient, IMailGunOptions options)
        {
            _httpClient = httpClient;
            _options = options;
            ToAddresses = new List<IEmailRecipient>();
            CcAddresses = new List<IEmailRecipient>();
            BccAddresses = new List<IEmailRecipient>();
            Attachments = new List<IEmailAttachment>();
            CustomHeaders = new Dictionary<string, string>();
            RecipientVariableDictionary = new Dictionary<string, Dictionary<string, object>>();
            Tags = new List<string>();

            Tracking = true;
            TrackingClicks = true;
            TrackingOpens = true;
        }

        public async Task<IEnumerable<IEmailResult>> Send(CancellationToken cancellationToken = default)
        {
            return await Send(_options.Domain, cancellationToken);
        }

        public async Task<IEnumerable<IEmailResult>> Send(string domain, CancellationToken cancellationToken = default)
        {
            if (FromAddress == null)
            {
                throw new SouthportMessagingException("The from address is required.");
            }

            if (ToAddressesValid.Any()==false && CcAddressesValid.Any()==false && BccAddressesValid.Any()==false)
            {
                throw new SouthportMessagingException("There must be at least 1 recipient.");
            }

            if (string.IsNullOrWhiteSpace(Html) && string.IsNullOrWhiteSpace(Text) && string.IsNullOrWhiteSpace(TemplateId))
            {
                throw new SouthportMessagingException("The message must have a message or reference a template.");
            }

            var formContents = GetMultipartFormDataContent();

            var results = new List<IEmailResult>();
            foreach (var formContent in formContents)
            {
                //var json = JsonConvert.SerializeObject(this, settings: new JsonSerializerSettings{ContractResolver = new CamelCasePropertyNamesContractResolver()});
                var message = new HttpRequestMessage(HttpMethod.Post, $"https://api.mailgun.net/v3/{domain}/messages") {Content = formContent.Value};
                message.Headers.Authorization = new BasicAuthenticationHeaderValue("api", _options.ApiKey);
                var responseMessage = await _httpClient.SendAsync(message, cancellationToken);
                results.Add(new EmailResult(formContent.Key, responseMessage));
            }


            return results;
        }

        private Dictionary<IEmailRecipient, MultipartFormDataContent> GetMultipartFormDataContent()
        {
            var contents = new Dictionary<IEmailRecipient, MultipartFormDataContent>();
            var toAddresses = ToAddressesValid;
            if (string.IsNullOrWhiteSpace(_options.TestEmailAddresses) == false)
            {
                var emailAddresses = _options.TestEmailAddresses.Split(',');
                toAddresses = emailAddresses.Select(emailAddress => new EmailRecipient(emailAddress)).ToList();
            }

            foreach (var emailRecipient in toAddresses)
            {
                // ReSharper disable once UseObjectOrCollectionInitializer
                var content = new MultipartFormDataContent($"----------{Guid.NewGuid():N}");

                #region Addresses

                content.Add(new StringContent(From), "from");
                if (string.IsNullOrWhiteSpace(_options.TestEmailAddresses))
                {
                    AddAddressToMultipartForm(emailRecipient, "to", ref content);
                    AddAddressesToMultipartForm(CcAddressesValid, "cc", ref content);
                    AddAddressesToMultipartForm(BccAddressesValid, "bcc", ref content);
                }
                else
                {
                    var emailAddresses = _options.TestEmailAddresses.Split(',');
                    var testAddresses = emailAddresses.Select(emailAddress => new EmailRecipient(emailAddress)).ToList();
                    AddAddressesToMultipartForm(testAddresses, "to", ref content);
                }

                #endregion

                #region Subject

                content.Add(new StringContent(Subject), "subject");

                #endregion

                #region Text/HTML

                AddStringContent(Text, "text", ref content);
                AddStringContent(Html, "html", ref content);
                AddStringContent(AmpHtml, "amp-html", ref content);

                #endregion

                #region Attachments

                foreach (var attachment in Attachments)
                {
                    var streamContent = new StreamContent(attachment.Content);
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

                #region Custom Headers

                foreach (var customHeader in CustomHeaders)
                {
                    AddStringContent(customHeader.Value, $"h:{customHeader.Key}", ref content);
                }

                #endregion

                #region Recipient Variables

                if (emailRecipient.Substitutions.Any())
                {
                    var recipientVariables = JsonConvert.SerializeObject(emailRecipient.Substitutions);
                    var stringContent = new StringContent(recipientVariables, Encoding.UTF8, "application/json");
                    content.Add(stringContent, "h:X-Mailgun-Variables");
                }

                #endregion

                contents[emailRecipient] = content;
            }

            return contents;
        }

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

        //private Dictionary<string, string> GetDictionary()
        //{
        //    var dictionary = new Dictionary<string, string>
        //    {
        //        ["from"] = From,
        //        ["to"] = To,
        //        ["subject"] = Subject,
        //        ["o:testmode"] = TestMode.ToString(),
        //        ["o:tracking"] = Tracking.ToString(),
        //        ["o:tracking-clicks"] = TrackingClicks.ToString(),
        //        ["o:tracking-opens"] = TrackingOpens.ToString(),
        //        ["o:skip-verification"] = SkipVerification.ToString()
        //    };


        //    if (CcAddresses.Any())
        //    {
        //        dictionary["cc"] = Cc;
        //    }

        //    if (BccAddresses.Any())
        //    {
        //        dictionary["bcc"] = Bcc;
        //    }

        //    if (string.IsNullOrWhiteSpace(Text) == false)
        //    {
        //        dictionary["text"] = Text;
        //    }

        //    if (string.IsNullOrWhiteSpace(Html) == false)
        //    {
        //        dictionary["html"] = Html;
        //    }

        //    if (string.IsNullOrWhiteSpace(Template) == false)
        //    {
        //        dictionary["template"] = Template;
        //        if (RecipientVariableDictionary.Any())
        //        {
        //            var recipientVariables = RecipientVariableDictionary.First();
        //            foreach (var variable in recipientVariables.Value)
        //            {
        //                dictionary[$"v:{variable.Key}"] = $"%recipient.{variable.Key}%";
        //            }
        //        }
        //    }

        //    if (string.IsNullOrWhiteSpace(TemplateVersion) == false)
        //    {
        //        dictionary["t:version"] = TemplateVersion;
        //    }

        //    if (string.IsNullOrWhiteSpace(TemplateText) == false)
        //    {
        //        dictionary["t:text"] = TemplateText;
        //    }

        //    if (string.IsNullOrWhiteSpace(Tag) == false)
        //    {
        //        dictionary["o:tag"] = Tag;
        //    }

        //    if (TestMode != null)
        //    {
        //        dictionary["o:testmode"] = TestMode.ToString();
        //    }

        //    if (Dkim != null)
        //    {
        //        dictionary["o:dkim"] = Dkim.ToString();
        //    }

        //    if (RequireTls != null)
        //    {
        //        dictionary["o:require-tls"] = RequireTls.ToString();
        //    }

        //    if (DeliveryTime != null)
        //    {
        //        dictionary["o:deliverytime"] = DeliveryTime?.ToString("ddd, dd MMM yyyy hh:mm:ss ") + "GMT";
        //    }

        //    if (string.IsNullOrWhiteSpace(CustomHeaders) == false)
        //    {
        //        dictionary["h:X-My-Header"] = CustomHeaders;
        //    }

        //    if (string.IsNullOrWhiteSpace(RecipientVariables) == false)
        //    {
        //        dictionary["recipient-variables"] = RecipientVariables;
        //    }

        //    return dictionary;
        //}
    }
}