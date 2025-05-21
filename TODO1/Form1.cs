using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TODO1
{
    public partial class Form1 : Form
    {
        private string currentFilePath = null;

        public Form1() : base()
        {
            InitializeComponent();
            InitializeCustomHandlers();
        }

        private void InitializeCustomHandlers()
        {
            this.dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
            this.createToolStripMenuItem.Click += createToolStripMenuItem_Click;
            this.deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
            this.activeToolStripMenuItem.Click += activeToolStripMenuItem_Click;
            this.completedToolStripMenuItem.Click += completedToolStripMenuItem_Click;
            this.resetFilterToolStripMenuItem.Click += resetFilterToolStripMenuItem_Click;
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

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 0)
            {
                var row = dataGridView1.Rows[e.RowIndex];
                using (var taskForm = new TaskForm())
                {
                    taskForm.InitialTaskName = row.Cells["task"].Value?.ToString();

                    if (DateTime.TryParse(row.Cells["Date"].Value?.ToString(), out var date))
                    {
                        taskForm.InitialDueDate = date;
                    }
                    else
                    {
                        taskForm.InitialDueDate = null;
                    }

                    taskForm.InitialStatus = row.Cells["type"].Value?.ToString();

                    if (taskForm.ShowDialog() == DialogResult.OK)
                    {
                        row.Cells["task"].Value = taskForm.TaskName;
                        row.Cells["Date"].Value = taskForm.DueDate.HasValue
                            ? taskForm.DueDate.Value.ToString("yyyy-MM-dd")
                            : "Не указано";
                        row.Cells["type"].Value = taskForm.Status;
                    }
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
    }
}