using EASendMail;
using PENFAD6DAL.Repository.CRM.Employee;
using PENFAD6DAL.Repository.Remittance.Contribution;
using System;
using System.Data;
using System.Net;
using System.Net.Mail;

namespace PENFAD6DAL.Repository.Setup.SystemSetup
{
    public class setup_InternetRepo
    {
        public string smtp { get; set; }
        public int port { get; set; }
        public string email_from { get; set; }
        public string email_password { get; set; }
        public string company_name { get; set; }
        public string website_address { get; set; }
        public string postal_address { get; set; }
        public string telephone_number { get; set; }

        //  readonly setup_InternetRepo internetRepo = new setup_InternetRepo();
        public void SendIt(string from, string pass, string subj, string msg, string to, string smtp, int port, string company_name)
        {
            try
            {
				MailMessage mail = new MailMessage();
				System.Net.Mail.SmtpClient SmtpServer = new System.Net.Mail.SmtpClient();
				System.Net.Mail.MailAddress fromaddress = new System.Net.Mail.MailAddress(from, company_name);
				mail.From = fromaddress;
				mail.To.Add(to);
				mail.Subject = subj;
				mail.Body = msg;
				mail.IsBodyHtml = true;
				SmtpServer.Port = port;
				SmtpServer.Host = smtp;
                SmtpServer.EnableSsl = true;
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
				SmtpServer.Credentials = new System.Net.NetworkCredential(from, pass);
				SmtpServer.Send(mail);

			}
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SendIt2(string from, string pass, string subj, string msg, string to, string smtp, int port, crm_EmployeeRepo employeeRepo, string attach)
        {
            try
            {
				MailMessage mail = new MailMessage();
				System.Net.Mail.SmtpClient SmtpServer = new System.Net.Mail.SmtpClient();
				System.Net.Mail.MailAddress fromaddress = new System.Net.Mail.MailAddress(from, company_name);

				System.Net.Mail.Attachment attachment;
				attachment = new System.Net.Mail.Attachment(attach);
				mail.Attachments.Add(attachment);

				mail.From = fromaddress;
				mail.To.Add(to);
				mail.Subject = subj;
				mail.Body = msg;
				mail.IsBodyHtml = true;
				SmtpServer.Port = port;
				SmtpServer.Host = smtp;
                SmtpServer.EnableSsl = true;
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
				SmtpServer.Credentials = new System.Net.NetworkCredential(from, pass);
				SmtpServer.Send(mail);
             



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SendIt3(string from, string pass, string subj, string msg, string to, string smtp, int port, Remit_Contribution_Upload_LogRepo employeeRepo, string attach)
        {
            try
            {
                MailMessage mail = new MailMessage();
                System.Net.Mail.SmtpClient SmtpServer = new System.Net.Mail.SmtpClient();
                System.Net.Mail.MailAddress fromaddress = new System.Net.Mail.MailAddress(from, company_name);

                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment(attach);
                mail.Attachments.Add(attachment);

                mail.From = fromaddress;
                mail.To.Add(to);
                mail.Subject = subj;
                mail.Body = msg;
                mail.IsBodyHtml = true;
                SmtpServer.Port = port;
                SmtpServer.Host = smtp;
                SmtpServer.EnableSsl = true;
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.Credentials = new System.Net.NetworkCredential(from, pass);
                SmtpServer.Send(mail);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SendIt4(string from, string pass, string subj, string msg, string to, string smtp, int port, Remit_ReceiptRepo ReceiptRepo, string attach)
        {
            try
            {
                MailMessage mail = new MailMessage();
                System.Net.Mail.SmtpClient SmtpServer = new System.Net.Mail.SmtpClient();
                System.Net.Mail.MailAddress fromaddress = new System.Net.Mail.MailAddress(from, company_name);

                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment(attach);
                mail.Attachments.Add(attachment);

                mail.From = fromaddress;
                mail.To.Add(to);
                mail.Subject = subj;
                mail.Body = msg;
                mail.IsBodyHtml = true;
                SmtpServer.Port = port;
                SmtpServer.Host = smtp;
                SmtpServer.EnableSsl = true;
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.Credentials = new System.Net.NetworkCredential(from, pass);
                SmtpServer.Send(mail);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static bool checkInternet()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

    }
}
