using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Southport.Messaging.Email.Core;
using Southport.Messaging.Email.Core.Result;

namespace Southport.Messaging.Email.MailGun
{
    public interface IMailGunMessage : IEmailMessage<IMailGunMessage>
    {
        string AmpHtml { get; set; }

        string TemplateVersion { get; set; }
        string TemplateText { get; set; }
        List<string> Tags { get; set; }
        bool? Dkim { get; set; }

        bool RequireTls { get; set; }
        bool SkipVerification { get; set; }
        Dictionary<string, string> CustomHeaders { get; set; }

        IMailGunMessage SetAmpHtml(string ampHtml);
        IMailGunMessage SetTag(string tag);
        IMailGunMessage SetTags(List<string> tags);
        IMailGunMessage SetDkim(bool dkim);

        IMailGunMessage SetRequireTls(bool requireTls);
        IMailGunMessage SetSkipVerification(bool verification);
        IMailGunMessage AddHeader(string key, string header);

        Task<IEnumerable<IEmailResult>> SubstituteAndSend(string domain, CancellationToken cancellationToken = default);
        Task<IEnumerable<IEmailResult>> SubstituteAndSend(CancellationToken cancellationToken = default);

    }
}