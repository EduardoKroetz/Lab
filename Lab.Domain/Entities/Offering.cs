using Lab.Domain.Common;

namespace Lab.Domain.Entities;

public class Offering : TenantEntity
{
    protected Offering() { } // EF
    public Offering(string name, string? description, decimal? price)
    {
        Name = name;
        Description = description;
        Price = price;

        Validate();
    }

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public decimal? Price { get; private set; }

    public void Update(string name, string? description, decimal? price)
    {
        Name = name;
        Description = description;
        Price = price;

        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Nome é obrigatório.");

        if (Price.HasValue && Price.Value < 0)
            throw new ArgumentException("O preço não pode ser negativo.");
    }
}
