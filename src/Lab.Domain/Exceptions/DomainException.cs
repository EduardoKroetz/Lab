namespace Lab.Domain.Exceptions;

public class DomainException : CustomException
{
    public DomainException(string message) : base(message)
    {

    }
}
