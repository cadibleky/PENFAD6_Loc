using System;
using System.Collections.Generic;
using System.Configuration;
using Aspose.Email;
using Aspose.Email.Mail;
using Polly;
using Serilog;

namespace TeksolServices
{
    public class TeksolEmailHandler
    {
        readonly List<EmailEntity> _emailList;
       // static ILogger _log;
        public TeksolEmailHandler(List<EmailEntity> emailList)
        {
            new License().SetLicense(LicenseHelper.LStream);
            _emailList = emailList;
          //  _log = CustomLogger();
        }
        public void SendEmail()
        {
            var policy = Policy.Handle<Exception>()
                .WaitAndRetry(Convert.ToInt32(ConfigurationManager.AppSettings["retryTime"]), retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            policy.Execute(SendMail);

        }

        void SendMail()
        {
            // Call Smtp object
            var client = GetSmtpClient2();
            var state = new object();

            //Add CC recipients
            //message.CC.Add("cc1@domain.com");
            //message.CC.Add("cc2@domain.com");

            // Add TO recipients
            var message = new MailMessage();
            foreach (var email in _emailList)
            {
                var depositTemplate = $@"<b>Dear CustomerName</b> <br/> <br/><font color=blue>Your User Name is UserId and  Password is UserPassword</font>";

                // Create a new instance of MailMessage class
#pragma warning disable CC0022 // Should dispose object
                message = new MailMessage
                {
                    Subject = $"Penfad authentication for {email.Name}",
                    IsBodyHtml = true,
                    // Set sender information
                    From = ConfigurationManager.AppSettings["emailSender"],
                    To = email.EmailAddress
                };
#pragma warning restore CC0022 // Should dispose object
                depositTemplate = depositTemplate.Replace("CustomerName", email.Name);
                depositTemplate = depositTemplate.Replace("UserId", email.User_Id);
                depositTemplate = depositTemplate.Replace("UserPassword", email.Password);
                // Set Html body
                message.HtmlBody = depositTemplate;
                var ar = client.BeginSend(message, Callback1, state);
                //Console.WriteLine($"Sending message to {email.EmailAddress}...");
                //var answer = Console.ReadLine();

                //// If the user canceled the send, and mail hasn't been sent yet,

                //if (answer != null && answer.StartsWith("c"))
                //{
                //    client.CancelAsyncOperation(ar);
                //}
            }

            // Clean up.
            //message.Dispose();
        }

        public static AsyncCallback Callback1 { get; } = delegate (IAsyncResult ar)
        {
            var task = (MailClientTask)ar;
            if (task.IsCanceled)
            {
                //_log.Information("Message to be sent canceled.");
            }

            if (task.ErrorInfo != null)
            {
               // _log.Information(task.ErrorInfo.ToString());
            }
            
            else
            {
               // _log.Information("Message Sent.");
            }
        };

        static SmtpClient GetSmtpClient2()
        {
            var client = new SmtpClient();
            client.Host = ConfigurationManager.AppSettings["smtpHost"];
            //Specify your mail user name
            client.Username = ConfigurationManager.AppSettings["smtpUser"];
            //Specify your mail password
            client.Password = ConfigurationManager.AppSettings["smtpPass"];
            //Specify your Port #
            client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"]);
            //Specify the security option
            client.SecurityOptions = SecurityOptions.SSLExplicit;
            return client;
        }
        public static ILogger CustomLogger()
        {
            var log = new LoggerConfiguration()
                .WriteTo.RollingFile(@"C:\programData\TeksolEmailLogs\EmailLog-{Date}.txt")
                .WriteTo.ColoredConsole()
                .CreateLogger();
            return log;
        }
    }
}
