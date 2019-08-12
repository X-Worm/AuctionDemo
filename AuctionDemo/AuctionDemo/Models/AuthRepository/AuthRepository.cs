//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;
//using System;
//using System.Collections.Generic;
//using System.IdentityModel;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web;

//namespace AuctionDemo.Models.AuthRepository
//{
//    public class AuthRepository : IDisposable
//    {
//        private OAuthContext authContext;
//        private UserManager<IdentityUser> userManager;

//        public AuthRepository()
//        {
//            authContext = new OAuthContext();
//            userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(authContext));
//        }

//        public async Task<IdentityResult> RegisterUser(User userModel)
//        {
//            userModel.Frozen_Balance = 0; 
//            userModel.Bid = null; userModel.Lot = null;
//            IdentityUser user = new IdentityUser
//            {
//                UserName = userModel.Name,
//                Id = userModel.User_Id.ToString()
//            };
//            int localUserId = 0;

//            var result =  IdentityUserExtension.CreateUser( userModel , ref localUserId);
//            user.Id = localUserId.ToString();
//            return result;
//        }

//        public async Task<IdentityUser> FindUser(string userName, string password)
//        {
//            return await userManager.FindAsync(userName, password);
//        }

//        public void Dispose()
//        {
//            authContext.Dispose();
//            userManager.Dispose();
//        }
//    }

//    public static class IdentityUserExtension
//    {
//        public  static IdentityResult CreateUser( User userModel , ref int id)
//        {
//            AuctionContext db = new AuctionContext();
//            // Validate model
//            // check if there is a user with the same login and password
//            var IsExist = db.User.Any(item => item.Password == userModel.Password && item.Login == userModel.Login);
//            if (IsExist) throw new BadRequestException("there is a user with the same login and password");

//            // Register new user
//            db.User.Add(userModel);
//            db.SaveChanges();

//            // Get User_id of created user 
//            id = db.User.OrderByDescending(item => item.User_Id).Select(item => item.User_Id).FirstOrDefault();

//            // Create default user configuration with default fields
//            User_Configuration localConfiguration = new User_Configuration();
//            localConfiguration.User_Id = (short)id;
//            localConfiguration.Auction_Finished = false;
//            localConfiguration.Bid_Placed_Higher = false;
//            localConfiguration.Bid_Win_Lot = false;
//            db.User_Configuration.Add(localConfiguration);
//            db.SaveChanges();

//            IdentityResult result = new IdentityResult();
//            return result;
//        }
//    }
//}