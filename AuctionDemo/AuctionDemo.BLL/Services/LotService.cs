using AuctionDemo.BLL.Extensions;
using AuctionDemo.DAL.Models;
using AuctionDemo.DAL.Models.Unit_of_Work;
using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using AuctionDemo.BLL.ExceptionHandler;

namespace AuctionDemo.BLL.Services
{
    public class LotService
    {
        private readonly UnitOfWork unitOfWork = new UnitOfWork();
        private readonly string ConnectionString;


        public void CreateNewLot(Lot lot, short? userId)
        {

            // Set lot start date
            lot.StartDate = DateTime.UtcNow;

            // set userId - owner of the lot
            lot.UserId = userId.Value;

            // Check if user input right date (Time of Lot must be in range (5 - 30 minutes))
            TimeSpan LotTime = lot.TimeOfLot.TimeOfDay;
            TimeSpan minTime = new TimeSpan(0, 5, 0);
            TimeSpan maxTime = new TimeSpan(0, 30, 0);
            if (LotTime <= minTime || LotTime >= maxTime) throw new NewBadRequestException("Invalid time of lot : time of lot must be in range (5 - 30 minutes)");

            // Check If user input right Initial_Price , initial_price must be bigger than 0
            if (lot.InitialPrice < 1) throw new NewBadRequestException("Start price must be bigger than 0 conventional units");

            // Set End date
            lot.EndDate = lot.StartDate + LotTime;

            // Ser user_id_winner to 0
            lot.UserIdWinner = 0;

            // post new lot
            unitOfWork.Lot.dbSet.Add(lot);
            unitOfWork.Save();

        }

        public void DeleteLot(short? LotId, short? userID)
        {
            if (LotId == null) throw new NewBadRequestException("Invalid LotId");

            // Check if user is owner of this lot
            var IsOwner = unitOfWork.Lot.dbSet.Where(item => item.LotId == LotId).Select(item => item.UserId).FirstOrDefault();

            if (IsOwner == 0)
            {
                // Lot was deleted
                throw new NewBadRequestException("Lot doesnt exist");
            }

            if (IsOwner != userID) throw new NewBadRequestException("Cannot edit or delete this lot : this user is not owner of lot");

            // Check if this Lot has bids
            var IsHasBids = unitOfWork.Bid.dbSet.Any(item => item.LotId == LotId);
            if (IsHasBids) throw new NewBadRequestException("Imposible update lot because it has bids");

            // Check if lot with LotId exist
            var IsExist = unitOfWork.Lot.dbSet.Any(item => item.LotId == LotId);
            if (!IsExist) throw new NewBadRequestException("Invalid LotId");
            else
            {
                var objectToDelete = unitOfWork.Lot.dbSet.Where(item => item.LotId == LotId).FirstOrDefault();
                unitOfWork.Lot.Delete(objectToDelete);
                unitOfWork.Save();
            }

        }

        public List<Lot> GetLots(string filterLotName, bool? isFinished, string filterPrice, string filterDate, int pagesize, int pagenumber, string sort)
        {
            if (isFinished == null) isFinished = true;

            // Price formating
            List<int> filterPriceRange = new List<int>();
            int minPrice = 0, maxPrice = 0;

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
            DateTime minDate = DateTime.UtcNow; DateTime maxDate = DateTime.UtcNow;

            if (string.IsNullOrEmpty(filterDate)) filterDate = "";
            else if (!filterDate.Contains(","))
            {
                filterDate = DateTime.MinValue.ToString() + "," + filterDate;
                filterDateRange = filterDate.Split(',').Select(item => DateTime.Parse(item)).ToList();
                minDate = filterDateRange[0]; maxDate = filterDateRange[1];
            }
            else
            {
                filterDateRange = filterDate.Split(',').Select(item => DateTime.Parse(item)).ToList();
                minDate = filterDateRange[0]; maxDate = filterDateRange[1];
            }


            var query = unitOfWork.Lot.dbSet.AsQueryable();
            query = string.IsNullOrEmpty(filterLotName) ? query : query.Where(item => item.Name.StartsWith(filterLotName));
            query = string.IsNullOrEmpty(filterPrice) ? query : query.Where(item => item.CurrentPrice <= maxPrice && item.CurrentPrice >= minPrice);
            query = string.IsNullOrEmpty(filterDate) ? query : query.Where(item => item.StartDate <= maxDate && item.StartDate >= minDate);
            query = isFinished.Value ? query.Where(item => item.UserIdWinner != 0) : query.Where(item => item.UserIdWinner == 0);
            query = string.IsNullOrEmpty(sort) ? query.OrderBy(item => item.LotId) : query.ApplySort(sort);
            query = query.Skip(pagesize * (pagenumber - 1)).Take(pagesize);

            return query.ToList();
        }

        public Lot GetLot(short? LotId)
        {
            if (LotId == null) throw new NewBadRequestException("Invalid Lot_Id");
            var result = unitOfWork.Lot.dbSet
                .Where(item => item.LotId == LotId).FirstOrDefault();

            return result;

        }

        public void UpdateLot(short? LotId, short? userID, Lot lot)
        {
            lot.Bid = null;


            if (LotId == null) throw new NewBadRequestException("Invalid Lot_Id");
            lot.LotId = LotId.Value;

            // Check if user is owner of this lot
            var IsOwner = unitOfWork.Lot.dbSet.Where(item => item.LotId == LotId).Any(item => item.UserId == userID);
            if (!IsOwner) throw new NewBadRequestException("Cannot edit or delete this lot : this user is not owner of lot");

            // Check if this Lot has bids
            var IsHasBids = unitOfWork.Bid.dbSet.Any(item => item.LotId == LotId);
            if (IsHasBids) throw new NewBadRequestException("Imposible update lot because it has bids");

            // Check if user update cloesed lot
            var LotEndDate = unitOfWork.Lot.dbSet.Where(item => item.LotId == LotId).Select(item => item.EndDate).FirstOrDefault();
            if (DateTime.UtcNow >= LotEndDate) throw new NewBadRequestException("Imposible update lot. This lot is cloesd");


            // User can change only Initial_Price , TimeOfLot , Name or Description
            var localLot = unitOfWork.Lot.dbSet.Where(item => item.LotId == LotId).FirstOrDefault();
            localLot.InitialPrice = lot.InitialPrice;
            localLot.Name = lot.Name;
            if (lot.Description != null) localLot.Description = lot.Description;


            if (lot.TimeOfLot != default(DateTime))
            {
                // Check if user change Time of lot in right range
                TimeSpan LotTime = lot.TimeOfLot.TimeOfDay;
                TimeSpan minTime = new TimeSpan(0, 5, 0);
                TimeSpan maxTime = new TimeSpan(0, 30, 0);
                if (LotTime <= minTime || LotTime >= maxTime) throw new NewBadRequestException("Invalid time of lot : time of lot must be in range (5 - 30 minutes)");

                localLot.TimeOfLot = lot.TimeOfLot;
            }

            // Update lot
            unitOfWork.Lot.Update(localLot);
            unitOfWork.Save();

        }
    }
}
