using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AuctionDemo.BLL.EmailBuilder
{
    public class EmailObject
    {
        public SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
        public NetworkCredential credentials { get; set; }
        public MailAddress addressee { get; set; }
        public MailAddress sender { get; set; }
        public MailMessage message { get; set; }

    }
}
