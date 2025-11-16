using System.Linq;
using System.Threading.Tasks;
using HelpDesk.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Infrastructure.Persistence;

namespace HelpDesk.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly HelpDeskDbContext _ctx;
        public GenericRepository(HelpDeskDbContext ctx) => _ctx = ctx;

        public async Task<T?> GetByIdAsync(int id) => await _ctx.Set<T>().FindAsync(id);
        public async Task AddAsync(T entity) => await _ctx.Set<T>().AddAsync(entity);
        public void Update(T entity) => _ctx.Set<T>().Update(entity);
        public void Remove(T entity) => _ctx.Set<T>().Remove(entity);
        public IQueryable<T> Query() => _ctx.Set<T>().AsQueryable();
    }
}
