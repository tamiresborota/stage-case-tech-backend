# Stage Consulting - Sistema de Mapeamento de Processos (Backend)

Backend da aplicação de mapeamento de processos empresariais, desenvolvido com C# e .NET 9.0 (usando Entity Framework Core como ORM).

## Tecnologias

- .NET 9.0
- Entity Framework Core
- SQLite
- Swagger

## Requisitos

- .NET 9.0 SDK

## Instalação

1. Clone o repositório
```bash
git clone https://github.com/seu-usuario/stage-case-tech-backend.git
cd stage-case-tech-backend
```

2. Restaure as dependências
```bash
dotnet restore
```

3. Execute as migrações (opcional: Banco de Dados SQLite incluso e com dados iniciais)
```bash
dotnet ef database update
```

4. Execute o projeto
```bash
dotnet run
```

A API estará disponível em http://localhost:5241.

---

## Estrutura do Projeto

Areas/: Modelos e rotas relacionadas às áreas da empresa

Processos/: Modelos e rotas relacionadas aos processos de negócio

DetalheProcesso/: Entidades relacionadas aos detalhes de processos

Data/: Contexto do banco de dados e configurações do Entity Framework


## Endpoints principais

Áreas: /areas

Processos: /processos

Processos por área: /areas/{areaId}/processos

Detalhes de processo: /processos/{processoId}/detalhes

Hierarquia de processos: /processos/{id}/hierarquia


## Licença

Este projeto foi desenvolvido como parte de um case técnico para Stage Consulting.
