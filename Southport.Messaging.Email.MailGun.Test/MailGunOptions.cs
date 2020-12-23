using Southport.Messaging.MailGun;

namespace Southport.Messaging.Email.MailGun.Tests
{
    public class MailGunOptions : IMailGunOptions
    {
        public string ApiKey { get; set; }
        public string Domain { get; set; }
        public string TestEmailAddresses { get; set; }
    }
}
