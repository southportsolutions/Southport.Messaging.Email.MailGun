using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Southport.Messaging.MailGun
{
    public class Attachment
    {
        public Stream Content { get; set; }
        public string AttachmentType { get; set; }
        public string AttachmentFilename { get; set; }
    }
}
