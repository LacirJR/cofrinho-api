# Cofrinho API

API minimalista em .NET 9 para simulação de um cofrinho (poupança) com base em objetivos e transações. O projeto está organizado em camadas (API, Application, Core e Infrastructure) e já inclui documentação interativa via Scalar (Swagger UI alternativa) e OpenAPI.

## Sumário
- Visão geral
- Arquitetura do projeto
- Requisitos
- Como executar localmente
- Documentação e endpoints
- Arquivos úteis (cofrinho-api.http)
- Convenções e próximos passos
- Suporte e contribuição
- Licença

---

## Visão geral
O objetivo do projeto é oferecer uma base para uma API de “cofrinho” que permita cadastrar objetivos financeiros e suas transações até atingir uma meta (quantidade alvo), com possibilidade de prazos e status do objetivo.

No estado atual do repositório, existe uma rota de exemplo em ObjetivoRoutes que retorna um “Hello World”, e a infraestrutura de documentação está pronta para evolução dos endpoints.

## Arquitetura do projeto
A solução é dividida em quatro projetos:

- cofrinho.api (Minimal API)
  - Ponto de entrada da aplicação (Program.cs)
  - Registro das rotas (por exemplo, Routes/ObjetivoRoutes.cs)
  - Documentação (SwaggerGen + Scalar)
- cofrinho.application
  - Modelos de aplicação (ex.: ResultViewModel)
  - Camada para orquestrar casos de uso (futuramente)
- cofrinho.core
  - Entidades e enums de domínio (Objetivo, Transacao, StatusObjetivoEnum, etc.)
  - Lógica de domínio comum
- cofrinho.infrastructure
  - Persistência (DbContext do EF Core: CofrinhoDbContext)
  - Extensões para DI e Mediator

Tecnologias principais:
- .NET 9 (Minimal APIs)
- System.Text.Json
- Swashbuckle (OpenAPI/Swagger)
- Scalar.AspNetCore (UI de documentação)
- Entity Framework Core (estrutura preparada no DbContext)
- MediatR (suporte a eventos de domínio na infraestrutura)

## Requisitos
- .NET SDK 9.x instalado
- IDE recomendada: JetBrains Rider, Visual Studio 2022 (ou VS Code com C# Dev Kit)

## Como executar localmente
1. Restaurar e compilar a solução:
   - dotnet restore
   - dotnet build
2. Executar a API:
   - dotnet run --project .\cofrinho.api\cofrinho.api.csproj
3. A aplicação, no perfil de desenvolvimento, inicia em:
   - URL base: http://localhost:5082
   - UI de documentação (Scalar): http://localhost:5082/scalar
   - Documento OpenAPI: http://localhost:5082/openapi/v1.json

Observações:
- O projeto usa WebApplication.CreateSlimBuilder. Foi configurado o DefaultJsonTypeInfoResolver em Program.cs para garantir a serialização JSON quando não há Source Generators.
- As configurações de ambiente são controladas por ASPNETCORE_ENVIRONMENT (Development por padrão no launchSettings.json).

## Documentação e endpoints
- UI interativa (Scalar): /scalar
- OpenAPI JSON: /openapi/v1.json

Endpoints atuais (exemplo):
- GET /api/objetivo/
  - Retorna 200 OK com "Hello World" (placeholder para futura listagem de objetivos)

Ao evoluir o projeto, recomenda-se adicionar exemplos e esquemas nas rotas para enriquecer a documentação.

## Arquivos úteis
- cofrinho.api/cofrinho-api.http
  - Arquivo de requisições HTTP (compatível com Rider, VS Code REST Client, etc.)
  - Exemplo incluso: GET {{cofrinho_api_HostAddress}}/api/objetivo/
  - Variável padrão: @cofrinho_api_HostAddress = http://localhost:5082

## Convenções e próximos passos
- Rotas agrupadas por contexto em cofrinho.api/Routes (ex.: ObjetivoRoutes)
- Domínio centralizado em cofrinho.core (entidades imutáveis com setters privados quando aplicável)
- Infrastructure contendo DbContext (CofrinhoDbContext) e integrações (ex.: MediatR, EF Core). A configuração de banco (connection string e migrations) ainda não foi definida neste repositório — adicionar Provider (ex.: SQL Server, PostgreSQL, SQLite) e migrations é um próximo passo natural.
- Application para orquestrar casos de uso, validações e mapeamentos entre domínio e API.

Ideias de evolução:
- CRUD de Objetivos (criar, detalhar, listar, atualizar, excluir)
- Registro de Transações associadas aos Objetivos
- Cálculo de progresso rumo à QuantidadeAlvo e status do objetivo
- Persistência real com EF Core + migrations
- Autenticação/Autorização (quando aplicável)
- Testes automatizados (unitários e de integração)

## Suporte e contribuição
Contribuições são bem-vindas! Sinta-se à vontade para abrir issues e pull requests. Antes de contribuir, considere alinhar convenções de estilo e arquitetura no repositório.

## Licença
Defina aqui a licença desejada (por exemplo, MIT). Caso não exista uma licença específica, todos os direitos permanecem reservados ao(s) autor(es).
