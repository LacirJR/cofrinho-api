# Cofrinho API

API minimalista em .NET 9 para simular um cofrinho (poupança) com base em Objetivos e Transações. O projeto segue uma arquitetura em camadas (API, Application, Core e Infrastructure) e inclui documentação via OpenAPI e UI interativa (Scalar).

Última atualização deste README: 2025-08-24 16:51

## Sumário
- Visão geral
- Arquitetura do projeto
- Requisitos
- Como executar localmente
- Documentação e endpoints
- Exemplo de requisições (HTTP file)
- Notas sobre mapeamento e padrões
- Próximos passos
- Suporte e contribuição
- Licença

---

## Visão geral
A API permite gerenciar Objetivos financeiros, contendo título, descrição, valor alvo (com moeda), prazo, status e categoria. Também há suporte para Transações relacionadas (estrutura no domínio). As rotas de Objetivo já estão implementadas com MediatR e mapeamento para ViewModels usando Mapster.

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
  - Persistência e Repositórios (ex.: TransacaoRepository)
  - Extensões de DI e integrações

Tecnologias principais:
- .NET 9 (Minimal APIs)
- MediatR (CQRS básico para Commands/Queries)
- Mapster (mapeamento de entidades para ViewModels)
- OpenAPI/Swagger + Scalar UI
- Flunt (validações de domínio)
- Entity Framework Core (estrutura preparada)

## Requisitos
- .NET SDK 9.x instalado
- IDE recomendada: JetBrains Rider, Visual Studio 2022 ou VS Code (C# Dev Kit)

## Como executar localmente
1) Restaurar e compilar a solução:
   - dotnet restore
   - dotnet build
2) Executar a API:
   - dotnet run --project .\cofrinho.api\cofrinho.api.csproj
3) Endereços padrão (Development):
   - URL base: http://localhost:5082
   - UI de documentação (Scalar): http://localhost:5082/scalar
   - Documento OpenAPI: http://localhost:5082/openapi/v1.json

Observações:
- O projeto utiliza Minimal API e está configurado para serialização System.Text.Json.
- ASPNETCORE_ENVIRONMENT controla o ambiente (Development por padrão no launchSettings.json).

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


## Notas sobre mapeamento e padrões
- Mapster está configurado nos ViewModels da camada de aplicação.
  - Objetivo -> ObjetivoViewModel: conversões para strings, descrição de enums via EnumExtensions.GetDescription, Url para string, e Dinheiro para DinheiroViewModel.
  - Há também um ListarObjetivosViewModel com configuração similar para cenários de listagem.
- Padrão CQRS simples com MediatR: Commands para escrita (Criar, Editar, Remover) e Queries para leitura (Listar).
- Paginacão: PagedResult<T> encapsula itens, total e metadados de página.

