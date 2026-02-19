using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.IdentityModel.Tokens;

namespace TaskManagerProto
{
    public class XMLmanager
    {
        static string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        static string rootsring = "tasks";
        static string elementsring = "task";
        static string Datafolder = "TmData";
        static string DataStatus = "Status.xml";
        static string DataTypes = "Types.xml";

        static string sID = "ID";
        static string sName = "TaskName";
        static string sDesc = "TaskDescription";
        static string sType = "TypeID";
        static string sStatus = "StatusID";
        static string sPriority = "Priority";
        static string sStartDate = "StartDate";
        static string sDeadLine = "DeadLine";

        static string Troot = "types";
        static string Telement = "type";

        static string Sroot = "statuses";
        static string Selement = "status";
        static string sStatusname = "Name";
        static string sStatusID = "ID";

        static string pathdata = Path.Combine(docPath, Datafolder);
        static string statusfile = Path.Combine(pathdata, DataStatus);
        static string typefile = Path.Combine(pathdata, DataTypes);       
        
        static string folderName = "SavedTasks";
        static string filename = "Tasks.xml";
        static string pathString = Path.Combine(docPath, folderName);
        static private string pathFile = Path.Combine(pathString, filename);

        public static void CheckifDataexists() 
        {
            if (Directory.Exists(pathdata))
            {
                if (!File.Exists(statusfile))
                {
                    File.Create(statusfile);
                    Checkifemptystatus();

                }
                if (!File.Exists(typefile)) 
                {
                    File.Create(typefile);
                    Checkifemptytype();
                }
            }
            else
            {
                Directory.CreateDirectory(pathdata);
                File.Create(statusfile);
                Checkifemptystatus();
                File.Create(typefile);
                Checkifemptytype();
            }
            Checkifemptystatus();
            Checkifemptytype();
        }

        public static void Checkifexist()
        {
            if (Directory.Exists(pathString))
            {
                if (!File.Exists(pathFile))
                {
                    File.Create(pathFile);
                    Checkifempty();
                }
            }
            else 
            {
                Directory.CreateDirectory(pathString);
                File.Create(pathFile);
                Checkifempty();
            }
            Checkifempty();
        }

        public static void Checkifempty()
        {
            if (!IsXmlFileNotEmpty(pathFile))
            {
                CreateXml();
            }
        }

        public static void Checkifemptystatus()
        {
            if (!IsXmlFileNotEmpty(statusfile))
            {
                CreatestatusXml();
            }
        }

        public static void Checkifemptytype()
        {
            if (!IsXmlFileNotEmpty(typefile))
            {
                CreatetypeXml();
            }
        }

        public static bool IsXmlFileNotEmpty(string filePath)
        {
            if (!File.Exists(filePath) || new FileInfo(filePath).Length == 0)
            {
                return false;
            }
            try
            {
                var doc = new XDocument();
                doc = XDocument.Load(filePath);
                return doc.Root != null;
            }
            catch (XmlException)
            {
                return false;
            }
        }

