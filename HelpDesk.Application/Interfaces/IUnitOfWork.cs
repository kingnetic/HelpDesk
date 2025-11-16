using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HelpDesk.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<T> Repository<T>() where T : class;
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
