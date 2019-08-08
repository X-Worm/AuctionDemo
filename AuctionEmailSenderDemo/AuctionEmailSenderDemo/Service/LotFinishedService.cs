using AuctionEmailSenderDemo.AuctionModel;
using System;
using System.Collections.Generic;
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
            AuctionModel.AuctionContext db = new AuctionModel.AuctionContext();


            // We need grab some info and send this info with email
            //1. get email of the user
            short Owner = db.Lot.Where(item => item.Lot_Id == LotId).Select(item => item.User_Id).FirstOrDefault();
            string EmailAddr = db.User.Where(item => item.User_Id == Owner).Select(item => item.Mail_Address).FirstOrDefault();

            // Check if user has config to get email
            var GetEmail = db.User_Configuration.Where(item => item.User_Id == Owner).Select(item => item.Auction_Finished).FirstOrDefault();

            //2. Get final price
            int FinalPrice = db.Bid.Where(item => item.Lot_Id == LotId).OrderByDescending(item => item.bid_Price).Select(item => item.bid_Price).FirstOrDefault();
            // 3. Set final Price to Lot
            Lot lot = db.Lot.Where(item => item.Lot_Id == LotId).FirstOrDefault();
            lot.Final_Price = FinalPrice;


            //4. Get bids history
            List<Bid> bids = db.Bid.Where(item => item.Lot_Id == LotId).OrderBy(item => item.Date).ToList();
            short LotWinnerId = 0;
            if (bids.Count != 0)
            {
                // 5. Get an information about lot winner 
                LotWinnerId = bids[bids.Count - 1].User_Id;
                // set lot user_Id_Winner
                lot.User_Id_Winner = LotWinnerId;
                db.SaveChanges();
            }
            else
            {
                // set user_id_winner = -1 if bids list is empty to prevent resending emails
                lot.User_Id_Winner = -1;
                db.SaveChanges();
            }

            User LotWinner = null;
            if (LotWinnerId != 0)
            {
                LotWinner = db.User.Where(item => item.User_Id == LotWinnerId).FirstOrDefault();

                // unfroze balance
                LotWinner.Frozen_Balance -= FinalPrice;
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
                    EmailBody.Append((i + 1).ToString() + ") Bid Price - " + bids[i].bid_Price.ToString() +
                        " , Bid Date - " + bids[i].Date + ".\n");
                }
                if (LotWinner != null)
                {
                    EmailBody.Append("Lot Winner : " + LotWinner.Name + "\n");
                    EmailBody.Append("Email of Winner : " + LotWinner.Mail_Address + "\n");
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
                NetworkCredential credentials = new NetworkCredential("vasilkindiy@gmail.com", "");
                smtp.Credentials = credentials;
                smtp.EnableSsl = true;
                if (credentials.Password == "") return;
                smtp.Send(m);

            }

            if (LotWinnerId != 0)
            {
                // Notify the winner that his lot win if he has config 
                GetEmail = db.User_Configuration.Where(item => item.User_Id == LotWinnerId).Select(item => item.Bid_Win_Lot).FirstOrDefault();
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
                    NetworkCredential credentials = new NetworkCredential("vasilkindiy@gmail.com", "");
                    smtp.Credentials = credentials;
                    smtp.EnableSsl = true;
                    if (credentials.Password == "") return;
                    smtp.Send(m);
                }
            }
        }
    }
}
