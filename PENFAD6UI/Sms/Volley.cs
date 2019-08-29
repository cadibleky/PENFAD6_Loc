using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SendSMS;
using Message = System.Web.Services.Description.Message;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.Repository.CRM.Employee;
using PENFAD6DAL.Repository.CRM.Employer;
using PENFAD6DAL.Repository.Setup.SystemSetup;

namespace IBankWebService.Utils
{
    public class Volley
    {

        private static string username = "";
        private static string password = "";

        private static string senderName = "";
        private static string validityPeriod = "4";

        readonly GlobalValue global_val = new GlobalValue();
        readonly setup_InternetRepo internetRepo = new setup_InternetRepo();
        public static Output PostRequest(Request request)
        {

            //////////////////////////////////////////////////////////////////////////
            string queryinternet = "select * from setup_company";

            using (OracleConnection connection = new OracleConnection(GlobalValue.ConString))
            {
                var con = new AppSettings();
                #region send email
                OracleCommand commandinternet = new OracleCommand(queryinternet, connection);
                connection.Open();
                OracleDataReader readerinternet;
                readerinternet = commandinternet.ExecuteReader();
                // Always call Read before accessing data.
                while (readerinternet.Read())
                {
                    username = (string)readerinternet["sms_id"];
                    password = (string)readerinternet["sms_password"];
                    senderName = (string)readerinternet["sms_name"];

                }


                #endregion


                //////////////////////////////////////////////////////////////////////


                string mobileNumber = request.Parameters.FirstOrDefault(k => k.Key == "to").Value;
                string messageToSend = request.Parameters.FirstOrDefault(k => k.Key == "text").Value;

                try
                {

                    var _phoneList = new List<string> { mobileNumber };

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
                        var auth = new Authentication { username = username, password = password };

                        // Set up mobile numbers here
                        var rList = new List<Recipient>();
                        rList.AddRange(_phoneList.Select(p => new Recipient { gsm = $"{p}" }));

                        // Set up message parameters here
                        var message = new MainMessage
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
                            messages = new[] { message }
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
                                using (var theContent = new StringContent(sr.ReadToEnd(), Encoding.UTF8, @"application/json"))
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

                                            return new Output()
                                            {
                                                result = true,
                                                message = Convert.ToString(result)
                                            };
                                        }
                                    }


                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    return new Output()
                    {
                        result = false,
                        message = ex.Message
                    };
                }
                return null;
            }


        }
    }
}
