using Android.Bluetooth;
using Android.Content;
using Java.IO;
using Java.Util;
using System;


namespace Wesley.Client.Droid
{
    /// <summary>
    /// 读取字节
    /// </summary>
    public class BtMsgReadEvent
    {
        public int bytes;
        public byte[] buffer;
        public Java.Lang.String message;

        public BtMsgReadEvent(int bytes, byte[] buffer)
        {
            this.buffer = buffer;
            this.bytes = bytes;
            this.message = new Java.Lang.String(buffer, 0, bytes);
        }
    }

    /// <summary>
    /// 蓝牙接口
    /// </summary>
    public interface IBtInterface
    {
        void BtStartDiscovery(Intent intent);
        void BtFinishDiscovery(Intent intent);
        void BtStatusChanged(Intent intent);
        void BtFoundDevice(Intent intent);
        void BtBondStatusChange(Intent intent);
        void BtPairingRequest(Intent intent);
    }

    /// <summary>
    /// 这个类完成所有的设置和管理蓝牙的工作 与其他设备的连接。
    /// 它有一个线程来侦听来电 连接，用于与设备连接的线程和线程连接时执行数据传输。
    /// </summary>
    public class BtService
    {
        //指示当前连接状态的常量

        /// <summary>
        /// 无状态
        /// </summary>
        public const int STATE_NONE = 0;
        /// <summary>
        /// 正在侦听传入连接
        /// </summary>
        public const int STATE_LISTEN = 1;
        /// <summary>
        /// 正在启动传出连接
        /// </summary>
        public const int STATE_CONNECTING = 2;
        /// <summary>
        /// 已经连接到远程设备
        /// </summary>
        public const int STATE_CONNECTED = 3;


        /// <summary>
        /// 创建服务器套接字时SDP记录的名称
        /// </summary>
        private readonly static string NAME = "BtService";

        /// <summary>
        /// 此应用程序的唯一UUID 00001101-0000-1000-8000-00805f9b34fb
        /// </summary>
        private readonly static UUID MY_UUID = UUID.FromString("0001101-0000-1000-8000-00805F9B34FB");

        private static BluetoothAdapter mAdapter;
        private static AcceptThread mAcceptThread;
        private static ConnectThread mConnectThread;
        private static ConnectedThread mConnectedThread;
        private static int mState;

        /// <summary>
        /// 上下文
        /// </summary>
        private static Context mContext;

        private readonly static object Lock = new object();


        public BtService(Context context)
        {
            mAdapter = BluetoothAdapter.DefaultAdapter;
            mState = STATE_NONE;
            mContext = context;
        }


        /// <summary>
        /// 获取连接状态
        /// </summary>
        /// <returns></returns>
        public int GetState()
        {
            lock (Lock)
            {
                return mState;
            }
        }


        private static void SetState(int state)
        {
            lock (Lock)
            {
                //Log.d(TAG, "setState() " + mState + " -> " + state);

                mState = state;

                // 如果需要显示bt状态，请使用下面的代码将新状态赋予处理程序，以便UI活动可以更新
                switch (mState)
                {
                    //等待连接
                    case STATE_LISTEN:
                        //EventBus.getDefault().post(new PrintMsgEvent(PrinterMsgType.MESSAGE_STATE_CHANGE, "等待连接"));
                        break;
                    //正在连接
                    case STATE_CONNECTING:
                        //EventBus.getDefault().post(new PrintMsgEvent(PrinterMsgType.MESSAGE_STATE_CHANGE, "正在连接"));
                        break;
                    //已连接
                    case STATE_CONNECTED:
                        // EventBus.getDefault().post(new PrintMsgEvent(PrinterMsgType.MESSAGE_STATE_CHANGE, "已连接"));
                        break;
                    //未连接
                    default:
                        //EventBus.getDefault().post(new PrintMsgEvent(PrinterMsgType.MESSAGE_STATE_CHANGE, "未连接"));
                        break;
                }
            }
        }

