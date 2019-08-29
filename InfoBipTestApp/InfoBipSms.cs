using System.Runtime.Serialization;

namespace TeksolServices
{
    [DataContract]
    public class InfoBipSms
    {
        [DataMember(Name = "authentication")]
        public Authentication authentication { get; set; }

        [DataMember(Name = "messages")]
        public Message[] messages { get; set; }
    }

    [DataContract]
    public class Authentication
    {
        [DataMember(Name = "username")]
        public string username { get; set; }

        [DataMember(Name = "password")]
        public string password { get; set; }
    }

    [DataContract]
    public class Message
    {
        [DataMember(Name = "sender")]
        public string sender { get; set; }

        [DataMember(Name = "text")]
        public string text { get; set; }

        [DataMember(Name = "type")]
        public string type { get; set; }

        [DataMember(Name = "ValidityPeriod")]
        public string ValidityPeriod { get; set; }

        [DataMember(Name = "recipients")]
        public Recipient[] recipients { get; set; }
    }

    [DataContract]
    public class Recipient
    {
        [DataMember(Name = "gsm")]
        public string gsm { get; set; }
    }
}