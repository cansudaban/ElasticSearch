using System.Net;

namespace ElasticSearch.API.DTOs
{
    public class ResponseDto<T>
    {
        public T? Data { get; set; }
        public List<String>? Errors { get; set; }
        public HttpStatusCode Status { get; set; }

        public static ResponseDto<T> Success<T>(T data, HttpStatusCode status)
        {
            return new ResponseDto<T> { Data = data, Status = status };
        }

        public static ResponseDto<T> Fail(List<String> errors, HttpStatusCode status)
        {
            return new ResponseDto<T>
            {
                Errors = errors,
                Status = status
            };
        }

        public static ResponseDto<T> Fail(string error, HttpStatusCode status)
        {
            return new ResponseDto<T>
            {
                Errors = new List<string> { error },
                Status = status
            };
        }
    }
}
