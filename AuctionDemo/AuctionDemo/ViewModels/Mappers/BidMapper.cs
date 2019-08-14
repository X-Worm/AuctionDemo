using AuctionDemo.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace AuctionDemo.ViewModels.Mappers
{
    public class BidMapper
    {
        public static Expression<Func<Bid, BidViewModel>> ToBidViewModel()
        {
            return x => new BidViewModel
            {
                BidId = x.BidId,
                Comments = x.Comments,
                BidPrice = x.BidPrice,
                Date = x.Date
            };
        }
    }
}