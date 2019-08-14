using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionDemo.DAL.Models.ExceptionContext
{
    public class SqlErrorLogging
    {
        public void InsertErrorLog(Exception_Log apiError)
        {
            try
            {
                // Add Exception Log to database
                ExceptionContext db = new ExceptionContext();
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
