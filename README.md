Gerenciador de Tarefas (TaskManager)
Descrição
O Gerenciador de Tarefas é uma aplicação de desktop simples desenvolvida em C# (Windows Forms) com um banco de dados SQL Server. A aplicação permite criar, visualizar, atualizar e excluir tarefas, além de marcar uma tarefa como completa ou incompleta.

Funcionalidades:
Adicionar Tarefa: O usuário pode criar uma nova tarefa com título e descrição.
Listar Tarefas: Exibe uma lista de todas as tarefas criadas com seus detalhes.
Atualizar Tarefa: Permite alterar o título, descrição e o status (completa ou não) de uma tarefa.
Excluir Tarefa: Remove uma tarefa existente.
Marcar como Completa/Incompleta: Altera o status de uma tarefa entre completa e incompleta.
O projeto utiliza programação assíncrona (async e await) para todas as operações CRUD.

Pré-requisitos
Antes de executar a aplicação, certifique-se de que você tem o seguinte instalado:

.NET Framework (versão usada no projeto, como 4.7.2 ou superior).
Visual Studio (ou qualquer outra IDE compatível com C# e Windows Forms).
SQL Server (ou o SQL Server Express) e o SQL Server Management Studio (SSMS) para configurar o banco de dados.
Configuração do Banco de Dados
Abra o SQL Server Management Studio (SSMS) e crie o banco de dados para a aplicação:

sql
Copiar código
CREATE DATABASE TaskManager;
GO

USE TaskManager;
GO

CREATE TABLE Tasks (
    TaskId INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(100),
    Description NVARCHAR(500),
    IsCompleted BIT,
    CreatedDate DATETIME
);
String de Conexão: Atualize o arquivo App.config do projeto com a string de conexão para o SQL Server. Exemplo de uma string de conexão:

xml
Copiar código
<connectionStrings>
  <add name="TaskManagerDB" 
       connectionString="Data Source=SEU_SERVIDOR;Initial Catalog=TaskManager;Integrated Security=True" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
Substitua SEU_SERVIDOR pelo nome do servidor SQL onde o banco de dados foi criado.

Como Executar
Clone ou baixe o projeto para o seu ambiente local.
Abra o projeto no Visual Studio.
Atualize a string de conexão no arquivo App.config conforme indicado na seção anterior.
Execute o script SQL fornecido para criar o banco de dados e a tabela.
Compile e execute o projeto.
Uso
1. Adicionar Tarefa:
Insira um título e uma descrição na interface e clique no botão "Adicionar" para salvar a nova tarefa no banco de dados.
2. Listar Tarefas:
Todas as tarefas serão listadas na interface principal, mostrando título, descrição, status e data de criação.
3. Atualizar Tarefa:
Selecione uma tarefa na lista e edite suas informações (título, descrição ou status) e clique em "Atualizar".
4. Excluir Tarefa:
Selecione uma tarefa e clique no botão "Excluir" para removê-la.
5. Marcar como Completa/Incompleta:
Selecione uma tarefa e clique no botão "Completar" ou "Incompletar" para alterar o status.
Estrutura do Projeto
MainForm.cs: Interface principal da aplicação (Windows Forms).
TaskManagerDB.cs: Classe responsável pela conexão com o banco de dados e execução das operações CRUD.
App.config: Arquivo de configuração do projeto, contendo a string de conexão ao banco de dados.
Tecnologias Utilizadas
C# (Windows Forms)
SQL Server (para gerenciamento do banco de dados)
Async/Await (para operações CRUD assíncronas)
Contribuição
Se quiser contribuir para este projeto, sinta-se à vontade para fazer um fork e enviar um pull request.

Licença
Este projeto está sob a licença MIT.

