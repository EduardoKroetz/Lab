namespace Lab.Api.Domain.Exceptions;

public class NotFoundException : BaseException
{
    public NotFoundException(string message) : base(message)
    {
    }
}
