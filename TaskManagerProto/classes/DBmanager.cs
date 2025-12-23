using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dapper;

namespace TaskManagerProto
{
    public class DBmanager
    {
        static string connectionString = "";
        static string dbName = "TODOTaskManager";
        static string serverName = "localhost\\SQLEXPRESS";
        public static void Connection()
        {
            try
            {;
                string NewconnectionString = $"Server={serverName};Integrated Security=True;";

                if (DatabaseExists(serverName, dbName, NewconnectionString))
                {
                    connectionString = $"Server={serverName};Database={dbName};Trusted_Connection=True;";
                }
                else
                {
                    MessageBox.Show($"База данных '{dbName}' НЕ найдена. Создаю...");
                    connectionString = $"Server={serverName};Integrated Security=True;";
                    CreateFullyDataBase();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка при при подсоединение к SQL серверу {ex.Message}");
            }
        }
        public static void CreateFullyDataBase()
        {
            CreateDatabse();
            CreateTables();
            AlterTables();
            InsertIntoTables();
            connectionString = $"Server={serverName};Database={dbName};Trusted_Connection=True;";
            Connection();
        }
        public static void CreateDatabse()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = $"CREATE DATABASE {dbName};";
                    connection.Execute(query);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка при создание базы данных {ex.Message}");
            }
        }

        public static void CreateTables() 
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = $"USE {dbName} " +
                        $"CREATE TABLE Task( " +
                        $"ID int PRIMARY KEY IDENTITY (1,1)," +
                        $"TaskName NVARCHAR(100) NOT NULL," +
                        $"TaskDescription NVARCHAR(500) NOT NULL," +
                        $"StatusID INT," +
                        $"TypeID INT," +
                        $"StartDate DateTime NOT NULL," +
                        $"Deadline DateTime NULL," +
                        $"Priority INT NOT NULL); " +
                        $"CREATE TABLE Task_Status(" +
                        $"ID INT PRIMARY KEY IDENTITY (1,1)," +
                        $"Name NVARCHAR(20) NOT NULL,); " +
                        $"CREATE TABLE Task_Type(" +
                        $"ID INT PRIMARY KEY IDENTITY (1,1)," +
                        $"Name NVARCHAR(20) NOT NULL,); ";
                    connection.Execute(query);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка при создание таблиц {ex.Message}");
            }

        }

        public static void AlterTables()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = $"USE {dbName} " +
                        "ALTER TABLE Task " +
                        "ADD FOREIGN KEY (StatusID) REFERENCES Task_Status(ID) " +
                        "ON DELETE CASCADE " +
                        "ON UPDATE SET NULL; " +
                        "ALTER TABLE Task " +
                        "ADD FOREIGN KEY (TypeID) REFERENCES Task_Type(ID) " +
                        "ON DELETE CASCADE" +
                        "ON UPDATE SET NULL;";
                    connection.Execute(query);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка при изменеине таблиц базы данных {ex.Message}");
            }
        }

        public static void InsertIntoTables()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = $"USE {dbName} " +
                    "INSERT INTO Task_Status (Name) " +
                    "VALUES ('Новая'),('В процессе'),('Готово'); " +
                    "INSERT INTO Task_Type (Name) " +
                    "VALUES ('Работа'),('Дом'),('Личное'); ";
                connection.Execute(query);
            }
        }

        public static bool DatabaseExists(string server, string dbName, string masterConnectionString)
        {
            string query = $"SELECT db_id('{dbName}') AS DatabaseID;";

            using (SqlConnection connection = new SqlConnection(masterConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != DBNull.Value)
                        {
                            int dbId = Convert.ToInt32(result);
                            return dbId > 0;
                        }
                        return false;
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show($"Ошибка при проверке базы данных: {ex.Message}");
                        return false;
                    }
                }
            }
        }

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
                connection.Execute(query, new 
                { 
                    TaskName = taskName, 
                    TaskDescription = taskDescription,
                    StatusID = statusID, 
                    TypeID = typeID,
                    StartDate = startDate, 
                    DeadLine = deadLine, 
                    Priority = priority });
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

        public static void UpdatePriority(int ID, Priority priority)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Task SET Priority = @Priority WHERE ID = @ID";
                connection.Execute(query, new
                {
                    Priority = priority,
                    ID = ID
                });
            }
        }


        public static void UpdateStatus(int ID, int statusID)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Task SET StatusID = @StatusID WHERE ID = @ID";
                connection.Execute(query, new
                {
                    StatusID = statusID,
                    ID = ID
                });
            }
        }

    }
}
