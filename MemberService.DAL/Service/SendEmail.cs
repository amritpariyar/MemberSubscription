using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace MemberService.DAL
{
    public class SendEmail
    {
        protected static string SMTP = "mail.gatewaytechnologies.com.np"; // SMTP Address
        protected static string FromMail = "info@gatewaytechnologies.com.np"; // email
        protected static string fromPass = ""; // Password
        protected static int PORT = 25;

        public static string LiveURL { get; set; }
        public static string UserName { get; set; }
        public static string PassWord { get; set; }    
        public static string PassKey { get; set; }    
        public static string userID { get; set; }
        public static string Subject { get; set; }
        public static string toMail { get; set; }

       

        public static bool SendSubscriptionDetail(string message)
        {
            try
            {
                MailMessage mail = new MailMessage(FromMail, toMail);

                int port = Convert.ToInt32(PORT);
                string login_id = userID;

                mail.Subject = Subject;
                mail.IsBodyHtml = true;
                mail.Body = message;

                SmtpClient smtp = new SmtpClient(SMTP, port);

                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(FromMail, fromPass);

                return SendMe(smtp, mail);
            }
            catch (Exception e)
            {
                return false;
            }            
        }
        private static bool SendMe(SmtpClient smtp, MailMessage mail)
        {
            try
            {
                smtp.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        
    }
}