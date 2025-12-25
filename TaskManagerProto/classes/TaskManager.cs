using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TaskManagerProto
{
    public partial class TaskManager : Form
    {
        private TextBox nameBox;
        private TextBox descriptionBox;
        private ComboBox statusComboBox;
        private ComboBox typeComboBox;
        private ComboBox priorityComboBox;
        private DateTimePicker dateTimePicker;
        private Label nameLabel;
        private Label descriptionLabel;
        private Label statusLabel;
        private Label typeLabel;
        private Label priorityLabel;
        private Label deadlineLabel;
        private Button saveButton;
        private Button cancelButton;
        private Panel mainPanel;

        public string TaskName => nameBox.Text;
        public string TaskDescription => descriptionBox.Text;
        public string SelectedStatus => statusComboBox.SelectedItem?.ToString();
        public string SelectedType => typeComboBox.SelectedItem?.ToString();
        public string SelectedPriority => priorityComboBox.SelectedItem?.ToString();
        public DateTime? Deadline => dateTimePicker.Value;

        public bool IsEditMode { get; set; }
        public int TaskId { get; private set; }

        public TaskManager()
        {
            InitializeComponent();
            IsEditMode = false;
            Text = "Добавить задачу";
            saveButton.Text = "Добавить задачу";
            saveButton.BackColor = Color.LightGreen;
            LoadData();
        }

        public TaskManager(int taskId) : this()
        {
            IsEditMode = true;
            TaskId = taskId;
            Text = "Изменить задачу";
            saveButton.Text = "Сохранить изменения";
            saveButton.BackColor = Color.LightBlue;
            LoadTaskData(taskId);
        }

        private void InitializeComponent()
        {
            Size = new Size(350, 500);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            Controls.Add(mainPanel);

            nameLabel = new Label()
            {
                Text = "Имя задачи:",
                Dock = DockStyle.Top,
                Height = 25,
                Padding = new Padding(5, 5, 0, 0)
            };

            nameBox = new TextBox()
            {
                Dock = DockStyle.Top,
                Height = 30,
                Margin = new Padding(5, 0, 5, 5)
            };

            descriptionLabel = new Label()
            {
                Text = "Описание задачи:",
                Dock = DockStyle.Top,
                Height = 25,
                Padding = new Padding(5, 5, 0, 0)
            };

            descriptionBox = new TextBox()
            {
                Dock = DockStyle.Top,
                Height = 80,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Margin = new Padding(5, 0, 5, 5)
            };

            statusLabel = new Label()
            {
                Text = "Статус задачи:",
                Dock = DockStyle.Top,
                Height = 25,
                Padding = new Padding(5, 5, 0, 0)
            };

            statusComboBox = new ComboBox()
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 30,
                Margin = new Padding(5, 0, 5, 5)
            };

            typeLabel = new Label()
            {
                Text = "Тип задачи:",
                Dock = DockStyle.Top,
                Height = 25,
                Padding = new Padding(5, 5, 0, 0)
            };

            typeComboBox = new ComboBox()
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 30,
                Margin = new Padding(5, 0, 5, 5)
            };

            priorityLabel = new Label()
            {
                Text = "Приоритет:",
                Dock = DockStyle.Top,
                Height = 25,
                Padding = new Padding(5, 5, 0, 0)
            };

            priorityComboBox = new ComboBox()
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 30,
                Margin = new Padding(5, 0, 5, 5)
            };

            deadlineLabel = new Label()
            {
                Text = "Дедлайн:",
                Dock = DockStyle.Top,
                Height = 25,
                Padding = new Padding(5, 5, 0, 0)
            };

            dateTimePicker = new DateTimePicker()
            {
                Dock = DockStyle.Top,
                Height = 30,
                Value = DateTime.Now.AddDays(7),
                Margin = new Padding(5, 0, 5, 5)
            };

            saveButton = new Button()
            {
                Height = 40,
                Dock = DockStyle.Bottom,
                Margin = new Padding(5, 10, 5, 5)
            };

            saveButton.Click += SaveButton_Click;

            cancelButton = new Button()
            {
                Text = "Отмена",
                Height = 40,
                Dock = DockStyle.Bottom,
                Margin = new Padding(5, 5, 5, 5)
            };

            cancelButton.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            mainPanel.Controls.AddRange(new Control[] {
                saveButton,
                cancelButton,
                dateTimePicker, deadlineLabel,
                priorityComboBox, priorityLabel,
                typeComboBox, typeLabel,
                statusComboBox, statusLabel,
                descriptionBox, descriptionLabel,
                nameBox, nameLabel
            });
        }

        private void LoadData()
        {
            var statuses = DBmanager.GetAllStatuses();
            foreach (var status in statuses)
            {
                statusComboBox.Items.Add(status.Name);
            }
            if (statusComboBox.Items.Count > 0)
                statusComboBox.SelectedIndex = 0;

            var types = DBmanager.GetAllTypes();
            foreach (var type in types)
            {
                typeComboBox.Items.Add(type.Name);
            }
            if (typeComboBox.Items.Count > 0)
                typeComboBox.SelectedIndex = 0;

            var priorities = Enum.GetValues(typeof(Priority));
            foreach (Priority priority in priorities)
            {
                priorityComboBox.Items.Add(priority.ToString());
            }
            if (priorityComboBox.Items.Count > 0)
                priorityComboBox.SelectedIndex = 0;
        }

        private void LoadTaskData(int taskId)
        {
            var task = DBmanager.GetTaskById(taskId);
            if (task != null)
            {
                nameBox.Text = task.TaskName;
                descriptionBox.Text = task.TaskDescription;
                dateTimePicker.Value = task.DeadLine ?? DateTime.Now.AddDays(7);

                var currentStatusName = DBmanager.GetTaskStatusName(Convert.ToInt32(task.StatusID));
                if (!string.IsNullOrEmpty(currentStatusName))
                {
                    for (int i = 0; i < statusComboBox.Items.Count; i++)
                    {
                        if (statusComboBox.Items[i].ToString() == currentStatusName)
                        {
                            statusComboBox.SelectedIndex = i;
                            break;
                        }
                    }
                }

                var currentTypeName = DBmanager.GetTaskTypeName(Convert.ToInt32(task.StatusID));
                if (!string.IsNullOrEmpty(currentTypeName))
                {
                    for (int i = 0; i < typeComboBox.Items.Count; i++)
                    {
                        if (typeComboBox.Items[i].ToString() == currentTypeName)
                        {
                            typeComboBox.SelectedIndex = i;
                            break;
                        }
                    }
                }

                for (int i = 0; i < priorityComboBox.Items.Count; i++)
                {
                    if (priorityComboBox.Items[i].ToString() == task.Priority.ToString())
                    {
                        priorityComboBox.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrEmpty(nameBox.Text.Trim()))
            {
                MessageBox.Show("Имя задачи не может быть пустым", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nameBox.Focus();
                return false;
            }

            if (nameBox.Text.Trim().Length > 100) 
            {
                MessageBox.Show($"Имя задачи не может быть длинее 100, Длина имени задачи: {nameBox.Text.Trim().Length}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nameBox.Focus();
                return false;
            }

            if (nameBox.Text.Trim().Length > 500)
            {
                MessageBox.Show($"Описание задачи не может быть длинее 500, Длина описания задачи: {descriptionBox.Text.Trim().Length}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nameBox.Focus();
                return false;
            }

            if (statusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите статус задачи", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                statusComboBox.Focus();
                return false;
            }

            if (typeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип задачи", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                typeComboBox.Focus();
                return false;
            }

            if (priorityComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите приоритет задачи", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                priorityComboBox.Focus();
                return false;
            }

            return true;
        }

        public int GetStatusId()
        {
            return DBmanager.GetStatusIdByName(SelectedStatus);
        }

        public int GetTypeId()
        {
            return DBmanager.GetTypeIdByName(SelectedType);
        }

        public Priority GetPriority()
        {
            if (Enum.TryParse<Priority>(SelectedPriority, out Priority priority))
            {
                return priority;
            }
            return Priority.Низкий;
        }
    }
}