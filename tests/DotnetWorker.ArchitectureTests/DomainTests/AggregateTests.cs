using System.Reflection;
using NetArchTest.Rules;

namespace DotnetWorker.ArchitectureTests.DomainTests;

public class AggregateTests
{
    [Fact]
    public void Aggregates_ShouldNotContain_OtherAggregatesAsProperties()
    {
        // Arrange
        IEnumerable<Type> aggregateTypes = Types.InAssembly(Domain.AssemblyReference.Assembly)
            .That()
            .ImplementInterface(typeof(Ardalis.SharedKernel.IAggregateRoot))
            .GetTypes();

        var aggregateTypeSet = new HashSet<Type>(aggregateTypes);

        var violatingAggregates = new List<string>();

        // Act
        foreach (Type aggregateType in aggregateTypes)
        {
            var properties = aggregateType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (aggregateTypeSet.Contains(property.PropertyType))
                {
                    violatingAggregates.Add($"{aggregateType.FullName} contains property {property.Name} of type {property.PropertyType.FullName}");
                }
            }
        }

        // Assert
        Assert.True(
            violatingAggregates.Count == 0,
            $"The following aggregates contain other aggregates as properties: {string.Join("; ", violatingAggregates)}");
    }
}
