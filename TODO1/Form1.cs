using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Data.SQLite;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TODO1
{
    public partial class Form1 : Form
    {
        private string currentFilePath = null;
        private string connectionString = @"Data Source=db.db";

        public Form1() : base()
        {
            InitializeComponent();
            InitializeCustomHandlers();

        }

        private void InitializeCustomHandlers()
        {
            this.dataGridView1.CellDoubleClick += createToolStripMenuItem_Click;
            this.button1.Click += createToolStripMenuItem_Click;
            this.button2.Click += deleteToolStripMenuItem_Click;
            this.button3.Click += activeToolStripMenuItem_Click;
            this.button4.Click += completedToolStripMenuItem_Click;
            this.button5.Click += resetFilterToolStripMenuItem_Click;
            this.newToolStripMenuItem.Click += newToolStripMenuItem_Click;
            this.openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            this.saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            this.saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
            this.exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
        }

        private void ResetFilter()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    row.Visible = true;
                }
            }
        }

        private void ApplyFilter(string status)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    row.Visible = (row.Cells["type"].Value?.ToString() == status);
                }
            }
        }

        private void ExportToCsv(string filePath)
        {
            try
            {
                using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    writer.WriteLine("Задача,Срок,Статус");
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            writer.WriteLine($"{row.Cells["task"].Value},{row.Cells["Date"].Value},{row.Cells["type"].Value}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения файла: {ex.Message}");
            }
        }

        private void ImportFromCsv(string filePath)
        {
            try
            {
                ResetFilter();
                dataGridView1.Rows.Clear();

                using (var reader = new StreamReader(filePath, Encoding.UTF8))
                {
                    reader.ReadLine();
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var parts = line.Split(',');
                        if (parts.Length == 3)
                        {
                            dataGridView1.Rows.Add(parts[0], parts[1], parts[2]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка чтения файла: {ex.Message}");
            }
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var taskForm = new TaskForm())
            {
                if (taskForm.ShowDialog() == DialogResult.OK)
                {
                    var dateStr = taskForm.DueDate.HasValue
                        ? taskForm.DueDate.Value.ToString("yyyy-MM-dd")
                        : "Не указано";

                    dataGridView1.Rows.Add(taskForm.TaskName, dateStr, taskForm.Status);
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.EndEdit();
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                if (!row.IsNewRow)
                {
                    dataGridView1.Rows.Remove(row);
                }
            }
        }

        private void activeToolStripMenuItem_Click(object sender, EventArgs e) => ApplyFilter("Active");
        private void completedToolStripMenuItem_Click(object sender, EventArgs e) => ApplyFilter("Completed");

        private void resetFilterToolStripMenuItem_Click(object sender, EventArgs e) => ResetFilter();

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                var result = MessageBox.Show("Сохранить текущий проект?", "Подтверждение", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes) saveToolStripMenuItem_Click(sender, e);
                else if (result == DialogResult.Cancel) return;
            }

            dataGridView1.Rows.Clear();
            currentFilePath = null;
            ResetFilter();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ImportFromCsv(openFileDialog1.FileName);
                currentFilePath = openFileDialog1.FileName;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                saveAsToolStripMenuItem_Click(sender, e);
            }
            else
            {
                ExportToCsv(currentFilePath);
            }
        }
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.DefaultExt = "csv";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog1.FileName;
                if (!filePath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    filePath += ".csv";
                }

                currentFilePath = filePath;
                ExportToCsv(currentFilePath);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();

                    var inputName = Interaction.InputBox("Введите название проекта:", "Название проекта");

                    if (!string.IsNullOrEmpty(inputName))
                    {
                        using (var cmd = new SQLiteCommand(conn))
                        {
                            cmd.CommandText = "INSERT INTO project(name) VALUES(@name)";
                            cmd.Parameters.AddWithValue("@name", inputName);
                            cmd.ExecuteNonQuery();

                            MessageBox.Show($"Проект {inputName} успешно добавлен.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ShowProjectSelectionDialog(async (selectedProjectId) =>
            {
                await ImportTasksAsync(selectedProjectId);
            });
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ShowProjectSelectionDialog(async (selectedProjectId) =>
            {
                await ExportTasksAsync(selectedProjectId);
            });
        }

        private void ShowProjectSelectionDialog(Action<int> onSelectedAction)
        {
            var form = new ProjectSelectionForm(GetAllProjectNames());
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                label2.Text = form.SelectedProjectName;
                onSelectedAction.Invoke(form.SelectedProjectId);
            }
        }

        private async Task ImportTasksAsync(int projectId)
        {
            ClearTaskTable();
            await LoadTasksForProject(projectId);
        }

        private async Task ExportTasksAsync(int projectId)
        {
            DeleteExistingTasks(projectId);
            InsertNewTasksFromGrid(projectId);
        }

        private List<string> GetAllProjectNames()
        {
            var names = new List<string>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT name FROM project", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            names.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return names;
        }

        private async Task LoadTasksForProject(int projectId)
        {
            dataGridView1.Rows.Clear();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT * FROM task WHERE project_id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", projectId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            AddRowToGrid(reader.GetString(2), reader.GetString(3), reader.GetString(4));
                        }
                    }
                }
            }
        }

        private void AddRowToGrid(string task, string date, string type)
        {
            dataGridView1.Rows.Add(task, date, type);
        }

        private void ClearTaskTable()
        {
            dataGridView1.Rows.Clear();
        }

        private void DeleteExistingTasks(int projectId)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM task WHERE project_id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", projectId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void InsertNewTasksFromGrid(int projectId)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("INSERT INTO task (project_id, task, date, type) VALUES (@project_id, @task, @date, @type)", conn))
                    {
                        cmd.Parameters.AddWithValue("@project_id", projectId);
                        cmd.Parameters.AddWithValue("@task", row.Cells["task"].Value?.ToString() ?? "Неизвестная задача");
                        cmd.Parameters.AddWithValue("@date", row.Cells["Date"].Value?.ToString() ?? "Не указано");
                        cmd.Parameters.AddWithValue("@type", row.Cells["type"].Value?.ToString() ?? "Active");
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
     }
}