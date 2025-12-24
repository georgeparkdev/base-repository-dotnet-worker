using NetArchTest.Rules;

namespace DotnetWorker.ArchitectureTests.DomainTests;

public class SpecificationsTests
{
    [Fact]
    public void Specifications_ShouldBe_SealedClasses()
    {
        // Arrange

        // Act
        TestResult result = Types.InAssembly(Domain.AssemblyReference.Assembly)
            .That()
            .Inherit(typeof(Ardalis.Specification.ISpecification<>))
            .Should()
            .BeSealed()
            .And()
            .BeClasses()
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "All specifications should be sealed classes.");
    }

    [Fact]
    public void Specifications_ShouldHave_SpecPostfix()
    {
        // Arrange

        // Act
        TestResult result = Types.InAssembly(Domain.AssemblyReference.Assembly)
            .That()
            .Inherit(typeof(Ardalis.Specification.ISpecification<>))
            .Should()
            .HaveNameEndingWith("Spec")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "All specifications should have names ending with 'Spec'.");
    }

    [Fact]
    public void Specifications_ShouldBeIn_SpecificationsNamespace()
    {
        // Arrange

        // Act
        TestResult result = Types.InAssembly(Domain.AssemblyReference.Assembly)
            .That()
            .Inherit(typeof(Ardalis.Specification.ISpecification<>))
            .Should()
            .ResideInNamespaceEndingWith(".Specifications")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "All specifications should reside in namespaces ending with '.Specifications'.");
    }
}
