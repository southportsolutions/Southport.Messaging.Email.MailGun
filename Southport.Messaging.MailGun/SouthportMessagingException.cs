using System;

namespace Southport.Messaging.MailGun
{
    public class SouthportMessagingException : Exception
    {
        public SouthportMessagingException(string message) : base(message){}
    }
}
