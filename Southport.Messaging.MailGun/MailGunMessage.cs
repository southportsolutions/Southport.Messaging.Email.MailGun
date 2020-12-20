using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Southport.Messaging.MailGun
{
    public class MailGunMessage
    {
        private readonly HttpClient _httpClient;
        private readonly MailGunOptions _options;

        #region FromAddress

        [JsonIgnore]
        public EmailAddress FromAddress { get; set; }

        public string From => FromAddress.ToString();

        public MailGunMessage AddFromAddress(EmailAddress address)
        {
            FromAddress = address;
            return this;
        }

        public MailGunMessage AddFromAddress(string address, string name = null)
        {
            FromAddress = new EmailAddress(address, name);
            return this;
        }

        #endregion

        #region ToAddresses

        [JsonIgnore]    
        public List<EmailAddress> ToAddresses { get; set; }

        public string To => string.Join(";", ToAddresses);

        public MailGunMessage AddToAddress(EmailAddress address)
        {
            ToAddresses.Add(address);
            return this;
        }

        public MailGunMessage AddToAddress(string address, string name = null)
        {
            ToAddresses.Add(new EmailAddress(address, name));
            return this;
        }

        public MailGunMessage AddToAddresses(List<EmailAddress> addresses)
        {
            ToAddresses = addresses;
            return this;
        }

        #endregion

        #region CcAddresses

        [JsonIgnore]
        public List<EmailAddress> CcAddresses { get; set; }
        public string Cc => string.Join(";", CcAddresses);

        public MailGunMessage AddCcAddress(EmailAddress address)
        {
            CcAddresses.Add(address);
            return this;
        }

        public MailGunMessage AddCcAddress(string address, string name = null)
        {
            CcAddresses.Add(new EmailAddress(address, name));
            return this;
        }

        public MailGunMessage AddCcAddresses(List<EmailAddress> addresses)
        {
            CcAddresses = addresses;
            return this;
        }

        #endregion

        #region BccAddresses

        [JsonIgnore]
        public List<EmailAddress> BccAddresses { get; set; }
        public string Bcc => string.Join(";", BccAddresses);
        
        public MailGunMessage AddBccAddress(EmailAddress address)
        {
            BccAddresses.Add(address);
            return this;
        }

        public MailGunMessage AddBccAddress(string address, string name = null)
        {
            BccAddresses.Add(new EmailAddress(address, name));
            return this;
        }

        public MailGunMessage AddBccAddresses(List<EmailAddress> addresses)
        {
            BccAddresses = addresses;
            return this;
        }

        #endregion

        #region Subject

        public string Subject { get; private set; }
        
        public MailGunMessage SetSubject(string subject)
        {
            Subject = subject;
            return this;
        }

        #endregion

        #region Text

        public string Text { get; set; }
        
        public MailGunMessage SetText(string text)
        {
            Text = text;
            return this;
        }

        #endregion

        #region HTML

        public string Html { get; set; }
        
        public MailGunMessage SetHtml(string html)
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

        public List<Attachment> Attachments { get; set; }
        
        public MailGunMessage AddAttachments(Attachment attachment)
        {
            Attachments.Add(attachment);
            return this;
        }

        public MailGunMessage AddAttachments(List<Attachment> attachments)
        {
            Attachments = attachments;
            return this;
        }

        #endregion

        #region Template

        public string Template { get; set; }
        
        public MailGunMessage SetTemplate(string template)
        {
            Template = template;
            return this;
        }

        #endregion

        #region TemplateVersion

        [JsonProperty("t:version")]
        public string TemplateVersion { get; set; }
        
        public MailGunMessage SetTemplateVersion(string templateVersion)
        {
            TemplateVersion = templateVersion;
            return this;
        }

        #endregion

        #region TemplateText

        [JsonProperty("t:text")]
        public string TemplateText { get; set; }
        
        public MailGunMessage SetTemplateText(string templateText)
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
        
        public MailGunMessage SetDeliveryTime(DateTime deliveryTime)
        {
            DeliveryTime = deliveryTime;
            return this;
        }

        #endregion

        #region TestMode

        [JsonProperty("o:testmode")]
        public bool? TestMode { get; set; }

        public MailGunMessage SetTestMode(bool testMode)
        {
            TestMode = testMode;
            return this;
        }

        #endregion

        #region Tracking

        [JsonProperty("o:tracking")]
        public bool Tracking { get; set; }

        public MailGunMessage SetTracking(bool tracking)
        {
            Tracking = tracking;
            return this;
        }

        #endregion

        #region TrackingClicks

        [JsonProperty("o:tracking-clicks")]
        public bool TrackingClicks { get; set; }
        
        public MailGunMessage SetTrackingClicks(bool tracking)
        {
            TrackingClicks = tracking;
            return this;
        }

        #endregion

        #region TrackingOpens

        [JsonProperty("o:tracking-opens")]
        public bool TrackingOpens { get; set; }
        
        public MailGunMessage SetTrackingOpens(bool tracking)
        {
            TrackingOpens = tracking;
            return this;
        }

        #endregion

        #region RequireTls

        [JsonProperty("o:require-tls")]
        public bool? RequireTls { get; set; }
        
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

        #region XMyHeader

        [JsonProperty("h:X-My-Header")]
        public string XMyHeader { get; set; }
        
        public MailGunMessage SetXMyHeader(string header)
        {
            XMyHeader = header;
            return this;
        }

        #endregion

        #region RecipientVariableDictionary

        [JsonIgnore]
        public Dictionary<string, Dictionary<string, string>> RecipientVariableDictionary { get; set; }
        [JsonProperty(PropertyName = "recipient-variables")]
        public string RecipientVariables { get; private set; }
        
        public MailGunMessage AddRecipientVariable(string emailAddress, string key, string value)
        {
            if (RecipientVariableDictionary.ContainsKey(emailAddress) == false)
            {
                RecipientVariableDictionary[emailAddress] = new Dictionary<string, string>();
            }

            var recipientDictionary = RecipientVariableDictionary[emailAddress];
            if (recipientDictionary == null)
            {
                recipientDictionary = new Dictionary<string, string>();
                RecipientVariableDictionary[emailAddress] = recipientDictionary;
            }

            recipientDictionary[key] = value;
            return this;
        }

        #endregion


        public MailGunMessage(HttpClient httpClient, MailGunOptions options)
        {
            _httpClient = httpClient;
            _options = options;
            ToAddresses = new List<EmailAddress>();
            CcAddresses = new List<EmailAddress>();
            BccAddresses = new List<EmailAddress>();
            Attachments = new List<Attachment>();

            Tracking = true;
            TrackingClicks = true;
            TrackingOpens = true;
        }

        public async Task<HttpResponseMessage> Send(CancellationToken cancellationToken = default)
        {
            if (FromAddress == null)
            {
                throw new Exception("The from address is required.");
            }

            if (ToAddresses.Count == 0 && CcAddresses.Count == 0 && BccAddresses.Count == 0)
            {
                throw new Exception("There must be at least 1 recipient.");
            }

            if (string.IsNullOrWhiteSpace(Html) && string.IsNullOrWhiteSpace(Text) && string.IsNullOrWhiteSpace(Template))
            {
                throw new Exception("The message must have a message or reference a template.");
            }

            if (RecipientVariableDictionary.Count > 0)
            {
                var recipientJson = new JObject();
                foreach (var recipient in RecipientVariableDictionary)
                {
                    var recipientJObject = new JObject();
                    recipientJson.Add(new JProperty(recipient.Key, recipientJObject));
                    foreach (var variable in recipient.Value)
                    {
                        recipientJObject.Add(new JProperty(variable.Key, variable.Value));
                    }
                }

                RecipientVariables = recipientJson.ToString();
            }

            //var json = JsonConvert.SerializeObject(this, settings: new JsonSerializerSettings{ContractResolver = new CamelCasePropertyNamesContractResolver()});
            var message = new HttpRequestMessage(HttpMethod.Post, $"https://api.mailgun.net/v3/{_options.Domain}/messages") {Content = new MultipartFormDataContent()};
            message.Headers.Authorization = new BasicAuthenticationHeaderValue("api", _options.ApiKey);

            var responseMessage = await _httpClient.SendAsync(message, cancellationToken);
            if (responseMessage.IsSuccessStatusCode)
            {
                return responseMessage;
            }

            throw new Exception($"Unable to send emails. {responseMessage}");
        }

        private MultipartFormDataContent GetMultipartFormDataContent()
        {
            var content = new MultipartFormDataContent($"----------{Guid.NewGuid():N}");
            content.Add(new StringContent(From), "from");
            foreach (var emailAddress in ToAddresses)
            {
                content.Add(new StringContent(emailAddress.ToString()), "to");
            }
        }

        private Dictionary<string, string> GetDictionary()
        {
            var dictionary = new Dictionary<string, string>
            {
                ["from"] = From,
                ["to"] = To,
                ["subject"] = Subject,
                ["o:testmode"] = TestMode.ToString(),
                ["o:tracking"] = Tracking.ToString(),
                ["o:tracking-clicks"] = TrackingClicks.ToString(),
                ["o:tracking-opens"] = TrackingOpens.ToString(),
                ["o:skip-verification"] = SkipVerification.ToString()
            };


            if (CcAddresses.Any())
            {
                dictionary["cc"] = Cc;
            }

            if (BccAddresses.Any())
            {
                dictionary["bcc"] = Bcc;
            }

            if (string.IsNullOrWhiteSpace(Text) == false)
            {
                dictionary["text"] = Text;
            }

            if (string.IsNullOrWhiteSpace(Html) == false)
            {
                dictionary["html"] = Html;
            }

            if (string.IsNullOrWhiteSpace(Template) == false)
            {
                dictionary["template"] = Template;
                if (RecipientVariableDictionary.Any())
                {
                    var recipientVariables = RecipientVariableDictionary.First();
                    foreach (var variable in recipientVariables.Value)
                    {
                        dictionary[$"v:{variable.Key}"] = $"%recipient.{variable.Key}%";
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(TemplateVersion) == false)
            {
                dictionary["t:version"] = TemplateVersion;
            }

            if (string.IsNullOrWhiteSpace(TemplateText) == false)
            {
                dictionary["t:text"] = TemplateText;
            }

            if (string.IsNullOrWhiteSpace(Tag) == false)
            {
                dictionary["o:tag"] = Tag;
            }

            if (TestMode != null)
            {
                dictionary["o:testmode"] = TestMode.ToString();
            }

            if (Dkim != null)
            {
                dictionary["o:dkim"] = Dkim.ToString();
            }

            if (RequireTls != null)
            {
                dictionary["o:require-tls"] = RequireTls.ToString();
            }

            if (DeliveryTime != null)
            {
                dictionary["o:deliverytime"] = DeliveryTime?.ToString("ddd, dd MMM yyyy hh:mm:ss ") + "GMT";
            }

            if (string.IsNullOrWhiteSpace(XMyHeader) == false)
            {
                dictionary["h:X-My-Header"] = XMyHeader;
            }

            if (string.IsNullOrWhiteSpace(RecipientVariables) == false)
            {
                dictionary["recipient-variables"] = RecipientVariables;
            }

            return dictionary;
        }
    }
}