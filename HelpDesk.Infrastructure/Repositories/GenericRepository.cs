using System.Linq;
using System.Threading.Tasks;
using HelpDesk.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Infrastructure.Persistence;

namespace HelpDesk.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly HelpDeskDbContext _context;
        public GenericRepository(HelpDeskDbContext context) => _context = context;

        public async Task<T?> GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id);
        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);
        public void Update(T entity) => _context.Set<T>().Update(entity);
        public void Remove(T entity) => _context.Set<T>().Remove(entity);
        public IQueryable<T> Query() => _context.Set<T>().AsQueryable();
        public async Task<System.Collections.Generic.List<T>> GetAllAsync(System.Threading.CancellationToken ct = default)
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync(ct);
        }
    }
}
