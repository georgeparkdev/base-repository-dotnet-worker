FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# COPY [".env", "."] # Uncomment if you have environment variables to copy
COPY ["global.json", "."]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["DotnetWorker.slnx", "."]

COPY ["src/DotnetWorker.Domain/DotnetWorker.Domain.csproj", "src/DotnetWorker.Domain/"]
COPY ["src/DotnetWorker.Application/DotnetWorker.Application.csproj", "src/DotnetWorker.Application/"]
COPY ["src/DotnetWorker.Infrastructure/DotnetWorker.Infrastructure.csproj", "src/DotnetWorker.Infrastructure/"]
COPY ["src/DotnetWorker.WorkerService/DotnetWorker.WorkerService.csproj", "src/DotnetWorker.WorkerService/"]

RUN dotnet restore "src/DotnetWorker.WorkerService/DotnetWorker.WorkerService.csproj"
COPY . .

WORKDIR "/src/src/DotnetWorker.WorkerService"

RUN dotnet build "DotnetWorker.WorkerService.csproj" -c Release -o /app/build

FROM build AS publish

RUN dotnet publish "DotnetWorker.WorkerService.csproj" -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "DotnetWorker.WorkerService.dll"]
