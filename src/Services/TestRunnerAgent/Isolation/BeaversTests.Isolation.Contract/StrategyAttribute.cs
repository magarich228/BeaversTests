namespace BeaversTests.Isolation.Contract;

[AttributeUsage(AttributeTargets.Class)]
public class StrategyAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}