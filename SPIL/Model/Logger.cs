using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPIL.model
{
    public class Logger
    {
        private string path;
        private List<string> logList = new List<string>();

        public Action<string> LogRecord;

        /// <summary>
        /// 在我的文件夾內 創建Log資料夾
        /// </summary>
        /// <param name="folderName"></param>
        public Logger(string folderName)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //我的文件夾
            path = $"{documents}\\{folderName}\\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }


        public void WriteLog(string logMessage , LogType type = LogType.Process)
        {
            
            try {
                //LOG 新增時間  單位到毫秒
                DateTime dateTime = DateTime.Now;
                string str = $"{dateTime.ToString("MM-dd-HH:mm:ss")} :{  dateTime.Millisecond}   {logMessage} ";

                var date = dateTime.ToString("yyyyMMdd");
                LogRecord?.Invoke(str);
                logList.Add(str);
                File.AppendAllText($"{path}\\LOG-{date}.txt", str+"\r\n");

                //如果行數太多 就備份檔案 名稱是 年月日+時
                if (logList.Count > 200000) {
                    var dateH = dateTime.ToString("yyyyMMdd");
                    File.WriteAllLines($"{path}\\LOG-{dateH}-BACKUP.txt", logList);
                    logList.Clear();
                    File.WriteAllText($"{path}\\LOG-{date}.txt", ""); //覆寫原有檔案
                }

            }
            catch (Exception ex) {
                File.AppendAllText($"{path}\\Log.txt", $"LOG紀錄錯誤 {ex.Message}");
                throw ex;
            }
        }

        public void WriteErrorLog(string logMessage)
        {

            try {
                //LOG 新增時間  單位到毫秒
                DateTime dateTime = DateTime.Now;
                string str = $"{dateTime.ToString("yyyy-MM-dd-HH")} :{  dateTime.Millisecond}   {logMessage} \r\n";
          
                var date = dateTime.ToString("yyyy-MM-dd");

                logList.Add(str);
                File.AppendAllText($"{path}\\Error{date}.txt", str);
                 
            }
            catch (Exception ex) {
                File.AppendAllText($"{path}\\Log.txt", $"LOG紀錄錯誤 {ex.Message}");
                throw ex;
            }

        }
 
    }

    public enum LogType
    {
        Process,
        Warning,
        Error,
        Trace

    }
}
