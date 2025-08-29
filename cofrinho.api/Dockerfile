FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["cofrinho.api/cofrinho.api.csproj", "cofrinho.api/"]
COPY ["cofrinho.application/cofrinho.application.csproj", "cofrinho.application/"]
COPY ["cofrinho.core/cofrinho.core.csproj", "cofrinho.core/"]
COPY ["cofrinho.infrastructure/cofrinho.infrastructure.csproj", "cofrinho.infrastructure/"]
RUN dotnet restore "cofrinho.api/cofrinho.api.csproj"
COPY . .
WORKDIR "/src/cofrinho.api"
RUN dotnet build "./cofrinho.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./cofrinho.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "cofrinho.api.dll"]
