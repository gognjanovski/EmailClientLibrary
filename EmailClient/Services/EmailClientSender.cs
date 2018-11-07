using EmailClient.Helpers;
using EmailClient.Models;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EmailClient.Services
{
    public class EmailClientSender : IEmailClientSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailClientSender(IOptions<EmailSettings> emailSettings)
        {
            this._emailSettings = emailSettings.Value;
        }


        public async Task SendHelloWorldEmail(string email, string name)
        {
            string template = "Templates.HelloWorldTemplate";

            RazorParser renderer = new RazorParser(typeof(EmailClient).Assembly);
            var body = renderer.UsingTemplateFromEmbedded(template,
                new HelloWorldViewModel { Name = name });

            await SendEmailAsync(email, "Email Subject", body);
        }

        private async Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SendGridClient(_emailSettings.ApiKey);
            var from = new EmailAddress(_emailSettings.From);
            var to = new EmailAddress(email);
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = message;

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            Response response;
            try
            {
                response = await client.SendEmailAsync(msg);
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("{0}", "SendEmailAsync Failed");
            }
        }

    }
}
