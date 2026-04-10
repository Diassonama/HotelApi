using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Application.Common
{
    public interface IEmailService
    {
        Task SendEmail(string to, string subject, string body);

        Task SendEmailWithAttachment(string to, string subject, string body, byte[] attachmentBytes, string attachmentFileName, string attachmentContentType = "application/pdf");

        Task SendEmailAfterRegister(string to, string emailConfirmationToken);

        //void SendEmailBeforeChangeEmail(string to, string emailConfirmationToken);

        //void SendEmailAfterChangeEmail(string to);

        Task SendEmailBeforeChangePassword(string to, string token);
    }
}