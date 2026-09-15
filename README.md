\# BackOffice



Sistema de back-office para gerenciamento de solicitações, usuários e fluxos de atendimento, desenvolvido em C#/.NET com arquitetura em camadas, persistência em SQL Server e testes automatizados.



\## Funcionalidades atuais



\- Criação de solicitações

\- Listagem de solicitações

\- Gerenciamento de usuários

\- Perfis de usuário

\- Persistência em banco de dados

\- Migrations com Entity Framework Core

\- Inicialização de dados com seeding

\- API REST

\- Testes unitários

\- Testes de aplicação

\- Testes de integração

\- Estrutura inicial do fluxo de autenticação



\## Arquitetura



O projeto está organizado em camadas com responsabilidades separadas:



```text

BackOffice.Api

&#x20;       ↓

BackOffice.Application

&#x20;       ↓

BackOffice.Domain



BackOffice.Infrastructure

&#x20;       ↓

SQL Server

