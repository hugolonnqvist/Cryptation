using System;

namespace Cryptation
{
    public class NoMessagesException : Exception
    {
        private int messagesAmount;

        public int MessagesAmount
        {
            get { return messagesAmount; }
            set { messagesAmount = value; }
        }

        public NoMessagesException(int messagesAmount)
        {
            this.messagesAmount = messagesAmount;
        }
    }
}