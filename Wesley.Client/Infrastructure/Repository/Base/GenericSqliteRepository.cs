//using SQLiteNetExtensionsAsync.Extensions;

namespace Wesley.Infrastructure.Repository.Base
{
    //public class GenericSqliteRepository<T> : IGenericRepository<T> where T : EntityBase, new()
    //{
    //    protected readonly DbContext _context;

    //    public GenericSqliteRepository(DbContext context)
    //    {
    //        _context = context;
    //    }

    //    public virtual async Task<int> AddAsync(T entity)
    //    {
    //        await _context.Database.InsertAsync(entity);
    //        return entity.Id;
    //    }
    //    public virtual async Task InsertWithChildrenAsync(T entity)
    //    {
    //        await _context.Database.InsertWithChildrenAsync(entity, recursive: true);
    //    }
    //    public virtual async Task UpdateWithChildrenAsync(T entity)
    //    {
    //        await _context.Database.UpdateWithChildrenAsync(entity);
    //    }
    //    public virtual async Task<int> AddRangeAsync(IEnumerable<T> entities)
    //    {
    //        return await _context.Database.InsertAllAsync(entities);
    //    }

    //    public virtual async Task<T> FindAsync(Expression<Func<T, bool>> predicate)
    //    {
    //        return await _context.Set<T>().Where(predicate).FirstOrDefaultAsync();
    //    }

    //    public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate)
    //    {
    //        return await _context.Set<T>().Where(predicate).ToListAsync();
    //    }
    //    public virtual async Task<List<T>> GetQueryAsync(string query)
    //    {
    //        return await _context.Database.QueryAsync<T>(query);
    //    }
    //    public virtual async Task<List<T>> GetTableWithAllChildrenAsync(Expression<Func<T, bool>> expression)
    //    {
    //        try
    //        {
    //            System.Threading.CancellationToken cancellationToken = System.Threading.CancellationToken.None;
    //            var result = await _context.Database.GetAllWithChildrenAsync<T>(expression, true, cancellationToken);
    //            return result;
    //        }
    //        catch(Exception ex)
    //        {
    //            Console.WriteLine(ex.Message);
    //            return null;
    //        }
    //    }

    //    public virtual async Task<IEnumerable<T>> GetAsync(int skip, int take)
    //    {
    //        return await _context.Set<T>().Skip(skip).Take(take).ToListAsync();
    //    }

    //    public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate, int skip, int take)
    //    {
    //        return await _context.Set<T>().Where(predicate).Skip(skip).Take(take).ToListAsync();
    //    }

    //    public virtual async Task RemoveAsync(T entity)
    //    {
    //        await _context.Database.DeleteAsync(entity);
    //    }
    //    public virtual async Task RemoveAllAsync(IEnumerable<T> entities)
    //    {
    //        await _context.Database.DeleteAllAsync(entities);
    //    }
    //    public virtual async Task UpdateAsync(T entity)
    //    {
    //        await _context.Database.UpdateAsync(entity);
    //    }
    //    public virtual async Task<int> ExecuteScriptsAsync(string query)
    //    {
    //        return await _context.Database.ExecuteAsync(query);
    //    }
    //}
}
