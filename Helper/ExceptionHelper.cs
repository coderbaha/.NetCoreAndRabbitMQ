using Entity.Database;
using System;

namespace Helper
{
    public class ExceptionHelper
    {

        public static string ReportInfoDailyDocumentsByCreateUser = "ReportInfoDailyDocumentsByCreateUser";
        public static string ReportInfoDailyDocumentsByCreateDate = "ReportInfoDailyDocumentsByCreateDate";
        public static string PdfCreateTriggerByCreateUser = "PdfCreateTriggerByCreateUser";
        public static string PdfCreateTriggerByCreateDate = "PdfCreateTriggerByCreateDate";
        public static string PdfCreateTriggerByCreateUserSOAPService = "PdfCreateTriggerByCreateUserSOAPService";
        public static string PdfCreateTriggerByCreateDateSOAPService = "PdfCreateTriggerByCreateDateSOAPService";
        public static string LogRepositoryAdd = "LogRepositoryAdd";
        public static string ReportInfoDailyDocuments = "ReportInfoDailyDocuments";
        public static string GetByCreateUser = "GetByCreateUser";
        public static string GetByCreateDate = "GetByCreateDate";
        public static string PdfCreateByCreateUser = "PdfCreateByCreateUser";
        public static string PdfCreateByCreateDate = "PdfCreateByCreateDate";
        public static string GetFileData = "GetFileData";
        public static string PdfCreater = "PdfCreater";
        public static string CheckPdfControl = "CheckPdfControl";
        public static string GetPdfControl = "GetPdfControl";
        public static string GetFileDataSOAP = "GetFileDataSOAP";
        public static string PdfCheck = "PdfCheck";
        public static string GetPdfCheck = "GetPdfCheck";
        public static string LogRepositoryInsert = "LogRepositoryInsert";
        public static string LogServiceInsert = "LogServiceInsert";
        public static string SuccessMessage = "Başarılı";
        public static string NoContent = "Içerik yok";
        public static string BadRequest = "Geçersiz istek";
        public static string Unauthorized = "Yetkisiz";
        public static string NotFound = "Bulunamadı";
        public static string ServerError = "Iç sunucu hatası";
        public static LogDto CreateLogModel(int statusCode, string source)
        {
            LogDto logDto = new LogDto();
            logDto.Log_Source = source;
            logDto.Log_StatusCode = statusCode;
            switch (statusCode)
            {
                case 200:
                    logDto.Log_Message = SuccessMessage;
                    break;
                case 204:
                    logDto.Log_Message = NoContent;
                    break;
                case 400:
                    logDto.Log_Message = BadRequest;
                    break;
                case 401:
                    logDto.Log_Message = Unauthorized;
                    break;
                case 404:
                    logDto.Log_Message = NotFound;
                    break;
                case 500:
                    logDto.Log_Message = ServerError;
                    break;

            }
            logDto.Log_Date = DateTime.Now;
            return logDto;
        }

    }
}
