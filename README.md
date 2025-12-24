# DotnetWorker

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-512bd4)](https://dotnet.microsoft.com/download)
[![Docker](https://img.shields.io/badge/Docker-Enabled-2496ed)](https://www.docker.com/)

**DotnetWorker** is a robust .NET Worker Service designed to monitor website status and health. Built with **Clean Architecture** and **Domain-Driven Design (DDD)** principles, it serves as a solid foundation for building scalable and maintainable background processing applications.

## Features

- 🏗️ **Clean Architecture**: Organized into Domain, Application, Infrastructure, and WorkerService layers for separation of concerns.
- 🎯 **Domain-Driven Design**: Implements DDD patterns like Aggregates, Entities, and Domain Events using `Ardalis.SharedKernel`.
- 🔄 **Worker Service**: Runs as a background service, perfect for long-running tasks and scheduled jobs.
- 🐳 **Docker Support**: Includes `Dockerfile` and `docker-compose.yml` for easy containerization and deployment.
- 📝 **Structured Logging**: Integrated with Serilog for comprehensive logging capabilities.
- 🧩 **Mediator**: Uses the Mediator pattern for loose coupling between application components.

## Getting Started

Follow these instructions to get the project up and running on your local machine for development and testing purposes.

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (Version 10.0 or later recommended)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (Optional, for containerized execution)

### Installation

1.  **Clone the repository:**

    ```bash
    git clone https://github.com/georgeparkdev/base-repository-dotnet-worker
    cd base-repository-dotnet-worker
    ```

2. **Rename the project (if needed):**

    ```bash
    ./scripts/rename-solution.sh
    ```


3.  **Setup the development environment:**

    You can use the provided setup script to initialize the environment.

    ```bash
    ./scripts/setup-dev.sh
    ```

### Running the Application

#### Local Development

To run the worker service locally:

```bash
dotnet run
```

#### Using Docker

To run the application using Docker Compose:

```bash
docker compose up -d
```

## Project Structure

The solution follows the Clean Architecture principles:

-   **`DotnetWorker.Domain`**: Contains the core business logic, entities (e.g., `WebsiteChecker`), and interfaces. This layer has no dependencies on other layers.
-   **`DotnetWorker.Application`**: Contains the application logic, commands, queries, and service interfaces. It depends only on the Domain layer.
-   **`DotnetWorker.Infrastructure`**: Implements the interfaces defined in the Application and Domain layers (e.g., data access, external service calls).
-   **`DotnetWorker.WorkerService`**: The entry point of the application. It configures the dependency injection container and hosts the background service.

## Contributing

We welcome contributions! Please read our [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct and the process for submitting pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
