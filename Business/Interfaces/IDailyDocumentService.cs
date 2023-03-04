using Entity.Business;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IDailyDocumentService
    {
        Task<T> ReportInfoDailyDocuments<T>(string apiUrl);
        ServiceResult<List<Document>> GetByCreateUser(string apiUrl, string createUser);
        ServiceResult<List<Document>> GetByCreateDate(string apiUrl, int startHour, int finishHour);
        ServiceResult<List<FileListModel>> PdfCreateByCreateUser(string apiUrl, string createUser);
        ServiceResult<List<FileListModel>> PdfCreateByCreateDate(string apiUrl, int startHour, int finishHour);
    }
}
