using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace TODO1
{
    public partial class ProjectSelectionForm : Form
    {
        private int selectedProjectId;
        public int SelectedProjectId => selectedProjectId;
        public ProjectSelectionForm(List<string> projects)
        {
            InitializeComponent();
            comboBoxProjects.Items.Clear();
            foreach (var p in projects)
            {
                comboBoxProjects.Items.Add(p);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            selectedProjectId = GetSelectedProjectId(comboBoxProjects.SelectedItem.ToString());
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private int GetSelectedProjectId(string projectName)
        {
            using (var conn = new SQLiteConnection(@"Data Source=db.db"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT id FROM project WHERE name=@name", conn))
                {
                    cmd.Parameters.AddWithValue("@name", projectName);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
    }
}
