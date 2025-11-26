using System.Linq;
using System.Threading.Tasks;

namespace HelpDesk.Application.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        IQueryable<T> Query();
        Task<System.Collections.Generic.List<T>> GetAllAsync(System.Threading.CancellationToken ct = default);
    }
}
