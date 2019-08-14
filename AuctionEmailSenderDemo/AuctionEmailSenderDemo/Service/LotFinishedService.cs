using AuctionEmailSenderDemo.AuctionModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AuctionEmailSenderDemo.Service
{
    public static class LotFinishedService
    {

        public static void NotifyOwnerAndWinner(short LotId)
        {
           AuctionContext db = new AuctionContext();


            // We need grab some info and send this info with email
            //1. get email of the user
            short Owner = db.Lot.Where(item => item.LotId == LotId).Select(item => item.UserId).FirstOrDefault();
            string EmailAddr = db.User.Where(item => item.UserId == Owner).Select(item => item.MailAddress).FirstOrDefault();

            // Check if user has config to get email
            var GetEmail = db.User_Configuration.Where(item => item.UserId == Owner).Select(item => item.AuctionFinished).FirstOrDefault();

            //2. Get final price
            int FinalPrice = db.Bid.Where(item => item.LotId == LotId).OrderByDescending(item => item.BidPrice).Select(item => item.BidPrice).FirstOrDefault();
            // 3. Set final Price to Lot
            Lot lot = db.Lot.Where(item => item.LotId == LotId).FirstOrDefault();
            lot.FinalPrice = FinalPrice;


            //4. Get bids history
            List<Bid> bids = db.Bid.Where(item => item.LotId == LotId).OrderBy(item => item.Date).ToList();
            short LotWinnerId = 0;
            if (bids.Count != 0)
            {
                // 5. Get an information about lot winner 
                LotWinnerId = bids[bids.Count - 1].UserId;
                // set lot userIdWinner
                lot.UserIdWinner = LotWinnerId;
                db.SaveChanges();
            }
            else
            {
                // set useridwinner = -1 if bids list is empty to prevent resending emails
                lot.UserIdWinner = -1;
                db.SaveChanges();
            }

            User LotWinner = null;
            if (LotWinnerId != 0)
            {
                LotWinner = db.User.Where(item => item.UserId == LotWinnerId).FirstOrDefault();

                // unfroze balance
                LotWinner.FrozenBalance -= FinalPrice;
                db.SaveChanges();

                // Add money to lotowner user account
                User localUser = db.User.Where(item => item.UserId == Owner).FirstOrDefault();
                localUser.Balance += LotWinner.FrozenBalance;
                db.User.Attach(localUser);
                db.Entry(localUser).State = EntityState.Modified;
                db.SaveChanges();

            }

            if (GetEmail.Value)
            {
                // Form Email 
                // Form email Body
                StringBuilder EmailBody = new StringBuilder(64);
                EmailBody.Append("Your lot is finished\n");
                EmailBody.Append("Final Price is : " + FinalPrice.ToString() + "\n");
                EmailBody.Append("Bids history:\n");
                for (int i = 0; i < bids.Count; i++)
                {
                    EmailBody.Append((i + 1).ToString() + ") Bid Price - " + bids[i].BidPrice.ToString() +
                        " , Bid Date - " + bids[i].Date + ".\n");
                }
                if (LotWinner != null)
                {
                    EmailBody.Append("Lot Winner : " + LotWinner.Name + "\n");
                    EmailBody.Append("Email of Winner : " + LotWinner.MailAddress + "\n");
                }
                else
                {
                    EmailBody.Append("There are no bids for this lot");
                }



                // Відправник
                MailAddress from = new MailAddress("vasilkindiy@gmail.com", "Auction");

                // Саме повідомлення
                string Email = EmailBody.ToString();
                // Отримувач
                MailAddress to = new MailAddress(EmailAddr);

                // Створення повідомлення
                MailMessage m = new MailMessage(from, to);
                // Тема листа
                m.Subject = "Lot is finished";
                // Текст повідомлення
                m.Body = Email;
                // Лист в формвті html
                m.IsBodyHtml = false;

                // адрес smtp-сервера і порт
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                // логін і пароль
                NetworkCredential credentials = new NetworkCredential("vasilkindiy@gmail.com", "vasasa00");
                smtp.Credentials = credentials;
                smtp.EnableSsl = true;
                if (credentials.Password == "") return;
                smtp.Send(m);

            }

            if (LotWinnerId != 0)
            {
                // Notify the winner that his lot win if he has config 
                GetEmail = db.User_Configuration.Where(item => item.UserId == LotWinnerId).Select(item => item.BidWinLot).FirstOrDefault();
                if (GetEmail.Value)
                {
                    // Form Email 

                    // Відправник
                    MailAddress from = new MailAddress("vasilkindiy@gmail.com", "Auction");

                    // Саме повідомлення
                    string Email = "Our congratulations!\nLot : " + lot.Name + " is finished and your bid win this lot!";
                    // Отримувач
                    MailAddress to = new MailAddress(EmailAddr);

                    // Створення повідомлення
                    MailMessage m = new MailMessage(from, to);
                    // Тема листа
                    m.Subject = "You win lot";
                    // Текст повідомлення
                    m.Body = Email;
                    // Лист в формвті html
                    m.IsBodyHtml = false;

                    // адрес smtp-сервера і порт
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    // логін і пароль
                    NetworkCredential credentials = new NetworkCredential("vasilkindiy@gmail.com", "vasasa00");
                    smtp.Credentials = credentials;
                    smtp.EnableSsl = true;
                    if (credentials.Password == "") return;
                    smtp.Send(m);
                }
            }
        }
    }
}
