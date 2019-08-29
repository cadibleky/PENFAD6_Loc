namespace PENFAD6DAL.Services
{
    public class MessageFactory
    {
        private MessageFactory(string value) { Value = value; }

        public string Value { get; set; }

        public static MessageFactory TeksolUrl => new MessageFactory("http://192.168.0.100");
        public static MessageFactory TeksolPort => new MessageFactory("5341");
        public static MessageFactory ClientUrl => new MessageFactory("http://localhost");
        public static MessageFactory ClientPort => new MessageFactory("5341");
    }
}