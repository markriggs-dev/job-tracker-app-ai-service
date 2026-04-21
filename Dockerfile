FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5006

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/AiService.Api/AiService.Api.csproj", "src/AiService.Api/"]
COPY ["src/AiService.Core/AiService.Core.csproj", "src/AiService.Core/"]
COPY ["src/AiService.Infrastructure/AiService.Infrastructure.csproj", "src/AiService.Infrastructure/"]
RUN dotnet restore "src/AiService.Api/AiService.Api.csproj"
COPY . .
RUN dotnet build "src/AiService.Api/AiService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/AiService.Api/AiService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AiService.Api.dll"]
