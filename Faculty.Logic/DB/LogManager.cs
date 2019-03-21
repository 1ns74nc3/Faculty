using Faculty.Logic.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Faculty.Logic.DB
{
    public class LogManager
    {
        public void AddEventLog(string message, string infoType)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Log log = new Log(message, infoType);
                db.Logs.Add(log);
                db.SaveChanges();
            }
        }

        public void AddExcaptionLog(string message)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                string infoType = "Exception";
                Log log = new Log(message, infoType);
                db.Logs.Add(log);
                db.SaveChanges();
                GetLogFile();
            }
        }

        public void GetLogFile()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var logs = db.Logs.ToList();
                string path = @"D:\FacultyLogs.txt";
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (FileStream fs = File.Create(path))
                {
                    foreach (var item in logs)
                    {
                        string logData = string.Format("{0}: {1}, Time: {2} "+ Environment.NewLine, 
                            item.InfoType, item.Message, item.Time.ToShortTimeString());
                        byte[] info = new UTF8Encoding(true).GetBytes(logData);
                        fs.Write(info, 0, info.Length);
                    }
                }
            }
        }

        public void RemoveAllLogs()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var logs = db.Logs.ToList();
                if (logs.Any())
                {
                    foreach(var item in logs)
                    {
                        db.Logs.Remove(item);
                    }
                }
                db.SaveChanges();              
            }
        }
    }
}
