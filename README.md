# job-tracker-app-ai-service

Handles AI and LLM integrations including job description parsing and message drafting.

## Technology
- .NET 8 Web API
- C#
- PostgreSQL
- Docker

## Getting started

```bash
dotnet restore
dotnet build
dotnet run --project src/AiService.Api
```

## Running with Docker

```bash
docker build -t job-tracker-app-ai-service .
docker run -p 5006:5006 job-tracker-app-ai-service
```

## Environment variables

| Variable | Description |
|----------|-------------|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string |
| `Auth0__Domain` | Auth0 domain |
| `Auth0__Audience` | Auth0 API audience |
| `Kafka__BootstrapServers` | Kafka broker address |

## Project structure

```
src/
  AiService.Api/          # Web API entry point, controllers, middleware
  AiService.Core/         # Domain models, interfaces, business logic
  AiService.Infrastructure/ # Data access, Kafka, external integrations
tests/
  AiService.UnitTests/
  AiService.IntegrationTests/
```
