using NetArchTest.Rules;

namespace DotnetWorker.ArchitectureTests;

public class LayerTests
{
    [Fact]
    public void DomainLayer_ShouldNotHaveDependencyOn_ApplicationLayer()
    {
        // Arrange

        // Act
        TestResult result = Types.InAssembly(Domain.AssemblyReference.Assembly)
            .Should()
            .NotHaveDependencyOn(Application.AssemblyReference.Assembly.GetName().Name)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "Domain layer should not depend on Application layer.");
    }

    [Fact]
    public void DomainLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
    {
        // Arrange

        // Act
        TestResult result = Types.InAssembly(Domain.AssemblyReference.Assembly)
            .Should()
            .NotHaveDependencyOn(Infrastructure.AssemblyReference.Assembly.GetName().Name)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "Domain layer should not depend on Infrastructure layer.");
    }

    [Fact]
    public void ApplicationLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
    {
        // Arrange

        // Act
        TestResult result = Types.InAssembly(Application.AssemblyReference.Assembly)
            .Should()
            .NotHaveDependencyOn(Infrastructure.AssemblyReference.Assembly.GetName().Name)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "Application layer should not depend on Infrastructure layer.");
    }

    [Fact]
    public void ApplicationLayer_ShouldNotHaveDependencyOn_WorkerServiceLayer()
    {
        // Arrange

        // Act
        TestResult result = Types.InAssembly(Application.AssemblyReference.Assembly)
            .Should()
            .NotHaveDependencyOn(WorkerService.AssemblyReference.Assembly.GetName().Name)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "Application layer should not depend on Worker Service layer.");
    }

    [Fact]
    public void InfrastructureLayer_ShouldNotHaveDependencyOn_WorkerServiceLayer()
    {
        // Arrange

        // Act
        TestResult result = Types.InAssembly(Infrastructure.AssemblyReference.Assembly)
            .Should()
            .NotHaveDependencyOn(WorkerService.AssemblyReference.Assembly.GetName().Name)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "Infrastructure layer should not depend on Worker Service layer.");
    }
}
