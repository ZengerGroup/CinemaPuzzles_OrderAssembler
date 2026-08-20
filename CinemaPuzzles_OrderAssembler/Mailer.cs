using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CinemaPuzzles_OrderAssembler
{
    internal class Mailer
    {
        SmtpClient Client;
        MailMessage Message;
        string JobNumber;
        public Mailer(string jobNumber)
        {
            Client = ConfigureSMTP();
            Message = ConfigureMessage();
            JobNumber = jobNumber;
        }
        public void SendMail(int[] puzzleCounts)
        {
            Message.Body = BuildMessage(puzzleCounts);
            string[] AttachedFiles = Directory.GetFiles(Configurator.Attachments);
            for (int i = 0; i < AttachedFiles.Length; i++)
            {
                if (Path.GetExtension(AttachedFiles[i]) == ".csv")
                {
                    FileStream FS = new FileStream(AttachedFiles[i], FileMode.Open, FileAccess.Read);
                    ContentType CT = new ContentType(MediaTypeNames.Text.Csv);
                    Message.Attachments.Add(new Attachment(FS, Path.GetFileName(AttachedFiles[i]), "text/plain"));
                }
                else
                {
                    FileStream FS = new FileStream(AttachedFiles[i], FileMode.Open, FileAccess.Read);
                    ContentType CT = new ContentType(MediaTypeNames.Application.Pdf);
                    Message.Attachments.Add(new Attachment(FS, Path.GetFileName(AttachedFiles[i]), "application/pdf"));
                }
            }
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            Client.Send(Message);
        }
        private SmtpClient ConfigureSMTP()
        {
            SmtpClient smtp = new SmtpClient("smtp.office365.com");
            smtp.TargetName = "STARTTLS/smtp.office365.com";
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(Configurator.MailAccount, Configurator.MailSecret);
            return smtp;
        }
        private MailMessage ConfigureMessage()
        {
            MailAddress from = new MailAddress(Configurator.MailAccount);
            MailAddress to = new MailAddress(Configurator.MailRecipient);
            //MailAddress to = new MailAddress("tim.owen@zenger.com");
            MailMessage message = new MailMessage(from, to);
            message.Subject = String.Format("Cinema Puzzles batch details for job {0} - {1}.", JobNumber, DateTime.Now.ToString("F"));
            message.IsBodyHtml = true;
            return message;
        }
        private string BuildMessage(int[] puzzleCounts)
        {
            string message = "Please see attached summary reports for todays Cinema Puzzles batch." + Environment.NewLine;
            message += BuildCountsTable(puzzleCounts);
            return message;
        }
        private string BuildCountsTable(int[] puzzleCounts)
        {
            return String.Format("<table><tr><th>Small</th><th>Large</th><th>Big</th><th>Other</th></tr><tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr></table>",
                puzzleCounts[0], puzzleCounts[1], puzzleCounts[2], puzzleCounts[3]);
        }
    }
}
