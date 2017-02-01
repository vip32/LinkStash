namespace LinkStash.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IRepository<T>
        where T : Entity
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByIdAsync(string id);

        Task<T> AddOrUpdateAsync(T entity);

        Task<long> CountAsync();

        Task<T> GetByEtagAsync(string etag);

        Task<T> FirstAsync(Expression<Func<T, bool>> expression);

        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);
    }
}
