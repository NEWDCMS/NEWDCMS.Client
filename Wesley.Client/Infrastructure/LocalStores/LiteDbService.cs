using Wesley.Client.Models;
using LiteDB.Async;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LiteDB;
using System.Linq.Expressions;

namespace Wesley.Client
{

    public interface ILiteDbAsyncService
    {
        ILiteDatabaseAsync Db { get; }
    }
    public class LiteDbAsyncService : ILiteDbAsyncService
    {
        private readonly ILiteDatabaseAsync _db;
        private readonly static string _databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "dcmsv4.db");

        public LiteDbAsyncService()
        {
            try
            {
                //Path.Combine(FileSystem.AppDataDirectory
                _db = new LiteDatabaseAsync(_databasePath);
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.Print(ex.StackTrace);
                throw new Exception("LiteDB System.IO.IOException: 共享冲突");
            }
        }

        public ILiteDatabaseAsync Db => _db;
    }


    public interface ILiteDbService<T>
    {
        LiteCollectionAsync<T> Table { get; }
        Task<bool> DeleteAsync(T t);
        Task<int> DeleteAllAsync();
        Task<List<T>> GetAllAsync();
        Task<T> GetAsync(int id);
        Task<int> InsertAsync(T t);
        Task<bool> UpsertAsync(T t);
        Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> predicate, int skip = 0, int limit = int.MaxValue);
    }
    public class LiteDbService<T> : ILiteDbService<T> where T : EntityBase
    {
        private readonly ILiteDbAsyncService _liteDbAsyncService;

        public LiteDbService(ILiteDbAsyncService liteDbAsyncService)
        {
            _liteDbAsyncService = liteDbAsyncService;
        }

        public LiteCollectionAsync<T> Table => _liteDbAsyncService.Db.GetCollection<T>();

        public async Task<bool> DeleteAsync(T t)
        {
            return await Table.DeleteAsync(t.Id);
        }

        public async Task<int> DeleteAllAsync()
        {
            return await Table.DeleteAllAsync();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await Table.Query().ToListAsync();
        }
        public async Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> predicate, int skip = 0, int limit = int.MaxValue)
        {
            return await Table.FindAsync(predicate, skip, limit);
        }

        public async Task<T> GetAsync(int id)
        {
            return await Table.FindByIdAsync(id);
        }

        public async Task<int> InsertAsync(T t)
        {
            return await Table.InsertAsync(t);
        }

        public async Task<bool> UpsertAsync(T t)
        {
            return await Table.UpdateAsync(t);
        }

    }
}
