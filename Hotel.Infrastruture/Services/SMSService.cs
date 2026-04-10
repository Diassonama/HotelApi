using Newtonsoft.Json;
using Serilog;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hotel.Application.Interfaces;
using Hotel.Application.Interfaces.Services;


namespace Hotel.Infrastruture.Services
{
    public class SMSService : ISMSService
    {
        private readonly IConfigService _config;
        private readonly ILogger _logger;
        string apiUri; 
        string apiSend;
        string apiToken;
        string senderName;
        string smsBodyConfirmation;
        string apiLoginUrl;
        string username;
        string password;
        string apiTestTokenUrl;
        public SMSService(IConfigService config, ILogger logger)
        {
            _config = config;
            _logger = logger;

            apiUri = _config.Getvalue("smsApiUri");
            apiSend = _config.Getvalue("smsApiSendSMS");
            apiToken = _config.Getvalue("smsUserToken");
            senderName = _config.Getvalue("smsSenderName");
            smsBodyConfirmation = _config.Getvalue("smsBodyConfirmation");
            apiLoginUrl = _config.Getvalue("smsApiLogin");
            username = _config.Getvalue("smsApiUsername");
            password = _config.Getvalue("smsApiPassword");
            apiTestTokenUrl = _config.Getvalue("smsApiTestToken");

        }
        public class SMSServiceLoginResult
        {
            public string token { get; set; }
        }

        public class SMSServiceErrorResult
        {
            public string status { get; set; }
            public string details { get; set; }
        }



        public async Task<HttpResponseMessage> SendConfirmationSMS(string phoneNumber, string token)
        {
            try
            {
                string message = smsBodyConfirmation;
                long to = long.Parse(phoneNumber);

                if (!phoneNumber.StartsWith("244"))
                {
                    phoneNumber = "244" + phoneNumber;
                }

                message = string.Format(message, token);

                return await SendSMS(phoneNumber, message);
            }
            catch (Exception e)
            {
                _logger.Error(e, "SMSService - SendConfirmationSMS - Fatal Error");
            }

            return new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.InternalServerError };
        }

        private async Task<HttpResponseMessage> Login()
        {
            try
            {
                if (_config.Getvalue("smsServiceEnabled") != "1")
                {
                    return new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.MethodNotAllowed, ReasonPhrase = "SMS service is disabled" };
                }

                string endpoint = $"{apiUri}{apiLoginUrl}";

                var data = new { username = username, password = password };

                using (var client = new HttpClient())
                {
                    var jsoncontent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(endpoint, jsoncontent);

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.Information(String.Format("SMSService Response = {0} | Request: Login"
                            , response.StatusCode));

                        var tokenResult = JsonConvert.DeserializeObject<SMSServiceLoginResult>(await response.Content.ReadAsStringAsync());

                       /*  if (!await SaveToken(tokenResult.token))
                            throw new Exception("Failed to save token on contextDb"); */
                    }
                    else
                    {
                        var errorContent = JsonConvert.DeserializeObject<SMSServiceErrorResult>(await response.Content.ReadAsStringAsync());

                        _logger.Error(String.Format("SMSService Error = {0} ErrorMessage = {1}: {2} | Request: Login"
                            , response.StatusCode, errorContent?.status, errorContent?.details));
                    }
                    return response;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "SMSService - Login - Fatal Error");
            }

            return new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.InternalServerError };
        }

        private async Task<HttpResponseMessage> TestTokenAndSender()
        {
            try
            {
                if (_config.Getvalue("smsServiceEnabled") != "1")
                {
                    return new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.MethodNotAllowed, ReasonPhrase = "SMS service is disabled" };
                }

                string endpoint = $"{apiUri}{apiTestTokenUrl}?token={apiToken}&sender={senderName}";

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.Information(String.Format("SMSService Response = {0} | Request: TestTokenAndSender"
                            , response.StatusCode));
                    }
                    else
                    {
                        var errorContent = JsonConvert.DeserializeObject<SMSServiceErrorResult>(await response.Content.ReadAsStringAsync());

                        _logger.Error(String.Format("SMSService Error = {0} ErrorMessage = {1}: {2} | Request: TestTokenAndSender"
                            , response.StatusCode, errorContent?.status, errorContent?.details));
                    }
                    return response;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "SMSService - TestTokenAndSender - Fatal Error");
            }

            return new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.InternalServerError };
        }

       /*  private async Task<bool> SaveToken(string token)
        {
            var tokenEntry = _context.AppConfig.FirstOrDefault(entry => entry.Key == AppConfigKeysEnum.smsUserToken.ToString());
            tokenEntry.Value = token;
            return (await _context.SaveChangesAsync(new CancellationToken())) > 0;
        } */

        public async Task<HttpResponseMessage> SendSMS(string phoneNumber, string message)
        {
            try
            {
                if (_config.Getvalue("smsServiceEnabled") != "1")
                {
                    return new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.MethodNotAllowed, ReasonPhrase = "SMS service is disabled" };
                }

                if (!(await TestTokenAndSender()).IsSuccessStatusCode)
                {
                    // If token doesn't work, login to get new token
                    (await Login()).EnsureSuccessStatusCode();

                    // After login, test token again, if it doesn't work, exception is thrown and no message is sent
                    (await TestTokenAndSender()).EnsureSuccessStatusCode();
                }


                //long to = long.Parse(phoneNumber); // ??

                if (!phoneNumber.StartsWith("244"))
                {
                    phoneNumber = "244" + phoneNumber;
                }

                string endpoint = $"{apiUri}{apiSend}?token={apiToken}";

                var data = new { sender = senderName, recipients = phoneNumber, text = message };


                using (var client = new HttpClient())
                {
                    var jsoncontent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(endpoint, jsoncontent);

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.Information(String.Format("SMSService Response = {0} | Request: SendSMS | Parameters: From = {1}, To = {2}, Message = {3}"
                            , response.StatusCode, senderName, phoneNumber, message));
                    }
                    else
                    {
                        var errorContent = JsonConvert.DeserializeObject<SMSServiceErrorResult>(await response.Content.ReadAsStringAsync());

                        _logger.Error(String.Format("SMSService Error = {0} ErrorMessage = {1}: {2} | Request: SendSMS | Parameters: From = {3}, To = {4}, Message = {5}"
                            , response.StatusCode, errorContent?.status, errorContent?.details, senderName, phoneNumber, message));
                    }
                    return response;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "SMSService - SendSMS - Fatal Error");
            }

            return new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.InternalServerError };

        }
    }
}