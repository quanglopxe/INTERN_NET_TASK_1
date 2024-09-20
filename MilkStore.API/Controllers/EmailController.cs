using Castle.Core.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MilkStore.Contract.Services.Interface;
using IEmailSender = MilkStore.Contract.Services.Interface.IEmailSender;
namespace MilkStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        public readonly IEmailSender _emailSender;
        public EmailController(IEmailSender emailSender) 
        {
            this._emailSender = emailSender;
        }
        [HttpGet]
        public async Task<IActionResult> MailSender(string mail, string subject, string message)
        {
            if (mail == null || subject == null || message == null)
            {
                return BadRequest("Không được để trống!!!!");
            }
            else
            {
                try
                {
                    await _emailSender.SendMailAsync(mail, subject, message);
                    return Ok("Đã gửi mail thành công!!!");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

    }
}
