using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjetoTasks
{
    public partial class Form1 : Form
    {
        // String de conexão com o banco de dados 
        private string connectionString = "Server=DESKTOP-HVL8FQ6\\MSSQLSERVER02;Database=TaskManager;Integrated Security=True;";

        // ID da tarefa selecionada para atualização ou exclusão
        private int selectedTaskId = -1;

        public Form1()
        {
            InitializeComponent();
            this.Click += Form1_Click; // Adiciona o evento Click ao formulário para limpar TextBoxes
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // Carrega as tarefas ao iniciar o formulário
            await AtualizarGridTarefasAsync();
        }

        // Método para carregar as tarefas na DataGridView
        private async Task AtualizarGridTarefasAsync()
        {
            dataGridViewTarefas.DataSource = null; // Limpa os dados existentes na DataGridView
            dataGridViewTarefas.Rows.Clear();

            string query = "SELECT * FROM Tasks"; // Consulta SQL para selecionar todas as tarefas

            try
            {
                DataTable dataTable = await SelectAsync(query); // Executa a consulta 
                dataGridViewTarefas.DataSource = dataTable; // Define os dados da DataGridView como o resultado da consulta
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar tarefas: " + ex.Message); // Exibe mensagem de erro se ocorrer uma exceção
            }
        }

        // Método para executar consultas SELECT no banco de dados 
        private async Task<DataTable> SelectAsync(string query)
        {
            using (var connection = await GetOpenConnectionAsync()) // Abre a conexão com o banco de dados 
            using (var command = new SqlCommand(query, connection))
            {
                var dataTable = new DataTable();
                using (var reader = await command.ExecuteReaderAsync()) // Executa o comando de consulta 
                {
                    dataTable.Load(reader); // Carrega os resultados da consulta para um DataTable
                }
                return dataTable; // Retorna o DataTable com os resultados da consulta
            }
        }

        // Método para executar comandos INSERT, UPDATE ou DELETE no banco de dados 
        private async Task<int> ExecuteNonQueryAsync(string query, SqlParameter[] parameters)
        {
            using (var connection = await GetOpenConnectionAsync()) // Abre a conexão com o banco de dados 
            using (var command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters); // Adiciona os parâmetros ao comando
                }
                return await command.ExecuteNonQueryAsync(); // Executa o comando  e retorna o número de linhas afetadas
            }
        }

        // Método para obter e abrir uma conexão com o banco de dados 
        private async Task<SqlConnection> GetOpenConnectionAsync()
        {
            var connection = new SqlConnection(connectionString); // Cria uma nova instância de conexão com o banco de dados
            await connection.OpenAsync(); // Abre a conexão 
            return connection; // Retorna a conexão aberta
        }

        // Método para limpar as TextBoxes e redefinir o ID da tarefa selecionada
        private void LimparTextBoxes()
        {
            txtID.Text = string.Empty; // Limpa o campo de ID da tarefa
            txtTitulo.Text = string.Empty; // Limpa o campo de título da tarefa
            txtDescricao.Text = string.Empty; // Limpa o campo de descrição da tarefa
            selectedTaskId = -1; // Reseta o ID da tarefa selecionada
            chkCompleta.Checked = false; // Desmarca a CheckBox de tarefa completa
        }

        // Evento Click do formulário para limpar as TextBoxes quando o usuário clicar fora da DataGridView
        private void Form1_Click(object sender, EventArgs e)
        {
            // Verifica se o clique foi fora da DataGridView
            Point pt = dataGridViewTarefas.PointToClient(Cursor.Position); // Converte as coordenadas do cursor para coordenadas relativas à DataGridView
            DataGridView.HitTestInfo hit = dataGridViewTarefas.HitTest(pt.X, pt.Y); // Obtém informações sobre o elemento da DataGridView clicado

            if (hit.Type == DataGridViewHitTestType.None)
            {
                LimparTextBoxes(); // Limpa as TextBoxes se o clique foi fora da DataGridView
            }
        }

        // Evento CellClick da DataGridView para selecionar uma tarefa e exibir seus detalhes nas TextBoxes
        private void dataGridViewTarefas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewTarefas.Rows[e.RowIndex]; // Obtém a linha da DataGridView clicada
                selectedTaskId = Convert.ToInt32(row.Cells["TaskId"].Value); // Obtém o ID da tarefa selecionada

                // Preenche as TextBoxes com os dados da tarefa selecionada, tratando DBNull se necessário
                txtID.Text = selectedTaskId.ToString();
                txtTitulo.Text = row.Cells["Title"].Value == DBNull.Value ? string.Empty : row.Cells["Title"].Value.ToString();
                txtDescricao.Text = row.Cells["Description"].Value == DBNull.Value ? string.Empty : row.Cells["Description"].Value.ToString();
                chkCompleta.Checked = row.Cells["IsCompleted"].Value != DBNull.Value && Convert.ToBoolean(row.Cells["IsCompleted"].Value);
            }
            else
            {
                LimparTextBoxes(); // Limpa as TextBoxes se clicar fora da tabela
            }
        }

        // Evento Click do botão "Inserir" para adicionar uma nova tarefa ao banco de dados
        private async void btnInserir_Click(object sender, EventArgs e)
        {
            string titulo = txtTitulo.Text.Trim(); // Obtém o título da nova tarefa
            string descricao = txtDescricao.Text.Trim(); // Obtém a descrição da nova tarefa

            if (!string.IsNullOrEmpty(titulo) && !string.IsNullOrEmpty(descricao))
            {
                string query = "INSERT INTO Tasks (Title, Description, IsCompleted, CreatedDate) VALUES (@Title, @Description, @IsCompleted, @CreatedDate)";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Title", titulo),
                    new SqlParameter("@Description", descricao),
                    new SqlParameter("@IsCompleted", false),
                    new SqlParameter("@CreatedDate", DateTime.Now)
                };

                try
                {
                    int rowsAffected = await ExecuteNonQueryAsync(query, parameters); // Executa o comando de inserção de forma assíncrona
                    MessageBox.Show($"Tarefa inserida com sucesso. Linhas afetadas: {rowsAffected}");
                    await AtualizarGridTarefasAsync(); // Atualiza a DataGridView com as novas tarefas após a inserção
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao inserir tarefa: " + ex.Message); // Exibe mensagem de erro se ocorrer uma exceção
                }
            }
            else
            {
                MessageBox.Show("Por favor, insira o título e a descrição da tarefa."); // Exibe mensagem se o título ou descrição estiverem vazios
            }
        }

        // Evento Click do botão "Atualizar" para atualizar uma tarefa existente no banco de dados
        private async void btnAtualizar_Click(object sender, EventArgs e)
        {
            if (selectedTaskId < 0)
            {
                MessageBox.Show("Selecione uma tarefa para atualizar."); // Exibe mensagem se nenhuma tarefa estiver selecionada
                return;
            }

            string titulo = txtTitulo.Text.Trim(); // Obtém o novo título da tarefa
            string descricao = txtDescricao.Text.Trim(); // Obtém a nova descrição da tarefa
            bool isCompleted = chkCompleta.Checked; // Obtém o novo status de conclusão da tarefa

            if (!string.IsNullOrEmpty(titulo) && !string.IsNullOrEmpty(descricao))
            {
                string query = "UPDATE Tasks SET Title = @Title, Description = @Description, IsCompleted = @IsCompleted WHERE TaskId = @TaskId";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Title", titulo),
                    new SqlParameter("@Description", descricao),
                    new SqlParameter("@IsCompleted", isCompleted),
                    new SqlParameter("@TaskId", selectedTaskId)
                };

                try
                {
                    int rowsAffected = await ExecuteNonQueryAsync(query, parameters); // Executa o comando de atualização 
                    MessageBox.Show($"Tarefa atualizada com sucesso. Linhas afetadas: {rowsAffected}");
                    await AtualizarGridTarefasAsync(); // Atualiza a DataGridView após a atualização da tarefa
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao atualizar tarefa: " + ex.Message); // Exibe mensagem de erro se ocorrer uma exceção
                }
            }
            else
            {
                MessageBox.Show("Por favor, insira o título e a descrição da tarefa."); // Exibe mensagem se o título ou descrição estiverem vazios
            }
        }

        // Evento Click do botão "Excluir" para excluir uma tarefa do banco de dados
        private async void btnExcluir_Click(object sender, EventArgs e)
        {
            if (selectedTaskId < 0)
            {
                MessageBox.Show("Selecione uma tarefa para excluir."); // Exibe mensagem se nenhuma tarefa estiver selecionada
                return;
            }

            string query = "DELETE FROM Tasks WHERE TaskId = @TaskId";
            SqlParameter parameter = new SqlParameter("@TaskId", selectedTaskId);

            try
            {
                int rowsAffected = await ExecuteNonQueryAsync(query, new SqlParameter[] { parameter }); // Executa o comando de exclusão 
                MessageBox.Show($"Tarefa excluída com sucesso. Linhas afetadas: {rowsAffected}");
                await AtualizarGridTarefasAsync(); // Atualiza a DataGridView após a exclusão da tarefa
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao excluir tarefa: " + ex.Message); // Exibe mensagem de erro se ocorrer uma exceção
            }
        }
    }
}
