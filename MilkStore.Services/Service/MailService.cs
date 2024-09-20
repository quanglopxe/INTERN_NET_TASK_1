using Castle.Core.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MilkStore.Contract.Services.Interface;
using System.Net.Mail;
using System.Net;
using SendGrid;
using SendGrid.Helpers.Mail;
namespace MilkStore.Services.Service
{
    public class MailService: Contract.Services.Interface.IEmailSender
    {
        private readonly string _apiKey = "8Y2BGRFFKTDCLYRZV71FF3RT";

        public async Task SendMailAsync(string? email, string? subject, string? message)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("chedat0000@gmail.com", "Chí Đạt");
            var to = new EmailAddress(email);
            var plainTextContent = message;
            var htmlContent = $"<strong>{message}</strong>";

            SendGridMessage msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            // Sử dụng await để đợi kết quả của SendEmailAsync
            var response = await client.SendEmailAsync(msg);
        }

    }
}
