# Biblioteca.API — .NET

API RESTful desenvolvida em **ASP.NET Core (.NET 8)** para o gerenciamento de uma biblioteca, como Checkpoint 5 (CP5) da disciplina **Advanced Business Development with .NET**.

A API cuida do cadastro e manutenção de **autores, categorias, livros, usuários e empréstimos**, com consulta de dados de livro por **ISBN** em um serviço externo (Open Library).

O projeto é organizado em **Clean Architecture** (Domain / Application / Infrastructure / API, mais IoC e Tests), com **Operation Result**, **erros customizados**, **paginação**, **HATEOAS**, **autenticação JWT com perfis**, **índices de banco**, **rate limiting**, **compressão de resposta** (Brotli / Gzip), **observabilidade** (Serilog e health checks) e uma suíte de **testes** (xUnit) de unidade e de integração contra o Oracle real.

**Vídeo de demonstração (máx. 5 min):** _<colar aqui o link do vídeo>_

---

## Integrantes

| Nome | RM |
|------|----|
| Victor Sabelli | RM566224 |
| Gustavo Crevelari | RM561408 |
| Lucca Gomes | RM561996 |
| Rafaela Ferreira | RM561671 |

---

## Repositório

[![GitHub](https://img.shields.io/badge/GitHub-Acessar%20Repositório-181717?style=for-the-badge&logo=github&logoColor=white)](https://github.com/Sabelli/CP5-dotnet-advanced-bizdev.git)

---

## Estrutura do Projeto

```
CP5-dotnet-advanced-bizdev/
├── Biblioteca.slnx                       # solução
├── Biblioteca.API/                       # Controllers, Program, Swagger, JWT, rate limiter, compressão, Serilog, health
│   ├── Controllers/                      # Autor, Categoria, Livro, Emprestimo, User, Auth
│   └── Config/                           # configuração do Serilog
├── Biblioteca.Application/               # regras de negócio
│   ├── Dtos/                             # Post*/Get*Dto, PageResponseDto<T>, LinkDto
│   ├── Interfaces/                       # contratos dos use cases
│   ├── Mappers/                          # Entity <-> DTO e links HATEOAS (PageLinks)
│   ├── Services/                         # TokenService (JWT), SenhaHasher (PBKDF2)
│   └── UseCases/                         # AutorUseCase, CategoriaUseCase, LivroUseCase, EmprestimoUseCase, UserUseCase
├── Biblioteca.Domain/                    # sem dependências de framework
│   ├── Entities/                         # *Entity, OperationResult<T>, EdicaoLivroModel
│   ├── Errors/                           # NoContentException, ConflitoException, RequisicaoInvalidaException, NaoAutorizadoException, ServicoExternoException
│   └── Interfaces/                       # repositórios, ILivroExternoService, ITokenService
├── Biblioteca.Infrastructure/            # EF Core + Oracle
│   ├── Data/                             # ApplicationContext, Migrations, Repositories
│   └── Service/                          # LivroExternoService (RestSharp -> Open Library)
├── Biblioteca.IoC/                       # Bootstrap.AddIoC (injeção de dependência)
└── Biblioteca.Tests/                     # xUnit
    ├── Unit/                             # use cases, OperationResult, mappers, token, hash (Moq)
    └── Integration/                      # WebApplicationFactory + Oracle real
```

Dependências entre projetos: `API → Application, IoC` · `Application → Domain` · `Infrastructure → Domain` · `IoC → Application, Domain, Infrastructure` · `Tests → todos`.

---

## Modelagem

- `Livro` **N:1** `Autor` — um autor tem vários livros.
- `Livro` **N:1** `Categoria` — uma categoria tem vários livros.
- `Emprestimo` **N:1** `Livro` e **N:1** `Usuario` — um livro pode ter vários empréstimos ao longo do tempo, mas só um aberto por vez.

Banco Oracle FIAP, schema **compartilhado**: todas as tabelas têm prefixo `tb_bib_` e o histórico de migrations usa a tabela própria `__BibliotecaMigrationsHistory`. A migration `initdb` cria também os índices:

| Índice | Uso |
|--------|-----|
| `IDX_bib_livro_isbn` (único) | busca e unicidade por ISBN |
| `IDX_bib_livro_titulo_autor` | busca por título/autor |
| `IDX_bib_usuario_email` (único) | login e checagem de e-mail duplicado |
| `IDX_bib_emprestimo_livro_devol` | "este livro está emprestado?" (`LivroId` + `DataDevolucao`) |

---

## Tecnologias

- .NET 8 / ASP.NET Core Web API (Controllers)
- Entity Framework Core 8.0.26 + Oracle.EntityFrameworkCore 8.23.26200
- Oracle Database (FIAP)
- JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer` 8.0.31)
- RestSharp 114.0.0 (serviço externo)
- Microsoft.AspNetCore.RateLimiting — fixed window limiter
- Microsoft.AspNetCore.ResponseCompression — Brotli + Gzip
- Swashbuckle.AspNetCore 6.6.2 + `.Annotations` e `.Filters` (Swagger/OpenAPI com botão Authorize)
- **Observabilidade:** Serilog.AspNetCore 8.0.3 e `AspNetCore.HealthChecks.Oracle` 8.0.1
- **Testes:** xUnit 2.5.3, Moq 4.20.72, Microsoft.AspNetCore.Mvc.Testing 8.0.30

---

## Instalação e Execução

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Acesso ao banco Oracle da FIAP (`oracle.fiap.com.br:1521`)

### 1. Clone o repositório

```bash
git clone https://github.com/Sabelli/CP5-dotnet-advanced-bizdev.git
cd CP5-dotnet-advanced-bizdev
```

### 2. Configure a string de conexão e a chave do JWT

Em `Biblioteca.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "Oracle": "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=oracle.fiap.com.br)(PORT=1521))) (CONNECT_DATA=(SERVER=DEDICATED)(SID=ORCL)));User Id=SEU_RM;Password=SUA_SENHA;"
  },
  "Secretkey": "<chave com pelo menos 32 caracteres para assinar o JWT>"
}
```

Não há variáveis de ambiente nem user-secrets. O serviço externo (Open Library) é público e não precisa de chave.

### 3. Migrations

Migration em `Biblioteca.Infrastructure/Data/Migrations/`:

- `initdb` — schema inicial (Autor, Categoria, Livro, Usuario, Emprestimo) e índices

Aplicar no banco (Package Manager Console, projeto padrão `Biblioteca.Infrastructure`, projeto de inicialização `Biblioteca.API`):

```powershell
Update-Database
```

Ou pela CLI, com o `dotnet-ef` instalado globalmente:

```bash
dotnet tool install --global dotnet-ef --version 8.0.26
dotnet ef database update --project Biblioteca.Infrastructure --startup-project Biblioteca.API
```

### 4. Execute a API

```bash
dotnet run --project Biblioteca.API
```

A API sobe em `http://localhost:5099` (ou `https://localhost:7046` no perfil `https`; ver `Biblioteca.API/Properties/launchSettings.json`).

