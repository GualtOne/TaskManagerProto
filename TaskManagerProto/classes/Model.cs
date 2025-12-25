using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerProto
{
    public class Task_Status
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class Task_Type
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public enum Priority
    {
        Низкий,
        Средний,
        Высокий
    }

    public class Task
    {
        public int ID { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public int? TypeID { get; set; }
        public int? StatusID { get; set; }

        public Priority Priority { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? DeadLine { get; set; }

    }
}
