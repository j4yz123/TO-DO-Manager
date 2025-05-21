using System;
using System.Windows.Forms;

namespace TODO1
{
    public partial class TaskForm : Form
    {
        public string TaskName { get; private set; }
        public DateTime? DueDate { get; private set; }
        public string Status { get; private set; }

        public TaskForm()
        {
            InitializeComponent();
            statusComboBox.Items.AddRange(new string[] { "Active", "Completed" });
        }

        public string InitialTaskName
        {
            get => taskTextBox.Text;
            set => taskTextBox.Text = value;
        }

        public DateTime? InitialDueDate
        {
            get => dateTimePicker.Checked ? (DateTime?)dateTimePicker.Value : null;
            set
            {
                if (value.HasValue)
                {
                    dateTimePicker.Checked = true;
                    dateTimePicker.Value = value.Value;
                }
                else
                {
                    dateTimePicker.Checked = false;
                }
            }
        }

        public string InitialStatus
        {
            get => statusComboBox.SelectedItem?.ToString() ?? "Active";
            set
            {
                if (value == "Active" || value == "Completed")
                {
                    statusComboBox.SelectedItem = value;
                }
                else
                {
                    statusComboBox.SelectedIndex = 0;
                }
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(taskTextBox.Text))
            {
                MessageBox.Show("Введите название задачи.");
                return;
            }

            TaskName = taskTextBox.Text;
            DueDate = dateTimePicker.Checked ? (DateTime?)dateTimePicker.Value : null;
            Status = statusComboBox.SelectedItem?.ToString() ?? "Active";
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) { components.Dispose(); }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.taskTextBox = new System.Windows.Forms.TextBox();
            this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.statusComboBox = new System.Windows.Forms.ComboBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            this.taskTextBox.Location = new System.Drawing.Point(12, 12);
            this.taskTextBox.Name = "taskTextBox";
            this.taskTextBox.Size = new System.Drawing.Size(300, 31);
            this.taskTextBox.TabIndex = 0;
            this.dateTimePicker.Checked = false;
            this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker.Location = new System.Drawing.Point(12, 50);
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size(150, 31);
            this.dateTimePicker.TabIndex = 1;
            this.statusComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.statusComboBox.FormattingEnabled = true;
            this.statusComboBox.Location = new System.Drawing.Point(170, 50);
            this.statusComboBox.Name = "statusComboBox";
            this.statusComboBox.Size = new System.Drawing.Size(142, 33);
            this.statusComboBox.TabIndex = 2;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(12, 90);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(150, 40);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(170, 90);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(142, 40);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 142);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.statusComboBox);
            this.Controls.Add(this.dateTimePicker);
            this.Controls.Add(this.taskTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TaskForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Редактирование задачи";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private TextBox taskTextBox;
        private DateTimePicker dateTimePicker;
        private ComboBox statusComboBox;
        private Button okButton;
        private Button cancelButton;
    }
}