using System;
using System.IO;
using System.Net;
using MailKit.Net.Smtp;
using MimeKit;
using Newtonsoft.Json;
using Serilog;
using SharedLib.Dto;
using SharedLib.General;

namespace CoreLogicLib.Comm
{
    public static class Communication
    {
        public static void SendAlertEmail(TrackedProduct tracker, string title = null, string msg = null)
        {
            try
            {
                Log.Debug("Attempting to send email alert: [{Tracker}] {Emails}", tracker.FriendlyName, tracker.AlertDestination.Emails);
                if (title == null)
                    title = $"Keyword alert {tracker.FriendlyName}, Go Go Go!";
                if (msg == null)
                    msg = $"Alerting on the tracker for the following page:{Environment.NewLine}{tracker.PageURL}";

                Communication.SendEmail(tracker, title, msg);
                Log.Information("Sent email alert: [{Tracker}] {Emails}", tracker.FriendlyName, tracker.AlertDestination.Emails);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occured on email alert");
            }
        }

        public static void SendAlertWebhookDiscord(TrackedProduct tracker, string _title = null, string _msg = null, string _color = "718317")
        {
            try
            {
                if (_title == null)
                {
                    _title = $"Keyword alert {tracker.FriendlyName}, Go Go Go!";
                }
                if (_msg == null)
                {
                    _msg = $"Alerting on the tracker for the following page:{Environment.NewLine}{tracker.PageURL}";
                }
                string jsonSend = JsonConvert.SerializeObject(new
                {
                    username = "ArbitraryBot",
                    avatar_url = Constants.WebHookAvatarURL,
                    embeds = new[]
                        {
                        new
                        {
                            description = _msg,
                            title = _title,
                            color = _color
                        }
                    },
                    content = $"<@&{tracker.AlertDestination.MentionString}>"
                });

                Log.Debug("Attempting to send webhook", tracker, jsonSend);
                WebRequest request = (HttpWebRequest)WebRequest.Create(tracker.AlertDestination.WebHookURL);
                request.ContentType = "application/json";
                request.Method = "POST";
                using (var sw = new StreamWriter(request.GetRequestStream()))
                {
                    sw.Write(jsonSend);
                }
                Log.Debug("Sent webhook post", tracker, jsonSend);

                var response = (HttpWebResponse)request.GetResponse();
                Log.Information("Posted webhook", tracker, jsonSend, response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failure occured when attempting to send tracker alert webhook", tracker);
            }
        }

        private static void SendEmail(TrackedProduct tracker, string title, string msg)
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(Constants.Config.SMTPEmailName, Constants.Config.SMTPEmailFrom));
            foreach (var address in tracker.AlertDestination.Emails)
            {
                mailMessage.Bcc.Add(new MailboxAddress(address.ToString()));
            }

            mailMessage.Subject = title;
            mailMessage.Body = new TextPart("plain")
            {
                Text = msg
            };

            using var smtpClient = new SmtpClient();
            smtpClient.Connect(Constants.Config.SMTPUrl, Constants.Config.SMTPPort,  MailKit.Security.SecureSocketOptions.Auto);
            smtpClient.Authenticate(Constants.Config.SMTPUsername, Constants.Config.SMTPPassword);
            smtpClient.Send(mailMessage);
            smtpClient.Disconnect(true);
        }

        public static bool SendEmail(string title, string msg, string[] emails)
        {
            try
            {
                var mailMessage = new MimeMessage();
                mailMessage.From.Add(new MailboxAddress(Constants.Config.SMTPEmailName, Constants.Config.SMTPEmailFrom));
                foreach (var address in emails)
                {
                    mailMessage.Bcc.Add(new MailboxAddress(address.ToString()));
                }

                mailMessage.Subject = title;
                mailMessage.Body = new TextPart("plain")
                {
                    Text = msg
                };

                using var smtpClient = new SmtpClient();
                smtpClient.Connect(Constants.Config.SMTPUrl, Constants.Config.SMTPPort, MailKit.Security.SecureSocketOptions.Auto);
                smtpClient.Authenticate(Constants.Config.SMTPUsername, Constants.Config.SMTPPassword);
                smtpClient.Send(mailMessage);
                smtpClient.Disconnect(true);
                return true;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failure occured attempting to send email");
                return false;
            }
        }
    }
}
