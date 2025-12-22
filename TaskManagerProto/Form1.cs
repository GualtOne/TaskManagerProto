using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskManagerProto
{
    public partial class Form1 : Form
    {
        public ListView taskListView;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private Panel panel;
        private ToolStrip toolStrip;
        private ListViewItem item;

        public Form1()
        {
            DBmanager.Connection();
            InitializeComponent();
            InitializeTaskListView();
            this.Text = "Менеджер задач";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            statusStrip.Items.Add(statusLabel);
            this.Controls.Add(statusStrip);

            panel = new Panel();
            panel.Dock = DockStyle.Fill;
            this.Controls.Add(panel);
            panel.Controls.Add(taskListView);
            InitializeToolstrip();
            RefreshTaskList();
            CheckDeadlineExpired();
            CheckDeadlineToday();
        }

        private void InitializeToolstrip()
        {
            toolStrip = new ToolStrip
            {
                Dock = DockStyle.Top

            };
            ToolStripDropDownButton filterbtn = new ToolStripDropDownButton()
            {
                Text = "Фильтр",
                ToolTipText = "Выбор способа сортировки"
            };
            ToolStripButton addbtn = new ToolStripButton()
            {
                Text = "Добавить задачу",
                ToolTipText = "Открывает окно создания задачи"
            };

            ToolStripDropDown dropDown = new ToolStripDropDown();
            ToolStripDropDownButton chckdl = new ToolStripDropDownButton()
            {
                Text = "Дедлайны",
                ToolTipText = "Проверяет дедлайном задач",
                DropDown = dropDown,
                DropDownDirection = ToolStripDropDownDirection.Default,
                Height = 100,
                Width = 100,
            };

            ToolStripButton chckdltoday = new ToolStripButton()
            {
                Text = "Сегодня",
                ToolTipText = "Проверяет наличие задач с дедлайном сегодня",
            };

            ToolStripButton chckdlexpired = new ToolStripButton()
            {
                Text = "Просроченные",
                ToolTipText = "Проверяет наличие задач с просроченым дедлайном",
            };

            chckdl.DropDown.Items.Add(chckdltoday);
            chckdl.DropDown.Items.Add(chckdlexpired);

            //удали это если не надо
            ToolStripButton testd = new ToolStripButton()
           {
               Text = "DEADLINE",
               ToolTipText = ""
           };

            addbtn.Click += (s, e) => AddTaskWindow();
            chckdltoday.Click += (s, e) => CheckDeadlineToday();
            chckdlexpired.Click += (s, e) => CheckDeadlineExpired();
            testd.Click += (s, e) => DEADLINE();
            panel.Controls.Add(toolStrip);
            toolStrip.Items.AddRange(new ToolStripItem[] { addbtn, filterbtn, chckdl, testd });
        }

        private void DEADLINE()
        {
            MessageBox.Show(taskListView.Items[0].Text);
        }


        private void AddTaskWindow()
        {
            using (var addForm = new TaskManager())
            {
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        int statusId = addForm.GetStatusId();
                        int typeId = addForm.GetTypeId();
                        Priority priority = addForm.GetPriority();

                        DBmanager.AddTask(
                            addForm.TaskName,
                            addForm.TaskDescription,
                            statusId,
                            typeId,
                            DateTime.Now,
                            addForm.Deadline,
                            priority
                        );

                        MessageBox.Show("Задача успешно добавлена", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        RefreshTaskList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при добавлении задачи: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void EditTaskWindow(int taskId)
        {
            using (var editForm = new TaskManager(taskId))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        int statusId = editForm.GetStatusId();
                        int typeId = editForm.GetTypeId();
                        Priority priority = editForm.GetPriority();
                        DBmanager.UpdateTask(
                            taskId,
                            editForm.TaskName,
                            editForm.TaskDescription,
                            statusId,
                            typeId,
                            editForm.Deadline,
                            priority
                        );

                        MessageBox.Show("Задача успешно обновлена", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        RefreshTaskList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при обновлении задачи: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void InitializeTaskListView()
        {
            taskListView = new ListView();
            taskListView.Dock = DockStyle.Fill;
            taskListView.View = View.Details;
            taskListView.FullRowSelect = true;
            taskListView.GridLines = true;
            taskListView.MultiSelect = false;

            taskListView.Columns.Add("ID", 50);
            taskListView.Columns.Add("Имя Задачи", 200);
            taskListView.Columns.Add("Статус", 120);
            taskListView.Columns.Add("Тип", 120);
            taskListView.Columns.Add("Приоритет", 100);
            taskListView.Columns.Add("Дата создания", 150);
            taskListView.Columns.Add("Дедлайн", 150);

            taskListView.ContextMenuStrip = CreateContextMenu();
        }

        private ContextMenuStrip CreateContextMenu()
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem QuickItem = new ToolStripMenuItem("Быстрые действия");
            ToolStripMenuItem Edititem = new ToolStripMenuItem("Редактировать задачу");
            ToolStripMenuItem DeleteItem = new ToolStripMenuItem("Удалить задачу");
            ToolStripMenuItem ShowDesc = new ToolStripMenuItem("Показать описание");

            ToolStripMenuItem StatusUP = new ToolStripMenuItem("Следующий статус");
            ToolStripMenuItem PriorityUP = new ToolStripMenuItem("Следующий приоритет");

            QuickItem.DropDown.Items.AddRange(new ToolStripItem[] { StatusUP, PriorityUP });

            Edititem.Click += (s, e) => EditSelectedItem();
            DeleteItem.Click += (s, e) => DeleteSelectedTask();
            ShowDesc.Click += (s, e) => ShowTaskDesc();

            contextMenu.Items.AddRange(new ToolStripItem[] { QuickItem, Edititem, DeleteItem, ShowDesc });
            return contextMenu;
        }

        private void RefreshTaskList()
        {
            try
            {
                taskListView.BeginUpdate();
                taskListView.Items.Clear();

                var tasks = DBmanager.GetTasks().ToList();

                foreach (var task in tasks)
                {
                    try
                    {
                        item = new ListViewItem(task.ID.ToString());
                        item.SubItems.Add(task.TaskName);
                        string statusName = DBmanager.GetTaskStatusName(task.StatusID);
                        string typeName = DBmanager.GetTaskTypeName(task.TypeID);

                        item.SubItems.Add(string.IsNullOrEmpty(statusName) ? "Не указан" : statusName);
                        item.SubItems.Add(string.IsNullOrEmpty(typeName) ? "Не указан" : typeName);
                        item.SubItems.Add(task.Priority.ToString());
                        item.SubItems.Add(task.StartDate.ToString("dd.MM.yyyy HH:mm"));
                        item.SubItems.Add(task.DeadLine?.ToString("dd.MM.yyyy HH:mm") ?? "Нет");
                        item.Tag = task;
                        taskListView.Items.Add(item);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Ошибка при добавлении задачи в список: {ex.Message}");
                    }
                }

                statusLabel.Text = $"Всего задач: {tasks.Count} | Последнее обновление: {DateTime.Now:HH:mm:ss}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении списка задач: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                taskListView.EndUpdate();
            }
        }

        private void EditSelectedItem()
        {
            Task selectedTask = GetSelectedTask();
            if (selectedTask != null)
            {
                try
                {
                    EditTaskWindow(selectedTask.ID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось открыть форму редактирования: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите задачу для редактирования.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DeleteSelectedTask()
        {
            Task selectedTask = GetSelectedTask();
            if (selectedTask != null)
            {
                try
                {
                    string taskName = selectedTask.TaskName;
                    DialogResult result = MessageBox.Show(
                        $"Вы уверены, что хотите удалить задачу '{taskName}' (ID: {selectedTask.ID})?",
                        "Подтверждение удаления",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        DBmanager.DeleteTask(selectedTask.ID);
                        RefreshTaskList();
                        MessageBox.Show($"Задача '{taskName}' успешно удалена.", "Информация",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось удалить задачу: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите задачу для удаления.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ShowTaskDesc()
        {
            Task selectedTask = GetSelectedTask();
            if (selectedTask != null)
            {
                try
                {
                    string statusName = DBmanager.GetTaskStatusName(selectedTask.StatusID);
                    string typeName = DBmanager.GetTaskTypeName(selectedTask.TypeID);

                    string info = $"ID: {selectedTask.ID}\n" +
                                  $"Название: {selectedTask.TaskName}\n" +
                                  $"Описание: {selectedTask.TaskDescription}\n" +
                                  $"Статус: {statusName}\n" +
                                  $"Тип: {typeName}\n" +
                                  $"Приоритет: {selectedTask.Priority}\n" +
                                  $"Дата создания: {selectedTask.StartDate:dd.MM.yyyy HH:mm}\n" +
                                  $"Дедлайн: {(selectedTask.DeadLine?.ToString("dd.MM.yyyy HH:mm") ?? "Не установлен")}";

                    MessageBox.Show(info, "Подробная информация о задаче",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось отобразить информацию о задаче: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private Task GetSelectedTask()
        {
            if (taskListView.SelectedItems.Count > 0)
            {
                return taskListView.SelectedItems[0].Tag as Task;
            }
            return null;
        }

        private void Sort()
        {
        }

        private void CheckDeadlineToday()
        {
            try
            {
                List<string> dla = new List<string>();
                for (int i = 0; i < taskListView.Items.Count; i++)
                {
                    DateTime deadline = Convert.ToDateTime(taskListView.Items[i].SubItems[6].Text);
                    if (deadline.Day == DateTime.Now.Day)
                    {
                        dla.Add(taskListView.Items[i].SubItems[1].Text);
                    }
                }
                if (dla.Count > 0)
                {    
                    string resualt = $"Сегодня дедланй у задачи/задач {string.Join("; ", dla)}.";
                    MessageBox.Show(resualt, "Дедлайны");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void CheckDeadlineExpired()
        {
            try
            {
                List<int> dla = new List<int>();
                for (int i = 0; i < taskListView.Items.Count; i++)
                {
                    DateTime deadline = Convert.ToDateTime(taskListView.Items[i].SubItems[6].Text);
                    if (deadline < DateTime.Now && deadline.Day != DateTime.Now.Day)
                    {
                        dla.Add(i);
                    }
                }
                if (dla.Count > 0)
                {
                    foreach (int item in dla)
                    {
                        taskListView.Items[item].BackColor = Color.Red;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}