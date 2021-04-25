using SQLite;
using System.IO;

namespace DCMS.Client
{
    public class DbContext
    {
        /// <summary>
        /// 本地存储数据库
        /// </summary>
        public static string DB_NAME = "dcms_client.db";
        public static string LocalFilePath;
        public SQLiteAsyncConnection Database { get; }
        /// <summary>
        /// 初始数据库上下文
        /// </summary>
        public DbContext()
        {
            Database = new SQLiteAsyncConnection(Path.Combine(LocalFilePath, DB_NAME));
        }
        /// <summary>
        /// 按类型从数据库中获取表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public AsyncTableQuery<T> Set<T>() where T : new()
        {
            return Database.Table<T>();
        }
    }
}
