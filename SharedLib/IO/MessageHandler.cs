using Serilog;
using SharedLib.Dto;
using SharedLib.General;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SharedLib.IO
{
    public class MessageHandler
    {
        private readonly MessagePrefix _messagePrefix;
        private readonly string _messageSeperator;
        private readonly int _messageCountMax;

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

        public static void Save()
        {
            try
            {
                if (!Directory.Exists(Constants.PathSavedData))
                {
                    Log.Debug($"SavedData path doesn't exist, attempting to create dir: {Constants.PathSavedData}");
                    Directory.CreateDirectory(Constants.PathSavedData);
                    Log.Information($"Created missing config dir: {Constants.PathSavedData}");
                }

                var saveFile = OSDynamic.GetFilePath(Constants.PathLogs, "AuditLog.log");

                Log.Debug($"MessageHandler saveFile = {saveFile}");
                if (File.Exists(saveFile))
                {
                    Log.Debug("Attempting to save over MessageHandler file");
                }
                else
                {
                    Log.Debug("Attempting to save a new MessageHandler file");
                }
                File.WriteAllText(saveFile, Constants.WatcherAuditLogs.Messages.ToString());
                Log.Debug("Successfully saved MessageHandler file");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to save MessageHandler file");
            }
        }
    }
}
