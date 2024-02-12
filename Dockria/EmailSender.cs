using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace WebApp
{
    public class EmailSender : IEmailSender
    {
        private EmailSettings _emailSettings { get; }
        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Execute(email, subject, htmlMessage).Wait();
            return Task.FromResult(0);
        }
        public async Task Execute(string email, string subject, string msg)
        {
            try
            {
                string toEmail = string.IsNullOrEmpty(email) ? _emailSettings.ToEmail : email;
                MailMessage message = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.UsernameEmail, "Docria")
                };
                message.To.Add(new MailAddress(toEmail));
                message.CC.Add(new MailAddress(_emailSettings.CcEmail));
                message.Subject = "Docria : " + subject;
                message.Body = msg;
                message.IsBodyHtml = true;
                message.Priority = MailPriority.High;
                using (SmtpClient smtp = new SmtpClient(_emailSettings.PrimaryDomain, _emailSettings.PrimaryPort))
                {
                    smtp.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {

                string str = ex.Message;
            }
        }
    }
}
