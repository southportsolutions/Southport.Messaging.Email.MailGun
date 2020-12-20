using System;
using System.Text.RegularExpressions;

namespace Southport.Messaging.MailGun
{
    public class EmailAddress
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public EmailAddress(string address, string name = null)
        {
            Address = address;
            Name = name;
        }

        public bool Validate()
        {
            return Validate(Address);
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrWhiteSpace(Name))
            {
                return Address;
            }

            return $"{Name} <{Address}>";
        }

        public const string RegexExpressions = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

        public static bool Validate(string address)
        {
            return Regex.IsMatch(address, RegexExpressions, RegexOptions.IgnoreCase);
        }
    }
}