using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace TaskManagerProto
{
    public class DBmanager
    {
        static string connectionString = "Server=DESKTOP-E34B530\\SQLEXPRESS;Database=TODOTaskManager;Trusted_Connection=True;";

        public static IEnumerable<Task> GetTasks() 
        {
            using (var connection = new SqlConnection(connectionString))
            {
                return connection.Query<Task>("SELECT * FROM Task");
            }
        }

        public static void DeleteTask(int ID)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Task WHERE ID = @ID";
                connection.Execute(query, new { ID = ID });
            }
        }

        public static void AddTask(string taskName, string taskDescription, int statusID, int typeID, DateTime startDate, DateTime? deadLine, Priority priority)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Task (TaskName, TaskDescription, StatusID, TypeID, StartDate, DeadLine , Priority) " +
                    "VALUES (@TaskName, @TaskDescription, @StatusID, @TypeID, @StartDate, @DeadLine , @Priority)";
                connection.Execute(query, new { TaskName = taskName, TaskDescription = taskDescription, StatusID = statusID, TypeID = typeID, 
                    StartDate = startDate, DeadLine = deadLine, Priority = priority });
            }
        }

        public static void UpdateTask(int ID, string taskName, string taskDescription, int statusID, int typeID, DateTime? deadLine, Priority priority)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Task SET TaskName = @TaskName, TaskDescription = @TaskDescription, " +
                    "StatusID = @StatusID, TypeID = @TypeID, DeadLine = @DeadLine, Priority = @Priority " +
                    "WHERE ID = @ID";
                connection.Execute(query, new
                {
                    TaskName = taskName,
                    TaskDescription = taskDescription,
                    StatusID = statusID,
                    TypeID = typeID,
                    DeadLine = deadLine,
                    Priority = priority,
                    ID = ID
                });
            }
        }


        public static string GetTaskStatusName(int ID)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Name FROM Task_Status WHERE ID = @ID";
                return connection.QueryFirstOrDefault<string>(query, new { ID = ID });
            }
        }

        public static string GetTaskTypeName(int ID)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Name FROM Task_Type WHERE ID = @ID";
                return connection.QueryFirstOrDefault<string>(query, new { ID = ID });
            }
        }

        public static Task GetTaskById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Task WHERE ID = @ID";
                return connection.QueryFirstOrDefault<Task>(query, new { ID = id });
            }
        }

        public static int GetStatusIdByName(string name)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT ID FROM Task_Status WHERE Name = @Name";
                var result = connection.QueryFirstOrDefault<int?>(query, new { Name = name });
                return result ?? 0;
            }
        }

        public static int GetTypeIdByName(string name)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT ID FROM Task_Type WHERE Name = @Name";
                var result = connection.QueryFirstOrDefault<int?>(query, new { Name = name });
                return result ?? 0;
            }
        }

        public static IEnumerable<Task_Status> GetAllStatuses()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                return connection.Query<Task_Status>("SELECT * FROM Task_Status");
            }
        }

        public static IEnumerable<Task_Type> GetAllTypes()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                return connection.Query<Task_Type>("SELECT * FROM Task_Type");
            }
        }


    }
}
