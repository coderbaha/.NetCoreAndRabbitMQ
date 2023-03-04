using System;

namespace Entity.Database
{
    public class LogDto
    {
        public string Log_Source { get; set; }
        public int Log_StatusCode { get; set; }
        public string Log_Message { get; set; }
        public DateTime Log_Date { get; set; }

    }
}