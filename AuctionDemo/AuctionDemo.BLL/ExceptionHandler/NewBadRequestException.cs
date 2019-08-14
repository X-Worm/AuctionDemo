using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionDemo.BLL.ExceptionHandler
{
    public class NewBadRequestException : Exception
    {

        public NewBadRequestException()
        {

        }

        public NewBadRequestException(string message) : base(message)
        {

        }

        
    }
}
