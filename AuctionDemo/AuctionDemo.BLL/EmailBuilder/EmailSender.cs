
using AuctionDemo.BLL.EmailBuilder;

namespace AuctionDemo.BLL.EmailBuilder
{
    public static class EmailSender
    {
        public static void SendEmail(string address, string subject, string body)
        {
            SendEmail sendEmail = new SendEmail();
            EmailBuilderImplement emailBuilder = new EmailBuilderImplement();
            sendEmail.SetEmailBuilder(emailBuilder);
            sendEmail.ConstructEmail(address, subject, body);

        }
    }
}
