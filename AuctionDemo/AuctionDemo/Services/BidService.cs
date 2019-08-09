using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AuctionDemo.Models;
using AuctionDemo.Models.Unit_of_Work;
using System.Linq.Dynamic;
using AuctionDemo.Extensions;
using System.Web.Http;
using System.Text;
using System.IO;
using System.IdentityModel;
using System.Net.Mail;
using System.Net;

namespace AuctionDemo.Services
{
    public class BidService
    {
        private readonly UnitOfWork unitOfWork = new UnitOfWork();
        private readonly string ConnectionString;

        public void CreateNewBid(Bid bid , short? UserId)
        {
            // Set User Id
            bid.User_Id = UserId.Value;

            // Set Current data to Date
            bid.Date = DateTime.Now;

            // Check if this lot exist
            var IsExist = unitOfWork.Lot.dbSet.Any(item => item.Lot_Id == bid.Lot_Id);
            if (!IsExist) throw new Exception("Lot with this Lot_id doesnt exist");

            // Check if user is not owner of this lot
            // Get All Lot_Id of this user
            var usersLots = unitOfWork.Lot.dbSet.Where(item => item.User_Id == bid.User_Id).Select(item => item.Lot_Id).ToList();

            // if usersLots containt bid.LotId 
            if (usersLots.Contains(bid.Lot_Id))
            {
                throw new Exception("User is owner of this lot and cannot make bid");
            }

            // Check if bid_Price is bigger than current highest bid_Price
            var highestBid = unitOfWork.Bid.dbSet.Where(item => item.Lot_Id == bid.Lot_Id)
                .OrderByDescending(item => item.bid_Price).Select(item => new {bid_price = item.bid_Price , user_id = item.User_Id }).FirstOrDefault();

            if (highestBid != null)
            {
                if (bid.bid_Price <= highestBid.bid_price)
                {
                    throw new Exception("bid_Price is invalid , Input bid_price higher than " + highestBid.bid_price.ToString());
                }

                // Check is User dosent set 2 bids one after another
                if (bid.User_Id == highestBid.user_id)
                {
                    throw new BadRequestException("User cannot set 2 bids one after another");
                }


                // Check if Lot has bids , if doesnt has than check if bid price is bigger than initial_price
                // if highestBid_price == 0 its mean that lot doesnt has bids
                if (highestBid.bid_price == 0)
                {
                    int initialPrice = unitOfWork.Lot.dbSet.Where(item => item.Lot_Id == bid.Lot_Id).Select(item => item.Initial_Price).FirstOrDefault();
                    if (bid.bid_Price <= initialPrice) throw new BadRequestException("Bid price must be higher than lot initial price - " + initialPrice.ToString());
                }
            }

            // Check if user doesnt make bid on closed lot 
            var LotEndDate = unitOfWork.Lot.dbSet.Where(item => item.Lot_Id == bid.Lot_Id).Select(item => item.End_Date).FirstOrDefault();
            if (bid.Date > LotEndDate) throw new Exception("This lot is cloesd");

            // Check if User has enough money
            var UserAccount = unitOfWork.User.dbSet.Where(item => item.User_Id == bid.User_Id).FirstOrDefault();
            if(bid.bid_Price <= UserAccount.Balance)
            {
                // Froze User Account
                UserAccount.Frozen_Balance += bid.bid_Price;
                UserAccount.Balance -= bid.bid_Price;
                unitOfWork.User.Update(UserAccount);
            }
            else
            {
                throw new Exception("Not enough money in the account");
            }

            // Unfroze lot of User with Highest bid
            // Get  current highest bid
            var topBid = unitOfWork.Bid.dbSet.Where(item => item.Lot_Id == bid.Lot_Id).OrderByDescending(item => item.Date).FirstOrDefault();
            
            // update user balance (unfroze)
            if(topBid != null)
            {
                var topUser_Id = topBid.User_Id;
                var localUser = unitOfWork.User.dbSet.Where(item => item.User_Id == topUser_Id).FirstOrDefault();
                localUser.Balance += topBid.bid_Price;
                localUser.Frozen_Balance -= topBid.bid_Price;
                unitOfWork.User.Update(localUser);

                // after unfrozing account we need notify user in email that someone place a bid higher if user set configuration for it
                var IsHasConfig = unitOfWork.User_Configuration.dbSet.Where(item => item.User_Id == topUser_Id).Select(item => item.Bid_Placed_Higher).FirstOrDefault();
                if(IsHasConfig != null && IsHasConfig.Value == true)
                {
                    //Send Email with information about new bid
                    SendEmail(bid, topUser_Id);
                }
            }

            // Set CurrentPrice of lot
            Lot CurrentLot = unitOfWork.Lot.dbSet.Where(item => item.Lot_Id == bid.Lot_Id).FirstOrDefault();
            CurrentLot.Current_Price = bid.bid_Price;
            unitOfWork.Lot.Update(CurrentLot);

            // Post new bid
            unitOfWork.Bid.dbSet.Add(bid);

            unitOfWork.Save();

            
        }

