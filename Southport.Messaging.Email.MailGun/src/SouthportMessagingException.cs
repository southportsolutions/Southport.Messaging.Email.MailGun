using System;

namespace Southport.Messaging.Email.MailGun
{
    public class SouthportMessagingException : Exception
    {
        public SouthportMessagingException(string message) : base(message){}
    }
}
