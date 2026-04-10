
using Hotel.Application.Common;
using Hotel.Infrastruture.Persistence.Context;
using Microsoft.Extensions.Configuration;
using Serilog;
using Newtonsoft.Json;
using System.Text;
using Hotel.Application.Interfaces;
using System.Net.Mail;
using System.Net;
using System.Net.Mime;
using System.IO;
using Hotel.Domain.Identity;

namespace Hotel.Infrastruture.Services
{
    public class EmailService : IEmailService

    {
        private readonly GhotelDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IEmailServerConfiguration _emailServerConfiguration;

        public EmailService(GhotelDbContext context, IConfiguration configuration, ILogger logger, IEmailServerConfiguration emailServerConfiguration)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _emailServerConfiguration = emailServerConfiguration;
        }

        public async Task SendEmail(string to, string subject, string body)
        {
            try
            {
                if (_emailServerConfiguration.EmailServiceEnabled != "1")
                {
                    return;
                }

                // create message
                var email = new SendEmailRequest()
                {
                    From = _emailServerConfiguration.From,
                    Subject = subject,
                    To = new string[] { to },
                    Content = body,
                    EmailConfiguration = new EmailConfiguration()
                    {
                        From = _emailServerConfiguration.From,
                        SmtpServer = _emailServerConfiguration.Server,
                        Port = _emailServerConfiguration.Port,
                        UserName = _emailServerConfiguration.Username,
                        Password = _emailServerConfiguration.Password
                    }
                };

                // send email
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync(_emailServerConfiguration.Endpoint + _emailServerConfiguration.Method, new StringContent(JsonConvert.SerializeObject(email), Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.Information(String.Format("EmailService Response = {0} | Request: sendemail | Parameters: {1}"
                            , response.StatusCode, JsonConvert.SerializeObject(email)));

                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseModel = JsonConvert.DeserializeObject<EmailServiceResult>(responseContent);
                        if (responseModel.success)
                            return;
                        _logger.Error(String.Format("EmailService Error = {0} | Request: sendemail | Parameters: {1}"
                            , response.StatusCode, JsonConvert.SerializeObject(email)));
                        return;

                    }
                    else
                    {
                        _logger.Error(String.Format("EmailService Error = {0} | Request: sendemail | Parameters: {1}"
                            , response.StatusCode, JsonConvert.SerializeObject(email)));
                    }

                    return;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "EmailService - SendEmail - Fatal Error");
            }
        }



        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var client = new SmtpClient
            {
                Host = _emailServerConfiguration.Server,
                Port = _emailServerConfiguration.Port,
                UseDefaultCredentials = false,
                //    DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = _emailServerConfiguration.UseSsl,   //      true,// bool.Parse(smtpSettings["UseSsl"]),
                Credentials = new NetworkCredential(
                    _emailServerConfiguration.From,
                    _emailServerConfiguration.Password
                )
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailServerConfiguration.From, "GHOTEL"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
                //  To = { toEmail }
            };

            mailMessage.To.Add(toEmail);

            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Email sending failed: {ex.Message}");
                _logger.Error(ex, $"EmailService - SendEmail - Fatal Error  {ex.Message}");
            }
            finally
            {
                client.Dispose();
            }
        }

        public async Task SendEmailWithAttachment(string to, string subject, string body, byte[] attachmentBytes, string attachmentFileName, string attachmentContentType = "application/pdf")
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("Destinatário de email inválido.", nameof(to));

            if (attachmentBytes == null || attachmentBytes.Length == 0)
                throw new ArgumentException("Anexo inválido para envio de email.", nameof(attachmentBytes));

            var client = new SmtpClient
            {
                Host = _emailServerConfiguration.Server,
                Port = _emailServerConfiguration.Port,
                UseDefaultCredentials = false,
                EnableSsl = _emailServerConfiguration.UseSsl,
                Credentials = new NetworkCredential(
                    _emailServerConfiguration.From,
                    _emailServerConfiguration.Password
                )
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailServerConfiguration.From, "GHOTEL"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            using var attachmentStream = new MemoryStream(attachmentBytes);
            var attachment = new Attachment(attachmentStream, attachmentFileName, attachmentContentType ?? MediaTypeNames.Application.Pdf);
            mailMessage.Attachments.Add(attachment);

            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "EmailService - SendEmailWithAttachment - Fatal Error");
                throw;
            }
            finally
            {
                client.Dispose();
            }
        }

        public async Task SendEmailAfterRegister(string to, string emailConfirmationToken)
        {
            try
            {
                string currentHost = _emailServerConfiguration.EmailConfirmUrl;
                currentHost = String.Format(currentHost, to, emailConfirmationToken);
                string emailBody = string.Format(_emailServerConfiguration.MessageBody, currentHost);

                //   var confirmationLink = string.Format("localhost:5055/api/Usuario/confirm-email?email={0}&token={1}",to,emailConfirmationToken);

                // string emailBody = string.Format(_emailServerConfiguration.MessageBody, confirmationLink);
                await SendEmailAsync(to, _emailServerConfiguration.MessageSubject, emailBody);
            }
            catch (Exception e)
            {
                _logger.Error(e, "EmailService - SendEmailAfterRegister - Fatal Error");
            }
        }


        public async Task SendEmailBeforeChangePassword(string to, string token)
        {
            try
            {
                string resetUrl = string.Format(_emailServerConfiguration.EmailChangePassswordUrl, token, to);
                string emailBody = string.Format(_emailServerConfiguration.EmailBodyBeforeChangePassword, resetUrl);
                await SendEmail(to, _emailServerConfiguration.EmailSubjectBeforeChangePassword, emailBody);
            }
            catch (Exception e)
            {
                _logger.Error(e, "EmailService - SendEmailBeforeChangePassword - Fatal Error");
            }
        }


        public class SendEmailRequest
        {
            public string From { get; set; }
            public string[] To { get; set; }
            public string Subject { get; set; }
            public string Content { get; set; }
            public EmailConfiguration EmailConfiguration { get; set; }

        }
        public class EmailConfiguration
        {
            public string From { get; set; }
            public string SmtpServer { get; set; }
            public int Port { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }

        }

        public class EmailServiceResult
        {
            public bool success { get; set; }
        }
    }
}