        /// <summary>
        /// 连接打印设备
        /// </summary>
        /// <param name="device">设备</param>
        public void Connect(BluetoothDevice device)
        {
            lock (Lock)
            {
                //Log.d(TAG, "connect to: " + device);

                //EventBus.getDefault().post(new PrintMsgEvent(PrinterMsgType.MESSAGE_TOAST, "正在连接蓝牙设备"));

                //如果设备正在连接蓝牙设备，则取消连接线程
                if (mState == STATE_CONNECTING)
                {
                    if (mConnectThread != null)
                    {
                        mConnectThread.Cancel();
                        mConnectThread = null;
                    }
                }

                // 已经连接，取消当前运行连接的任何线程
                if (mConnectedThread != null)
                {
                    mConnectedThread.Cancel();
                    mConnectedThread = null;
                }

                //重新启动线程以连接给定的设备
                mConnectThread = new ConnectThread(device);
                mConnectThread.Start();

                //更改连接状态
                SetState(STATE_CONNECTING);
            }
        }

        /// <summary>
        /// 启动ConnectedThread开始管理蓝牙连接
        /// </summary>
        /// <param name="socket">建立连接的蓝牙插座</param>
        /// <param name="device">已连接的蓝牙设备</param>
        /// <param name="socketType"></param>
        public static void Connected(BluetoothSocket socket, BluetoothDevice device, string socketType)
        {
            lock (Lock)
            {
                //Log.d(TAG, "connected, Socket Type:" + socketType);

                // 取消完成连接的线程
                if (mConnectThread != null)
                {
                    mConnectThread.Cancel();
                    mConnectThread = null;
                }

                // 取消当前运行连接的任何线程
                if (mConnectedThread != null)
                {
                    mConnectedThread.Cancel();
                    mConnectedThread = null;
                }

                // 取消接受线程，因为我们只想连接到一个设备
                if (mAcceptThread != null)
                {
                    mAcceptThread.Cancel();
                    mAcceptThread = null;
                }

                // 启动线程来管理连接并执行传输
                mConnectedThread = new ConnectedThread(socket);
                mConnectedThread.Start();

                // 将连接设备的名称发送回UI活动
                //EventBus.getDefault().post(new PrintMsgEvent(PrinterMsgType.MESSAGE_TOAST, "蓝牙设备连接成功"));

                SetState(STATE_CONNECTED);

                // 调用打印队列打印
                PrintQueue.GetQueue(mContext).Print();
            }

        }

        /// <summary>
        /// 停止所有线程
        /// </summary>
        public void Stop()
        {
            lock (Lock)
            {
                //Log.d(TAG, "stop");

                if (mConnectThread != null)
                {
                    mConnectThread.Cancel();
                    mConnectThread = null;
                }

                if (mConnectedThread != null)
                {
                    mConnectedThread.Cancel();
                    mConnectedThread = null;
                }

                if (mAcceptThread != null)
                {
                    mAcceptThread.Cancel();
                    mAcceptThread = null;
                }
                SetState(STATE_NONE);
            }
        }

        /// <summary>
        /// 以非同步方式写入ConnectedThread
        /// </summary>
        /// <param name="out"></param>
        public void Write(byte[] @out)
        {
            // 临时对象
            ConnectedThread r;

            // 同步ConnectedThread的副本
            lock (this)
            {
                if (mState != STATE_CONNECTED)
                    return;

                r = mConnectedThread;
            }
            // 执行非同步写入
            r.Write(@out);
        }

        /// <summary>
        /// 休眠时间后以非同步方式写入ConnectedThread
        /// </summary>
        /// <param name="out"></param>
        /// <param name="sleepTime"></param>
        public void Write(byte[] @out, long sleepTime)
        {
            //临时对象
            ConnectedThread r;
            //同步ConnectedThread的副本
            lock (this)
            {
                if (mState != STATE_CONNECTED)
                    return;

                r = mConnectedThread;
            }

            //执行非同步写入
            r.Write(@out, sleepTime);
        }

