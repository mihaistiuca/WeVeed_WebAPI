using System.Collections.Generic;
using System.Linq;

namespace Resources.Base.Responses
{
    public class PropertyError
    {
        public string PropertyName { get; set; }

        public List<string> Errors { get; set; } = new List<string>();
    }

    public class BaseResponse
    {
        public BaseResponse(bool isSuccess)
        {
            IsSuccess = isSuccess;
            Status = 200;
        }

        public BaseResponse(List<PropertyError> errors, int statusCode)
        {
            Errors = errors;
            Status = statusCode;
        }

        public BaseResponse(List<string> generalErrors, int statusCode)
        {
            GeneralErrors = generalErrors;
            Status = statusCode;
        }

        public BaseResponse(List<PropertyError> errors, List<string> generalErrors, int statusCode)
        {
            Errors = errors;
            GeneralErrors = generalErrors;
            Status = statusCode;
        }

        public bool IsSuccess { get; set; }

        public int Status { get; set; }

        public List<string> GeneralErrors { get; set; } = new List<string>();

        public List<PropertyError> Errors { get; set; } = new List<PropertyError>();

        public virtual object Data { get; set; }
    }

    public class BaseResponse<T> : BaseResponse where T : class
    {
        public BaseResponse(T data) : base(true)
        {
            Data = data;
        }

        public BaseResponse(T data, bool isSuccess) : base(isSuccess)
        {
            Data = data;
        }

        public BaseResponse(T data, List<PropertyError> errors, int statusCode) : base(errors, statusCode)
        {
            Data = data;
        }

        public new T Data { get; set; }
    }

    public class IdResponse
    {
        public string Id { get; set; }
    }
}
