using Entity.Database;
using System;
using System.IO;

namespace Helper
{
    public class FileLogHelper
    {
        public static void LogFile(string filePath, LogDto logDto)
        {
            try
            {
                using StreamWriter logFile =
                    new(filePath,
                        append: true);
                logFile.WriteLineAsync(logDto.Log_Source + ":" + logDto.Log_Message + ":" + logDto.Log_StatusCode + ":" + logDto.Log_Date);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static void LogFileTryCatch(string filePath, string source, string message)
        {
            try
            {
                using StreamWriter logFile =
                    new(filePath,
                        append: true);
                logFile.WriteLineAsync(source + ":" + message);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
