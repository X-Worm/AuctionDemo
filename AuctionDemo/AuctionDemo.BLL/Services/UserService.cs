using AuctionDemo.BLL.ExceptionHandler;
using AuctionDemo.DAL.Models;
using AuctionDemo.DAL.Models.Unit_of_Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionDemo.BLL.Services
{
    public class UserService
    {
        private readonly UnitOfWork unitOfWork = new UnitOfWork();
        private readonly string ConnectionString;

        public int? GetAccount(short? UserId)
        {
            if (UserId != null)
            {
                return unitOfWork.User.dbSet.Where(item => item.UserId == UserId).Select(item => item.Balance).FirstOrDefault();
            }
            else throw new NewBadRequestException("Server Error, invalid User_Id");
        }

        public int AddMoney(short? UserId, int amount)
        {
            if (UserId == null) throw new NewBadRequestException("Server Error, invalid User_Id");
            else
            {
                var userAccount = unitOfWork.User.dbSet.Where(item => item.UserId == UserId).FirstOrDefault();
                userAccount.Balance += amount;
                unitOfWork.User.Update(userAccount);
                unitOfWork.Save();
            }
            return amount;
        }


        public void RegistrateUser(User user)
        {
            // check if there is a user with the same login and password
            var IsNotUnique = unitOfWork.User.dbSet.Any(item => item.Login == user.Login && item.Password == user.Password);
            if (IsNotUnique) throw new NewBadRequestException("there is a user with the same login and password");

            else
            {
                unitOfWork.User.dbSet.Add(user);
            }
            unitOfWork.Save();
        }

        public int WidthdrawFromAccount(short? UserId, int amount)
        {
            // Check if User can widthraw this amount
            var UserBalance = unitOfWork.User.dbSet.Where(item => item.UserId == UserId).FirstOrDefault();
            if (UserBalance.Balance < amount) throw new NewBadRequestException("You cant widthraw this amount , maximum amount to widthraw is " + UserBalance.Balance.ToString());
            else
            {
                UserBalance.Balance -= amount;
                unitOfWork.User.Update(UserBalance);
                unitOfWork.Save();
            }
            return amount;
        }
    }
}
