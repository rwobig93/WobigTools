using SharedLib.Dto;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SharedLib.IO
{
    public class MessageHandler
    {
        private MessagePrefix _messagePrefix;
        private string _messageSeperator;
        private int _messageCountMax;

        public MessageHandler(MessagePrefix messagePrefix = MessagePrefix.Log, string messageSeperator = "~", int messageCountMax = 20)
        {
            _messagePrefix = messagePrefix;
            _messageSeperator = messageSeperator;
            _messageCountMax = messageCountMax;
        }

        private void NofityMessagesChanged() => MessagesChanged?.Invoke();

        public event Action MessagesChanged;
        public List<string> Messages { get; private set; } = new List<string>();
        public void AddMessage(string message)
        {
            switch (_messagePrefix)
            {
                case MessagePrefix.Log:
                    Messages.Add($"{DateTime.Now.ToLocalTime().ToString("s", DateTimeFormatInfo.InvariantInfo)} {_messageSeperator} {message}");
                    break;
                case MessagePrefix.None:
                    Messages.Add($"{_messageSeperator} {message}".TrimStart());
                    break;
                default:
                    break;
            }
            if (Messages.Count > _messageCountMax)
                Messages.RemoveAt(0);
            NofityMessagesChanged();
        }
    }
}