        public void SendEmail(Bid bid , short topUser)
        {
            // Get email addres of User
            string AddressToSend = unitOfWork.User.dbSet.Where(item => item.User_Id == topUser).Select(item => item.Mail_Address).FirstOrDefault();

            // Get UserName of user that set a higher bid
            string NewTopUser = unitOfWork.User.dbSet.Where(item => item.User_Id == bid.User_Id).Select(item => item.Name).FirstOrDefault();

            // Form email address
            StringBuilder emailText = new StringBuilder("" , 64 );
            
            emailText.Append("<h2 style = \"color: red; \">" + NewTopUser + " placed a higher bid</h2>");
            emailText.Append("<h3>Bid info</h3>");
            emailText.Append("<p>Bid price : " + bid.bid_Price.ToString() + "</p>");
            emailText.Append("<p>Bid date : " + bid.Date.ToString() + "</p>");



            // Відправник
            MailAddress from = new MailAddress("vasilkindiy@gmail.com", "Auction");
            // Отримувач
            MailAddress to = new MailAddress(AddressToSend);

            // Створення повідомлення
            MailMessage m = new MailMessage(from, to);
            // Тема листа
            m.Subject = "Someone place a higher bid";
            // Текст повідомлення
            m.Body = emailText.ToString();
            // Лист в формвті html
            m.IsBodyHtml = true;


            // адрес smtp-сервера і порт
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            // логін і пароль
            NetworkCredential credentials = new NetworkCredential("vasilkindiy@gmail.com", "");
            smtp.Credentials = credentials;
            smtp.EnableSsl = true;
            if (credentials.Password == "") throw new BadRequestException("Invalid email Credentials");
            smtp.Send(m);
        }

        public List<Bid> GetAllBids(short lotId, string order, string filterPrice, string filterDate, int pageSize = 5, int pageNumber = 1)
        {
            List<int> toFilterPrice = new List<int>();
            // Split filter Price
            if (filterPrice != "" && filterPrice != null)
            {
                List<string> filterPriceList = filterPrice.Split(',').ToList();
                if (filterPriceList.Count == 1)
                {
                    filterPriceList.Add("0");
                    filterPriceList.Reverse();
                }
                toFilterPrice = filterPriceList.Select(item => Convert.ToInt32(item)).ToList();
                if (toFilterPrice[1] < toFilterPrice[0]) throw new BadRequestException("Invalid filterPrice values");
            }
            else
            {
                // disable filtering if filterPrice is empty
                toFilterPrice.Add(0); toFilterPrice.Add(Int32.MaxValue);
            }

            // split filterDate
            List<DateTime> toFilterDate = new List<DateTime>();
            if (filterDate != "" && filterDate != null)
            {
                List<string> filterDateList = filterDate.Split(',').ToList();
                if (filterDateList.Count == 1)
                {
                    filterDateList.Add(DateTime.MinValue.ToString());
                    filterDateList.Reverse();
                }
                toFilterDate = filterDateList.Select(item => Convert.ToDateTime(item)).ToList();
                if (toFilterDate[1] < toFilterDate[0]) throw new BadRequestException("Invalid filterPrice values");
            }
            else
            {
                // disable filtering if filterDate is empty
                toFilterDate.Add(DateTime.MinValue); toFilterDate.Add(DateTime.MaxValue);
            }



            int minFilterPrice = toFilterPrice[0];
            int maxFilterPrice = toFilterPrice[1];

            DateTime minFilterDate = toFilterDate[0];
            DateTime maxFilterDate = toFilterDate[1];

            var result = unitOfWork.Bid.dbSet
                .Where(item => item.Lot_Id == lotId)
                .OrderBy(item => item.Bid_Id) // Default sort
                .Where(item => item.bid_Price >= minFilterPrice && item.bid_Price <= maxFilterPrice) // apply price filtering
                .Where(item => item.Date >= minFilterDate && item.Date <= maxFilterDate) // apply date filtering
                .ApplySort(order)
                .Skip(pageSize * (pageNumber - 1)).Take(pageSize);


            return result.ToList();
        }

    }
}