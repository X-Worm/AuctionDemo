using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionDemo.BLL.EmailBuilder
{
    public abstract class EmailBuilder
    {
        protected EmailObject emailObject { get; set; }
        public void CreateNewEmailObject()
        {
            emailObject = new EmailObject();
        }
        // Get ready email
        public EmailObject GetEmailObject()
        {
            return emailObject;
        }

        // Needed step to create email
        public abstract void SetNetworkCredentials();
        public abstract void SetSender();
        public abstract void SetAddressee(string address);
        public abstract void SetMailMessageConfig();
        public abstract void SetSubjectOfEmail(string subject);
        public abstract void SetBodyOfEmail(string body);
        public abstract void SetBodyConfiguration();
        public abstract void SendEmail();
    }
}
