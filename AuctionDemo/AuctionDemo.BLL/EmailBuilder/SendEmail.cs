using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionDemo.BLL.EmailBuilder
{
    public class SendEmail
    {
        private EmailBuilder builder;
        public void SetEmailBuilder(EmailBuilder IBuilder)
        {
            builder = IBuilder;
        }

        public EmailObject GetEmail()
        {
            return builder.GetEmailObject();
        }

        public void ConstructEmail(
             string addresseeAddress,
             string subject,
             string body
             )
        {
            builder.CreateNewEmailObject();
            builder.SetNetworkCredentials();
            builder.SetSender();
            builder.SetAddressee(addresseeAddress);
            builder.SetMailMessageConfig();
            builder.SetSubjectOfEmail(subject);
            builder.SetBodyOfEmail(body);
            builder.SetBodyConfiguration();
            builder.SendEmail();
        }
    }
}
