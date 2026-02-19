using System;
using System.Xml.Serialization;

namespace TaskManagerProto
{
    [Serializable]
    [XmlType("status")]
    public class Task_Status
    {
        [XmlElement("ID")]
        public int ID { get; set; }
        [XmlElement("Name")]
        public string Name { get; set; }
    }

    [Serializable]
    [XmlType("type")]
    public class Task_Type
    {
        [XmlElement("ID")]
        public int ID { get; set; }
        [XmlElement("Name")]
        public string Name { get; set; }
    }

    public enum Priority
    {
        Низкий,
        Средний,
        Высокий
    }

    [Serializable]
    [XmlType("task")]
    public class Task
    {
        [XmlElement("ID")]
        public int ID { get; set; } = 0;
        [XmlElement("TaskName")]
        public string TaskName { get; set; }
        [XmlElement("TaskDescription")]
        public string TaskDescription { get; set; }
        [XmlElement("TypeID")]
        public int? TypeID { get; set; }
        [XmlElement("StatusID")]
        public int? StatusID { get; set; }
        [XmlElement("Priority")]
        public Priority Priority { get; set; }
        [XmlElement("StartDate")]
        public DateTime StartDate { get; set; }
        [XmlElement("DeadLine")]
        public DateTime? DeadLine { get; set; }

    }
}
