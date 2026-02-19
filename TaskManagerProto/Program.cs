using System;
using System.Windows.Forms;

namespace TaskManagerProto
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            XMLmanager.Checkifexist();
            XMLmanager.CheckifDataexists();
            Application.Run(new Form1());
        }
    }
}
