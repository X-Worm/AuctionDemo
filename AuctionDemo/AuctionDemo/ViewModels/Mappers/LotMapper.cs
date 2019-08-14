using AuctionDemo.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace AuctionDemo.ViewModels.Mappers
{
    public class LotMapper
    {
        public static Expression<Func<Lot, LotViewModel>> ToLotViewModel()
        {
            return x => new LotViewModel
            {
                LotId = x.LotId,
                Name = x.Name,
                Description = x.Description,
                InitialPrice = x.InitialPrice,
                FinalPrice = x.FinalPrice,
                TimeOfLot = x.TimeOfLot
            };
        }
    }
}