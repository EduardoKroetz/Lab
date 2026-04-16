namespace Lab.Api.Application.DTOs;

public class ResponseDto
{
    public ResponseDto(string errorMessage)
    {
        Errors = [errorMessage];
        Data = null;
    }

    public ResponseDto(List<string> errors)
    {
        Errors = errors;
        Data = null;
    }

    public ResponseDto(object data)
    {
        Data = data;
    }

    public ResponseDto()
    {
        Data = null;
    }

    public List<string> Errors { get; set; } = [];
    public object? Data { get; set; }
}
