using System.Net;
using System.Net.Mail;

namespace Web.Areas.Admin.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("cacaohh04@gmail.com", "iugpbldsemkupphu")
            };
            return client.SendMailAsync(
                new MailMessage(from: "cacaohh04@gmail.com",
                to: email, subject: subject, body: message));
        }
    }
}