### 5. Acesse o Swagger

```
http://localhost:5099/swagger
```

Fluxo rápido:

1. `POST /api/user` com `role: "admin"` e depois `POST /api/auth` para obter o `jwt`.
2. Em **Authorize**, informe só o token (o Swagger acrescenta `Bearer`).
3. Crie Autor e Categoria, depois Livro, depois Empréstimo.

---

## Testes

Testes automatizados com **xUnit** no projeto `Biblioteca.Tests` — **73 testes**, cobrindo:

- **Unidade** — use cases com repositórios mockados via **Moq**, `OperationResult`, mappers e links HATEOAS, `TokenService` e `SenhaHasher`.
- **Integração** — `WebApplicationFactory` (`Microsoft.AspNetCore.Mvc.Testing`) subindo a API em Development contra o **Oracle real** (sem InMemory/SQLite). Só o `ILivroExternoService` é substituído por mock, para não depender da internet. Cada teste apaga apenas as linhas `tb_bib_*` que criou, e os dois usuários fixos de teste (`admin.teste@biblioteca.com` e `leitor.teste@biblioteca.com`) são reaproveitados. Os testes são idempotentes.

### Executar todos os testes

```bash
dotnet test
```

### Filtrar por trait

Os testes de integração usam o trait `Integracao`, categorizado por assunto:

```bash
dotnet test --filter "Integracao=Auth"
dotnet test --filter "Integracao=Erros"
dotnet test --filter "Integracao=Paginacao"
dotnet test --filter "Integracao=Hateoas"
dotnet test --filter "Integracao=ServicoExterno"
```

---

## Comportamentos Transversais

### Paginação

Todos os endpoints de listagem aceitam `Deslocamento` e `RegistroRetornado`:

| Param | Default | Regras |
|-------|---------|--------|
| `Deslocamento` | `0` | valores negativos são tratados como `0` |
| `RegistroRetornado` | `30` | aplicado quando vem `0` ou negativo |

Exemplo: `GET /api/livro?Deslocamento=0&RegistroRetornado=10`

A resposta (`200 OK`) vem em um envelope com metadados de paginação e links HATEOAS (`PageResponseDto<T>`), ou `204 No Content` quando vazio. Ordenação por `Id` ascendente.

