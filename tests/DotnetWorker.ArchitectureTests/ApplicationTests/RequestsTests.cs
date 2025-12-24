using NetArchTest.Rules;

namespace DotnetWorker.ArchitectureTests.ApplicationTests;

public class RequestsTests
{
    [Fact]
    public void Requests_ShouldBe_Sealed()
    {
        // Arrange

        // Act
        TestResult result = Types.InAssembly(Application.AssemblyReference.Assembly)
            .That()
            .ImplementInterface(typeof(Mediator.ICommand<>))
            .Or()
            .ImplementInterface(typeof(Mediator.IQuery<>))
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        Assert.True(
            result.IsSuccessful,
            $"The following request types are not sealed: {string.Join(", ", result.FailingTypes?.Select(t => t.FullName) ?? [])}");
    }

    [Fact]
    public void Requests_Should_ReturnResultObject()
    {
        // Arrange
        var assembly = Application.AssemblyReference.Assembly;

        // Act
        var requestTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType &&
                          (i.GetGenericTypeDefinition() == typeof(Mediator.ICommand<>) ||
                           i.GetGenericTypeDefinition() == typeof(Mediator.IQuery<>))))
            .ToList();

        var invalid = new List<Type>();

        foreach (var type in requestTypes)
        {
            var iface = type.GetInterfaces()
                .First(i => i.IsGenericType &&
                            (i.GetGenericTypeDefinition() == typeof(Mediator.ICommand<>) ||
                             i.GetGenericTypeDefinition() == typeof(Mediator.IQuery<>)));

            var responseType = iface.GetGenericArguments()[0];

            var isArdalisResult =
                responseType == typeof(Ardalis.Result.Result) ||
                (responseType.IsGenericType &&
                 responseType.GetGenericTypeDefinition() == typeof(Ardalis.Result.Result<>));

            if (!isArdalisResult)
            {
                invalid.Add(type);
            }
        }

        // Assert
        Assert.True(
            !invalid.Any(),
            $"The following request types do not return Ardalis.Result.Result<T>: {string.Join(", ", invalid.Select(t => t.FullName))}");
    }

    [Fact]
    public void Requests_NamingConvention_ShouldBeFollowed()
    {
        // Arrange

        // Act
        TestResult result = Types.InAssembly(Application.AssemblyReference.Assembly)
            .That()
            .ImplementInterface(typeof(Mediator.ICommand<>))
            .Should()
            .HaveNameEndingWith("Command")
            .GetResult();

        TestResult result2 = Types.InAssembly(Application.AssemblyReference.Assembly)
            .That()
            .ImplementInterface(typeof(Mediator.IQuery<>))
            .Should()
            .HaveNameEndingWith("Query")
            .GetResult();

        // Assert
        Assert.True(
            result.IsSuccessful,
            $"The following command types do not follow the naming convention: {string.Join(", ", result.FailingTypes?.Select(t => t.FullName) ?? [])}");

        Assert.True(
            result2.IsSuccessful,
            $"The following query types do not follow the naming convention: {string.Join(", ", result2.FailingTypes?.Select(t => t.FullName) ?? [])}");
    }

    [Fact]
    public void RequestHandlers_ShouldBe_SealedClasses()
    {
        // Arrange

        // Act
        TestResult result = Types.InAssembly(Application.AssemblyReference.Assembly)
            .That()
            .ImplementInterface(typeof(Mediator.ICommandHandler<,>))
            .Or()
            .ImplementInterface(typeof(Mediator.IQueryHandler<,>))
            .Should()
            .BeSealed()
            .And()
            .BeClasses()
            .GetResult();

        // Assert
        Assert.True(
            result.IsSuccessful,
            $"The following request handler types are not sealed classes: {string.Join(", ", result.FailingTypes?.Select(t => t.FullName) ?? [])}");
    }

    [Fact]
    public void RequestHandlers_NamingConvention_ShouldBeFollowed()
    {
        // Arrange

        // Act
        TestResult result = Types.InAssembly(Application.AssemblyReference.Assembly)
            .That()
            .ImplementInterface(typeof(Mediator.ICommandHandler<,>))
            .Or()
            .ImplementInterface(typeof(Mediator.IQueryHandler<,>))
            .Should()
            .HaveNameEndingWith("Handler")
            .GetResult();

        // Assert
        Assert.True(
            result.IsSuccessful,
            $"The following request handler types do not follow the naming convention: {string.Join(", ", result.FailingTypes?.Select(t => t.FullName) ?? [])}");
    }

    [Fact]
    public void NotificationHandlers_NamingConvention_ShouldBeFollowed()
    {
        // Arrange

        // Act
        TestResult result = Types.InAssembly(Application.AssemblyReference.Assembly)
            .That()
            .ImplementInterface(typeof(Mediator.INotificationHandler<>))
            .Should()
            .HaveNameEndingWith("EventHandler")
            .GetResult();

        // Assert
        Assert.True(
            result.IsSuccessful,
            $"The following notification handler types do not follow the naming convention: {string.Join(", ", result.FailingTypes?.Select(t => t.FullName) ?? [])}");
    }
}
