# 🏆 DevBol - Sistema Administrativo de Bolão

O **DevBol** é uma plataforma administrativa robusta para gestão de apostas e competições. Desenvolvido com **.NET 8/9**, o foco do projeto é demonstrar práticas avançadas de arquitetura, processamento assíncrono e segurança.

## 🏗️ Arquitetura do Sistema
O projeto utiliza **Clean Architecture** e **DDD**, dividindo as responsabilidades de forma clara:

* **DevBol.Domain.Models**: Entidades puras e modelos de domínio totalmente isolados.
* **DevBol.Domain**: Interfaces, contratos, notificações e validações de negócio.
* **DevBol.Application**: Orquestração com **Unit of Work** e serviços de aplicação.
* **DevBol.Infrastructure**: Persistência com **EF Core** e **PostgreSQL**.
* **DevBol.Presentation (MVC)**: Interface administrativa rica, contendo o motor de processamento em segundo plano.

## ⚙️ Diferencial: Background Processing (Worker)
Implementamos um **Hosted Service** (`RankingWorker.cs`) na camada de apresentação que desacopla o processamento pesado da interface do usuário. Isso garante que os rankings sejam atualizados em real-time sem onerar a performance das requisições HTTP.

## 🛡️ Segurança de Ambiente
Configuração rigorosa de **Secrets Management**. Arquivos de configuração reais (`appsettings.json`) são protegidos via `.gitignore`, utilizando modelos `.example.json` para facilitar o setup por novos desenvolvedores sem expor credenciais.

## 🚀 Próximos Passos
- [ ] Implementação de **ASP.NET Identity** para controle de acesso refinado.
- [ ] Desenvolvimento do **Front-end em Angular** para experiência de usuário moderna.
- [ ] Configuração de **CI/CD** via GitHub Actions para deploy automatizado no Azure.