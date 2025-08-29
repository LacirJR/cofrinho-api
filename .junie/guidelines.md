# Cofrinho API — Development Guidelines (advanced, project‑specific)

This document captures project‑specific knowledge for advanced contributors. Keep it concise and pragmatic; prefer verified commands and examples.

Last verified: 2025‑08‑27

## 1) Build and Configuration

- Target: .NET 9 (SDK 9.x required)
- Projects: cofrinho.api (Minimal API), cofrinho.application, cofrinho.core, cofrinho.infrastructure (EF Core + Npgsql)
- Build commands (from repo root):
  - dotnet restore
  - dotnet build

### 1.1 Runtime configuration (environment variables)
The API reads ALL environment variables into IConfiguration at startup (Program.cs: Env.Load + foreach Environment.GetEnvironmentVariables), so standard env var injection works.

Required/meaningful variables:
- DB_CONNECTION: Npgsql connection string used by cofrinho.infrastructure.Extensions.ServiceCollectionExtensions.AddInfrastructure to configure DbContext.
  - Example (local PostgreSQL):
    - DB_CONNECTION=Host=localhost;Port=5432;Database=cofrinho;Username=postgres;Password=postgres;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=50;
- LICENSE_MEDIATR: Optional MediatR license key (builder.Services.AddMediatR(cfg => cfg.LicenseKey = ...)). If not provided, MediatR still registers and runs with open‑source package defaults.
- ASPNETCORE_ENVIRONMENT: Affects error handling middleware choice. Development shows DeveloperExceptionPage; non‑development uses UseExceptionHandler("/error").

Notes:
- JSON serialization is configured to use enum as string (System.Text.Json JsonStringEnumConverter) both for MVC and HttpJsonOptions. OpenAPI is enabled at /openapi/{documentName}.json and Scalar UI at /scalar.
- Scalar options are customized for dark mode and sidebar.

### 1.2 Database and Migrations
- Provider: PostgreSQL (Npgsql)
- Schema: Cofrinho
- Migrations live in cofrinho.infrastructure\Persistence\Migrations (Initial migration exists as of 2025‑08‑26).
- Automatic migration on startup: Program.cs calls await app.MigrateDatabaseAsync(); which resolves CofrinhoDbContext and runs context.Database.MigrateAsync().
  - Implication: If DB_CONNECTION is invalid or DB is unreachable, the API will fail during startup.

Common EF commands (from repo root):
- Add migration: dotnet ef migrations add <Name> --project .\cofrinho.infrastructure --startup-project .\cofrinho.api
- Update DB: dotnet ef database update --project .\cofrinho.infrastructure --startup-project .\cofrinho.api
- Tooling: Ensure the dotnet-ef global tool is installed: dotnet tool install -g dotnet-ef

### 1.3 Running the API
- dotnet run --project .\cofrinho.api\cofrinho.api.csproj
- Default dev URLs (check your launch settings/port):
  - Scalar UI: http://localhost:5082/scalar
  - OpenAPI: http://localhost:5082/openapi/v1.json
- Ensure DB_CONNECTION env var is set before start if you want DB access to succeed; otherwise startup migration will fail.

## 2) Testing
There is no dedicated test project in the repository yet. Below are the verified steps to add and run tests locally. The example uses MSTest and targets cofrinho.core, which contains deterministic value objects useful for fast unit tests.

### 2.1 Create a new test project (example)
- Create a test project (xUnit/NUnit/MSTest all work; verified here with MSTest):
  - dotnet new mstest -n Cofrinho.Tests -o .\.tmp_tests
  - dotnet add .\.tmp_tests reference .\cofrinho.core\cofrinho.core.csproj
  - dotnet add .\.tmp_tests package Microsoft.NET.Test.Sdk --version 17.11.1
  - Optionally add to solution for IDE discovery:
    - dotnet sln cofrinho-api.sln add .\.tmp_tests\Cofrinho.Tests.csproj

