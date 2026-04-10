using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Application.Interfaces.Services
{
    public interface ISMSService
    {
        Task<HttpResponseMessage> SendSMS(string phoneNumber, string message);

        Task<HttpResponseMessage> SendConfirmationSMS(string phoneNumber, string token);

       
    }
}