using System.Net;

namespace coatify.Api.ErrorHandling;

public class ErrorResponse 
{
    public HttpStatusCode StatusCode { get; set; }
    public string Message { get; set; }

   
}