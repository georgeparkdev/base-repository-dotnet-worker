# ExampleService

This folder is used to provide an example how to define custom service that requires 3rd party tools integration. For example, some Ecommerce API, Authentication and etc.

The overral approach:
1. Define service interfaces on domain or application layer (it's up to your preferrences).
2. Define service implementation here, in Infrastructure layer (for example, if you are integrating with Steam, just create a "Steam" folder and put all Steam-related files there (service implementation, config (to use via IOptions), helper classes (like delegating handlers for auth and etc.))).
3. Register services in DI container (InfrastructureServicesCollectionExtensions)
4. Optionally, write tests with mocks to test how your system will behave when different scenarios occur.

> Don't forget to remove this folder, the method call from `InfrastructureServicesCollectionExtensions` and section in [appsettings.json](../../DotnetWorker.WorkerService/appsettings.json)
