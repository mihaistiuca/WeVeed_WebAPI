using Resources.Base.Responses;
using System.Collections.Generic;

namespace Resources.Base.Exception
{
    public class HttpStatusCodeException : System.Exception
    {
        public HttpStatusCodeException(int statusCode)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCodeException(int statusCode, List<string> messages)
        {
            StatusCode = statusCode;
            GeneralErrors = messages;
        }

        public HttpStatusCodeException(int statusCode, List<PropertyError> propertyErrors)
        {
            StatusCode = statusCode;
            PropertyErrors = propertyErrors;
        }

        public int StatusCode { get; set; }

        public List<PropertyError> PropertyErrors { get; set; }

        public List<string> GeneralErrors { get; set; }
    }
}
