using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AuctionDemo.DAL.Models
{
    public class OAuthContext : IdentityDbContext<IdentityUser>
    {
        public OAuthContext()
            : base("name=AuctionContext1", throwIfV1Schema: false)
        {
        }

        public static OAuthContext Create()
        {
            return new OAuthContext();
        }
    }
}
