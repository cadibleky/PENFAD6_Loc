using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Polly;
using Serilog;

namespace TeksolServices
{
    [Guid("78BF0F6C-0A71-4CD5-950B-E8333F852442")]
    public class TeksolSmsHandler
    {
        private readonly ILogger _log;
        private readonly string _message;
        private List<string> _phoneList;

        public TeksolSmsHandler()
        {
            // empty constructor
            _log = CustomLogger();
        }

        public TeksolSmsHandler(List<string> phoneList, string message)
        {
            _phoneList = phoneList;
            _message = message;
            _log = CustomLogger();
        }

        public void SendSmsUsingInfoBipChannel()
        {
            var policy = Policy.Handle<Exception>()
                .WaitAndRetry(Convert.ToInt32(ConfigurationManager.AppSettings["retryTime"]),
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            policy.Execute(SendSms);
        }

        private void SendSms()
        {
            using (var t = new Task(SendSmsAsync))
            {
                t.Start();
                _log.Information("Sending SMS via InfoBip  Channel...");
            }
        }

        private async void SendSmsAsync()
        {
            try
            {
                // Create HttpClient instance to use for the app
                using (var aClient = new HttpClient())
                {
                    // Uri is where we are posting to:
                    var theUri = new Uri("https://api.infobip.com/api/v3/sendsms/json");

                    // I have to let the server know I accept json data when it replies so this will add that header
                    aClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    aClient.DefaultRequestHeaders.Host = theUri.Host;

                    // Class that will be serialized into Json and posted
                    // Setup login credentials here
                    var auth = new Authentication
                    {
                        username = ConfigurationManager.AppSettings["username"],
                        password = ConfigurationManager.AppSettings["password"]
                    };

                    // Set up mobile numbers here
                    var rList = new List<Recipient>();
                    rList.AddRange(_phoneList.Select(p => new Recipient {gsm = $"{p}"}));

                    // Set up message parameters here
                    var message = new Message
                    {
                        sender = ConfigurationManager.AppSettings["senderName"],
                        text = _message,
                        type = "longSMS",
                        ValidityPeriod = ConfigurationManager.AppSettings["validityPeriod"],
                        recipients = rList.ToArray()
                    };

                    var infobip = new InfoBipSms
                    {
                        authentication = auth,
                        messages = new[] {message}
                    };

                    // Create a Json Serializer for our type
                    var jsonSer = new DataContractJsonSerializer(typeof(InfoBipSms));

                    // use the serializer to write the object to a MemoryStream
                    using (var ms = new MemoryStream())
                    {
                        jsonSer.WriteObject(ms, infobip);
                        ms.Position = 0;

                        // use a Stream reader to construct the StringContent (Json)
                        using (var sr = new StreamReader(ms))
                        {
                            using (
                                var theContent = new StringContent(sr.ReadToEnd(), Encoding.UTF8, @"application/json"))
                            {
                                // use the HTTP client to POST some content ( ‘theContent’ not yet defined).
                                // Post the data
                                var aResponse = await aClient.PostAsync(theUri, theContent);

                                if (aResponse.IsSuccessStatusCode)
                                {
                                    _log.Information($"Status code is {aResponse.StatusCode}");
                                    using (var content = aResponse.Content)
                                    {
                                        // ... Read the string.
                                        var result = await content.ReadAsStringAsync();

                                        // ... Display the result.
                                        if (result != null &&
                                            result.Length >= 5)
                                        {
                                            _log.Information($"{result}...");
                                        }
                                    }
                                }
                                else
                                {
                                    // show the response status code
                                    _log.Error($"HTTP Status: {aResponse.StatusCode} - Reason: {aResponse.ReasonPhrase}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Exception ---> {ex}");
                var log = new LoggerConfiguration()
                    .WriteTo.Email("", "", "", mailSubject: "",
                        networkCredential:
                            new NetworkCredential
                            {
                                Domain = "",
                                Password = "",
                                SecurePassword = new SecureString(),
                                UserName = ""
                            })
                    .CreateLogger();
            }
        }

        public string ExecuteSms(string username, string password, string mobileNumber, string senderName,
            string validityPeriod, string messageToSend)
        {
            try
            {
                _phoneList = new List<string> {mobileNumber};

                // Create HttpClient instance to use for the app
                using (var aClient = new HttpClient())
                {
                    // Uri is where we are posting to:
                    var theUri = new Uri("https://api.infobip.com/api/v3/sendsms/json");

                    // I have to let the server know I accept json data when it replies so this will add that header
                    aClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    aClient.DefaultRequestHeaders.Host = theUri.Host;

                    // Class that will be serialized into Json and posted
                    // Setup login credentials here
                    var auth = new Authentication {username = username, password = password};

                    // Set up mobile numbers here
                    var rList = new List<Recipient>();
                    rList.AddRange(_phoneList.Select(p => new Recipient {gsm = $"{p}"}));

                    // Set up message parameters here
                    var message = new Message
                    {
                        sender = senderName,
                        text = messageToSend,
                        type = "longSMS",
                        ValidityPeriod = validityPeriod,
                        recipients = rList.ToArray()
                    };

                    var infobip = new InfoBipSms
                    {
                        authentication = auth,
                        messages = new[] {message}
                    };

                    // Create a Json Serializer for our type
                    var jsonSer = new DataContractJsonSerializer(typeof(InfoBipSms));

                    // use the serializer to write the object to a MemoryStream
                    using (var ms = new MemoryStream())
                    {
                        jsonSer.WriteObject(ms, infobip);
                        ms.Position = 0;

                        // use a Stream reader to construct the StringContent (Json)
                        using (var sr = new StreamReader(ms))
                        {
                            using (
                                var theContent = new StringContent(sr.ReadToEnd(), Encoding.UTF8, @"application/json"))
                            {
                                // use the HTTP client to POST some content ( ‘theContent’ not yet defined).
                                // Post the data
                                var aResponse = aClient.PostAsync(theUri, theContent);

                                using (var content = aResponse.Result)
                                {
                                    // ... Read the string.
                                    if (content != null)
                                    {
                                        var result = content.StatusCode;
                                        // ... Display the result.
                                        _log.Information($"{result}...");
                                        return Convert.ToString(result);
                                    }
                                }

                                // show the response status code
                                // _log.Error($"HTTP Status: {aResponse.StatusCode} - Reason: {aResponse.ReasonPhrase}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Exception ---> {ex}");
                var log = new LoggerConfiguration()
                    .WriteTo.Email("", "", "", mailSubject: "",
                        networkCredential:
                            new NetworkCredential
                            {
                                Domain = "",
                                Password = "",
                                SecurePassword = new SecureString(),
                                UserName = ""
                            })
                    .CreateLogger();
                return ex.Message;
            }
            return null;
        }

        public static ILogger CustomLogger()
        {
            var log = new LoggerConfiguration()
                .WriteTo.RollingFile(@"C:\programData\TeksolSmsLogs\SmsLog-{Date}.txt")
                .WriteTo.ColoredConsole()
                .CreateLogger();
            return log;
        }
    }
}