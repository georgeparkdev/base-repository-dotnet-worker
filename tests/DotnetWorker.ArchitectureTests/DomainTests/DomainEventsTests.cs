using NetArchTest.Rules;

namespace DotnetWorker.ArchitectureTests.DomainTests;

public class DomainEventsTests
{
    [Fact]
    public void DomainEvents_ShouldBe_SealedClasses()
    {
        // Arrange

        // Act
        TestResult result = Types.InAssembly(Domain.AssemblyReference.Assembly)
            .That()
            .Inherit(typeof(Ardalis.SharedKernel.DomainEventBase))
            .Should()
            .BeSealed()
            .And()
            .BeClasses()
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "All domain events should be sealed classes.");
    }

    [Fact]
    public void DomainEvents_ShouldHave_EventPostfix()
    {
        // Arrange

        // Act
        TestResult result = Types.InAssembly(Domain.AssemblyReference.Assembly)
            .That()
            .Inherit(typeof(Ardalis.SharedKernel.DomainEventBase))
            .Should()
            .HaveNameEndingWith("Event")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "All domain events should have names ending with 'Event'.");
    }

    [Fact]
    public void DomainEvents_ShouldBeIn_EventsNamespace()
    {
        // Arrange

        // Act
        TestResult result = Types.InAssembly(Domain.AssemblyReference.Assembly)
            .That()
            .Inherit(typeof(Ardalis.SharedKernel.DomainEventBase))
            .Should()
            .ResideInNamespaceEndingWith(".Events")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "All domain events should reside in namespaces ending with '.Events'.");
    }
}
