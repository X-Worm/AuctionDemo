using AuctionDemo.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace AuctionDemo.ViewModels.Mappers
{
    public class UserMapper
    {
        public static Expression<Func<User, UserViewModel>> ToLotViewModel()
        {
            return x => new UserViewModel
            {
                Name = x.Name,
                Balance = x.Balance,
                FrozenBalance = x.FrozenBalance

            };
        }
    }
}