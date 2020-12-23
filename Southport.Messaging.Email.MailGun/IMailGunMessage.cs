using System.Collections.Generic;
using Southport.Messaging.Email.Core;

namespace Southport.Messaging.Email.MailGun
{
    public interface IMailGunMessage : IEmailMessage
    {
        string AmpHtml { get; set; }

        string TemplateVersion { get; set; }
        string TemplateText { get; set; }
        List<string> Tags { get; set; }
        bool? Dkim { get; set; }

        bool RequireTls { get; set; }
        bool SkipVerification { get; set; }
        Dictionary<string, string> CustomHeaders { get; set; }
        MailGunMessage SetAmpHtml(string ampHtml);
        MailGunMessage SetTag(string tag);
        MailGunMessage SetTags(List<string> tags);
        MailGunMessage SetDkim(bool dkim);

        MailGunMessage SetRequireTls(bool requireTls);
        MailGunMessage SetSkipVerification(bool verification);
        MailGunMessage AddHeader(string key, string header);
    }
}