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

        public enum SortKind
        {
            ByID,
            ByName,
            ByDate,
            ByDeadline,
            ByType,
            ByStatus,
            ByPriority,
        }

        bool deadlinetodaybuttonpressed = false;
        bool deadlinexpiredbuttonpressed = false;
        bool deadlinetomorowdbuttonpressed = false;

        public Form1()
        {
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
            RefreshTaskList(SortKind.ByID);
            CheckDeadlineToday();
            CheckDeadlineTomorow();
        }

        private void InitializeToolstrip()
        {
            toolStrip = new ToolStrip
            {
                Dock = DockStyle.Top
            };

            ToolStripDropDown dropDownfilter = new ToolStripDropDown();
            ToolStripDropDownButton filterbtn = new ToolStripDropDownButton()
            {
                Text = "Сортировка",
                ToolTipText = "Выбор способа сортировки",
                DropDown = dropDownfilter,
                DropDownDirection = ToolStripDropDownDirection.Default,
                Height = 100,
                Width = 100,
            };

            ToolStripButton sortbyid = new ToolStripButton()
            {
                Text = "По ID",
                ToolTipText = "Сортирует по ID (0-9)",
                Dock = DockStyle.Fill,
            };

            ToolStripButton sortbyname = new ToolStripButton()
            {
                Text = "По имени",
                ToolTipText = "Сортирует по Имени (0-9 ,A-Z, А-Я)",
                Dock = DockStyle.Fill,
            };

            ToolStripButton sortbypriority = new ToolStripButton()
            {
                Text = "По проритету",
                ToolTipText = "Сортирует по проритету (High, medium, low)",
                Dock = DockStyle.Fill,
            };

            ToolStripButton sortbystatus = new ToolStripButton()
            {
                Text = "По статусу",
                ToolTipText = "Сортирует по статусу (Новая, В процессе, Готово)",
                Dock = DockStyle.Fill,
            };

            ToolStripButton sortbytype = new ToolStripButton()
            {
                Text = "По типу",
                ToolTipText = "Сортирует по типу (Работа, Дом, Личное)",
                Dock = DockStyle.Fill,
            };

            ToolStripButton sortbydeadline = new ToolStripButton()
            {
                Text = "По дедлайну",
                ToolTipText = "Сортирует по дедлайну (сначала близкие)",
                Dock = DockStyle.Fill,
            };

            ToolStripButton sortbydate = new ToolStripButton()
            {
                Text = "По дате создания",
                ToolTipText = "Сортирует по дате создания (более недавние)",
                Dock = DockStyle.Fill,
            };

            filterbtn.DropDown.Items.AddRange(new ToolStripItem[] { sortbyid, sortbyname, sortbystatus, sortbytype, sortbypriority, sortbydeadline, sortbydate });

            ToolStripButton addbtn = new ToolStripButton()
            {
                Text = "Добавить задачу",
                ToolTipText = "Открывает окно создания задачи"
            };


            ToolStripDropDown dropDownDeaadline = new ToolStripDropDown();

            ToolStripDropDownButton chckdl = new ToolStripDropDownButton()
            {
                Text = "Дедлайны",
                ToolTipText = "Проверяет дедлайном задач",
                DropDown = dropDownDeaadline,
                DropDownDirection = ToolStripDropDownDirection.Default,
                Height = 100,
                Width = 100,
            };

            ToolStripButton chckdltoday = new ToolStripButton()
            {
                Text = "Сегодня",
                ToolTipText = "Проверяет наличие задач с дедлайном сегодня",
                Dock = DockStyle.Fill,
            };

            ToolStripButton chckdlexpired = new ToolStripButton()
            {
                Text = "Просроченные",
                ToolTipText = "Проверяет наличие задач с просроченым дедлайном",
                Dock = DockStyle.Fill,
            };

            ToolStripButton chckdltomorow = new ToolStripButton()
            {
                Text = "Завтра",
                ToolTipText = "Проверяет наличие задач с дедлайном завтра",
                Dock = DockStyle.Fill,
            };

            chckdl.DropDown.Items.AddRange(new ToolStripItem[] { chckdltoday, chckdlexpired, chckdltomorow });

            sortbypriority.Click += (s, e) => RefreshTaskList(SortKind.ByPriority);
            sortbyid.Click += (s, e) => RefreshTaskList(SortKind.ByID);
            sortbyname.Click += (s, e) => RefreshTaskList(SortKind.ByName);
            sortbystatus.Click += (s, e) => RefreshTaskList(SortKind.ByStatus);
            sortbytype.Click += (s, e) => RefreshTaskList(SortKind.ByType);
            sortbydeadline.Click += (s, e) => RefreshTaskList(SortKind.ByDeadline);
            sortbydate.Click += (s, e) => RefreshTaskList(SortKind.ByDate);


            addbtn.Click += (s, e) => AddTaskWindow();
            chckdltoday.Click += (s, e) => { deadlinetodaybuttonpressed = true; CheckDeadlineToday(); };
            chckdlexpired.Click += (s, e) => { deadlinexpiredbuttonpressed = true; CheckDeadlineExpired(); };
            chckdltomorow.Click += (s, e) => { deadlinetomorowdbuttonpressed = true; CheckDeadlineTomorow(); };
            panel.Controls.Add(toolStrip);
            toolStrip.Items.AddRange(new ToolStripItem[] { addbtn, filterbtn, chckdl});
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
                        RefreshTaskList(SortKind.ByID);
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
                        RefreshTaskList(SortKind.ByID);
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

            StatusUP.Click += (s, e) => NextStatus();
            PriorityUP.Click += (s, e) => NextPriority();

            contextMenu.Items.AddRange(new ToolStripItem[] { QuickItem, Edititem, DeleteItem, ShowDesc });
            return contextMenu;
        }

        private void RefreshTaskList(SortKind sort)
        {
            try
            {
                taskListView.BeginUpdate();
                taskListView.Items.Clear();

                var tasks = DBmanager.GetTasks();
                switch (sort)
                {
                    case SortKind.ByID:
                        tasks = DBmanager.GetTasks().ToList().OrderBy(p => p.ID);
                        break;
                    case SortKind.ByName:
                        tasks = DBmanager.GetTasks().ToList().OrderBy(p => p.TaskName);
                        break;
                    case SortKind.ByDeadline:
                        tasks = DBmanager.GetTasks().ToList().OrderBy(p => p.DeadLine);
                        break;
                    case SortKind.ByDate:
                        tasks = DBmanager.GetTasks().ToList().OrderBy(p => p.StartDate);
                        break;
                    case SortKind.ByPriority:
                        tasks = DBmanager.GetTasks().ToList().OrderByDescending(p => p.Priority);
                        break;
                    case SortKind.ByStatus:
                        tasks = DBmanager.GetTasks().ToList().OrderBy(p => p.StatusID);
                        break;
                    case SortKind.ByType:
                        tasks = DBmanager.GetTasks().ToList().OrderBy(p => p.TypeID);
                        break;
                }
                foreach (var task in tasks)
                {
                    try
                    {
                        item = new ListViewItem(task.ID.ToString());
                        item.SubItems.Add(task.TaskName);
                        string statusName = DBmanager.GetTaskStatusName(Convert.ToInt32(task.StatusID));
                        string typeName = DBmanager.GetTaskTypeName(Convert.ToInt32(task.TypeID));

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

                statusLabel.Text = $"Всего задач: {tasks.Count()} | Последнее обновление: {DateTime.Now:HH:mm:ss}";
                CheckDeadlineExpired();
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
                        RefreshTaskList(SortKind.ByID);
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
                    string statusName = DBmanager.GetTaskStatusName(Convert.ToInt32(selectedTask.StatusID));
                    string typeName = DBmanager.GetTaskTypeName(Convert.ToInt32(selectedTask.TypeID));

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
            else
            {
                MessageBox.Show("Выберите задачу.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void NextStatus()
        {
            Task selectedTask = GetSelectedTask();
            int statuscount = DBmanager.GetAllStatuses().Count();
            if (selectedTask != null)
            {
                int ID = selectedTask.ID;
                int statusID = Convert.ToInt32(selectedTask.StatusID);
                try
                {
                    if (statuscount > 0)
                    {
                        if (statusID < statuscount)
                        {
                            DBmanager.UpdateStatus(ID, statusID + 1);
                        }
                        else if (statusID >= statuscount)
                        {
                            DBmanager.UpdateStatus(ID, statusID - (statuscount - 1));
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            else 
            {
                MessageBox.Show("Выберите задачу для изменения.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            RefreshTaskList(SortKind.ByID);
        }
        
        private void NextPriority()
        {
            Task selectedTask = GetSelectedTask();
            if (selectedTask != null)
            {
                Priority priority = selectedTask.Priority;
                try
                {
                    switch (priority)
                    {
                        case Priority.Низкий:
                            DBmanager.UpdatePriority(selectedTask.ID, Priority.Средний);
                            break;
                        case Priority.Средний:
                            DBmanager.UpdatePriority(selectedTask.ID, Priority.Высокий);
                            break;
                        case Priority.Высокий:
                            DBmanager.UpdatePriority(selectedTask.ID, Priority.Низкий);
                            break;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            else
            {
                MessageBox.Show("Выберите задачу для изменения.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            RefreshTaskList(SortKind.ByID);
        }

        private void CheckDeadlineTomorow()
        {
            try
            {
                List<string> dla = new List<string>();
                for (int i = 0; i < taskListView.Items.Count; i++)
                {
                    DateTime deadline = Convert.ToDateTime(taskListView.Items[i].SubItems[6].Text);
                    if (deadline.Day == DateTime.Now.Day + 1)
                    {
                        dla.Add(taskListView.Items[i].SubItems[1].Text);
                    }
                }
                if (dla.Count > 0)
                {
                    string c = "";
                    if (dla.Count > 1)
                    {
                        c = "задач";
                    }
                    else if (dla.Count == 1)
                    {
                        c = "задачи";
                    }
                    string resualt = $"Завтра дедланй у {c} {string.Join("; ", dla)}.";
                    MessageBox.Show(resualt, "Дедлайны");
                }
                else if (dla.Count <= 0 && deadlinetomorowdbuttonpressed)
                {
                    MessageBox.Show("Дедлайнов завтра нет", "Дедлайны");
                    deadlinetomorowdbuttonpressed = false;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
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
                    string c = "";
                    if (dla.Count > 1)
                    {
                        c = "задач";
                    }
                    else if(dla.Count == 1)
                    {
                        c = "задачи";
                    }
                    string resualt = $"Сегодня дедланй у {c} {string.Join("; ", dla)}.";
                    MessageBox.Show(resualt, "Дедлайны");
                }
                else if (dla.Count <= 0 && deadlinetodaybuttonpressed)
                {
                    MessageBox.Show("Сегодня нет дедлайнов", "Дедлайны");
                    deadlinetodaybuttonpressed = false;
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
                if (dla.Count > 0 && !deadlinexpiredbuttonpressed)
                {
                    foreach (int item in dla)
                    {
                        taskListView.Items[item].BackColor = Color.Red;
                    }
                }
                else if (dla.Count > 0 && deadlinexpiredbuttonpressed)
                {
                    foreach (int item in dla)
                    {
                        taskListView.Items[item].BackColor = Color.Red;
                    }
                    MessageBox.Show("Задачи с просроченным дедлайном помечены", "Дедлайны");
                    deadlinexpiredbuttonpressed = false;
                }
                else if (dla.Count <= 0 && deadlinexpiredbuttonpressed)
                {
                    MessageBox.Show("Нету задач с просроченным дедлайном", "Дедлайны");
                    deadlinexpiredbuttonpressed = false;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}