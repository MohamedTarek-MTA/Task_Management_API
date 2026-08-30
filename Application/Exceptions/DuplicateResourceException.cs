namespace Task_Management_API.Application.Exceptions
{
    public class DuplicateResourceException : Exception
    {
        public int StatusCode { get;  }
        public DuplicateResourceException(string message, int statusCode = 409) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
