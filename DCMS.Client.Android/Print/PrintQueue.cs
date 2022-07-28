using Android.Bluetooth;
using Android.Content;
using System;
using System.Collections.Generic;
using System.Linq;


namespace DCMS.Client.Droid
{
    /// <summary>
    /// 打印队列
    /// </summary>
    public class PrintQueue
    {
        private static object Lock = new object();

        /// <summary>
        /// 队列实例
        /// </summary>
        private static PrintQueue mInstance;

        /// <summary>
        /// 上下文
        /// </summary>
        private static Context mContext;

        /// <summary>
        /// 打印队列
        /// </summary>
        private List<byte[]> mQueue;

        /// <summary>
        /// 蓝牙适配器
        /// </summary>
        private BluetoothAdapter mAdapter;

        /// <summary>
        /// 蓝牙服务
        /// </summary>
        private BtService mBtService;

        private PrintQueue() { }

        /// <summary>
        /// 获取打印队列
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static PrintQueue GetQueue(Context context)
        {
            if (null == mInstance)
            {
                mInstance = new PrintQueue();
            }
            if (null == mContext)
            {
                mContext = context;
            }
            return mInstance;
        }

        /// <summary>
        /// 将打印字节添加到队列
        /// </summary>
        /// <param name="bytes"></param>
        public void Add(byte[] bytes)
        {
            lock (Lock)
            {
                if (null == mQueue)
                {
                    mQueue = new List<byte[]>();
                }
                if (null != bytes)
                {
                    mQueue.Add(bytes);
                }
                Print();
            }
        }

        /// <summary>
        /// 将打印字节添加到队列
        /// </summary>
        /// <param name="bytesList"></param>
        public void Add(List<byte[]> bytesList)
        {
            lock (Lock)
            {
                if (null == mQueue)
                {
                    mQueue = new List<byte[]>();
                }
                if (null != bytesList)
                {
                    mQueue.AddRange(bytesList);
                }
                Print();
            }
        }

        /// <summary>
        /// 打印队列
        /// </summary>
        public void Print()
        {
            lock (Lock)
            {
                try
                {
                    if (null == mQueue || mQueue.Count() <= 0)
                    {
                        return;
                    }
                    if (null == mAdapter)
                    {
                        mAdapter = BluetoothAdapter.DefaultAdapter;
                    }
                    if (null == mBtService)
                    {
                        mBtService = new BtService(mContext);
                    }

                    //没有连接时，重新连接
                    if (mBtService.GetState() != BtService.STATE_CONNECTED)
                    {
                        if (!string.IsNullOrEmpty(App.BtAddress))
                        {
                            BluetoothDevice device = mAdapter.GetRemoteDevice(App.BtAddress);
                            //连接
                            mBtService.Connect(device);

                            return;
                        }
                    }

                    while (mQueue.Count() > 0)
                    {
                        mBtService.Write(mQueue[0]);
                        mQueue.RemoveAt(0);
                    }
                }
                catch (Exception)
                {
                    //e.printStackTrace();
                }
            }
        }

        public void Clear()
        {
            if (mQueue != null && mQueue.Any())
            {
                mQueue.Clear();
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (mQueue != null && mQueue.Any())
                {
                    mQueue.Clear();
                }

                if (null != mBtService)
                {
                    mBtService.Stop();
                    mBtService = null;
                }

                if (null != mAdapter)
                {
                    mAdapter = null;
                }
            }
            catch (Exception e)
            {
                //e.printStackTrace();
            }
        }

        /// <summary>
        /// 更改蓝牙状态时，如果打印机正在使用，连接它，否则什么都不做
        /// </summary>
        public void TryConnect()
        {
            try
            {
                if (string.IsNullOrEmpty(App.BtAddress))
                {
                    return;
                }
                if (null == mAdapter)
                {
                    mAdapter = BluetoothAdapter.DefaultAdapter;
                }
                if (null == mAdapter)
                {
                    return;
                }
                if (null == mBtService)
                {
                    mBtService = new BtService(mContext);
                }
                if (mBtService.GetState() != BtService.STATE_CONNECTED)
                {
                    if (!string.IsNullOrEmpty(App.BtAddress))
                    {
                        BluetoothDevice device = mAdapter.GetRemoteDevice(App.BtAddress);
                        mBtService.Connect(device);
                        return;
                    }
                }
                else
                {


                }
            }
            catch (Exception e)
            {
                //e.printStackTrace();
            }

        }

        /// <summary>
        /// 将打印命令发送给打印机
        /// </summary>
        /// <param name="bytes"></param>
        public void Write(byte[] bytes)
        {
            try
            {
                if (null == bytes || bytes.Length <= 0)
                {
                    return;
                }
                if (null == mAdapter)
                {
                    mAdapter = BluetoothAdapter.DefaultAdapter;
                }
                if (null == mBtService)
                {
                    mBtService = new BtService(mContext);
                }
                if (mBtService.GetState() != BtService.STATE_CONNECTED)
                {
                    if (!string.IsNullOrEmpty(App.BtAddress))
                    {
                        BluetoothDevice device = mAdapter.GetRemoteDevice(App.BtAddress);
                        mBtService.Connect(device);
                        return;
                    }
                }
                mBtService.Write(bytes);
            }
            catch (Exception e)
            {
                //e.printStackTrace();
            }
        }
    }
}