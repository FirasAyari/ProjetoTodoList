# Todo List App - API (.NET Core) + Frontend (Angular)

Este é um sistema simples de gerenciamento de tarefas (to-do list) com autenticação de usuário.

## Pré-requisitos

Antes de começar, garanta que você tenha as seguintes ferramentas instaladas:

* **SDK do .NET:** Versão 8.0 ou superior ([Download .NET](https://dotnet.microsoft.com/download))
* **SQL Server:** Qualquer edição, como Express, Developer ou LocalDB (geralmente instalado com o Visual Studio).
* **Node.js e npm:** Versão LTS recomendada ([Download Node.js](https://nodejs.org/)) (npm vem junto com o Node.js).
* **Angular CLI:** Instalado globalmente (`npm install -g @angular/cli`). Verifique com `ng version`.
* **Git:** Para clonar o repositório ([Download Git](https://git-scm.com/downloads)).
* **IDE/Editor de Código:**
    * Visual Studio 2022 ou superior (Recomendado para o backend .NET) OU Visual Studio Code.
    * Visual Studio Code (Recomendado para o frontend Angular).

## Configuração do Backend (.NET Core API)

1.  **Clonar o Repositório:**
   

2.  **Abrir a Solução:**
    * Abra o arquivo da solução (`.sln`) no Visual Studio 2022 (ou superior) OU abra a pasta da solução no VS Code.

3.  **Configurar a Connection String do Banco de Dados:**
    * Abra o arquivo `TodoList.Api/appsettings.json`.
    * Localize a seção `ConnectionStrings`.
    * Verifique/Atualize o valor de `DefaultConnection` para apontar para a sua instância local do SQL Server. Exemplos comuns para o `Server`:
        * `(localdb)\mssqllocaldb` (SQL Server LocalDB padrão do VS)
        * `.` (Instância padrão local)
        * `localhost` (Instância padrão local)
        * `.\SQLEXPRESS` (Instância Express nomeada)
        * `SEU_SERVIDOR\SUA_INSTANCIA`
    * **Importante:** Certifique-se que `Trusted_Connection=True;` esteja presente se você usa Autenticação do Windows no SQL Server. Se usar login/senha do SQL, ajuste a string de conexão apropriadamente (remova `Trusted_Connection` e adicione `User ID=seu_usuario;Password=sua_senha;`).
    * O nome do banco (`Database=TodoListDb_Dev`) será criado automaticamente na próxima etapa.

  

4.  **Configurar Segredos do JWT (Chave Secreta):**
    * A chave secreta do JWT (`Jwt:Key`) **NÃO DEVE** ser armazenada diretamente no `appsettings.json` em um repositório real. Para desenvolvimento local, usaremos a ferramenta User Secrets.
    * Abra um terminal ou Prompt de Comando **na pasta do projeto API** (`TodoList.Api`).
    * **Gere uma chave secreta forte e aleatória.** Você pode usar um gerador online ou o PowerShell:
        * *PowerShell:* `$bytes = New-Object Byte[] 32; [System.Security.Cryptography.RandomNumberGenerator]::Fill($bytes); [System.Convert]::ToBase64String($bytes)`
        * *Copie a chave gerada.*
    * Execute o comando a seguir **na pasta `TodoList.Api`**, substituindo `"SUA_CHAVE_SECRETA_GERADA_AQUI"` pela chave que você gerou:
        ```bash
        dotnet user-secrets set "Jwt:Key" "SUA_CHAVE_SECRETA_GERADA_AQUI"
        ```
    * **Configure Issuer/Audience:** Verifique os valores de `Jwt:Issuer` e `Jwt:Audience` em `appsettings.json`. Eles devem corresponder à URL HTTPS que sua API usará durante a execução (ex: `https://localhost:7271`, verifique `Properties/launchSettings.json`). Você também pode movê-los para User Secrets se preferir:
        ```bash
        dotnet user-secrets set "Jwt:Issuer" "https://localhost:7271"
        dotnet user-secrets set "Jwt:Audience" "https://localhost:7271"
        ```

5.  **Aplicar Migrações (Criar Banco e Tabelas):**
    * Como os arquivos de migração estão no repositório, você só precisa aplicar essas migrações ao seu banco de dados local. Isso criará o banco de dados (se não existir) e todas as tabelas (`TaskItems`, `AspNetUsers`, etc.).
    * **Opção 1: Usando Package Manager Console (PMC) no Visual Studio:**
        * Abra a solução no Visual Studio.
        * Vá em `Tools` -> `NuGet Package Manager` -> `Package Manager Console`.
        * Certifique-se que o "Default project" selecionado seja `TodoList.Infrastructure`.
        * Execute o comando:
            ```powershell
            Update-Database
            ```
            *(O PMC geralmente usa o projeto de startup (`TodoList.Api`) automaticamente para obter a connection string).*
    * **Opção 2: Usando .NET CLI (Terminal):**
        * Navegue no terminal até a pasta raiz da sua solução (onde está o arquivo `.sln`).
        * Execute o comando:
            ```bash
            dotnet ef database update --project TodoList.Infrastructure --startup-project TodoList.Api
            ```
    * Aguarde a execução. Você deve ver mensagens indicando que as migrações foram aplicadas. Verifique seu SQL Server para confirmar que o banco `TodoListDb_Dev` e as tabelas foram criados.

## Configuração do Frontend (Angular)

1.  **Navegar até a Pasta do Frontend:**
    * Abra um terminal ou prompt de comando separado.
    * Navegue até a pasta do projeto Angular:
        ```bash
        cd TodoList.Client
        ```
        *(Ou o nome que você deu para a pasta do frontend).*

2.  **Instalar Dependências:**
    * Execute o comando para baixar e instalar todos os pacotes definidos no `package.json`:
        ```bash
        npm install
        ```



## Executando a Aplicação

Você precisa executar o Backend e o Frontend simultaneamente.

1.  **Executar o Backend API:**
    * **No Visual Studio:** Certifique-se que `TodoList.Api` está definido como projeto de startup (em negrito no Solution Explorer). Pressione F5 ou o botão "Start" (com HTTPS). Anote a URL HTTPS que ele usa (ex: `https://localhost:7271`).
    * **Via .NET CLI:** Navegue até a pasta raiz da solução e execute `dotnet run --project TodoList.Api`. Observe a saída para ver em qual URL ele está escutando.

2.  **Executar o Frontend Angular:**
    * Em um terminal **separado**, navegue até a pasta do frontend (`TodoList.Client`).
    * Execute o comando:
        ```bash
        ng serve -o
        ```
    * Isso compilará o aplicativo Angular, iniciará um servidor de desenvolvimento e abrirá seu navegador padrão, geralmente em `http://localhost:4200`.

3.  **Acessar a Aplicação:**
    * Use a aplicação no navegador que abriu (`http://localhost:4200`).
    * Você deve conseguir se registrar, fazer login e gerenciar suas tarefas.