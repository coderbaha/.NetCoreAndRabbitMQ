using System;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IGetFileService
    {
        Task<T> GetFileData<T>(string apiUrl, string filename);
        void PdfCreater(string path, Byte[] data, Guid guid);
        string CheckPdfControl(string path, string filename);
        string GetPdfControl(string path, string filename);
    }
}
