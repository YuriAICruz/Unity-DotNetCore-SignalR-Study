namespace DefaultNamespace
{
    public class HttpResponse<T>
    {
        public T Response;
        public bool Success;
        public int StatusCode;

        public HttpResponse(T response, int statusCode, bool success)
        {
            Response = response;
            StatusCode = statusCode;
            Success = success;
        }

        public HttpResponse(int statusCode, bool success)
        {
            StatusCode = statusCode;
            Success = success;
        }
    }
}