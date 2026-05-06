# TaskFlow.API - Gerenciador de Tarefas com .NET 8

![.NET Version](https://img.shields.io/badge/.NET-8.0-blue)
![License](https://img.shields.io/badge/license-MIT-green)
![JWT](https://img.shields.io/badge/auth-JWT-orange)

## 📌 Sobre o Projeto

API RESTful para gerenciamento de tarefas com autenticação JWT, paginação, soft delete e arquitetura limpa. Desenvolvida com boas práticas para demonstrar domínio em desenvolvimento backend moderno.

### 🎯 Problema que resolve
- Gestão ineficiente de tarefas sem controle de usuário
- Ausência de autenticação segura
- Falta de filtros e paginação em listagens

### 🛠️ Tecnologias

| Tecnologia | Versão | Finalidade |
|------------|--------|------------|
| .NET | 8.0 | Framework principal |
| Entity Framework | 8.0 | ORM e persistência |
| SQLite | - | Banco de dados local |
| JWT Bearer | 8.0 | Autenticação segura |
| AutoMapper | 12.0 | Mapeamento DTO/Entity |
| Swagger | 6.5 | Documentação interativa |

## 🚀 Funcionalidades

- ✅ Registro e autenticação de usuários com JWT
- ✅ CRUD completo de tarefas
- ✅ Soft Delete com restauração
- ✅ Paginação e ordenação em listagens
- ✅ Filtros por status, prioridade e data
- ✅ Documentação Swagger com suporte a Bearer Token
- ✅ Arquitetura Clean Code (Domain, Application, Infrastructure, API)

## 📊 Resultados Obtidos

- API totalmente funcional com suporte a múltiplos usuários
- Documentação automática via Swagger/OpenAPI
- Código organizado e preparado para escalabilidade
- Redução de complexidade em consultas com paginação

## 🔧 Como Executar

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQLite](https://www.sqlite.org/download.html) (opcional)

### Passos

```bash
# Clone o repositório
git clone https://github.com/seu-usuario/TaskFlow.API.git
cd TaskFlow.API

# Restaure os pacotes
dotnet restore

# Execute as migrações
cd src/TaskFlow.API
dotnet ef database update

# Inicie a aplicação
dotnet run --urls "http://localhost:5000"
