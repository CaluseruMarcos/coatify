namespace coatify.Api.ErrorHandling;
using System.Net;
public class GlobalExceptionHandler : ErrorResponse
{

    public GlobalExceptionHandler(HttpStatusCode statusCode, string message) 
    {
        this.StatusCode = statusCode;
        this.Message = message;

    }
    
    public static  GlobalExceptionHandler HandleException(Exception ex)
    {
        

        switch (ex)
        {
            case BadHttpRequestException badRequest:
                return new GlobalExceptionHandler((HttpStatusCode)badRequest.StatusCode, badRequest.Message);
            case ArgumentException:
                return new GlobalExceptionHandler(HttpStatusCode.BadRequest, ex.Message);
            case KeyNotFoundException:
                return new GlobalExceptionHandler(HttpStatusCode.NotFound, ex.Message);
            default:
                return new GlobalExceptionHandler(HttpStatusCode.InternalServerError, "An unexpected error occurred.");
        }
   
    }

}