using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;


namespace WebApplication.Repo
{
    public class MailSenderRepository
    {
        private readonly IConfiguration _configuration;

        public MailSenderRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task<string> Send( string to, string subject, string body)
        {
            string from= _configuration["EmailConfiguration:SmtpUsername"];
            var SMTP= _configuration["EmailConfiguration:SmtpServer"];
            var user= _configuration["EmailConfiguration:SmtpUsername"];
            var psssword= _configuration["EmailConfiguration:SmtpPassword"];
            var result = "";
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = body
            };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect(SMTP, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(user, psssword);
            try
            {
                smtp.Send(email);
                result = "1";
                
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
            finally
            {
                smtp.Disconnect(true);
                
            }

            return Task.FromResult(result);

        }


    }
}
