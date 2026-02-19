using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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
        private ToolStripTextBox SearchBox;

        
        public enum SortKind
        {
            ByID,
            ByName,
            ByStatus,
            ByType,
            ByPriority,
            ByDate,
            ByDeadline,
        }

        bool deadlinetodaybuttonpressed = false;
        bool deadlinexpiredbuttonpressed = false;
        bool deadlinetomorowdbuttonpressed = false;
        bool startrefresh = true;
        bool formloaded = false;
        private int sortColumn = -1;

        public Form1()
        {
            InitializeComponent();
            InitializeTaskListView();
            this.Text = "Менеджер задач";
            this.Size = new Size(935, 600);
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
            formloaded = true;
        }

        private void InitializeToolstrip()
        {
            toolStrip = new ToolStrip
            {
                Dock = DockStyle.Top,
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
                ToolTipText = "Сортирует по проритету (Высокий, Средний, низкий)",
                Dock = DockStyle.Fill,
            };

            ToolStripButton sortbystatus = new ToolStripButton()
            {
                Text = "По статусу",
                ToolTipText = "Сортирует по статусу (Новая, В процессе, Готова)",
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
                ToolTipText = "Открывает окно создания задачи",
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

            ToolStripButton chckdltodayonly = new ToolStripButton()
            {
                Text = "Только Сегодня",
                ToolTipText = "Выводит только задачи с дедлайном сегодня",
                Dock = DockStyle.Fill,
            };

            ToolStripButton chckdlexpired = new ToolStripButton()
            {
                Text = "Просроченные",
                ToolTipText = "Проверяет наличие задач с просроченым дедлайном",
                Dock = DockStyle.Fill,
            };

            ToolStripButton chckdlexpronly = new ToolStripButton()
            {
                Text = "Только просроченные",
                ToolTipText = "Выводит только задачи с просроченным дедлайном ",
                Dock = DockStyle.Fill,
            };

            ToolStripButton chckdltomorow = new ToolStripButton()
            {
                Text = "Завтра",
                ToolTipText = "Проверяет наличие задач с дедлайном завтра",
                Dock = DockStyle.Fill,
            };

            ToolStripButton chckdltomorowonly = new ToolStripButton()
            {
                Text = "Только завтра",
                ToolTipText = "Выводит только задачи с дедлайном завтра",
                Dock = DockStyle.Fill,
            };

            ToolStripButton refreshlist = new ToolStripButton()
            {
                Text = "Обновить лист",
                ToolTipText = "Покажет лист как при включение программы"
            };

            ToolStripButton tip = new ToolStripButton()
            {
                Text = "Подсказка",
                ToolTipText = "Подсказака на счёт интрефейса"
            };

            ToolStripDropDown searchdropdown = new ToolStripDropDown();

            ToolStripLabel seacrhtype = new ToolStripLabel()
            {
                Text = "Поиск: ",
                ToolTipText = "поиск",
            };

            ToolStripButton testbutton = new ToolStripButton()
            {
                Text = "test"
            };
            testbutton.Click += (s, e) => TestFunction();

            SearchBox = new ToolStripTextBox()
            {
                BorderStyle = BorderStyle.FixedSingle,
                Size = new Size(257, 50),
                ToolTipText = "Поиск",
            };

            SearchBox.TextChanged += new EventHandler(searchBox_TextChanged);

            chckdl.DropDown.Items.AddRange(new ToolStripItem[] { chckdltoday, chckdltodayonly, chckdlexpired, chckdlexpronly, chckdltomorow, chckdltomorowonly });

            sortbypriority.Click += (s, e) => Sort(SortKind.ByPriority);
            sortbyid.Click += (s, e) => Sort(SortKind.ByID);
            sortbyname.Click += (s, e) => Sort(SortKind.ByName);
            sortbystatus.Click += (s, e) => Sort(SortKind.ByStatus);
            sortbytype.Click += (s, e) => Sort(SortKind.ByType);
            sortbydeadline.Click += (s, e) => Sort(SortKind.ByDeadline);
            sortbydate.Click += (s, e) => Sort(SortKind.ByDate);


            addbtn.Click += (s, e) => AddTaskWindow();
            refreshlist.Click += (s, e) => { startrefresh = true;  RefreshTaskList(); };
            tip.Click += (s, e) => ShowTip();
            chckdltoday.Click += (s, e) => { deadlinetodaybuttonpressed = true; CheckDeadlineToday(); };
            chckdlexpired.Click += (s, e) => { deadlinexpiredbuttonpressed = true; CheckDeadlineExpired(); };
            chckdltomorow.Click += (s, e) => { deadlinetomorowdbuttonpressed = true; CheckDeadlineTomorow(); };
            chckdltodayonly.Click += (s, e) => { deadlinetodaybuttonpressed = true; ShowDeadlineToday(); };
            chckdlexpronly.Click += (s, e) => { deadlinexpiredbuttonpressed = true; ShowDeadlineExpired(); };
            chckdltomorowonly.Click += (s, e) => { deadlinetomorowdbuttonpressed = true; ShowDeadlineTomorod(); };
            panel.Controls.Add(toolStrip);
            toolStrip.Items.AddRange(new ToolStripItem[] { addbtn, filterbtn, chckdl, refreshlist, tip, seacrhtype, SearchBox, /*testbutton*/});
        }

        private void ShowTip()
        {
            MessageBox.Show($"Дедлайны:\n Красный - просроченная, " +
                "Оранжевая - сегодня, " +
                "Желтый - завтра", "Подсказка");
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

                        XMLmanager.AddTask(
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
                        XMLmanager.UpdateTask(
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
            taskListView.Columns.Add("Дедлайн", 190);

            taskListView.ColumnClick += new ColumnClickEventHandler(ColumnClick);
            taskListView.ContextMenuStrip = CreateContextMenu();
        }

        private void ColumnClick(object o, ColumnClickEventArgs e)
        {
            if (e.Column != sortColumn)
            {
                sortColumn = e.Column;
                taskListView.Sorting = SortOrder.Ascending;
            }
            else 
            {
                if (taskListView.Sorting == SortOrder.Ascending) 
                    taskListView.Sorting = SortOrder.Descending;
                else
                    taskListView.Sorting = SortOrder.Ascending;
            }
            taskListView.ListViewItemSorter = new ListViewItemComparer(e.Column, taskListView.Sorting);
        }

        class ListViewItemComparer : IComparer
        {
            private int col;
            private SortOrder sortOrder;
            public ListViewItemComparer()
            {
                col = 0;
                sortOrder = SortOrder.Ascending;
            }
            public ListViewItemComparer(int column, SortOrder sortorder)
            {
                col = column;
                this.sortOrder = sortorder;
            }
            public int Compare(object x, object y)
            {
                int returnval = 0;
                int numFirst, numSecond;
                switch (col) 
                {
                    case 0:
                        numFirst = Convert.ToInt32(((ListViewItem)x).SubItems[col].Text);
                        numSecond = Convert.ToInt32(((ListViewItem)y).SubItems[col].Text);
                        if (numFirst < numSecond)
                            returnval = -1;
                        else if (numFirst > numSecond) 
                            returnval = 1;
                        else
                            returnval = 0;
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        returnval = String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
                        break;
                    case 5:
                    case 6:
                        try
                        {
                            System.DateTime firstDate = 
                                DateTime.Parse(((ListViewItem)x).SubItems[col].Text);
                            System.DateTime secondtDate = 
                                DateTime.Parse(((ListViewItem)y).SubItems[col].Text);
                            returnval = DateTime.Compare(firstDate, secondtDate);
                        }
                        catch
                        {
                            returnval = String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
                        }
                        break;
                }

                if (sortOrder == SortOrder.Descending)
                    returnval *= -1;

                return returnval;
            }
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

        private void ShowDeadlineExpired()
        {
            try
            {
                int count = 0;
                taskListView.BeginUpdate();
                taskListView.Items.Clear();

                var tasks = XMLmanager.GetTasks();
                foreach (var task in tasks)
                {
                    DateTime? deadline = task.DeadLine;
                    if (deadline.HasValue && deadline.Value.Date < DateTime.Now.Date && deadline.Value.Date != DateTime.Now.Date)
                    {
                        try
                        {
                            ListViewItem item = new ListViewItem(task.ID.ToString());

                            item.SubItems.Add(task.TaskName);

                            string statusName = XMLmanager.GetTaskStatusName(Convert.ToInt32(task.StatusID));
                            string typeName = XMLmanager.GetTaskTypeName(Convert.ToInt32(task.TypeID));

                            item.SubItems.Add(string.IsNullOrEmpty(statusName) ? "Не указан" : statusName);
                            item.SubItems.Add(string.IsNullOrEmpty(typeName) ? "Не указан" : typeName);
                            item.SubItems.Add(task.Priority.ToString());

                            string deadlineStr = deadline.HasValue
                                ? deadline.Value.ToString("dd.MM.yyyy HH:mm")
                                : "Нет";

                            item.SubItems.Add(task.StartDate.ToString("dd.MM.yyyy HH:mm"));
                            item.SubItems.Add(deadlineStr);

                            item.Tag = task;
                            taskListView.Items.Add(item);
                            count++;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Ошибка при добавлении задачи в список: {ex.Message}");
                        }
                    }
                }

                statusLabel.Text = $"Всего задач: {count} | Последнее обновление: {DateTime.Now:HH:mm:ss} | Просроченные";
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

        private void ShowDeadlineTomorod()
        {
            try
            {
                int count = 0;
                taskListView.BeginUpdate();
                taskListView.Items.Clear();

                IEnumerable<Task> tasks = XMLmanager.GetTasks();
                foreach (var task in tasks)
                {
                    DateTime? deadline = task.DeadLine;
                    if (deadline.Value.Day == DateTime.Now.Day + 1 && deadline.Value.Year == DateTime.Now.Year && deadline.Value.Month == DateTime.Now.Month)
                    {
                        try
                        {
                            ListViewItem item = new ListViewItem(task.ID.ToString());

                            item.SubItems.Add(task.TaskName);

                            string statusName = XMLmanager.GetTaskStatusName(Convert.ToInt32(task.StatusID));
                            string typeName = XMLmanager.GetTaskTypeName(Convert.ToInt32(task.TypeID));

                            item.SubItems.Add(string.IsNullOrEmpty(statusName) ? "Не указан" : statusName);
                            item.SubItems.Add(string.IsNullOrEmpty(typeName) ? "Не указан" : typeName);
                            item.SubItems.Add(task.Priority.ToString());

                            string deadlineStr = deadline.HasValue
                                ? deadline.Value.ToString("dd.MM.yyyy HH:mm")
                                : "Нет";

                            item.SubItems.Add(task.StartDate.ToString("dd.MM.yyyy HH:mm"));
                            item.SubItems.Add(deadlineStr);

                            item.Tag = task;
                            taskListView.Items.Add(item);
                            count++;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Ошибка при добавлении задачи в список: {ex.Message}");
                        }
                    }
                }

                statusLabel.Text = $"Всего задач: {count} | Последнее обновление: {DateTime.Now:HH:mm:ss} | Завтра";
                CheckDeadlineTomorow();
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


        private void ShowDeadlineToday()
        {
            try
            {
                int count = 0;
                taskListView.BeginUpdate();
                taskListView.Items.Clear();

                var tasks = XMLmanager.GetTasks();
                foreach (var task in tasks)
                {
                    DateTime? deadline = task.DeadLine;
                    if (deadline.HasValue && deadline.Value.Day == DateTime.Now.Day && deadline.Value.Year == DateTime.Now.Year && deadline.Value.Month == DateTime.Now.Month)
                    {
                        try
                        {
                            ListViewItem item = new ListViewItem(task.ID.ToString());

                            item.SubItems.Add(task.TaskName);

                            string statusName = XMLmanager.GetTaskStatusName(Convert.ToInt32(task.StatusID));
                            string typeName = XMLmanager.GetTaskTypeName(Convert.ToInt32(task.TypeID));

                            item.SubItems.Add(string.IsNullOrEmpty(statusName) ? "Не указан" : statusName);
                            item.SubItems.Add(string.IsNullOrEmpty(typeName) ? "Не указан" : typeName);
                            item.SubItems.Add(task.Priority.ToString());

                            string deadlineStr = deadline.HasValue
                                ? deadline.Value.ToString("dd.MM.yyyy HH:mm")
                                : "Нет";

                            item.SubItems.Add(task.StartDate.ToString("dd.MM.yyyy HH:mm"));
                            item.SubItems.Add(deadlineStr);

                            item.Tag = task;
                            taskListView.Items.Add(item);
                            count++;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Ошибка при добавлении задачи в список: {ex.Message}");
                        }
                    }
                }

                statusLabel.Text = $"Всего задач: {count} | Последнее обновление: {DateTime.Now:HH:mm:ss} | Сегодня";
                CheckDeadlineToday();
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


        private void Sort(SortKind sort)
        {
            if (taskListView.Sorting == SortOrder.Ascending)
                taskListView.Sorting = SortOrder.Descending;
            else
                taskListView.Sorting = SortOrder.Ascending;

            int sortype = Convert.ToInt32(sort);
            taskListView.ListViewItemSorter = new ListViewItemComparer(sortype, taskListView.Sorting);
        }

        private void RefreshTaskList()
        {
            try
            {
                taskListView.BeginUpdate();
                taskListView.Items.Clear();

                if (startrefresh)
                {
                    taskListView.Sorting = SortOrder.Descending;
                    Sort(SortKind.ByID);
                }

                var tasks = XMLmanager.GetTasks();
                foreach (var task in tasks)
                {
                    try
                    {
                        item = new ListViewItem(task.ID.ToString());
                        item.SubItems.Add(task.TaskName);
                        string statusName = XMLmanager.GetTaskStatusName(Convert.ToInt32(task.StatusID));
                        string typeName = XMLmanager.GetTaskTypeName(Convert.ToInt32(task.TypeID));

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
                statusLabel.Text = $"Всего задач: {tasks.Count()} | Последнее обновление: {DateTime.Now:HH:mm:ss} | Обычный";
                CheckDeadlineExpired();
                CheckDeadlineToday();
                CheckDeadlineTomorow();
                startrefresh = false;
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
                        XMLmanager.DeleteTask(selectedTask.ID);
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
                    string statusName = XMLmanager.GetTaskStatusName(Convert.ToInt32(selectedTask.StatusID));
                    string typeName = XMLmanager.GetTaskTypeName(Convert.ToInt32(selectedTask.TypeID));

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
            int statuscount = XMLmanager.GetAllStatuses().Count();
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
                            XMLmanager.UpdateStatus(ID, statusID + 1);
                        }
                        else if (statusID >= statuscount)
                        {
                            XMLmanager.UpdateStatus(ID, statusID - (statuscount - 1));
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
            RefreshTaskList();
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
                            XMLmanager.UpdatePriority(selectedTask.ID, Priority.Средний);
                            break;
                        case Priority.Средний:
                            XMLmanager.UpdatePriority(selectedTask.ID, Priority.Высокий);
                            break;
                        case Priority.Высокий:
                            XMLmanager.UpdatePriority(selectedTask.ID, Priority.Низкий);
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
            RefreshTaskList();
        }

        private void CheckDeadlineTomorow()
        {
            try
            {
                List<string> dla = new List<string>();
                List<int> dlaa = new List<int>();
                for (int i = 0; i < taskListView.Items.Count; i++)
                {
                    DateTime deadline = Convert.ToDateTime(taskListView.Items[i].SubItems[6].Text);
                    if (deadline.Day == DateTime.Now.Day + 1 && deadline.Year == DateTime.Now.Year && deadline.Month == DateTime.Now.Month)
                    {
                        dla.Add(taskListView.Items[i].SubItems[1].Text);
                        dlaa.Add(i);
                    }
                }
                if (dla.Count > 0 && !formloaded)
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
                if (dla.Count > 0 && !deadlinetomorowdbuttonpressed)
                {
                    foreach (int item in dlaa)
                    {
                        taskListView.Items[item].BackColor = Color.Yellow;
                    }
                }
                else if (dla.Count > 0 && deadlinetomorowdbuttonpressed)
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
                    foreach (int item in dlaa)
                    {
                        taskListView.Items[item].BackColor = Color.Yellow;
                    }
                    MessageBox.Show("Задачи с дедлайном завтра, помечены", "Дедлайны");
                    deadlinetomorowdbuttonpressed = false;
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
                List<int> dlaa = new List<int>();
                for (int i = 0; i < taskListView.Items.Count; i++)
                {
                    DateTime deadline = Convert.ToDateTime(taskListView.Items[i].SubItems[6].Text);
                    if (deadline.Day == DateTime.Now.Day && deadline.Year == DateTime.Now.Year && deadline.Month == DateTime.Now.Month)
                    {
                        dla.Add(taskListView.Items[i].SubItems[1].Text);
                        dlaa.Add(i);
                    }
                }
                if (dla.Count > 0 && !formloaded)
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
                    string resualt = $"Сегодня дедланй у {c} {string.Join("; ", dla)}.";
                    MessageBox.Show(resualt, "Дедлайны");
                }
                if (dla.Count > 0 && !deadlinetodaybuttonpressed)
                {
                    foreach (int item in dlaa)
                    {
                        taskListView.Items[item].BackColor = Color.Orange;
                    }
                }
                else if (dla.Count > 0 && deadlinetodaybuttonpressed)
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
                    string resualt = $"Сегодня дедланй у {c} {string.Join("; ", dla)}.";
                    MessageBox.Show(resualt, "Дедлайны");
                    foreach (int item in dlaa)
                    {
                        taskListView.Items[item].BackColor = Color.Orange;
                    }
                    MessageBox.Show("Задачи с дедлайном сегодня, помечены", "Дедлайны");
                    deadlinetodaybuttonpressed = false;
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

        void TestFunction() 
        {
            //MessageBox.Show();
        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            if (SearchBox.Text.Length > 0)
            {
                try 
                {
                    var allTasks = XMLmanager.GetTasks();
                    List<ListViewItem> foundItems = new List<ListViewItem>();


                    taskListView.BeginUpdate();
                    taskListView.Items.Clear();
                    foreach (var task in allTasks)
                    {
                        if (task.TaskName.IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            XMLmanager.GetTaskStatusName(Convert.ToInt32(task.StatusID)).IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            XMLmanager.GetTaskTypeName(Convert.ToInt32(task.TypeID)).IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            task.Priority.ToString().IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            task.StartDate.ToString().IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            task.DeadLine.ToString().IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            ListViewItem item = new ListViewItem(task.ID.ToString());
                            item.SubItems.Add(task.TaskName);

                            string statusName = XMLmanager.GetTaskStatusName(Convert.ToInt32(task.StatusID));
                            string typeName = XMLmanager.GetTaskTypeName(Convert.ToInt32(task.TypeID));

                            item.SubItems.Add(string.IsNullOrEmpty(statusName) ? "Не указан" : statusName);
                            item.SubItems.Add(string.IsNullOrEmpty(typeName) ? "Не указан" : typeName);
                            item.SubItems.Add(task.Priority.ToString());
                            item.SubItems.Add(task.StartDate.ToString("dd.MM.yyyy HH:mm"));
                            item.SubItems.Add(task.DeadLine?.ToString("dd.MM.yyyy HH:mm") ?? "Нет");
                            item.Tag = task;

                            foundItems.Add(item);
                        }
                    }

                    if (foundItems.Count > 0)
                    {
                        taskListView.Items.AddRange(foundItems.ToArray());
                        if (taskListView.Items.Count > 0)
                        {
                            taskListView.TopItem = taskListView.Items[0];
                            taskListView.Items[0].Selected = true;
                        }
                    }
                    taskListView.EndUpdate();

                    statusLabel.Text = $"Найдено задач: {foundItems.Count} | Поиск: '{SearchBox.Text}'";

                    CheckDeadlineExpired();
                    CheckDeadlineToday();
                    CheckDeadlineTomorow();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка");
                }
            }
            else 
            {
                RefreshTaskList();
            }
        }
    }
}