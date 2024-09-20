using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace MilkStore.Services.EmailSettings
{
    public class EmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            // Lấy giá trị từ IOptions
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("MilkStore", _emailSettings.SenderEmail));
            email.To.Add(new MailboxAddress("Customer", toEmail));
            email.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            email.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                // Kết nối với máy chủ SMTP sử dụng STARTTLS
                await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.SenderPassword);
                await smtp.SendAsync(email);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
                smtp.Dispose();
            }
        }
        //public async Task SendOrderStatusUpdateEmail(string customerEmail, string orderId, string orderStatus)
        //{
        //    var subject = $"Trạng thái đơn hàng #{orderId} đã được cập nhật";
        //    var body = $"Kính gửi quý khách hàng, đơn hàng của quý khách đã được cập nhật sang: {orderStatus}. Cảm ơn quý khách đã tin dùng!";

        //    await SendEmailAsync(customerEmail, subject, body);
        //}
        //public async Task SendFeedbackResponseEmail(string customerEmail, string feedback)
        //{
        //    var subject = "Phản hồi ý kiến của quý khách hàng";
        //    var body = $"MilkStore chào quý khách hàng, chúng tôi đã nhận được ý kiến đóng góp của quý khách như sau: {feedback}. Cảm ơn quý khách đã góp ý giúp MilkStore phát triển hơn!" +
        //        $" Chúng tôi sẽ xem xét và cải thiện! Cảm ơn quý khách đã sử dụng sản phẩm của chúng tôi!";

        //    await SendEmailAsync(customerEmail, subject, body);
        //}
    }
}