        /// <summary>
        /// 指示连接尝试失败并通知UI活动
        /// </summary>
        private static void ConnectionFailed()
        {
            //将失败消息发送回活动
            //EventBus.getDefault().post(new PrintMsgEvent(PrinterMsgType.MESSAGE_TOAST, "蓝牙连接失败,请重启打印机再试"));
            SetState(STATE_NONE);
            //重新启动服务以重新启动侦听模式
            Start();
        }

        /**
         * Start the chat service. Specifically start AcceptThread to begin a
         * session in listening (server) mode. Called by the Activity onResume()
         * 开始聊天服务。 具体启动AcceptThread开始一个
          会话在监听（服务器）模式。 由Activity onResume（）调用
         */
        public static void Start()
        {
            lock (Lock)
            {
                // Log.d(TAG, "start");

                // Cancel any thread attempting to make a connection
                if (mConnectThread != null)
                {
                    mConnectThread.Cancel();
                    mConnectThread = null;
                }

                // Cancel any thread currently running a connection
                if (mConnectedThread != null)
                {
                    mConnectedThread.Cancel();
                    mConnectedThread = null;
                }

                SetState(STATE_LISTEN);

                // Start the thread to listen on a BluetoothServerSocket
                if (mAcceptThread == null)
                {
                    mAcceptThread = new AcceptThread();
                    mAcceptThread.Start();
                }
            }
        }

        /// <summary>
        /// 指示连接已丢失并通知UI活动
        /// </summary>
        private static void ConnectionLost()
        {
            //将失败消息发送回活动
            //EventBus.getDefault().post(new PrintMsgEvent(PrinterMsgType.MESSAGE_TOAST, "蓝牙连接断开"));
            SetState(STATE_NONE);

            //重新启动服务以重新启动侦听模式
            Start();
        }

        /// <summary>
        /// 用于连接接受的线程
        /// </summary>
        private class AcceptThread : Java.Lang.Thread
        {
            // 本地服务器套接字
            private readonly BluetoothServerSocket mmServerSocket;
            //private readonly string mSocketType;

            public AcceptThread()
            {
                BluetoothServerSocket tmp = null;

                //创建新的侦听服务器套接字
                try
                {
                    tmp = mAdapter.ListenUsingInsecureRfcommWithServiceRecord(NAME, MY_UUID);
                }
                catch (Exception)
                {
                    //Log.e(TAG, "Socket Type: " + mSocketType + "listen() failed", e);
                }
                mmServerSocket = tmp;
            }

            public override void Run()
            {
                try
                {
                    //Log.d(TAG, "Socket Type: " + mSocketType+ "BEGIN mAcceptThread" + this);

                    //SetName("AcceptThread" + mSocketType);

                    BluetoothSocket socket = null;

                    // 如果没有连接，请听服务器套接字
                    while (mState != STATE_CONNECTED)
                    {
                        try
                        {
                            // 这是一个阻塞调用，仅在连接成功或出现异常时返回
                            socket = mmServerSocket.Accept();
                        }
                        catch (Exception e)
                        {
                            //Log.e(TAG, "Socket Type: " + mSocketType + "accept() failed", e);
                            break;
                        }

                        // 如果连接被接受
                        if (socket != null)
                        {
                            lock (this)
                            {
                                switch (mState)
                                {
                                    case STATE_LISTEN:
                                    case STATE_CONNECTING:
                                        // 情况正常。启动连接的线程
                                        Connected(socket, socket.RemoteDevice, "");
                                        break;
                                    case STATE_NONE:
                                    case STATE_CONNECTED:
                                        // 未准备好或已连接。终止新套接字。
                                        try
                                        {
                                            socket.Close();
                                        }
                                        catch (IOException)
                                        {
                                            //Log.e(TAG, "Could not close unwanted socket", e);
                                        }
                                        break;
                                }
                            }
                        }
                    }

                    //Log.i(TAG, "END mAcceptThread, socket Type: " + mSocketType);
                }
                catch (Exception)
                {
                    //e.printStackTrace();
                }

            }

