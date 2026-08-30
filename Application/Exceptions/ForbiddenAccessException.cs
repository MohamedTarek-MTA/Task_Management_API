namespace Task_Management_API.Application.Exceptions
{
    public class ForbiddenAccessException: Exception
    {
        public int StatusCode { get;  }
        public ForbiddenAccessException(string message, int statusCode = 403) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
