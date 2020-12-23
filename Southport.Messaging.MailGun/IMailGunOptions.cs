namespace Southport.Messaging.MailGun
{
    public interface IMailGunOptions
    {
        string ApiKey { get; set; }
        string Domain { get; set; }
        string TestEmailAddresses { get; set; }
    }
}