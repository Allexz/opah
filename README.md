# AccountingOffice

Sistema de gestão contábil para escritórios contábeis - desenvolvido como prova prática para a Opah.  
**Requisitos não funcionais**  
Um comerciante precisa controlar o seu fluxo de caixa diário com os lançamentos(débitos e créditos), também precisa de um relatório que disponibilize o saldo diário consolidado.  
A implementação do relatório está em conclusão, assim como uma maior cobertura de testes. O trabalho continua sendo executado.


## 🚀 Tecnologias Utilizadas

### Backend
- **C# 10.0** - Linguagem de programação principal
- **.NET 10** - Framework de desenvolvimento
- **Entity Framework Core 10** - ORM para acesso a dados
- **RabbitMQ** - Mensageria e comunicação assíncrona
- **SQL Server** - Banco de dados relacional
- **Serilog** - Logging avançado
- **Elasticsearch** - Indexação e busca de logs

### Arquitetura
- **Domain-Driven Design (DDD)**
- **Clean Architecture**
- **CQRS** - Command Query Responsibility Segregation
- **Dependency Injection**
- **SOLID Principles**

### Testes
- **xUnit** - Framework de testes unitários
- **FluentAssertions** - Biblioteca de assertions

### DevOps
- **Docker** - Containerização
- **Docker Compose** - Orquestração de containers
- **GitHub Actions** - CI/CD pipelines

## 📁 Estrutura do Projeto

```
├── AccountingOffice.ApiService      # API REST
├── AccountingOffice.Application     # Camada de aplicação (casos de uso)
├── AccountingOffice.Domain          # Modelo de domínio
├── AccountingOffice.Infrastructure  # Implementações concretas
└── AccountOffice.Tests             # Testes unitários
```

## 🏗️ Funcionalidades Principais

- Gestão de contas a pagar e receber
- Controle de parcelas e pagamentos
- Cadastro de pessoas físicas e jurídicas
- Multi-tenant (múltiplos escritórios)
- Event sourcing com RabbitMQ
- Logging centralizado com Serilog/Elasticsearch

## 🐳 Docker

```bash
docker-compose up -d
```

## 📦 Variáveis de Ambiente

- `ConnectionStrings:DefaultConnection` - String de conexão com o banco
- `RabbitMQ:Host` - Host do RabbitMQ
- `ElasticConfiguration:Uri` - URI do Elasticsearch
- `Serilog:MinimumLevel:Default` - Nível mínimo de log
