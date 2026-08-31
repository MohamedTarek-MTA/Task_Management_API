namespace Task_Management_API.API.Middlewares
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private static int _counter = 0;
        private static DateTime _lastRequestDate = DateTime.Now;
        private const int _maxRequestsPerSecond = 10;

        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context) 
        {
            _counter++;
            if(DateTime.Now.Subtract(_lastRequestDate).TotalSeconds >= 10)
            {
                _counter = 1;
                _lastRequestDate = DateTime.Now;
                await _next(context);
            }
            else
            {
                if(_counter > _maxRequestsPerSecond)
                {
                    _lastRequestDate = DateTime.Now;
                    context.Response.StatusCode = 429;
                    return;
                }
                else
                {
                    _lastRequestDate = DateTime.Now;
                    await _next(context);
                }
            }
        }
    }
}
