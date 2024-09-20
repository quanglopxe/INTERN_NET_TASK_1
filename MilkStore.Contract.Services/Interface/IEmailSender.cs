using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.Contract.Services.Interface
{
    public interface IEmailSender
    {
        public Task SendMailAsync(string? email, string? subject, string? message);
    }
}
