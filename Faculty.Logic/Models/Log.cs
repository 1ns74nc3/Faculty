using System;
using System.Collections.Generic;
using System.Text;

namespace Faculty.Logic.Models
{
    public class Log : ModelsBase
    {
        public string Message { get; set; }
        public string InfoType { get; set; }
        public DateTime Time { get; set; }

        public Log()
        {

        }

        //create log object for adding to database
        public Log(string message, string infoType)
        {
            Message = message;
            InfoType = infoType;
            Time = DateTime.Now;
        }
    }
}
