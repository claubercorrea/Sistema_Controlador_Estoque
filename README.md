# Sistema de Gestão de Dados Operacionais

## 📋 Descrição
Este projeto é uma solução full-stack desenvolvida para automatizar e otimizar a correção de dados críticos no setor operacional. O sistema foi projetado para eliminar processos manuais lentos, garantindo a integridade, a segurança e a rastreabilidade total das informações processadas.

Como desenvolvedor, foquei em uma arquitetura robusta, escalável e segura para o ambiente de produção, separando rigidamente as responsabilidades entre o front-end e o back-end.

---

## 🛠️ Tecnologias Utilizadas

### **Back-end**
*   **Linguagem & Framework:** C# / .NET 8
*   **Arquitetura:** ASP.NET Core MVC (Service-Oriented Architecture)
*   **Banco de Dados:** SQL Server com Entity Framework Core (EF Core)
*   **Segurança:** JWT (JSON Web Tokens) para autenticação stateless

### **Front-end**
*   **Estrutura Visual:** Bootstrap (Interface responsiva e profissional)
*   **Dinâmica & Comportamento:** JavaScript (Manipulação de DOM, requisições assíncronas e controle de estados da interface)

---

## 🔒 Funcionalidades de Segurança e Controle
O sistema implementa uma camada de segurança rigorosa para proteger os dados operacionais:

*   **Autenticação e Autorização:** Controle de acesso baseado em *roles* (Admin vs. Usuário Comum).
*   **Segurança por Design:** Uso de DTOs (Data Transfer Objects) para evitar *over-posting* e proteger a estrutura interna das tabelas do banco de dados.
*   **Fluxo de Primeiro Acesso:** Obrigatoriedade de alteração de senha no primeiro login para garantir a integridade da conta.
*   **Gestão de Suporte:** Funcionalidade administrativa para reset de senhas, centralizando a governança dos acessos.
*   **Acesso de Emergência (Break-Glass):** Implementação de uma conta administrativa oculta e de contingência, com isolamento total de privilégios para manutenções críticas de urgência.

---

## 📐 Diferenciais Arquiteturais

*   **Isolamento de Regras de Negócio:** Toda a lógica reside na camada de *Services*, mantendo as *Controllers* finas e focadas estritamente no fluxo de requisições.
*   **Interface Contextual Adaptativa:** Utilizando JavaScript integrado às *claims* do JWT, a interface do usuário oculta ou exibe elementos (como botões e menus críticos) de acordo com o nível de permissão do usuário logado.
*   **Evolução do Banco de Dados:** Controle total, versionamento e automação do esquema do banco de dados via *Migrations* do EF Core.

---

## 🚀 Roadmap de Evolução
*   [ ] **Deploy em Produção:** Hospedagem da aplicação e do banco de dados em servidor dedicado para uso real da equipe.
*   [ ] **Trilha de Auditoria:** Criação de logs internos e assíncronos para registrar ações críticas (especialmente do usuário de emergência).
*   [ ] **Guia do Usuário:** Documentação simples de uso para os colegas do setor facilitarem a adoção da ferramenta.