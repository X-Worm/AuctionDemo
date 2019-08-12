using AuctionDemo.BLL.Extensions;
using AuctionDemo.DAL.Models;
using AuctionDemo.DAL.Models.Unit_of_Work;
using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionDemo.BLL.Services
{
    public class LotService
    {
        private readonly UnitOfWork unitOfWork = new UnitOfWork();
        private readonly string ConnectionString;


        public void CreateNewLot(Lot lot, short? userId)
        {

            // Set lot start date
            lot.Start_Date = DateTime.Now;

            // set userId - owner of the lot
            lot.User_Id = userId.Value;

            // Check if user input right date (Time of Lot must be in range (5 - 30 minutes))
            TimeSpan LotTime = lot.Time_Of_Lot.TimeOfDay;
            TimeSpan minTime = new TimeSpan(0, 5, 0);
            TimeSpan maxTime = new TimeSpan(0, 30, 0);
            if (LotTime <= minTime || LotTime >= maxTime) throw new BadRequestException("Invalid time of lot : time of lot must be in range (5 - 30 minutes)");

            // Check If user input right Initial_Price , initial_price must be bigger than 0
            if (lot.Initial_Price < 1) throw new BadRequestException("Start price must be bigger than 0 conventional units");

            // Set End date
            lot.End_Date = lot.Start_Date + LotTime;

            // Ser user_id_winner to 0
            lot.User_Id_Winner = 0;

            // post new lot
            unitOfWork.Lot.dbSet.Add(lot);
            unitOfWork.Save();

        }

        public void DeleteLot(short? LotId, short? userID)
        {
            if (LotId == null) throw new BadRequestException("Invalid LotId");

            // Check if user is owner of this lot
            var IsOwner = unitOfWork.Lot.dbSet.Where(item => item.Lot_Id == LotId).Select(item => item.User_Id).FirstOrDefault();

            if (IsOwner == 0)
            {
                // Lot was deleted
                throw new BadRequestException("Lot doesnt exist");
            }

            if (IsOwner != userID) throw new BadRequestException("Cannot edit or delete this lot : this user is not owner of lot");

            // Check if this Lot has bids
            var IsHasBids = unitOfWork.Bid.dbSet.Any(item => item.Lot_Id == LotId);
            if (IsHasBids) throw new BadRequestException("Imposible update lot because it has bids");

            // Check if lot with LotId exist
            var IsExist = unitOfWork.Lot.dbSet.Any(item => item.Lot_Id == LotId);
            if (!IsExist) throw new BadRequestException("Invalid LotId");
            else
            {
                var objectToDelete = unitOfWork.Lot.dbSet.Where(item => item.Lot_Id == LotId).FirstOrDefault();
                unitOfWork.Lot.Delete(objectToDelete);
                unitOfWork.Save();
            }

        }

        public List<Lot> GetLots(string filterLotName, bool isFinished, string filterPrice, string filterDate, int pagesize, int pagenumber, string sort)
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
                // disable filtering if filterPrice is empty
                toFilterDate.Add(DateTime.MinValue); toFilterDate.Add(DateTime.MaxValue);
            }



            int minFilterPrice = toFilterPrice[0];
            int maxFilterPrice = toFilterPrice[1];

            DateTime minFilterDate = toFilterDate[0];
            DateTime maxFilterDate = toFilterDate[1];


            if (isFinished)
            {
                var result = unitOfWork.Lot.dbSet
                .OrderBy(item => item.Lot_Id) // Default sort
                .Where(item => item.User_Id_Winner > 0 || item.User_Id_Winner == -1) // if lot is finished and has bids or is finished and doesnt has userIdWinner
                .Where(item => item.Name.StartsWith(filterLotName)) // apply filterLotName
                .Where(item => item.Current_Price >= minFilterPrice && item.Current_Price <= maxFilterPrice) // aply price filtering
                .Where(item => item.Start_Date >= minFilterDate && item.Start_Date <= maxFilterDate) // aply date filtering
                .ApplySort(sort)
                .Skip(pagesize * (pagenumber - 1)).Take(pagesize);

                return result.ToList();
            }
            else
            {
                var result = unitOfWork.Lot.dbSet
                    .OrderBy(item => item.Lot_Id) // Default sort
                    .Where(item => item.User_Id_Winner == 0)
                    .Where(item => item.Name.StartsWith(filterLotName)) // apply filterLotName
                    .Where(item => item.Current_Price >= minFilterPrice && item.Current_Price <= maxFilterPrice) // aply price filtering
                    .Where(item => item.Start_Date >= minFilterDate && item.Start_Date <= maxFilterDate) // aply date filtering
                    .ApplySort(sort)
                    .Skip(pagesize * (pagenumber - 1)).Take(pagesize);

                return result.ToList();
            }
        }

        public Lot GetLot(short? LotId)
        {
            if (LotId == null) throw new BadRequestException("Invalid Lot_Id");
            var result = unitOfWork.Lot.dbSet
                .Where(item => item.Lot_Id == LotId).FirstOrDefault();

            return result;

        }

        public void UpdateLot(short? LotId, short? userID, Lot lot)
        {
            lot.Bid = null;


            if (LotId == null) throw new BadRequestException("Invalid Lot_Id");
            lot.Lot_Id = LotId.Value;

            // Check if user is owner of this lot
            var IsOwner = unitOfWork.Lot.dbSet.Where(item => item.Lot_Id == LotId).Any(item => item.User_Id == userID);
            if (!IsOwner) throw new BadRequestException("Cannot edit or delete this lot : this user is not owner of lot");

            // Check if this Lot has bids
            var IsHasBids = unitOfWork.Bid.dbSet.Any(item => item.Lot_Id == LotId);
            if (IsHasBids) throw new BadRequestException("Imposible update lot because it has bids");

            // Check if user update cloesed lot
            var LotEndDate = unitOfWork.Lot.dbSet.Where(item => item.Lot_Id == LotId).Select(item => item.End_Date).FirstOrDefault();
            if (DateTime.Now >= LotEndDate) throw new Exception("Imposible update lot. This lot is cloesd");


            // User can change only Initial_Price , TimeOfLot , Name or Description
            var localLot = unitOfWork.Lot.dbSet.Where(item => item.Lot_Id == LotId).FirstOrDefault();
            localLot.Initial_Price = lot.Initial_Price;
            localLot.Name = lot.Name;
            if (lot.Description != null) localLot.Description = lot.Description;


            if (lot.Time_Of_Lot != default(DateTime))
            {
                // Check if user change Time of lot in right range
                TimeSpan LotTime = lot.Time_Of_Lot.TimeOfDay;
                TimeSpan minTime = new TimeSpan(0, 5, 0);
                TimeSpan maxTime = new TimeSpan(0, 30, 0);
                if (LotTime <= minTime || LotTime >= maxTime) throw new BadRequestException("Invalid time of lot : time of lot must be in range (5 - 30 minutes)");

                localLot.Time_Of_Lot = lot.Time_Of_Lot;
            }

            // Update lot
            unitOfWork.Lot.Update(localLot);
            unitOfWork.Save();

        }
    }
}
