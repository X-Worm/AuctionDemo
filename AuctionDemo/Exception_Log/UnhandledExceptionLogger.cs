using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace Exception_Log
{
    public class UnhandledExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            var ex = context.Exception;
            
            string strLogText = Environment.NewLine + "Message ---\n{0}" + ex.Message;
            strLogText += Environment.NewLine + "Source ---\n{0}" + ex.Source;
            strLogText += Environment.NewLine + "StackTrace ---\n{0}" + ex.StackTrace;
            strLogText += Environment.NewLine + "TargetSite ---\n{0}" + ex.TargetSite;

            if (ex.InnerException != null)
            {
                strLogText += Environment.NewLine + "Inner Exception is {0}" + ex.InnerException;//error prone
            }
            if (ex.HelpLink != null)
            {
                strLogText += Environment.NewLine + "HelpLink ---\n{0}" + ex.HelpLink;//error prone
            }

            var requestedURi = (string)context.Request.RequestUri.AbsoluteUri ;
            var requestMethod = context.Request.Method.ToString();
            var timeUtc = DateTime.Now;

            SqlErrorLogging sqlErrorLogging = new SqlErrorLogging();
            Exception_Log.Models.Exception_Log apiError = new Exception_Log.Models.Exception_Log()
            {
                Request_Id = Guid.NewGuid(),
               Exception_Message = strLogText,
                Route = requestedURi,
                Method = requestMethod,
                Exception_Date = DateTime.Now,
                RequestHeaders = context.Request.Headers.ToString()
            };
            sqlErrorLogging.InsertErrorLog(apiError);
        }
    }
}
