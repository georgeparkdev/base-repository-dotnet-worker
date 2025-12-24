using System.Reflection;
using NetArchTest.Rules;

namespace DotnetWorker.ArchitectureTests.DomainTests;

public class EntitiesTests
{
    [Fact]
    public void Entities_ShouldBe_SealedClasses()
    {
        // Arrange

        // Act
        TestResult result = Types.InAssembly(Domain.AssemblyReference.Assembly)
            .That()
            .Inherit(typeof(Ardalis.SharedKernel.EntityBase))
            .Should()
            .BeClasses()
            .And()
            .BeSealed()
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "All entities should be sealed classes.");
    }

    [Fact]
    public void Entities_ShouldHave_PrivateParameterlessConstructor()
    {
        // Arrange
        IEnumerable<Type> entityTypes = Types.InAssembly(Domain.AssemblyReference.Assembly)
            .That()
            .Inherit(typeof(Ardalis.SharedKernel.EntityBase))
            .GetTypes();

        // Act
        var failingTypes = new List<Type>();
        foreach (Type entityType in entityTypes)
        {
            ConstructorInfo[] constructors = entityType.GetConstructors(BindingFlags.NonPublic |
                                                                        BindingFlags.Instance);

            if (!constructors.Any(c => c.IsPrivate && c.GetParameters().Length == 0))
            {
                failingTypes.Add(entityType);
            }
        }

        // Assert
        Assert.True(
            failingTypes.Count == 0,
            $"The following entities do not have a private parameterless constructor: {string.Join(", ", failingTypes.Select(t => t.FullName))}");
    }
}
