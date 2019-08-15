using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AuctionDemo.BLL.EmailBuilder
{
   public class EmailBuilderImplement : EmailBuilder
    {
        public override void SendEmail()
        {
            emailObject.smtp.EnableSsl = true;
            emailObject.smtp.Send(emailObject.message);
        }

        public override void SetAddressee(string address)
        {
            emailObject.addressee = new MailAddress(address);
        }

        public override void SetBodyConfiguration()
        {
            emailObject.message.IsBodyHtml = true;
        }

        public override void SetBodyOfEmail(string body)
        {
            emailObject.message.Body = body;
        }

        public override void SetMailMessageConfig()
        {
            emailObject.message = new MailMessage(emailObject.sender, emailObject.addressee);
            emailObject.smtp.Credentials = emailObject.credentials;
        }

        public override void SetNetworkCredentials()
        {
            emailObject.credentials = new System.Net.NetworkCredential("vasilkindiy@gmail.com", "vasasa00");
        }

        public override void SetSender()
        {
            emailObject.sender = new MailAddress("vasilkindiy@gmail.com", "Auction");
        }

        public override void SetSubjectOfEmail(string subject)
        {
            emailObject.message.Subject = subject;
        }
    }
}
