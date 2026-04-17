namespace Lab.Api.Domain.Entities.Base;

public class BaseEntity
{
    protected BaseEntity() // EF
    {
    }

    public Guid Id { get; private set; }
}
