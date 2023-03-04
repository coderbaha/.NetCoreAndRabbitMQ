using Entity.Business;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IGetFileSOAPService
    {
        FileModelResponse GetFileDataSOAP(string apiUrl, string filename);
    }
}
