using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Interfaces;
using Hotel.Infrastruture.Persistence.Context;
using Microsoft.Identity.Client;

namespace Hotel.Infrastruture.Services
{
    public class EmailServerConfiguration : IEmailServerConfiguration
    {
        private readonly GhotelDbContext _context;
        public string Endpoint { get; }
        public string Method { get; }
        public string Server { get; }
        public int Port { get; }
        public string Username { get; }
        public string Password { get; }
        public bool UseSsl { get; }
        public string From { get; }
        public string MessageBody { get; }
        public string MessageSubject { get; }
        public string EmailServiceEnabled { get; }
        public string EmailBodyBeforeChangePassword { get; }
        public string EmailSubjectBeforeChangePassword { get; }
        public string EmailChangePassswordUrl { get; }
        public string EmailConfirmUrl { get; }

        public EmailServerConfiguration(GhotelDbContext context)
        {
            _context = context;
            Endpoint = GetConfigValue("emailApiUri");// "http://admin.pape.gov.ao:3006/api/"
            Server = GetConfigValue("emailSMTPServer"); // "webmail.maptss.gov.ao"
            Port = int.TryParse(GetConfigValue("emailSMTPPort"), out var port) ? port : 0;// "587"
            Username = GetConfigValue("emailUser");
            Password = GetConfigValue("emailPassword");
            Method = GetConfigValue("emailApiSendEmail");// "sendemail"
            From = GetConfigValue("emailFrom");// "sigpape@maptss.gov.ao"
            MessageBody = GetConfigValue("emailBodyRegisteredBO");
            MessageSubject = GetConfigValue("emailSubjectRegisteredBO");
            EmailServiceEnabled = GetConfigValue("emailServiceEnabled");
            EmailBodyBeforeChangePassword = GetConfigValue("emailBodyBeforeChangePassword");
            EmailSubjectBeforeChangePassword = GetConfigValue("emailSubjectBeforeChangePassword");
            EmailChangePassswordUrl = GetConfigValue("emailChangePassswordUrl");
            UseSsl =  bool.Parse(GetConfigValue("useSsl"));
            EmailConfirmUrl = GetConfigValue("emailConfirmUrl");

        }
        private string GetConfigValue(string key)
        {
            return _context.AppConfig
                .Where(entry => entry.Key == key)
                .Select(entry => entry.Value)
                .FirstOrDefault();
        }
    }
}