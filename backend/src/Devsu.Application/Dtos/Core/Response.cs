using System.Text.Json.Serialization;

namespace Devsu.Application.Dtos.Core;

public class Response
{
    public bool IsSuccess => string.IsNullOrEmpty(Message) && !IsNotFound;
    public string? Message { get; set; }
    
    [JsonIgnore]
    public bool IsNotFound { get; set; }
}

public class Response<T> : Response
{
    public T? Data { get; set; }

    public Response()
    {
    }
    
    public Response(T data)
    {
        Data = data;
    }

    public Response(string message)
    {
        Message = message;
    }
}