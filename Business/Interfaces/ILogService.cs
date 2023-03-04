using Entity.Database;

namespace Business.Interfaces
{
    public interface ILogService
    {
        void Insert(LogDto logDto);
    }
}
