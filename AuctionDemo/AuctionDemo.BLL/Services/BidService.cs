using AuctionDemo.BLL.ExceptionHandler;
using AuctionDemo.BLL.Extensions;
using AuctionDemo.DAL.Models;
using AuctionDemo.DAL.Models.Unit_of_Work;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AuctionDemo.BLL.Services
{
    public class BidService
    {
        private readonly UnitOfWork unitOfWork = new UnitOfWork();
        private readonly string ConnectionString;

        public void CreateNewBid(Bid bid, short? UserId)
        {
            // Set User Id
            bid.UserId = UserId.Value;

            // Set Current data to Date
            bid.Date = DateTime.UtcNow;

            // Check if this lot exist
            var IsExist = unitOfWork.Lot.dbSet.Any(item => item.LotId == bid.LotId);
            if (!IsExist) throw new NewBadRequestException("Lot with this Lotid doesnt exist");

            // Check if user is not owner of this lot
            // Get All LotId of this user
            var usersLots = unitOfWork.Lot.dbSet.Where(item => item.UserId == bid.UserId).Select(item => item.LotId).ToList();

            // if usersLots containt bid.LotId 
            if (usersLots.Contains(bid.LotId))
            {
                throw new NewBadRequestException("User is owner of this lot and cannot make bid");
            }

            // Check if bidPrice is bigger than current highest bidPrice
            var highestBid = unitOfWork.Bid.dbSet.Where(item => item.LotId == bid.LotId)
                .OrderByDescending(item => item.BidPrice).Select(item => new { bidprice = item.BidPrice, userid = item.UserId }).FirstOrDefault();

            if (highestBid != null)
            {
                if (bid.BidPrice <= highestBid.bidprice)
                {
                    throw new NewBadRequestException("bidPrice is invalid , Input bidprice higher than " + highestBid.bidprice.ToString());
                }

                // Check is User dosent set 2 bids one after another
                if (bid.UserId == highestBid.userid)
                {
                    throw new NewBadRequestException("User cannot set 2 bids one after another");
                }


                // Check if Lot has bids , if doesnt has than check if bid price is bigger than initialprice
                // if highestBidprice == 0 its mean that lot doesnt has bids
                if (highestBid.bidprice == 0)
                {
                    int initialPrice = unitOfWork.Lot.dbSet.Where(item => item.LotId == bid.LotId).Select(item => item.InitialPrice).FirstOrDefault();
                    if (bid.BidPrice <= initialPrice) throw new NewBadRequestException("Bid price must be higher than lot initial price - " + initialPrice.ToString());
                }
            }

            // Check if user doesnt make bid on closed lot 
            var LotEndDate = unitOfWork.Lot.dbSet.Where(item => item.LotId == bid.LotId).Select(item => item.EndDate).FirstOrDefault();
            if (bid.Date > LotEndDate) throw new NewBadRequestException("This lot is cloesd");

            // Check if User has enough money
            var UserAccount = unitOfWork.User.dbSet.Where(item => item.UserId == bid.UserId).FirstOrDefault();
            if (bid.BidPrice <= UserAccount.Balance)
            {
                // Froze User Account
                UserAccount.FrozenBalance += bid.BidPrice;
                UserAccount.Balance -= bid.BidPrice;
                unitOfWork.User.Update(UserAccount);
            }
            else
            {
                throw new NewBadRequestException("Not enough money in the account");
            }

            // Unfroze lot of User with Highest bid
            // Get  current highest bid
            var topBid = unitOfWork.Bid.dbSet.Where(item => item.LotId == bid.LotId).OrderByDescending(item => item.Date).FirstOrDefault();

            // update user balance (unfroze)
            if (topBid != null)
            {
                var topUserId = topBid.UserId;
                var localUser = unitOfWork.User.dbSet.Where(item => item.UserId == topUserId).FirstOrDefault();
                localUser.Balance += topBid.BidPrice;
                localUser.FrozenBalance -= topBid.BidPrice;
                unitOfWork.User.Update(localUser);

                // after unfrozing account we need notify user in email that someone place a bid higher if user set configuration for it
                var IsHasConfig = unitOfWork.User_Configuration.dbSet.Where(item => item.UserId == topUserId).Select(item => item.BidPlacedHigher).FirstOrDefault();
                if (IsHasConfig != null && IsHasConfig.Value == true)
                {
                    //Send Email with information about new bid
                    SendEmail(bid, topUserId);
                }
            }

            // Set CurrentPrice of lot
            Lot CurrentLot = unitOfWork.Lot.dbSet.Where(item => item.LotId == bid.LotId).FirstOrDefault();
            CurrentLot.CurrentPrice = bid.BidPrice;
            unitOfWork.Lot.Update(CurrentLot);

            // Post new bid
            unitOfWork.Bid.dbSet.Add(bid);

            unitOfWork.Save();


        }

        public void SendEmail(Bid bid, short topUser)
        {
            // Get email addres of User
            string AddressToSend = unitOfWork.User.dbSet.Where(item => item.UserId == topUser).Select(item => item.MailAddress).FirstOrDefault();

            // Get UserName of user that set a higher bid
            string NewTopUser = unitOfWork.User.dbSet.Where(item => item.UserId == bid.UserId).Select(item => item.Name).FirstOrDefault();

            // Form email address
            StringBuilder emailText = new StringBuilder("", 64);

            emailText.Append("<h2 style = \"color: red; \">" + NewTopUser + " placed a higher bid</h2>\n");
            emailText.Append("<h3>Bid info</h3>\n");
            emailText.Append("<p>Bid price : " + bid.BidPrice.ToString() + "</p>\n");
            emailText.Append("<p>Bid date : " + bid.Date.ToString() + "</p>");


            // Сформувати повідомлення
            AuctionDemo.BLL.EmailBuilder.EmailSender.SendEmail(AddressToSend, "Someone place a higher bid", emailText.ToString());


            //// Відправник
            //MailAddress from = new MailAddress("vasilkindiy@gmail.com", "Auction");
            //// Отримувач
            //MailAddress to = new MailAddress(AddressToSend);

            //// Створення повідомлення
            //MailMessage m = new MailMessage(from, to);
            //// Тема листа
            //m.Subject = "Someone place a higher bid";
            //// Текст повідомлення
            //m.Body = emailText.ToString();
            //// Лист в формвті html
            //m.IsBodyHtml = true;


            //// адрес smtp-сервера і порт
            //SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            //// логін і пароль
            //NetworkCredential credentials = new NetworkCredential("vasilkindiy@gmail.com", "vasasa00");
            //smtp.Credentials = credentials;
            //smtp.EnableSsl = true;
            //if (credentials.Password == "") throw new NewBadRequestException("Invalid email Credentials");
            //smtp.Send(m);
        }

        public List<Bid> GetAllBids(short lotId, string order, string filterPrice, string filterDate, int pageSize = 5, int pageNumber = 1)
        {
            // Filtering
            // Price formating
            List<int> filterPriceRange = new List<int>();
            int minPrice = 0, maxPrice = int.MaxValue;

            if (string.IsNullOrEmpty(filterPrice)) filterPrice = "";
            else if (!filterPrice.Contains(","))
            {
                filterPrice = "0," + filterPrice;
                filterPriceRange = filterPrice.Split(',').Select(item => int.Parse(item)).ToList();
                minPrice = filterPriceRange[0]; maxPrice = filterPriceRange[1];
            }
            else
            {
                filterPriceRange = filterPrice.Split(',').Select(item => int.Parse(item)).ToList();
                minPrice = filterPriceRange[0]; maxPrice = filterPriceRange[1];
            }

            // Date formating
            List<DateTime> filterDateRange = new List<DateTime>();
            DateTime minDate = new DateTime(2000, 1, 1); DateTime maxDate = new DateTime(2100 , 1 , 1 );

            if (string.IsNullOrEmpty(filterDate)) filterDate = "";
            else if (!filterDate.Contains(","))
            {
                filterDate = DateTime.MinValue.ToString() + "," + filterDate;
                filterDateRange = filterDate.Split(',').Select(item => DateTime.Parse(item)).ToList();
                minDate = new DateTime(2000, 1, 1); maxDate = filterDateRange[1];
            }
            else
            {
                filterDateRange = filterDate.Split(',').Select(item => DateTime.Parse(item)).ToList();
                minDate = filterDateRange[0]; maxDate = filterDateRange[1];
            }


            AuctionContext db = new AuctionContext();
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter("@LotId" ,         lotId),
                new SqlParameter("@PriceMin" ,      minPrice),
                new SqlParameter("@PriceMax" ,      maxPrice),
                new SqlParameter("@DateMin" ,       minDate),
                new SqlParameter("@DateMax" ,       maxDate),
                new SqlParameter("@StartRowIndex" , pageSize*(pageNumber-1) ),
                new SqlParameter("@PageSize" ,      pageSize )
                
                
                
            };
            var result = db.Database.SqlQuery<Bid>("dbo.GetBids @LotId , @PriceMin , @PriceMax ,@DateMin , @DateMax , @StartRowIndex , @PageSize ", parameters[0] , parameters[1] , parameters[2] , parameters[3] , parameters[4] , parameters[5] , parameters[6]).ToList();

            result = string.IsNullOrEmpty(order) ? result : result.AsQueryable().ApplySort(order).ToList();
            return result;
        }

    }
}
