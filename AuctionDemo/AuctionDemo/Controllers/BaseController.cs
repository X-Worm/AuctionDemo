using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Security.Claims;
using System.Web.Http;
using System.Configuration;
using Microsoft.Owin.Security;
using System.Web;

namespace AuctionDemo.Controllers
{
    /// <summary>
    /// Represent base api controller
    /// </summary>
    public class BaseController : ApiController
    {
        /// <summary>
        /// Gets the authenticated user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        internal virtual short UserId
        {

            get
            {
                if (User.Identity.IsAuthenticated)
                {
                    return short.Parse(User.Identity.GetUserId());
                }

                return -1;
            }
        }

        /// <summary>
        /// Gets the Auction connection string.
        /// </summary>
        /// <value>
        /// The Auction connection string.
        /// </value>
        public virtual string AuctionConnectionString => ConfigurationManager.ConnectionStrings["AuctionContext"].ConnectionString;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }


    }


    public static class IdentityExtentions
    {
        //
        // Summary:
        //     Return the user id using the UserIdClaimType
        //
        // Parameters:
        //   identity:
        public static string GetUserId(this IIdentity identity)
        {
            var userContext = HttpContext.Current.User.Identity;
            var id_user = ((ClaimsIdentity)userContext).Claims.FirstOrDefault(x => x.Type == "User_Id").Value;

            return id_user;
        }
    }

    
}