- Sample test against Dinheiro value object (cofrinho.core\ValueObjects\Dinheiro.cs):
  - Create file .tmp_tests\DinheiroTests.cs with contents:
    
    using cofrinho.core.Enums;
    using cofrinho.core.ValueObjects;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    namespace Cofrinho.Tests;
    
    [TestClass]
    public class DinheiroTests
    {
        [TestMethod]
        public void Somar_DeveSomarMesmoTipoDeMoeda()
        {
            var a = Dinheiro.Criar(10m, TipoMoedaEnum.BRL);
            var b = Dinheiro.Criar(5m, TipoMoedaEnum.BRL);
            var c = a.Somar(b);
            Assert.IsTrue(c.IsValid);
            Assert.AreEqual(15m, c.Valor);
            Assert.AreEqual(TipoMoedaEnum.BRL, c.MoedaEnum);
        }
    }

### 2.2 Run tests
- Fast path from the repo root:
  - dotnet test .\.tmp_tests\Cofrinho.Tests.csproj -v minimal

This flow was verified during preparation of this document with a temporary MSTest project referencing cofrinho.core and two tests over Dinheiro. The temporary project was removed afterward to keep the repo clean.

### 2.3 Guidelines for adding tests going forward
- Prefer per-layer test projects to keep responsibilities clear:
  - cofrinho.core: pure unit tests for VOs, entities, domain rules.
  - cofrinho.application: tests for handlers (MediatR), mapping profiles, pagination helpers.
  - infrastructure: integration tests with an ephemeral PostgreSQL (e.g., Testcontainers or docker‑compose) if needed.
  - api: minimal endpoint tests can target handlers or run against the in‑memory test server (WebApplicationFactory) if adopted.
- Deterministic domain logic exists in:
  - ValueObjects: Dinheiro (sum/subtract same currency, validation), Url (format validation).
  - Enums + extensions: string conversion/description.
- Keep tests independent of database unless explicitly testing persistence.

## 3) Additional Development Notes

### 3.1 Architectural conventions
- CQRS with MediatR: application layer defines Commands and Queries; API maps routes to MediatR requests.
- Mapping: Mapster configured via services.AddMapster() in AddApplication; ViewModels in application layer define mapping configurations.
- EF Core: DbContext schema "Cofrinho"; entities derive from BaseEntity and set timestamps in overridden SaveChangesAsync; domain events are dispatched via IMediator after save (CofrinhoDbContext -> _mediator.DispatchDomainEvents(this)).
- Specifications: Ardalis.Specification is integrated; SpecificationEvaluator.Default is registered as singleton.

### 3.2 Configuration patterns and pitfalls
- Environment loading: Program.cs imports all environment variables into configuration; keys like DB_CONNECTION and LICENSE_MEDIATR must be present in the environment (or .env via DotNetEnv) before starting.
- Automatic migrations on start: convenient locally; in production consider gating or running migrations separately to avoid startup race conditions.
- JSON: enums serialized as strings; keep OpenAPI schema filters aligned (EnumAsStringSchemaFilter).

### 3.3 Useful run/debug tips
- Start API: dotnet run --project .\cofrinho.api
- If startup fails immediately, check:
  - DB_CONNECTION reachability and credentials
  - Missing env vars (e.g., when running from IDE profiles)
- Swagger/Scalar for manual verification:
  - http://localhost:5082/openapi/v1.json
  - http://localhost:5082/scalar

### 3.4 EF Core model/migration hygiene
- After changing entities/configurations in infrastructure or domain, run a new migration from repo root:
  - dotnet ef migrations add <Name> --project .\cofrinho.infrastructure --startup-project .\cofrinho.api
- Verify generated SQL (ensure schema Cofrinho) and apply locally: dotnet ef database update
- Ensure Npgsql package versions remain compatible with your PostgreSQL server.

### 3.5 Code style
- C# 12/.NET 9; nullable enabled across projects.
- Prefer immutable ValueObjects (e.g., Dinheiro, Url) with factory methods like Criar.
- Validation via Flunt; check Notifiable.IsValid and Notifications where appropriate.
- Mapster for DTO/ViewModel mapping; avoid manual mapping in endpoints.

### 3.6 Background and pipeline hooks
- WebApplicationExtensions.MigrateDatabaseAsync is invoked at startup.
- If adding background services or hosted tasks, register in api or infrastructure with care to respect DI scopes and DbContext lifetime.

---
Maintenance: When you update project conventions, reflect them here and in readme.md. Keep temporary test artifacts out of source control; the verified example above demonstrates creation and removal flow.