            public void Cancel()
            {
                // Log.d(TAG, "Socket Type" + mSocketType + "cancel " + this);
                try
                {
                    mmServerSocket.Close();
                }
                catch (Exception)
                {
                    //Log.e(TAG, "Socket Type" + mSocketType + "close() of server failed", e);
                }
            }
        }

        /// <summary>
        /// 用于设备连接的线程
        /// </summary>
        private class ConnectThread : Java.Lang.Thread
        {
            private readonly BluetoothSocket mmSocket;
            private readonly BluetoothDevice mmDevice;
            //private readonly string mSocketType;

            public ConnectThread(BluetoothDevice device)
            {
                mmDevice = device;
                BluetoothSocket tmp = null;

                // 为与给定BluetoothDevice的连接获取BluetoothSocket
                try
                {
                    //bthSocket = device?.CreateRfcommSocketToServiceRecord(UUID.FromString("0001101-0000-1000-8000-00805F9B34FB"));
                    tmp = device.CreateRfcommSocketToServiceRecord(MY_UUID);
                }
                catch (Exception)
                {
                    //Log.e(TAG, "Socket Type: " + mSocketType + "create() failed", e);
                }

                mmSocket = tmp;
            }

            public override void Run()
            {
                try
                {
                    //Log.i(TAG, "BEGIN mConnectThread SocketType:" + mSocketType);

                    //SetName("ConnectThread" + mSocketType);

                    // 总是取消发现，因为它会减慢连接速度
                    mAdapter.CancelDiscovery();

                    // 连接到BluetoothSocket
                    try
                    {
                        // 这是一个阻塞调用，仅在连接成功或出现异常时返回
                        mmSocket.Connect();
                    }
                    catch (Exception e)
                    {
                        // 发生异常，关闭套接字
                        try
                        {
                            mmSocket.Close();
                        }
                        catch (IOException)
                        {
                            //Log.e(TAG, "unable to close() " + mSocketType + " socket during connection failure", e2);
                        }

                        //连接失败通知
                        ConnectionFailed();

                        return;
                    }

                    // 重置连接线程，因为我们完成了
                    lock (this)
                    {
                        mConnectThread = null;
                    }

                    // 启动已经连接的线程
                    Connected(mmSocket, mmDevice, "");
                }
                catch (Exception)
                {
                    //e.printStackTrace();
                }
            }

            public void Cancel()
            {
                try
                {
                    mmSocket.Close();
                }
                catch (Exception)
                {
                    // Log.e(TAG, "close() of connect " + mSocketType+ " socket failed", e);
                }
            }
        }

        /// <summary>
        /// 用于设备已经建立连接的线程
        /// </summary>
        private class ConnectedThread : Java.Lang.Thread
        {
            private readonly BluetoothSocket mmSocket;
            private readonly System.IO.Stream? mmInStream;
            private readonly System.IO.Stream? mmOutStream;

            public ConnectedThread(BluetoothSocket socket)
            {
                //Log.d(TAG, "create ConnectedThread: " + socketType);
                mmSocket = socket;
                System.IO.Stream? tmpIn = null;
                System.IO.Stream? tmpOut = null;

                // 获取Bluetoolsocket输入和输出流
                try
                {
                    tmpIn = socket.InputStream;
                    tmpOut = socket.OutputStream;
                }
                catch (Exception)
                {
                    //Log.e(TAG, "temp sockets not created", e);
                }

                mmInStream = tmpIn;
                mmOutStream = tmpOut;
            }

