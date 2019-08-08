using AuctionDemo.Models;
using AuctionDemo.Models.Unit_of_Work;
using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace AuctionDemo.Services
{
    public class UserConfigurationService
    {
        private readonly UnitOfWork unitOfWork = new UnitOfWork();
        private readonly string ConnectionString;

        public User_Configuration GetUserConfigurations(short? userId)
        {
            // return user configuration
            var result = unitOfWork.User_Configuration.dbSet.Where(item => item.User_Id == userId).FirstOrDefault();
            if (result == null)
            {
                throw new BadRequestException("user doesnt have configurations");
            }
            return result;
        }

        public User_Configuration PostUserConfigurations(short? userId , User_Configuration configuration)
        {
            // find users configuration
            var currentUserConfiguration = unitOfWork.User_Configuration.dbSet.Where(item => item.User_Id == userId).FirstOrDefault();

            if(currentUserConfiguration == null)
            {
                //post new configurations
                configuration.User_Id = userId.Value;
                unitOfWork.User_Configuration.dbSet.Add(configuration);
            }
            else
            {
                //Update Configurations
                currentUserConfiguration.Auction_Finished = configuration.Auction_Finished;
                currentUserConfiguration.Bid_Placed_Higher = configuration.Bid_Placed_Higher;
                currentUserConfiguration.Bid_Win_Lot = configuration.Bid_Win_Lot;
                unitOfWork.User_Configuration.Update(currentUserConfiguration);
            }
            unitOfWork.Save();

            return configuration;
        }

        public User_Configuration PutUserConfigurations(short? userId,User_Configuration configuration)
        {
            // find users configuration
            var currentUserConfiguration = unitOfWork.User_Configuration.dbSet.Where(item => item.User_Id == userId).FirstOrDefault();

            if (currentUserConfiguration == null)
            {
                //post new configurations
                configuration.User_Id = userId.Value;
                unitOfWork.User_Configuration.dbSet.Add(configuration);
            }
            else
            {
                // Update configurations
                currentUserConfiguration.Auction_Finished = configuration.Auction_Finished;
                currentUserConfiguration.Bid_Placed_Higher = configuration.Bid_Placed_Higher;
                currentUserConfiguration.Bid_Win_Lot = configuration.Bid_Win_Lot;
            }
            unitOfWork.Save();

            return configuration;
        }
    }
}