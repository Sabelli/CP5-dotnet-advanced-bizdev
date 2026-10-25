# Biblioteca.API

API REST de gerenciamento de biblioteca (autores, categorias, livros e empréstimos), feita em **.NET 8** com Controllers para o CP5 de *Advanced Business Development with .NET* (FIAP, 2026).

**Vídeo de demonstração (máx. 5 min):** _<colar aqui o link do vídeo>_

## Integrantes

| Nome completo | RM |
|---|---|
| _<preencher>_ | _<preencher>_ |

## O que a API faz

- CRUD de **Autor**, **Categoria** e **Livro** (Livro pertence a um Autor e a uma Categoria).
- **Empréstimo** de livros: emprestar, devolver, consultar. Um livro já emprestado não pode ser emprestado de novo (409) e um empréstimo já devolvido não pode ser devolvido outra vez (409).
- Consulta de dados de um livro por **ISBN** no serviço externo [Open Library](https://openlibrary.org/dev/docs/api/books).
- Cadastro de usuário e login com **JWT**. `leitor` consulta; `admin` também cria, altera e remove.

## Arquitetura (Clean Architecture)

```
Biblioteca.API            -> Application, IoC        (Controllers, Program, Swagger, JWT, rate limiter, compressão, Serilog, health)
Biblioteca.Application    -> Domain                  (UseCases, DTOs, Mappers, HATEOAS, TokenService, hash de senha)
Biblioteca.Domain         (sem dependências)         (Entidades, OperationResult, NoContentException, interfaces)
Biblioteca.Infrastructure -> Domain                  (EF Core + Oracle, Repositories, LivroExternoService com RestSharp)
Biblioteca.IoC            -> Application, Domain, Infrastructure  (Bootstrap.AddIoC)
Biblioteca.Tests          -> todos                   (xUnit + Moq; integração com Oracle real)
```

| Requisito do CP5 | Onde está |
|---|---|
| Arquitetura em camadas | Projetos acima; o Domain não referencia nada e não tem atributos do EF (mapeamento em `ApplicationContext`) |
| Operation Result | `Domain/Entities/OperationResult.cs`; todo UseCase devolve `OperationResult<T>` e o Controller só repassa `StatusCode` + `Value`/`Error` |
| Erros customizados | `Domain/Errors/NoContentException`, **lançada** pelos Repositories quando não há registro e **convertida** pelos UseCases: 404 em consulta única, 204 em lista vazia, 400 em erro inesperado. Regras de negócio retornam 409 (empréstimo) |
| Serviço externo | `Infrastructure/Service/LivroExternoService.cs` (RestSharp → Open Library). Timeout de 5 s e retry manual (3 tentativas, backoff linear) apenas em 5xx/timeout/erro de rede; falha final vira `NoContentException` → 404 |
| PageResult | `PageResponseDto<T>` com `Items`, `Deslocamento`, `RegistroRetornado`, `TotalRegistros` e `Links`. `Deslocamento < 0` vira 0 e `RegistroRetornado <= 0` vira 30 |
| HATEOAS | `LinkDto` (`href`, `rel`, `method`). Cada recurso traz `self`/`update`/`delete` (empréstimo traz `devolver_emprestimo` só enquanto está aberto) e cada página traz `self`, `create_*`, `next` e `prev` (`Application/Mappers`) |
| Autenticação/Autorização | JWT Bearer (HS256, 8 h, claims de nome e role). `[Authorize]` nas leituras, `[Authorize(Roles = "admin")]` em POST/PUT/DELETE de Autor, Categoria e Livro. `api/user` e `api/auth` são anônimos. Senhas com PBKDF2-SHA256 (100 mil iterações, salt aleatório) |
| Otimização | `AsNoTracking` em toda leitura, `Count` + `OrderBy` + `Skip/Take` (paginação no banco), `Include` só em Livro/Empréstimo, índices (abaixo), compressão Brotli/Gzip, rate limiter |
| Testes | `Biblioteca.Tests` (73 testes): unidade com Moq e integração via `WebApplicationFactory` contra o Oracle real |
| Extras | Serilog (console + arquivo), health checks `/health/live` e `/health/db`, Swagger com botão Authorize |

### Banco de dados

Oracle FIAP. O schema é **compartilhado**, então todas as tabelas têm prefixo `tb_bib_` e o histórico de migrations usa a tabela própria `__BibliotecaMigrationsHistory`. A migration `initdb` cria também os índices:

| Índice | Uso |
|---|---|
| `IDX_bib_livro_isbn` (único) | busca e unicidade por ISBN |
| `IDX_bib_livro_titulo_autor` | busca por título/autor |
| `IDX_bib_usuario_email` (único) | login e checagem de e-mail duplicado |
| `IDX_bib_emprestimo_livro_devol` | "este livro está emprestado?" (`LivroId` + `DataDevolucao`) |

## Pacotes

Mesmas versões do projeto do professor.

- **API:** JwtBearer 8.0.31, Swashbuckle.AspNetCore / .Annotations 6.6.2, .Filters 6.1.0, Serilog.AspNetCore 8.0.3, AspNetCore.HealthChecks.Oracle 8.0.1, EF Core 8.0.26
- **Application:** JwtBearer 8.0.31
- **Infrastructure / IoC:** EF Core 8.0.26, Oracle.EntityFrameworkCore 8.23.26200, RestSharp 114.0.0
- **Tests:** xunit 2.5.3, Microsoft.NET.Test.Sdk 17.8.0, coverlet 6.0.0, Moq 4.20.72, Mvc.Testing 8.0.30

## Como rodar

Requisitos: .NET 8 SDK e acesso ao Oracle da FIAP (`oracle.fiap.com.br:1521`).

### Configuração

Tudo fica em `Biblioteca.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": { "Oracle": "Data Source=(...HOST=oracle.fiap.com.br...(SID=ORCL)));User Id=<RM>;Password=<senha>;" },
  "Secretkey": "<chave com pelo menos 32 caracteres para assinar o JWT>"
}
```

Não há variáveis de ambiente nem user-secrets. O serviço externo (Open Library) é público e não precisa de chave.

### Banco

```bash
dotnet tool restore
dotnet ef database update --project Biblioteca.Infrastructure --startup-project Biblioteca.API
```

### API

```bash
dotnet run --project Biblioteca.API
```

Swagger em `http://localhost:5099/swagger` (ambiente Development). Fluxo rápido:

1. `POST /api/user` com `role: "admin"` e depois `POST /api/auth` para obter o `jwt`.
2. Em **Authorize**, informe o token (apenas o JWT, o Swagger acrescenta `Bearer`).
3. Crie Autor e Categoria, depois Livro, depois Empréstimo.

### Testes

```bash
dotnet test
```

Os testes de integração usam o **Oracle real** (sem InMemory/SQLite), sobem a API em Development e substituem somente o `ILivroExternoService` por um mock para não depender da internet. Cada teste apaga as linhas `tb_bib_*` que criou. Dois usuários fixos de teste (`admin.teste@biblioteca.com` e `leitor.teste@biblioteca.com`) são criados na primeira execução e reaproveitados. Os testes são idempotentes.

## Endpoints

Todos exigem `Authorization: Bearer <jwt>`, exceto `api/user`, `api/auth` e `/health/*`.

| Método | Rota | Perfil | Retornos |
|---|---|---|---|
| POST | `api/user` | anônimo | 201, 409 (e-mail já usado), 400 (validação) |
| POST | `api/auth` | anônimo | 200 com `jwt`, 404 (usuário ou senha inválidos) |
| GET | `api/autor`, `api/categoria`, `api/livro`, `api/emprestimo` `?Deslocamento=&RegistroRetornado=` | autenticado | 200 (página), 204 (vazia), 429 |
| GET | `api/{recurso}/{id}` | autenticado | 200, 404 |
| POST / PUT / DELETE | `api/autor`, `api/categoria`, `api/livro` (`/{id}` em PUT/DELETE) | admin | 201 / 200, 404, 400, 403 |
| GET | `api/livro/isbn/{isbn}` | autenticado | 200, 404 (ISBN não encontrado), 400 |
| POST | `api/emprestimo` (`livroId`, `usuarioId`, `diasEmprestimo`) | autenticado | 201, 404, 409 (livro já emprestado) |
| PUT | `api/emprestimo/{id}/devolver` | autenticado | 200, 404, 409 (já devolvido) |
| GET | `/health/live`, `/health/db` | anônimo | 200 |

Resposta de exemplo (`GET api/livro?Deslocamento=0&RegistroRetornado=1`):

```json
{
  "items": [{ "id": 2, "isbn": "9788535902778", "titulo": "Dom Casmurro", "anoPublicacao": 1899,
              "autor": "Machado de Assis", "categoria": "Romance",
              "links": [{ "href": "/api/livro/2", "rel": "self", "method": "GET" },
                        { "href": "/api/livro/2", "rel": "update_livro", "method": "PUT" },
                        { "href": "/api/livro/2", "rel": "delete_livro", "method": "DELETE" }] }],
  "deslocamento": 0, "registroRetornado": 1, "totalRegistros": 1,
  "links": [{ "href": "/api/livro?Deslocamento=0&RegistroRetornado=1", "rel": "self", "method": "GET" },
            { "href": "/api/livro", "rel": "create_livro", "method": "POST" }]
}
```
