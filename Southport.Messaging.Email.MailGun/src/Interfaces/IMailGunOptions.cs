namespace Southport.Messaging.Email.MailGun
{
    public interface IMailGunOptions
    {
        string ApiKey { get; set; }
        string Domain { get; set; }
        string TestEmailAddresses { get; set; }
    }

    public class MailGunOptions : IMailGunOptions
    {
        public string ApiKey { get; set; }
        public string Domain { get; set; }
        public string TestEmailAddresses { get; set; }
    }
}