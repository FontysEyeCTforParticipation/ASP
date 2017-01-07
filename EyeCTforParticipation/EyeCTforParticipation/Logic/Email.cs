using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace EyeCTforParticipation.Logic
{
    public class Email
    {
        SmtpClient client;
        MailMessage mail;
        public Email(string from, string to, string subject, string body)
        {
            client = new SmtpClient();
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "smtp.mailgun.org";
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("<snip>", "<snip>");
            mail = new MailMessage(from, to);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
        }

        public void Send()
        {
            client.Send(mail);
        }
    }
}