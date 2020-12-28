namespace Southport.Messaging.Email.MailGun
{
    public interface IMailGunMessageFactory
    {
        IMailGunMessage Create();
    }
}