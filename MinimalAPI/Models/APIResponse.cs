using System.Net;

namespace MinimalAPI.Models
{
    public class APIResponse
    {
        public APIResponse()
        {
            ErrorMessages = new List<string>();
        }
        public bool IsSuccess { get; set; }
        public Object Result { get; set; }

        public HttpStatusCode statusCode { get; set; }

        public List<string> ErrorMessages { get; set; }

    }
}
