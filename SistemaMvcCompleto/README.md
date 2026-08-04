# Sistema MVC Completo

## Table of Contents

<details>

   <summary>Contents</summary>

1. [🚀 Principais Tecnologias e Recursos](#-principais-tecnologias-e-recursos)
1. [📁 Estrutura do Projeto](#-estrutura-do-projeto)
1. [🛠️ Configuração e Instalação](#-configurao-e-instalao)
   1. [Pré-requisitos](#pr-requisitos)
   1. [Passos para Execução](#passos-para-execuo)
1. [🔐 Acesso Inicial (Seed)](#-acesso-inicial-seed)
1. [🖼️ Imagens do Projeto](#-imagens-do-projeto)

</details>

Este é um projeto completo desenvolvido em **ASP.NET Core MVC** utilizando as melhores práticas de mercado para arquitetura, segurança e organização de código.

## 🚀 Principais Tecnologias e Recursos

- **ASP.NET Core MVC**: Estrutura robusta para desenvolvimento web.
- **Entity Framework Core**: ORM para comunicação simplificada com banco de dados SQL Server.
- **ASP.NET Core Identity**: Sistema completo de autenticação e autorização com suporte a perfis de acesso (**Admin** e **Usuario**).
- **Rate Limiting**: Limitação de taxa de requisições integrada no pipeline para prevenir ataques de força bruta (ex: na rota de Login).
- **Injeção de Dependência**: Serviços desacoplados e registrados no contêiner nativo.
- **Exportação/Importação Excel**: Serviço dedicado para manipulação e relatórios em planilhas Excel (`IExcelService`).
- **Filtros Customizados**: Controle de fluxo refinado (ex: `PrimeiroAcessoFilter` para direcionar novos usuários).

---

## 📁 Estrutura do Projeto

Abaixo está uma visão geral da arquitetura de pastas dentro de `SistemaMvcCompleto`:

```text
├── Controllers/       # Controladores da aplicação (Account, Admin, Produto, Usuario, etc.)
├── Data/              # Contexto do Banco de Dados (DbContext) e Inicializador (Seed Data)
├── Filter/            # Filtros de Ação e Autorização customizados
├── Migrations/        # Histórico de alterações do Banco de Dados pelo EF Core
├── Models/            # Entidades principais do domínio (ex: User, Produto)
├── Services/          # Camada de negócios e serviços (Excel, Token, Produtos, etc.)
├── TDO/ (DTOs)        # Objetos de Transferência de Dados para validação e segurança
├── Views/             # Páginas Razor (HTML/CSS Dinâmico)
└── wwwroot/           # Arquivos estáticos (JavaScript, CSS, Imagens)
```

---

## 🛠️ Configuração e Instalação

### Pré-requisitos
- [.NET SDK](https://dotnet.microsoft.com/download) (versão compatível com o projeto)
- [SQL Server](https://www.microsoft.com/sql-server/) instalado e rodando localmente (ou instância remota)

### Passos para Execução

1. **Clonar/Abrir o repositório:**
   Abra a pasta do projeto no VS Code, Visual Studio ou no seu terminal de preferência.

2. **Configurar a string de conexão:**
   Abra o arquivo [appsettings.json](file:///C:/Users/ADM/OneDrive/Desktop/github/ProjetosGrandes/SistemaMvcCompleto/SistemaMvcCompleto/appsettings.json) (ou `appsettings.Development.json`) e ajuste a string de conexão com os dados do seu banco de dados SQL Server:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=SEU_SERVIDOR;Database=SistemaMvcCompleto;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
   }
   ```

3. **Restaurar Dependências:**
   ```bash
   dotnet restore
   ```

4. **Executar Migrations (Criar o Banco de Dados):**
   ```bash
   dotnet ef database update
   ```

5. **Iniciar a Aplicação:**
   ```bash
   dotnet run --project SistemaMvcCompleto
   ```
   A aplicação estará disponível em `https://localhost:5001` ou porta indicada pelo console.

---

## 🔐 Acesso Inicial (Seed)

O projeto possui um inicializador automático (`DbInitializer`) que popula os perfis necessários e cria o primeiro usuário administrador no primeiro carregamento do sistema.

**Credenciais do Administrador Padrão:**
- **Usuário:** `admin@sistema.com`
- **Senha:** `Admin@1234`

## 🖼️ Imagens do Projeto

``` html

Pagina de login
```
<img src="./SistemaMvcCompleto/img/Captura de tela 2026-08-03 220924.png" />

``` html
Pagina inicial

```
<img src="./SistemaMvcCompleto/img/Captura de tela 2026-08-03 221115.png" />

``` html

Menu
```
<img src="./SistemaMvcCompleto/img/Captura de tela 2026-08-03 221245.png" />