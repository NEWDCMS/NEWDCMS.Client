using Android.App;
using Android.Content;
using Android.OS;


namespace DCMS.Client.Droid
{
    /// <summary>
    /// 蓝牙打印服务
    /// </summary>
    [Service(Name = "com.dcms.clientv3.MyBtService")]
    public class MyBtService : IntentService
    {
        public static readonly string EXTRA_NOTIFICATION_CONTENT = "notification_content";
        private static readonly string CHANNEL_ID = "com.dcms.clientv3.print";
        private static readonly string CHANNEL_NAME = "dcms_print_channel";
        private NotificationUtil notificationUtil;
        public MyBtService() : base("MyBtService")
        {
        }

        public MyBtService(string name) : base(name)
        {

        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (intent == null)
            {
                return StartCommandResult.NotSticky;
            }

            string content = "DCMS正在运行打印....";

            if (intent != null)
                content = intent.GetStringExtra(EXTRA_NOTIFICATION_CONTENT);

            //创建Notification
            notificationUtil = new NotificationUtil(MainActivity.Instance,
                Resource.Mipmap.ic_launcher,
                "DCMS",
                content,
                CHANNEL_ID,
                CHANNEL_NAME);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                //将服务放置前台
                //startForeground() 中的id 和notification 不能为0 和 null
                StartForeground(NotificationUtil.NOTIFICATION_ID, notificationUtil.GetNotification());
            }

            return base.OnStartCommand(intent, flags, startId);
        }


        public override void OnDestroy()
        {
            //取消通知
            if (notificationUtil != null)
            {
                notificationUtil.CancelNotification();
                notificationUtil = null;
            }

            base.OnDestroy();
        }


        /// <summary>
        /// 退出服务
        /// </summary>
        public void ExitService()
        {
            StopForeground(true);
            this.StopSelf();
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        protected override void OnHandleIntent(Intent intent)
        {
            if (intent == null || intent.Action == null)
            {
                return;
            }

            //PutExtra
            var ptype = intent.GetIntExtra("PType", 0);

            //测试
            if (intent.Action.Equals(PrintUtil.ACTION_PRINT_TEST))
            {
                PrintTest(ptype);
            }
            //else if (intent.Action.Equals(PrintUtil.ACTION_PRINT_TEST_TWO))
            //{
            //    PrintTesttwo(3);
            //}
            //else if (intent.Action.Equals(PrintUtil.ACTION_PRINT_BITMAP))
            //{
            //    PrintBitmapTest();
            //}
        }

        /// <summary>
        /// 单次打印测试
        /// </summary>
        private void PrintTest(int ptype = 58)
        {
            int type = Settings.PrintStyleSelected;
            var podm = new PrintOrderDataMaker(this, "", type, PrinterWriter.HEIGHT_PARTING_DEFAULT);
            var printData = podm.GetPrintData(type);
            PrintQueue.GetQueue(Application.Context).Add(printData);
        }

        ///// <summary>
        ///// 多次打印测试
        ///// </summary>
        ///// <param name="num"></param>
        //private void PrintTesttwo(int num)
        //{
        //    try
        //    {
        //        //ArrayList<byte[]> bytes = new ArrayList<byte[]>();
        //        //for (int i = 0; i < num; i++)
        //        //{
        //        //    String message = "蓝牙打印测试\n蓝牙打印测试\n蓝牙打印测试\n\n";
        //        //    bytes.add(GPrinterCommand.reset);
        //        //    bytes.add(message.getBytes("gbk"));
        //        //    bytes.add(GPrinterCommand
        //        //            .print);
        //        //    bytes.add(GPrinterCommand.print);
        //        //    bytes.add(GPrinterCommand.print);
        //        //}
        //        //PrintQueue.getQueue(getApplicationContext()).add(bytes);
        //    }
        //    catch (UnsupportedEncodingException e)
        //    {
        //        //e.printStackTrace();
        //    }
        //}

        //private void Print(byte[] byteArrayExtra)
        //{
        //    //if (null == byteArrayExtra || byteArrayExtra.length <= 0)
        //    //{
        //    //    return;
        //    //}
        //    //PrintQueue.getQueue(getApplicationContext()).add(byteArrayExtra);
        //}

        ///// <summary>
        ///// 打印图片测试
        ///// </summary>
        //private void PrintBitmapTest()
        //{
        //    //BufferedInputStream bis;
        //    //try
        //    //{
        //    //    bis = new BufferedInputStream(getAssets().open(
        //    //            "icon_empty_bg.bmp"));
        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    e.printStackTrace();
        //    //    return;
        //    //}
        //    //Bitmap bitmap = BitmapFactory.decodeStream(bis);
        //    //PrintPic printPic = PrintPic.getInstance();
        //    //printPic.init(bitmap);
        //    //if (null != bitmap)
        //    //{
        //    //    if (bitmap.isRecycled())
        //    //    {
        //    //        bitmap = null;
        //    //    }
        //    //    else
        //    //    {
        //    //        bitmap.recycle();
        //    //        bitmap = null;
        //    //    }
        //    //}
        //    //byte[] bytes = printPic.printDraw();
        //    //ArrayList<byte[]> printBytes = new ArrayList<byte[]>();
        //    //printBytes.add(GPrinterCommand.reset);
        //    //printBytes.add(GPrinterCommand.print);
        //    //printBytes.add(bytes);
        //    //Log.e("BtService", "image bytes size is :" + bytes.length);
        //    //printBytes.add(GPrinterCommand.print);
        //    //PrintQueue.getQueue(getApplicationContext()).add(bytes);
        //}

    }
}