using Southport.Messaging.Email.Core;

namespace Southport.Messaging.Email.MailGun
{
    public interface IMailGunMessageFactory : IEmailMessageFactory<IMailGunMessage>
    {
        new IMailGunMessage Create();


    }
}