            public override void Run()
            {
                //Log.i(TAG, "BEGIN mConnectedThread");

                //缓冲
                byte[] buffer = new byte[1024];
                int bytes;

                // 连接时继续收听InputStream
                while (true)
                {
                    try
                    {
                        // 从输入流读取
                        bytes = mmInStream.Read(buffer);
                        // 发送数据
                        //EventBus.getDefault().post(new BtMsgReadEvent(bytes, buffer));
                    }
                    catch (IOException e)
                    {
                        //Log.e(TAG, "disconnected", e);

                        ConnectionLost();

                        // 重新启动服务以重新启动侦听模式
                        BtService.Start();

                        break;
                    }
                    catch (Exception)
                    {
                        ConnectionLost();
                        BtService.Start();
                        break;
                    }
                }
            }

            /// <summary>
            /// 写入连接到输出流
            /// </summary>
            /// <param name="buffer"></param>
            public void Write(byte[] buffer)
            {
                try
                {
                    mmOutStream.Write(buffer);
                }
                catch (Exception)
                {
                    //Log.e(TAG, "Exception during write", e);
                }
            }

            /// <summary>
            /// 睡眠后写入连接的输出流
            /// </summary>
            /// <param name="buffer"></param>
            /// <param name="sleepTime"></param>
            public void Write(byte[] buffer, long sleepTime)
            {
                try
                {
                    Java.Lang.Thread.Sleep(sleepTime);
                    mmOutStream.Write(buffer);
                }
                catch (Exception)
                {
                    // Log.e(TAG, "Exception during write", e);
                }
            }

            /// <summary>
            /// 取消套接字
            /// </summary>
            public void Cancel()
            {
                try
                {
                    mmSocket.Close();
                }
                catch (Exception)
                {
                    //Log.e(TAG, "close() of connect socket failed", e);
                }
            }
        }

    }


    public class PrintMsgEvent
    {
        public int type;
        public string msg;

        public PrintMsgEvent(int type, string msg)
        {
            this.type = type;
            this.msg = msg;
        }
    }

    public class PrinterMsgType
    {
        public static int MESSAGE_STATE_CHANGE = 1;
        public static int MESSAGE_TOAST = 2;
    }

    public class GPrinterCommand
    {
        public static byte[] left = new byte[] { 0x1b, 0x61, 0x00 };// 靠左
        public static byte[] center = new byte[] { 0x1b, 0x61, 0x01 };// 居中
        public static byte[] right = new byte[] { 0x1b, 0x61, 0x02 };// 靠右
        public static byte[] bold = new byte[] { 0x1b, 0x45, 0x01 };// 选择加粗模式
        public static byte[] bold_cancel = new byte[] { 0x1b, 0x45, 0x00 };// 取消加粗模式
        public static byte[] text_normal_size = new byte[] { 0x1d, 0x21, 0x00 };// 字体不放大
        public static byte[] text_big_height = new byte[] { 0x1b, 0x21, 0x10 };// 高加倍
        public static byte[] text_big_size = new byte[] { 0x1d, 0x21, 0x11 };// 宽高加倍
        public static byte[] reset = new byte[] { 0x1b, 0x40 };//复位打印机
        public static byte[] print = new byte[] { 0x0a };//打印并换行
        public static byte[] under_line = new byte[] { 0x1b, 0x2d, 2 };//下划线
        public static byte[] under_line_cancel = new byte[] { 0x1b, 0x2d, 0 };//下划线

        /**
         * 走纸
         * @param n 行数
         * @return 命令
         */
        public static byte[] WalkPaper(byte n)
        {
            return new byte[] { 0x1b, 0x64, n };
        }

        /**
         * 设置横向和纵向移动单位
         *
         * @param x 横向移动
         * @param y 纵向移动
         * @return 命令
         */
        public static byte[] Move(byte x, byte y)
        {
            return new byte[] { 0x1d, 0x50, x, y };
        }

    }

}