```json
{
  "items": [
    {
      "id": 2, "isbn": "9788535902778", "titulo": "Dom Casmurro", "anoPublicacao": 1899,
      "autor": "Machado de Assis", "categoria": "Romance",
      "links": [
        { "href": "/api/livro/2", "rel": "self", "method": "GET" },
        { "href": "/api/livro/2", "rel": "update_livro", "method": "PUT" },
        { "href": "/api/livro/2", "rel": "delete_livro", "method": "DELETE" }
      ]
    }
  ],
  "deslocamento": 0,
  "registroRetornado": 1,
  "totalRegistros": 1,
  "links": [
    { "href": "/api/livro?Deslocamento=0&RegistroRetornado=1", "rel": "self", "method": "GET" },
    { "href": "/api/livro", "rel": "create_livro", "method": "POST" }
  ]
}
```

### HATEOAS

- Cada recurso traz `self`, `update_*` e `delete_*`.
- `Emprestimo` traz `devolver_emprestimo` somente enquanto está aberto.
- Cada página traz `self`, `create_*`, `next` e `prev` (os dois últimos quando existem).

### Rate Limiting

Fixed window (`politica_listagem`) aplicada em todo GET de listagem (Autor, Categoria, Livro, Empréstimo): **30 requisições a cada 10 segundos**, sem fila. Ao exceder: **429 Too Many Requests**.

### Compressão de Resposta

Brotli e Gzip habilitados (nível `Fastest`), negociados via header `Accept-Encoding`.

```bash
curl -H "Accept-Encoding: gzip" -i http://localhost:5099/api/autor -H "Authorization: Bearer <jwt>"
```

A resposta deve trazer `Content-Encoding: gzip` (ou `br`, se negociado Brotli).

### Autenticação e Autorização

| Situação | Resultado |
|----------|-----------|
| Sem token ou token inválido | 401 |
| `leitor` tentando POST/PUT/DELETE de Autor, Categoria ou Livro | 403 |
| `leitor` consultando | 200 / 204 |
| `admin` | acesso total |
| Empréstimos | qualquer usuário autenticado |
| `api/user`, `api/auth`, `api/health`, `/health/*` | anônimos |

Senhas armazenadas com PBKDF2-SHA256 (100 mil iterações, salt aleatório). Login com e-mail ou senha incorretos retorna 401 ("E-mail ou senha incorretos.").

---

## Observabilidade

### Logging (Serilog)

Console + arquivo (`logs/api-<data>.log`, rotação diária, retenção de 7 arquivos). Nível mínimo `Information`; `Microsoft.AspNetCore` e `Microsoft.EntityFrameworkCore` em `Warning`.

### Health Check

| Endpoint | Tipo | 200 | 503 |
|----------|------|-----|-----|
| `GET /health/live` | liveness | processo saudável | processo travado |
| `GET /health/db` | readiness | banco Oracle acessível | banco indisponível |
| `GET /api/health` | relatório completo (Swagger) | tudo saudável | alguma verificação falhou |

`GET /api/health` (`HealthController`) devolve o status geral e, por verificação (`self`, `oracle`), status, duração e erro.

---

## Rotas

Todas exigem `Authorization: Bearer <jwt>`, exceto `api/user`, `api/auth`, `api/health` e `/health/*`.

### Usuários — `/api/user`

| Método | Rota | Descrição | Retorno |
|--------|------|-----------|---------|
| POST | `/api/user` | Cadastra usuário | 201 / 409 (e-mail já usado) / 400 |

```json
{ "nome": "Maria Silva", "email": "maria@biblioteca.com", "password": "Senha123", "confirmPassword": "Senha123", "role": "admin" }
```

> `role` aceita `admin` ou `leitor`.

### Autenticação — `/api/auth`

| Método | Rota | Descrição | Retorno |
|--------|------|-----------|---------|
| POST | `/api/auth` | Login, devolve `jwt` | 200 / 401 (e-mail ou senha incorretos) / 400 |

```json
{ "email": "maria@biblioteca.com", "senha": "Senha123" }
```

---

### Autores — `/api/autor`

| Método | Rota | Descrição | Perfil | Retorno |
|--------|------|-----------|--------|---------|
| GET | `/api/autor` | Lista autores (paginado) | autenticado | 200 / 204 / 429 |
| GET | `/api/autor/{id}` | Busca autor por id | autenticado | 200 / 404 |
| POST | `/api/autor` | Cria autor | admin | 201 / 400 / 403 |
| PUT | `/api/autor/{id}` | Atualiza autor | admin | 200 / 404 / 400 / 403 |
| DELETE | `/api/autor/{id}` | Remove autor | admin | 200 / 404 / 403 |

**POST / PUT — Body:**
```json
{ "nome": "Machado de Assis", "nacionalidade": "Brasileira" }
```

