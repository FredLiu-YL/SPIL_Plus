using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPIL
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 注册未处理异常事件处理程序
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

            Application.Run(new Form1());



        }

        // 未处理异常事件处理程序
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            string systemPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\SPIL";

          


            // 在这里处理未处理的异常，可以显示错误信息、记录日志等
            MessageBox.Show($"An unhandled exception occurred: {e.Exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            
            DateTime dateTime = DateTime.Now;     
            var date = dateTime.ToString("yyyyMMdd");
            string str = $"{dateTime.ToString("MM-dd-HH:mm:ss")} : { e.Exception} ";
            File.AppendAllText($"{systemPath}\\UnhandledExceptionLOG.txt", str); //覆寫原有檔案

        }

    }
}
