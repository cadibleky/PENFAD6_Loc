using System;
using System.Collections.Generic;

namespace TeksolServices
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                const string msg = "Just finished with my testing for the sms using infobip channel. Moving on to email alerts. Life is sweet with C#.";
                //var tsh = new TeksolSmsHandler(new List<string> {"233500390299", "233577683529", "233577683533"}, msg);
                var teh = new TeksolEmailHandler(new List<EmailEntity>
                {
                    new EmailEntity {EmailAddress = "sir_lutterodt@yahoo.com", Password = "GHC4556.89", Name = "Luda Asante"},
                    new EmailEntity {EmailAddress = "troasfl@gmail.com", Password = "GHC856.89", Name = "Troas"}
                });
                //tsh.SendSmsUsingInfoBipChannel();
                teh.SendEmail();
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}