using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HelpDesk.Application.Interfaces;
using HelpDesk.Infrastructure.Persistence;

namespace HelpDesk.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HelpDeskDbContext _ctx;
        private readonly Dictionary<Type, object> _repos = new();
        public UnitOfWork(HelpDeskDbContext ctx) => _ctx = ctx;

        public IGenericRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            if (!_repos.ContainsKey(type)) _repos[type] = new GenericRepository<T>(_ctx);
            return (IGenericRepository<T>)_repos[type]!;
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default) => _ctx.SaveChangesAsync(ct);
    }
}
