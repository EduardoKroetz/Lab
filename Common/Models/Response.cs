namespace Lab.Api.Common.Models;

public class Response
{
    public Response(string errorMessage)
    {
        Errors = [errorMessage];
        Data = null;
    }

    public Response(List<string> errors)
    {
        Errors = errors;
        Data = null;
    }

    public Response(object data)
    {
        Data = data;
    }

    public Response()
    {
        Data = null;
    }

    public List<string> Errors { get; set; } = [];
    public object? Data { get; set; }
}