---

### Categorias — `/api/categoria`

| Método | Rota | Descrição | Perfil | Retorno |
|--------|------|-----------|--------|---------|
| GET | `/api/categoria` | Lista categorias (paginado) | autenticado | 200 / 204 / 429 |
| GET | `/api/categoria/{id}` | Busca categoria por id | autenticado | 200 / 404 |
| POST | `/api/categoria` | Cria categoria | admin | 201 / 400 / 403 |
| PUT | `/api/categoria/{id}` | Atualiza categoria | admin | 200 / 404 / 400 / 403 |
| DELETE | `/api/categoria/{id}` | Remove categoria | admin | 200 / 404 / 403 |

**POST / PUT — Body:**
```json
{ "nome": "Romance" }
```

---

### Livros — `/api/livro`

| Método | Rota | Descrição | Perfil | Retorno |
|--------|------|-----------|--------|---------|
| GET | `/api/livro` | Lista livros (paginado) | autenticado | 200 / 204 / 429 |
| GET | `/api/livro/{id}` | Busca livro por id | autenticado | 200 / 404 |
| GET | `/api/livro/isbn/{isbn}` | Consulta o livro no serviço externo (Open Library) | autenticado | 200 / 404 (ISBN não encontrado) / 400 / 502 (serviço externo indisponível) |
| POST | `/api/livro` | Cria livro | admin | 201 / 404 (autor, categoria ou ISBN inexistente) / 409 (ISBN já cadastrado) / 400 / 403 / 502 (serviço externo indisponível) |
| PUT | `/api/livro/{id}` | Atualiza livro | admin | 200 / 404 / 409 (ISBN já cadastrado em outro livro) / 400 / 403 / 502 (serviço externo indisponível) |
| DELETE | `/api/livro/{id}` | Remove livro | admin | 200 / 404 / 403 |

**POST / PUT — Body:**
```json
{
  "isbn": "9788535902778",
  "titulo": "Dom Casmurro",
  "anoPublicacao": 1899,
  "autorId": 1,
  "categoriaId": 1
}
```

> `titulo` e `anoPublicacao` são opcionais: se omitidos, são preenchidos com os dados da Open Library. O que o cliente enviar prevalece.

---

### Empréstimos — `/api/emprestimo`

| Método | Rota | Descrição | Retorno |
|--------|------|-----------|---------|
| GET | `/api/emprestimo` | Lista empréstimos (paginado) | 200 / 204 / 429 |
| GET | `/api/emprestimo/{id}` | Busca empréstimo por id | 200 / 404 |
| POST | `/api/emprestimo` | Empresta um livro | 201 / 404 / 409 (livro já emprestado) / 400 |
| PUT | `/api/emprestimo/{id}/devolver` | Devolve o livro | 200 / 404 / 409 (já devolvido) |

**POST — Body:**
```json
{ "livroId": 1, "usuarioId": 1, "diasEmprestimo": 14 }
```

> `diasEmprestimo` vai de 1 a 90 (padrão 14).

---

### Health — `/health`

| Método | Rota | Descrição | Retorno |
|--------|------|-----------|---------|
| GET | `/health/live` | Liveness | 200 / 503 |
| GET | `/health/db` | Readiness — banco Oracle | 200 / 503 |
| GET | `/api/health` | Relatório de todas as verificações | 200 / 503 |

---

## Evidências

A demonstração completa (Swagger com CRUD, `dotnet test` verde, erros 404/204/409/400, consulta por ISBN, paginação, links HATEOAS e login 200/401/403) está no vídeo, linkado no topo deste README.

---

## Observações

- Livro já emprestado não pode ser emprestado de novo (409) e empréstimo já devolvido não pode ser devolvido outra vez (409).
- `POST` e `PUT /api/livro` validam a existência de Autor e Categoria (404), recusam ISBN já cadastrado (409, checado antes da consulta externa) e consultam o ISBN na Open Library: ISBN inexistente retorna 404 e serviço externo fora retorna 502, sem salvar.
- E-mail de usuário é único (índice no banco e checagem na aplicação), com 409 em duplicata.
- Todas as respostas usam DTOs dedicados (`Get*Dto`), nunca a entidade EF crua.
- Repositories usam `AsNoTracking` e lançam `NoContentException`, que os UseCases convertem em `OperationResult`.
- Regras de negócio lançam exceptions próprias com mensagem específica, que os UseCases convertem em `OperationResult` com o status correspondente: `ConflitoException` (409, ex.: "E-mail já cadastrado."), `RequisicaoInvalidaException` (400, ex.: "E-mail inválido."), `NaoAutorizadoException` (401) e `ServicoExternoException` (502).
