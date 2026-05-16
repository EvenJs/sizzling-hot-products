

using Domain;

namespace Application.Tests.Builders;

public class ProductBuilder
{
    private string _id = "P1";
    private string _name = "Test Product";

    public ProductBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public ProductBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public Product Build()
    {
        return new Product(_id, _name);
    }
}