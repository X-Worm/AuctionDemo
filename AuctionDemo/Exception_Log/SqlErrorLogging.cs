using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exception_Log.Models;

namespace Exception_Log
{
    public class SqlErrorLogging
    {
        public void InsertErrorLog(Exception_Log.Models.Exception_Log apiError)
        {
            try
            {
                // Add Exception Log to database
                AuctionContext db = new AuctionContext();
                db.Exception_Log.Add(apiError);
                db.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
