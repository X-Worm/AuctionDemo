using AuctionDemo.BLL.ExceptionHandler;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace Exception_Log
{
    public class UserException : Exception
    { }

    public class GlobalExceptionHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.BadRequest);
            string s = context.Exception.GetType().ToString();

            if (context.Exception.GetType() == typeof(NewBadRequestException))
            {
                result = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new JsonContent(new
                    {
                        Message = context.Exception.Message //return exception
                    }),
                    ReasonPhrase = "Bad Request",
                    StatusCode = HttpStatusCode.BadRequest
                };
            }
            else 
            {
                result = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new JsonContent(new
                    {
                        Message = context.Exception.Message //return exception
                    }),
                    ReasonPhrase = "Internal Server Error",
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }

            
               
                
           
            context.Result = new ErrorMessageResult(context.Request, result);
        }

        public class ErrorMessageResult : IHttpActionResult
        {
            private HttpRequestMessage _request;
            private readonly HttpResponseMessage _httpResponseMessage;

            public ErrorMessageResult(HttpRequestMessage request, HttpResponseMessage httpResponseMessage)
            {
                _request = request;
                _httpResponseMessage = httpResponseMessage;
            }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(_httpResponseMessage);
            }
        }
    }


    public class JsonContent : HttpContent
    {

        private readonly MemoryStream _Stream = new MemoryStream();
        public JsonContent(object value)
        {

            Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var jw = new JsonTextWriter(new StreamWriter(_Stream));
            jw.Formatting = Formatting.Indented;
            var serializer = new JsonSerializer();
            serializer.Serialize(jw, value);
            jw.Flush();
            _Stream.Position = 0;

        }
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return _Stream.CopyToAsync(stream);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _Stream.Length;
            return true;
        }
    }
}
