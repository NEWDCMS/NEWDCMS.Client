using Wesley.Client.Models;
using Wesley.Client.Models.Census;
using Shiny;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wesley.Client
{
    /// <summary>
    /// 用于客户端本地数据存储
    /// </summary>
    public class LocalDatabase : SQLiteAsyncConnection
    {
        /// <summary>
        /// SqliteConnection
        /// </summary>
        /// <param name="platform"></param>
        public LocalDatabase(IPlatform platform) : base(Path.Combine(platform.AppData.FullName, "dcms.db"))
        {
            try
            {
                var conn = this.GetConnection();
                conn.CreateTable<VisitStore>();
                conn.CreateTable<TrackingModel>();
                conn.CreateTable<NotificationEvent>();
                conn.CreateTable<PushEvent>();
                conn.CreateTable<ErrorLog>();
                conn.CreateTable<MessageInfo>();
                conn.CreateTable<GpsEvent>();
                conn.CreateTable<GeofenceEvent>();
            }
            catch (Exception)
            {

            }
        }

        public AsyncTableQuery<VisitStore> VisitStoreEvents => this.Table<VisitStore>();
        public AsyncTableQuery<NotificationEvent> NotificationEvents => this.Table<NotificationEvent>();
        public AsyncTableQuery<PushEvent> PushEvents => this.Table<PushEvent>();
        public AsyncTableQuery<ErrorLog> ErrorLogEvents => this.Table<ErrorLog>();
        public AsyncTableQuery<MessageInfo> MessageInfoEvents => this.Table<MessageInfo>();
        public AsyncTableQuery<TrackingModel> LocationSyncEvents => this.Table<TrackingModel>();
        public AsyncTableQuery<GpsEvent> GpsEvents => this.Table<GpsEvent>();
        public AsyncTableQuery<GeofenceEvent> GeofenceEvents => this.Table<GeofenceEvent>();


        /// <summary>
        /// 对象保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task SaveItemAsync<T>(T item)
        {
            await this.InsertAsync(item);
        }

        public async Task ResetErrorLogs()
        {
            await this.DropTableAsync<ErrorLog>();
            await this.CreateTableAsync<ErrorLog>();
        }

        public async Task ResetMessageInfos()
        {
            await this.DropTableAsync<MessageInfo>();
            await this.CreateTableAsync<MessageInfo>();
        }

        public async Task ResetLocationSyncEvents()
        {
            await this.DropTableAsync<TrackingModel>();
            await this.CreateTableAsync<TrackingModel>();
        }

        public async Task ResetVisitStores()
        {
            await this.DropTableAsync<VisitStore>();
            await this.CreateTableAsync<VisitStore>();
        }

        #region 签到

        public async Task<bool> CheckVisitStore(int terminalId)
        {
            try
            {
                var results = await this.Table<VisitStore>()
                    .Where(c => c.TerminalId == terminalId)
                    .ToListAsync();

                bool exits = false;
                if (results.Any())
                {
                    foreach (var s in results)
                    {
                        if (s.SignOutDateTime.ToString("yyyy-MM-dd").Equals(DateTime.Now.ToString("yyyy-MM-dd")))
                        {
                            exits = true;
                            break;
                        }
                    }
                }
                return exits;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region 位置

        public async Task RemoveTopLocation()
        {
            try
            {
                var result = await this.Table<TrackingModel>().FirstOrDefaultAsync();
                if (result != null)
                    await this.DeleteAsync(result);
            }
            catch (Exception)
            {

            }
        }

        public async Task<List<TrackingModel>> GetAllLocations()
        {
            return await this.Table<TrackingModel>().ToListAsync();
        }

        #endregion

        #region 日志


        #endregion

        #region 消息

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="msgId"></param>
        /// <returns></returns>
        public async Task RemoveMessageInfo(int msgId)
        {
            var result = await this.Table<MessageInfo>().Where(c => c.Id == msgId).FirstOrDefaultAsync();
            if (result != null)
            {
                await this.DeleteAsync(result);
            }
        }

        /// <summary>
        /// 设置已读
        /// </summary>
        /// <param name="messageInfo"></param>
        /// <returns></returns>
        public async Task SetPending(int msgId, bool read)
        {
            var result = await this.Table<MessageInfo>().Where(c => c.Id == msgId).FirstOrDefaultAsync();
            if (result != null)
            {
                result.IsRead = read;
                await this.UpdateAsync(result);
            }
        }

        /// <summary>
        /// 获取全部消息
        /// </summary>
        /// <returns></returns>
        public async Task<List<MessageInfo>> GetAllMessageInfos()
        {
            return await this.Table<MessageInfo>().ToListAsync();
        }

        /// <summary>
        /// 获取全部消息
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public async Task<List<MessageInfo>> GetAllMessageInfos(int[] types)
        {
            try
            {
                List<MessageInfo> messageInfos = await this
                          .Table<MessageInfo>()
                          .Where(c => types.Contains(c.MTypeId))
                          .OrderByDescending(c => c.Id)
                          .ToListAsync();

                return messageInfos;
            }
            catch (Exception)
            {
                return new List<MessageInfo>();
            }
        }

        #endregion
    }
}
