using AuctionDemo.DAL.Models;
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

        public static void NotifyOwner(short LotId , log4net.ILog log)
        {
            AuctionContext db = new AuctionContext();

            // We need grab some info and send this info with email
            //1. get email of the user
            short Owner = db.Lot.Where(item => item.LotId == LotId).Select(item => item.UserId).FirstOrDefault();
            string EmailAddr = db.User.Where(item => item.UserId == Owner).Select(item => item.MailAddress).FirstOrDefault();

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

                // reduce winner balance and unfroze balance
                LotWinner.FrozenBalance -= FinalPrice;
                db.SaveChanges();

                // add money to owner account
                User lotOwner = db.User.Where(item => item.UserId == Owner).FirstOrDefault();
                lotOwner.Balance += FinalPrice;
                db.SaveChanges();

            }


            // Check if user has config to get email
            var GetEmail = db.User_Configuration.Where(item => item.UserId == Owner).Select(item => item.AuctionFinished).FirstOrDefault();

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
                    EmailBody.Append("<h1>There are no bids for this lot</h1>");
                }

                log.Info("Sending Email to Lot owner");
                // Send Email
                try
                {
                    EmailSender.SendEmail(EmailAddr, "Auction", EmailBody.ToString());
                    log.Info("Email to lot owner with address: " + EmailAddr.ToString() + ", with final Price: " + FinalPrice.ToString() + " is sent");
                }
                catch
                {
                    log.Error("Error while sending mail to Lot owner");
                }
            }
        }

        public static void NotifyWinner(short LotId , log4net.ILog log)
        {
            AuctionContext db = new AuctionContext();

            //1.Get userId of the winner
            var userId = db.Bid.Where(item => item.LotId == LotId).OrderByDescending(item => item.BidPrice).Select(item => item.UserId).FirstOrDefault();

            //2. Get email address of this user
            if (userId != 0)
            {
                var emailAddress = db.User.Where(item => item.UserId == userId).Select(item => item.MailAddress).FirstOrDefault();

                // 3. Get lot name
                var lotName = db.Lot.Where(item => item.LotId == LotId).Select(item => item.Name).FirstOrDefault();

                // Form email body
                string emailBody = "Our congratulations!\nLot : " + lotName + " is finished and your bid win this lot!";

                // Get User Config
                var IsSentEmail = db.User_Configuration.Where(item => item.UserId == userId).Select(item => item.BidWinLot).FirstOrDefault();
                // Send Email
                if (IsSentEmail.Value)
                {
                    log.Info("Sending Email to Lot winner");
                    try
                    {
                        EmailSender.SendEmail(emailAddress, "Auction", emailBody);
                        log.Info("Email to lot winner with address: " + emailAddress.ToString() + " is sent");
                    }
                    catch
                    {
                        log.Error("Error while sending mail to Lot winner");
                    }
                }
            }
        }
    }
}
