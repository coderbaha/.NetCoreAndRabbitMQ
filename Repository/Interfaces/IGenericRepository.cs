using System.Collections.Generic;

namespace Repository.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        List<T> GetAll();
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
