using AuctionDemo.BLL.ExceptionHandler;
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
    public class UserConfigurationService
    {
        private readonly UnitOfWork unitOfWork = new UnitOfWork();
        private readonly string ConnectionString;

        public User_Configuration GetUserConfigurations(short? userId)
        {
            // return user configuration
            var result = unitOfWork.User_Configuration.dbSet.Where(item => item.UserId == userId).FirstOrDefault();
            if (result == null)
            {
                throw new NewBadRequestException("user doesnt have configurations");
            }
            return result;
        }

        public User_Configuration PostUserConfigurations(short? userId, User_Configuration configuration)
        {
            // find users configuration
            var currentUserConfiguration = unitOfWork.User_Configuration.dbSet.Where(item => item.UserId == userId).FirstOrDefault();

            if (currentUserConfiguration == null)
            {
                //post new configurations
                configuration.UserId = userId.Value;
                unitOfWork.User_Configuration.dbSet.Add(configuration);
            }
            else
            {
                //Update Configurations
                currentUserConfiguration.AuctionFinished = configuration.AuctionFinished;
                currentUserConfiguration.BidPlacedHigher = configuration.BidPlacedHigher;
                currentUserConfiguration.BidWinLot = configuration.BidWinLot;
                unitOfWork.User_Configuration.Update(currentUserConfiguration);
            }
            unitOfWork.Save();

            return configuration;
        }

        public User_Configuration PutUserConfigurations(short? userId, User_Configuration configuration)
        {
            // find users configuration
            var currentUserConfiguration = unitOfWork.User_Configuration.dbSet.Where(item => item.UserId == userId).FirstOrDefault();

            if (currentUserConfiguration == null)
            {
                //post new configurations
                configuration.UserId = userId.Value;
                unitOfWork.User_Configuration.dbSet.Add(configuration);
            }
            else
            {
                // Update configurations
                currentUserConfiguration.AuctionFinished = configuration.AuctionFinished;
                currentUserConfiguration.BidPlacedHigher = configuration.BidPlacedHigher;
                currentUserConfiguration.BidWinLot = configuration.BidWinLot;
            }
            unitOfWork.Save();

            return configuration;
        }
    }
}
