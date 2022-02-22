namespace Cryptation
{
    public class Message
    {
        private string username;
        private int messageId;
        private string text;

        public string Username
        {
            get { return username; }
        }

        public int MessageId
        {
            get { return messageId; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        public Message(string username, int messageId, string text)
        {
            this.username = username;
            this.messageId = messageId;
            this.text = text;
        }

        public override string ToString()
        {
            return $"{username} #//# {messageId} #//# {text}".ToString();
        }
    }
}