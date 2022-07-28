using DCMS.Client.Models;
using Shiny;
using Shiny.Infrastructure;
using Shiny.Jobs;
using System;

namespace DCMS.Client
{
    ///// <summary>
    ///// 后台工作日志
    ///// </summary>
    //public class JobLoggerTask : IShinyStartupTask
    //{
    //    private readonly IJobManager _jobManager;
    //    private readonly LocalDatabase _conn;
    //    private readonly ISerializer _serializer;

    //    public JobLoggerTask(IJobManager jobManager, ISerializer serializer, LocalDatabase conn)
    //    {
    //        this._jobManager = jobManager;
    //        this._serializer = serializer;
    //        this._conn = conn;
    //    }

    //    public void Start()
    //    {
    //        //工作启动时订阅
    //        this._jobManager.JobStarted.SubscribeAsync(args => this._conn.InsertAsync(new JobLog
    //        {
    //            JobIdentifier = args.Identifier,
    //            JobType = args.Type.FullName,
    //            Started = true,
    //            Timestamp = DateTime.Now,
    //            Parameters = this._serializer.Serialize(args.Parameters)
    //        }));

    //        //工作完成时订阅
    //        this._jobManager.JobFinished.SubscribeAsync(args => this._conn.InsertAsync(new JobLog
    //        {
    //            JobIdentifier = args.Job.Identifier,
    //            JobType = args.Job.Type.FullName,
    //            Error = args.Exception?.ToString(),
    //            Parameters = this._serializer.Serialize(args.Job.Parameters),
    //            Timestamp = DateTime.Now
    //        }));
    //    }
    //}
}
