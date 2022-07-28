namespace DCMS.Client.Droid.AutoUpdater
{
    /*
    public class UpdateAgent
    {
        //单例实现
        private volatile static UpdateAgent _instance = null;
        private static readonly object lockHelper = new object();


        static DownloadManager downloadManager;
        static DownloadManager.Request downLoadRequest;


        static Context _context;
        static long _downloadID = -1;


        public string Title
        {
            get;
            set;
        } = "下载";

        public string Description
        {
            get;
            set;
        } = "下载中...";

        public DownloadNetwork AllowedNetworkType
        {
            get;
            set;
        } = DownloadNetwork.Wifi;


        public DownloadMode DownloadMode
        {
            get;
            set;
        } = DownloadMode.Always;

        /// <summary>
        /// Gets or sets the download directory. 
        /// </summary>
        /// <value>The download directory.</value>
        public string DownloadDirectory
        {
            get;
            set;
        } = AEnvironment.DirectoryDownloads;

        private static JFile file;

        public JFile File
        {
            get
            {
                return file;
            }
            set
            {
                file = value;
            }
        }


        private UpdateAgent()
        {
        }


        public static UpdateAgent GetInstance(Context context)
        {
            if (_instance == null)
            {
                lock (lockHelper)
                {
                    if (_instance == null)
                        _instance = new UpdateAgent();
                }
            }
            _context = context;
            downloadManager = DownloadManager.FromContext(context);
            //downloadManager = (DownloadManager)context.GetSystemService (Context.DownloadService);
            return _instance;

        }

        /// <summary>
        /// 开始下载
        /// </summary>
        public void Start(string apkUrl, string apkName)
        {
            var receiver = new DownLoadCompleteReceiver();
            // 注册广播接收器，当下载完成时自动安装
            _context.RegisterReceiver(receiver, new IntentFilter(DownloadManager.ActionDownloadComplete));

            if (DownloadMode != DownloadMode.Always)
            {
                var filePath = AEnvironment.GetExternalStoragePublicDirectory(DownloadDirectory).Path;
                file = new JFile(filePath, apkName);
                if (file.Exists())
                {
                    Console.WriteLine(file.Path);
                    if (DownloadMode == DownloadMode.Overwrite)
                    {
                        file.Delete();
                        //更新下载中的显示
                        //ContentValues values = new ContentValues ();
                        //values.Put ("deleted", 1);
                        //COLUMN_DELETED = "deleted";
                        //CONTENT_URI = Uri.parse("content://downloads/my_downloads");
                        //ALL_DOWNLOADS_CONTENT_URI = Uri.parse("content://downloads/all_downloads")
                        //_context.ContentResolver.Update (ContentUris.WithAppendedId (AUri.Parse ("content://downloads/my_downloads"), _downloadID), values, null, null);
                        _context.ContentResolver.Delete(AUri.Parse("content://downloads/my_downloads"), "_data=?", new string[] { file.Path });

                    }
                    else
                    {
                        //发送广播
                        var intent = new Intent();
                        intent.SetAction(DownloadManager.ActionDownloadComplete);
                        _context.SendBroadcast(intent);
                        return;
                    }
                }
            }

            //判断是否有正在下载的相同任务
            DownloadManager.Query downloadQuery = new DownloadManager.Query();
            //根据下载状态查询下载任务
            downloadQuery.SetFilterByStatus(DownloadStatus.Running);
            //根据任务编号查询下载任务信息
            //downloadQuery.setFilterById(id);
            var cursor = downloadManager.InvokeQuery(downloadQuery);

            if (cursor != null)
            {
                if (cursor.MoveToNext())
                {
                    if (apkUrl.Equals(cursor.GetString(cursor.GetColumnIndex(DownloadManager.ColumnUri))))
                    {
                        Toast.MakeText(_context, "已存在的下载任务", ToastLength.Short).Show();
                        cursor.Close();
                        return;
                    }
                }
                cursor.Close();
            }

            downLoadRequest = new DownloadManager.Request(AUri.Parse(apkUrl));
            downLoadRequest.SetTitle(Title);
            downLoadRequest.SetDescription(Description);
            downLoadRequest.AllowScanningByMediaScanner();
            downLoadRequest.SetVisibleInDownloadsUi(true);
            //设置在什么网络情况下进行下载
            downLoadRequest.SetAllowedNetworkTypes(AllowedNetworkType);
            //设置通知栏标题
            downLoadRequest.SetNotificationVisibility(DownloadVisibility.Visible | DownloadVisibility.VisibleNotifyCompleted);
            //设置下载文件的mineType
            downLoadRequest.SetMimeType("application/com.trinea.download.file");
            //downLoadRequest.SetDestinationInExternalFilesDir
            //设置保存位置
            downLoadRequest.SetDestinationInExternalPublicDir(DownloadDirectory, apkName);
            //保存任务ID
            _downloadID = downloadManager.Enqueue(downLoadRequest);
        }

        /// <summary>
        /// 删除当前下载任务
        /// </summary>
        public int Remove()
        {
            return downloadManager.Remove(_downloadID);
        }


        /// <summary>
        /// Queries the download status.
        /// </summary>
        /// <returns>The download status.</returns>
        static int QueryDownloadStatus()
        {
            var result = -1;
            DownloadManager.Query query = new DownloadManager.Query().SetFilterById(_downloadID);
            var cursor = downloadManager.InvokeQuery(query);

            if (cursor != null)
            {
                if (cursor.MoveToFirst())
                {
                    result = cursor.GetInt(cursor.GetColumnIndex(DownloadManager.ColumnStatus));
                }
                cursor.Close();
            }

            return result;
        }


        class DownLoadCompleteReceiver : BroadcastReceiver
        {

            #region implemented abstract members of BroadcastReceiver

            public override void OnReceive(Context context, Intent intent)
            {
                if (intent.Action.Equals(DownloadManager.ActionDownloadComplete))
                {
                    var downloadID = intent.GetLongExtra(DownloadManager.ExtraDownloadId, -1);

                    Intent install = new Intent(Intent.ActionView);

                    install.AddCategory(Intent.CategoryDefault);
                    install.AddFlags(ActivityFlags.NewTask);

                    if (downloadID == -1)
                    {
                        install.SetDataAndType(AUri.Parse("file://" + file.AbsolutePath), "application/vnd.android.package-archive");
                    }
                    else if (_downloadID == downloadID && QueryDownloadStatus() == (int)DownloadStatus.Successful)
                    {

                        var downloadFileUri = downloadManager.GetUriForDownloadedFile(downloadID);

                        install.SetDataAndType(downloadFileUri, "application/vnd.android.package-archive");
                    }
                    context.StartActivity(install);
                }
                context.UnregisterReceiver(this);
            }

            #endregion

        }
    }

    public enum DownloadMode
    {
        /// <summary>
        /// 已包含下载文件 取消下载
        /// </summary>
        Exist,
        /// <summary>
        /// 覆盖现有文件
        /// </summary>
        Overwrite,
        /// <summary>
        /// 一直下载
        /// </summary>
        Always
    }

    */
}