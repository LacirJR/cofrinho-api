# Cofrinho API

API minimalista em .NET 9 para simular um cofrinho (poupança) com base em Objetivos e Transações. O projeto segue uma arquitetura em camadas (API, Application, Core e Infrastructure) e inclui documentação via OpenAPI e UI interativa (Scalar).

Última atualização deste README: 2025-08-29 01:23

## Sumário
- Visão geral
- Arquitetura do projeto
- Requisitos
- Variáveis de ambiente
- Banco de dados e migrações
- Como executar localmente
- Documentação e endpoints
- Testes
- Notas sobre mapeamento e padrões
- Suporte e contribuição
- Licença

---

## Visão geral
A API permite gerenciar Objetivos financeiros, contendo título, descrição, valor alvo (com moeda), prazo, status e categoria. Também há suporte para Transações relacionadas (estrutura no domínio). As rotas de Objetivo já estão implementadas com MediatR e mapeamento para ViewModels usando Mapster. Um serviço em segundo plano calcula rendimentos diariamente com base em dias úteis (RendimentoHostedService).

## Arquitetura do projeto
Solução dividida em quatro projetos:

- cofrinho.api (Minimal API)
  - Ponto de entrada (Program.cs)
  - Registro das rotas (ex.: Routes/ObjetivoRoutes.cs)
  - Documentação (Swagger/OpenAPI + Scalar UI)
- cofrinho.application
  - Commands e Queries (MediatR)
  - ViewModels e utilitários de aplicação (ex.: ResultViewModel, PagedResult)
  - Mapeamentos com Mapster para saída da API (ex.: ObjetivoViewModel)
- cofrinho.core
  - Entidades (Objetivo, Transacao, etc.) e Enums (StatusObjetivoEnum, CategoriaEnum, TipoMoedaEnum)
  - Value Objects (Dinheiro, Url) e extensões (EnumExtensions)
  - Regras de domínio, validações (Flunt)
- cofrinho.infrastructure
  - Persistência, DbContext (schema "Cofrinho"), Repositórios e Migrations
  - Extensões de DI e integrações (ex.: serviços de dias úteis, relógio do sistema)

Tecnologias principais:
- .NET 9 (Minimal APIs)
- MediatR (CQRS básico para Commands/Queries)
- Mapster (mapeamento de entidades para ViewModels)
- OpenAPI/Swagger + Scalar UI
- Flunt (validações de domínio)
- Entity Framework Core + Npgsql (PostgreSQL)

## Requisitos
- .NET SDK 9.x instalado
- IDE recomendada: JetBrains Rider, Visual Studio 2022 ou VS Code (C# Dev Kit)

## Variáveis de ambiente
A API carrega todas as variáveis de ambiente no startup e permite injetá-las via IConfiguration.

- DB_CONNECTION: string de conexão Npgsql usada para configurar o DbContext.
  - Exemplo: `Host=localhost;Port=5432;Database=cofrinho;Username=postgres;Password=postgres;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=50;`
- LICENSE_MEDIATR: opcional; licença do MediatR (se não informada, usa defaults do pacote OSS).
- ASPNETCORE_ENVIRONMENT: controla middleware de erros (Development usa DeveloperExceptionPage; outros usam ExceptionHandler em `/error`).

Dica: você pode usar um arquivo .env localmente; todas as variáveis serão lidas na inicialização.

## Banco de dados e migrações
- Provider: PostgreSQL (Npgsql)
- Schema: `Cofrinho`
- Migrations em: `cofrinho.infrastructure\Persistence\Migrations`
- Migração automática no startup: a aplicação executa `context.Database.MigrateAsync()` (WebApplicationExtensions.MigrateDatabaseAsync). Se o DB_CONNECTION estiver inválido ou o banco inacessível, o startup falhará.

Comandos úteis (a partir da raiz):
- Adicionar migration: `dotnet ef migrations add <Nome> --project .\cofrinho.infrastructure --startup-project .\cofrinho.api`
- Atualizar banco: `dotnet ef database update --project .\cofrinho.infrastructure --startup-project .\cofrinho.api`

Pré-requisito: instale a ferramenta EF Core se necessário: `dotnet tool install -g dotnet-ef`.

## Como executar localmente
1) Restaurar e compilar a solução:
   - `dotnet restore`
   - `dotnet build`
2) Definir variáveis de ambiente necessárias (especialmente DB_CONNECTION) antes de iniciar.
3) Executar a API:
   - `dotnet run --project .\cofrinho.api\cofrinho.api.csproj`
4) Endereços padrão (Development):
   - URL base: http://localhost:5082
   - UI de documentação (Scalar): http://localhost:5082/scalar
   - Documento OpenAPI: http://localhost:5082/openapi/v1.json

Observações:
- Serialização via System.Text.Json com enums como string.
- ASPNETCORE_ENVIRONMENT controla o ambiente.
- Migrações automáticas rodam no startup; garanta que o banco esteja acessível.

## Documentação e endpoints
UI e especificação:
- UI interativa (Scalar): /scalar
- OpenAPI JSON: /openapi/v1.json

Rotas de Objetivo (group: /api/objetivo):
- GET /api/objetivo?Page={int}&PageSize={int}
  - Lista paginada de objetivos
  - Query: Page (default 1), PageSize (default 10)
  - Retornos:
    - 200 OK: ResultViewModel<PagedResult<ObjetivoViewModel>> quando há itens
    - 204 No Content: quando não há itens
    - 400 Bad Request: em caso de erro
- POST /api/objetivo
  - Cria um novo objetivo
  - Body: CriarObjetivoCommand (campos como Titulo, Descricao, ValorAlvo, TipoMoeda, Prazo, Categoria)
  - Retornos: 201 Created (sucesso) ou 400 Bad Request (erro)
- PUT /api/objetivo/{id}
  - Edita um objetivo existente
  - Body: EditarObjetivoByIdCommand (o id é passado na rota)
  - Retornos: 200 OK ou 400 Bad Request
- DELETE /api/objetivo/{id}
  - Remove (soft delete) um objetivo
  - Retornos: 200 OK ou 400 Bad Request

Modelo de saída (ObjetivoViewModel) mapeado de Objetivo inclui, entre outros:
- Id, Titulo, Descricao, ImagemUrl, ValorAlvo (DinheiroViewModel), Prazo, Status (string), Categoria (descrição), DataCriacao

## Testes
Há um projeto de testes `cofrinho.tests` com cobertura para Core e Application.

Como executar:
- A partir da raiz: `dotnet test .\cofrinho.tests\cofrinho.tests.csproj -v minimal`

Boas práticas:
- Testes de domínio (ValueObjects, entidades) no `cofrinho.core`.
- Testes de Commands/Queries e mapeamentos no `cofrinho.application`.
- Evite dependência do banco nos testes unitários.

## Notas sobre mapeamento e padrões
- Mapster está configurado nos ViewModels da camada de aplicação.
  - Objetivo -> ObjetivoViewModel: conversões para strings, descrição de enums via EnumExtensions.GetDescription, Url para string, e Dinheiro para DinheiroViewModel.
- Padrão CQRS com MediatR: Commands para escrita (Criar, Editar, Remover) e Queries para leitura (Listar).
- Paginação: PagedResult<T> encapsula itens, total e metadados de página.
- Background: serviço de rendimento periódico baseado em dias úteis (consome BrasilApiDiasUteisService).


