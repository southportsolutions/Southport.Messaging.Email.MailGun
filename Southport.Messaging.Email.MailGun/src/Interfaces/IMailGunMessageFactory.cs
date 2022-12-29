using Southport.Messaging.Email.Core;

namespace Southport.Messaging.Email.MailGun
{
    public interface IMailGunMessageFactory : IEmailMessageFactory
    {
        new IMailGunMessage Create();


    }
}