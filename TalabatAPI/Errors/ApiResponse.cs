namespace Talabat.API.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }

        public ApiResponse(int statusCode, string? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDeafultMessageForStatusCode(statusCode);
        }

        private string? GetDeafultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "You have Made a Bad Request",
                401 => "You are not Authorized",
                404 => "Resource was not Found",
                500 => "Internal Server Error",
                _ => null
            };
        }
    }
}
