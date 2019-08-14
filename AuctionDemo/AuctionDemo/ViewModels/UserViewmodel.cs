using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuctionDemo.ViewModels
{
    /// <summary>
    /// Represents view model of user 
    /// </summary>
    public class UserViewModel
    {
        /// <summary>
        /// Name of User
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Balance of user
        /// </summary>
        public int Balance { get; set; }

        /// <summary>
        /// FrozenBalance of User
        /// </summary>
        public int FrozenBalance { get; set; }


    }
}