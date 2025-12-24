# Contributing to DotnetWorker

Thank you for your interest in contributing to DotnetWorker! This document provides guidelines and information to help you contribute effectively.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (version 10.0.101 or later)
- Git

## Development Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/georgeparkdev/base-repository-dotnet-worker
   cd base-repository-dotnet-worker
   ```

2. **Run the development setup script:**
   ```bash
   ./scripts/setup-dev.sh
   ```
   This script will guide you through optional setup steps like building Docker images.

   Alternatively, you can set up manually:
   - Ensure .NET 10 SDK is installed
   - Restore dependencies: `dotnet restore`

## Building the Project

Build the solution:
```bash
dotnet build DotnetWorker.sln
```

## Running Tests

Run all tests:
```bash
dotnet test
```

The project includes:
- Unit tests
- Integration tests
- Architecture tests

## Coding Standards

- Follow the settings in `Directory.Build.props`:
  - Nullable reference types enabled
  - Implicit usings enabled
  - Latest C# language version
  - XML documentation generation enabled
  - Warnings treated as errors
- Code analysis with StyleCop.Analyzers and SonarAnalyzer.CSharp
- Use meaningful variable and method names
- Add XML comments for public APIs (if needed)

## Pull Request Process

1. Fork the repository and create a feature branch from `dev` (`users/{your-username}/feautres/{feature-description}`)
2. Make your changes following the coding standards
3. Ensure all tests pass locally
4. Update documentation if needed
5. Commit your changes with clear, descriptive messages
6. Push to your fork and create a pull request

## Continuous Integration

Pull requests will trigger automated CI checks:
- Build verification
- Test execution
- License compliance checks

Ensure your changes pass all CI checks before requesting review.

## Reporting Issues

When reporting bugs or requesting features:
- Use GitHub Issues
- Provide clear descriptions and steps to reproduce
- Include relevant logs, error messages, or screenshots
- Specify your environment (.NET version, OS, etc.)

## License

By contributing, you agree that your contributions will be licensed under the same license as the project (see [LICENSE](LICENSE) file).