        public static void CreatestatusXml()
        {
            try
            {
                XDocument statuses = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                        new XElement(Sroot,
                new XElement(Selement,
                    new XElement(sStatusID, 1),
                    new XElement(sStatusname, "Новая")
                ),
                new XElement(Selement,
                    new XElement(sStatusID, 2),
                    new XElement(sStatusname, "В процессе")
                ),
                new XElement(Selement,
                    new XElement(sStatusID, 3),
                    new XElement(sStatusname, "Готово")
                )
            ));
                statuses.Save(statusfile);
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void CreatetypeXml() 
        {
            try
            {
                XDocument types = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(Troot,
                new XElement(Telement,
                    new XElement(sStatusID, 1),
                    new XElement(sStatusname, "Работа")
                ),
                new XElement(Telement,
                    new XElement(sStatusID, 2),
                    new XElement(sStatusname, "Дом")
                ),
                new XElement(Telement,
                    new XElement(sStatusID, 3),
                    new XElement(sStatusname, "Личное")
                )
            ));
                types.Save(typefile);
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        public static void CreateXml() 
        {
            try
            {
                XDocument tasks = new XDocument(
                            new XDeclaration("1.0", "utf-8", "yes"),
                            new XElement("tasks"));
                tasks.Save(pathFile);
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        public static IEnumerable<Task> GetTasks()
        {
            XmlRootAttribute root = new XmlRootAttribute("tasks");
            XmlAttributes attrs = new XmlAttributes { XmlRoot = root };
            XmlAttributeOverrides overrides = new XmlAttributeOverrides();
            overrides.Add(typeof(List<Task>), attrs);

            XmlSerializer serializer = new XmlSerializer(typeof(List<Task>), overrides);
            List<Task> tasks = new List<Task>();
            try
            {

                if (File.Exists(pathFile) && new FileInfo(pathFile).Length > 0)
                {                
                    var doc = XDocument.Load(pathFile);
                    if (!(doc.Root == null) && doc.Root.HasElements)
                    {
                        using (var fs = new StreamReader(pathFile, Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
                        {
                            tasks = (List<Task>)serializer.Deserialize(fs);
                        }
                    }
                }
            }
            catch(XmlException xex) 
            {
                MessageBox.Show(xex.Message, "Ошибка");
            }
            catch (Exception ex) 
            {
                MessageBox.Show (ex.Message, "Ошибка");
            }

            return tasks;
        }


        public static void AddTask(string taskName, string taskDescription, int? statusID, int? typeID, DateTime startDate, DateTime? deadLine, Priority priority)
        {
            int lastid = 0;
            if(GetTasks().Count() > 0) 
            {
                lastid = GetTasks().Last().ID;
            }
            XDocument xdoc = XDocument.Load(pathFile);
            var root = xdoc.Element(rootsring);
            if (root != null)
            {
                root.Add(new XElement(elementsring,
                            new XElement(sID, lastid + 1),
                            new XElement(sName, taskName),
                            new XElement(sDesc, taskDescription),
                            new XElement(sType, typeID),
                            new XElement(sStatus, statusID),
                            new XElement(sPriority, priority),
                            new XElement(sStartDate, startDate),
                            new XElement(sDeadLine, deadLine)
                            ));

                xdoc.Save(pathFile);
            }
        }

        public static void DeleteTask(int ID)
        {
            XDocument xdoc = XDocument.Load(pathFile);
            var root = xdoc.Element(rootsring);
            if (root != null)
            {
                var bob = root.Elements(elementsring)
                    .FirstOrDefault(p => p.Element("ID")?.Value == ID.ToString());
                if (bob != null)
                {
                    bob.Remove();
                    xdoc.Save(pathFile);
                }
            }
        }

        public static bool UpdateTask(int ID, string taskName, string taskDescription, int? statusID, int? typeID, DateTime? deadLine, Priority priority)
        {
            try
            {
                if (!File.Exists(pathFile))
                {
                    MessageBox.Show("Файл задач не найден.");
                    return false;
                }

                XDocument xdoc = XDocument.Load(pathFile);
                var taskElement = xdoc.Root?
                                       .Elements(elementsring)
                                       .FirstOrDefault(p => (int?)p.Element("ID") == ID);

                if (taskElement == null)
                {
                    MessageBox.Show($"Задача с ID {ID} не найдена.");
                    return false;
                }

                var nameElem = taskElement.Element(sName);
                if (nameElem != null) nameElem.Value = taskName ?? "";

                var descElem = taskElement.Element(sDesc);
                if (descElem != null) descElem.Value = taskDescription ?? "";

                var statusElem = taskElement.Element(sStatus);
                if (statusElem != null)
                    statusElem.Value = statusID?.ToString() ?? "";

                var typeElem = taskElement.Element(sType);
                if (typeElem != null)
                    typeElem.Value = typeID?.ToString() ?? "";

                var deadlineElem = taskElement.Element(sDeadLine);
                if (deadlineElem != null)
                {
                    deadlineElem.Value = deadLine.HasValue
                        ? deadLine.Value.ToString("yyyy-MM-ddTHH:mm:ss")
                        : "";
                }

                var priorityElem = taskElement.Element(sPriority);
                if (priorityElem != null) 
                {
                    priorityElem.Value = priority.ToString();
                }

                xdoc.Save(pathFile);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении задачи: {ex.Message}");
                return false;
            }
        }

        public static string GetTaskTypeName(int ID)
        {
            try
            {
                XDocument xdoc = XDocument.Load(typefile);
                var typeElement = xdoc.Root?
                                      .Elements(Telement)
                                      .FirstOrDefault(p => (string)p.Element(sID) == ID.ToString());
                return typeElement?.Element(sStatusname)?.Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки XML: {ex.Message}");
                return null;
            }
        }

        public static string GetTaskStatusName(int ID)
        {
            try
            {
                XDocument xdoc = XDocument.Load(statusfile);
                var typeElement = xdoc.Root?
                                      .Elements(Selement)
                                      .FirstOrDefault(p => (string)p.Element(sID) == ID.ToString());
                return typeElement?.Element(sStatusname)?.Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки XML: {ex.Message}");
                return null;
            }
        }

        public static Task GetTaskById(int id)
        {
            if (!File.Exists(pathFile)) return null;

            XDocument xdoc = XDocument.Load(pathFile);
            XElement taskElement = xdoc.Root?
                                        .Elements("task")
                                        .FirstOrDefault(e => (int?)e.Element("ID") == id);

            if (taskElement == null) return null;

            XmlSerializer serializer = new XmlSerializer(typeof(Task));

            using (var reader = taskElement.CreateReader())
            {
                return (Task)serializer.Deserialize(reader);
            }
        }

        public static int GetStatusIdByName(string name)
        {
            try
            {
                XDocument xdoc = XDocument.Load(statusfile);
                var typeElement = xdoc.Root?
                                      .Elements(Selement)
                                      .FirstOrDefault(p => (string)p.Element(sStatusname) == name);
                return Convert.ToInt32(typeElement?.Element(sID)?.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки XML: {ex.Message}");
                return 0;
            }
        }

        public static int GetTypeIdByName(string name)
        {
            try
            {
                XDocument xdoc = XDocument.Load(typefile);
                var typeElement = xdoc.Root?
                                      .Elements(Telement)
                                      .FirstOrDefault(p => (string)p.Element(sStatusname) == name);
                return Convert.ToInt32(typeElement?.Element(sID)?.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки XML: {ex.Message}");
                return 0;
            }
        }

        public static IEnumerable<Task_Status> GetAllStatuses()
        {

            XmlRootAttribute root = new XmlRootAttribute(Sroot);
            XmlAttributes attrs = new XmlAttributes();
            attrs.XmlRoot = root;
            XmlAttributeOverrides overrides = new XmlAttributeOverrides();
            overrides.Add(typeof(List<Task_Status>), attrs);

            var serializer = new XmlSerializer(typeof(List<Task_Status>), overrides);
            var statuses = new List<Task_Status>();

            try
            {
                if (File.Exists(statusfile) && new FileInfo(statusfile).Length > 0)
                {
                    using (var reader = new StreamReader(statusfile, Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
                    {
                        statuses = (List<Task_Status>)serializer.Deserialize(reader);
                    }
                }
            }
            catch (XmlException xex)
            {
                MessageBox.Show($"Ошибка в XML (строка {xex.LineNumber}, позиция {xex.LinePosition}): {xex.Message}", "Ошибка");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }

            return statuses;
        }

        public static IEnumerable<Task_Type> GetAllTypes()
        {
            XmlRootAttribute root = new XmlRootAttribute(Troot);
            XmlAttributes attrs = new XmlAttributes();
            attrs.XmlRoot = root;
            XmlAttributeOverrides overrides = new XmlAttributeOverrides();
            overrides.Add(typeof(List<Task_Type>), attrs);

            var serializer = new XmlSerializer(typeof(List<Task_Type>), overrides);
            var types = new List<Task_Type>();

            try
            {
                if (File.Exists(typefile) && new FileInfo(typefile).Length > 0)
                {
                    using (var reader = new StreamReader(typefile, Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
                    {
                        types = (List<Task_Type>)serializer.Deserialize(reader);
                    }
                }
            }
            catch (XmlException xex)
            {
                MessageBox.Show($"Ошибка в XML (строка {xex.LineNumber}, позиция {xex.LinePosition}): {xex.Message}", "Ошибка");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }

            return types;
        }

        public static void UpdatePriority(int ID, Priority priority)
        {
            try
            {
                if (!File.Exists(pathFile))
                {
                    MessageBox.Show("Файл задач не найден.");
                }

                XDocument xdoc = XDocument.Load(pathFile);
                var taskElement = xdoc.Root?
                                       .Elements(elementsring)
                                       .FirstOrDefault(p => (int?)p.Element("ID") == ID);

                if (taskElement == null)
                {
                    MessageBox.Show($"Задача с ID {ID} не найдена.");
                }

                var priorityElem = taskElement.Element(sPriority);
                if (priorityElem != null)
                {
                    priorityElem.Value = priority.ToString();
                }

                xdoc.Save(pathFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении задачи: {ex.Message}");
            }
        }

        public static void UpdateStatus(int ID, int? statusID)
        {
            try
            {
                if (!File.Exists(pathFile))
                {
                    MessageBox.Show("Файл задач не найден.");
                }

                XDocument xdoc = XDocument.Load(pathFile);
                var taskElement = xdoc.Root?
                                       .Elements(elementsring)
                                       .FirstOrDefault(p => (int?)p.Element("ID") == ID);

                if (taskElement == null)
                {
                    MessageBox.Show($"Задача с ID {ID} не найдена.");
                }

                var statusElem = taskElement.Element(sStatus);
                if (statusElem != null)
                    statusElem.Value = statusID?.ToString() ?? "";

                xdoc.Save(pathFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении задачи: {ex.Message}");
            }
        }
    }
}
