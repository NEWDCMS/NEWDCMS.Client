using Acr.UserDialogs;
using Android;
using Android.Bluetooth;
using Android.OS;
using AndroidX.Core.Content;
using DCMS.Client.Droid.Services;
using DCMS.Client.Models;
using DCMS.Client.Models.Global;
using DCMS.Client.Services;
using Google.Android.Material.Snackbar;
using Java.Util;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;


[assembly: Xamarin.Forms.Dependency(typeof(AndroidBlueToothService))]
namespace DCMS.Client.Droid.Services
{
    public class AndroidBlueToothService : IBlueToothService
    {
        private CancellationTokenSource _cancellationToken { get; set; }

        public AbstractBill Data { get; set; } = null;

        #region 蓝牙

        public async void Print(AbstractBill bill, int type = 58, int repeatPrintNum = 1)
        {
            try
            {
                if (repeatPrintNum == 0)
                    repeatPrintNum = 1;

                if (repeatPrintNum > 2)
                    repeatPrintNum = 2;

                if (bill == null)
                {
                    //测试
                    var content = MainActivity.Instance.ApplicationContext;
                    PrintQueue.GetQueue(content).Clear();
                    var podm = new PrintOrderDataMaker(content, "", type, PrinterWriter.HEIGHT_PARTING_DEFAULT);
                    if (podm != null)
                    {
                        var printData = podm.GetPrintData(type);
                        if (printData != null && printData.Any())
                        {
                            for (int p = 0; p < repeatPrintNum; p++)
                            {
                                PrintQueue.GetQueue(content).Add(printData);
                                //System.Diagnostics.Debug.Print($"Print---------------------->{p}");
                            }
                        }
                    }
                }
                else
                {
                    var content = MainActivity.Instance.ApplicationContext;
                    PrintQueue.GetQueue(content).Clear();

                    var podm = new PrintOrderDataMaker(content, "", type, PrinterWriter.HEIGHT_PARTING_DEFAULT);
                    if (podm != null)
                    {
                        var printData = podm.GetBillPrintData(type, bill);
                        if (printData != null && printData.Any())
                        {
                            for (int p = 0; p < repeatPrintNum; p++)
                            {
                                PrintQueue.GetQueue(content).Add(printData);
                                //System.Diagnostics.Debug.Print($"Print---------------------->{p}");
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                await UserDialogs.Instance.AlertAsync("拒绝操作，请确保打印机连接适配");
            }
        }

        /// <summary>
        /// 连接到设备
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<bool> ConnectDevice(Printer printer)
        {
            bool connected = false;
            if (printer == null)
                return connected;

            var address = printer.Address;

            BluetoothDevice device = null;
            BluetoothSocket bthSocket = null;
            _cancellationToken = new CancellationTokenSource();
            while (_cancellationToken.IsCancellationRequested == false)
            {
                BluetoothAdapter adapter;
                try
                {
                    Thread.Sleep(250);

                    adapter = BluetoothAdapter.DefaultAdapter;

                    if (adapter == null)
                        System.Diagnostics.Debug.Write("找不到蓝牙适配器!");
                    else
                        System.Diagnostics.Debug.Write("找到适配器!");

                    if (!adapter.IsEnabled)
                        System.Diagnostics.Debug.Write("蓝牙适配器未启用.");
                    else
                        System.Diagnostics.Debug.Write("找到适配器!");

                    foreach (var bondedDevice in adapter.BondedDevices)
                    {
                        if (bondedDevice.Address.ToUpper().IndexOf(address.ToUpper()) >= 0)
                        {
                            device = bondedDevice;
                            break;
                        }
                    }

                    if (device == null)
                        await UserDialogs.Instance.AlertAsync("找不到命名设备.");
                    else
                    {
                        bthSocket = device?.CreateRfcommSocketToServiceRecord(UUID.FromString("0001101-0000-1000-8000-00805F9B34FB"));
                        //bthSocket = bthServerSocket.Accept();

                        adapter.CancelDiscovery();

                        if (bthSocket != null)
                        {
                            bthSocket?.Connect();
                            if (bthSocket.IsConnected)
                            {
                                connected = true;
                                App.BtAddress = device.Address;
                                _cancellationToken.Cancel();
                            }
                        }
                        else
                        {
                            await UserDialogs.Instance.AlertAsync("适配不到设备，重新尝试");
                        }
                    }
                }
                catch (Exception)
                {
                    await UserDialogs.Instance.AlertAsync("适配不到设备，重新尝试", okText: "取消");
                    if (!_cancellationToken.IsCancellationRequested)
                    {
                        _cancellationToken.Cancel();
                    }
                }
                finally
                {
                    if (bthSocket != null)
                        bthSocket.Close();

                    device = null;
                }
            }

            return connected;
        }

        /// <summary>
        /// 断开
        /// </summary>
        public void Disconnect()
        {
            if (_cancellationToken != null)
            {
                _cancellationToken.Cancel();
            }
        }

        public void PrintStop()
        {
            try
            {
                var content = MainActivity.Instance.ApplicationContext;
                PrintQueue.GetQueue(content).Disconnect();
            }
            catch (Exception)
            {
                //e.printStackTrace();
            }
        }


        /// <summary>
        /// 配对设备
        /// </summary>
        /// <returns></returns>
        public ObservableRangeCollection<Printer> PairedDevices()
        {
            var devices = new ObservableRangeCollection<Printer>();

            using (var bluetoothAdapter = BluetoothAdapter.DefaultAdapter)
            {
                if (bluetoothAdapter != null)
                {
                    //判断蓝牙是否开启
                    if (!bluetoothAdapter.IsEnabled)
                        bluetoothAdapter.Enable();

                    if (!bluetoothAdapter.IsDiscovering)
                        bluetoothAdapter.StartDiscovery();


                    var btdevices = bluetoothAdapter?.BondedDevices.ToList();
                    if (btdevices != null)
                    {
                        foreach (var bd in btdevices)
                        {
                            if (bd != null)
                            {
                                devices.Add(new Printer()
                                {
                                    LocalName = bd.Name,
                                    Address = bd.Address,
                                    //"DC:1D:30:3A:37:A2"
                                    Status = App.BtAddress.Equals(bd.Address),
                                    Name = bd.Name
                                });
                            }
                        }
                    }
                }
            }

            return devices;
        }


        #endregion

        public const int REQUEST_BLUETOOTH = 1;
        private readonly string[] permissions = { Manifest.Permission.Bluetooth };
        private TaskCompletionSource<bool> tcsPermissions;
        public Task<bool> GetPermissionsAsync()
        {
            tcsPermissions = new TaskCompletionSource<bool>();
            if ((int)Build.VERSION.SdkInt < 23)
            {
                tcsPermissions.TrySetResult(true);
            }
            else
            {
                var currentActivity = MainActivity.Instance;

                if (ContextCompat.CheckSelfPermission(currentActivity, Manifest.Permission.Bluetooth) != (int)Android.Content.PM.Permission.Granted)
                {
                    RequestBluetoothPermission();
                }
                else
                {
                    tcsPermissions.TrySetResult(true);
                }
            }

            return tcsPermissions.Task;
        }

        private void RequestBluetoothPermission()
        {
            var currentActivity = MainActivity.Instance;
            if (currentActivity.ShouldShowRequestPermissionRationale(Manifest.Permission.Bluetooth))
            {
                Snackbar.Make(currentActivity.FindViewById(Android.Resource.Id.Content),
                    "应用程序需要蓝牙权限.",
                    Snackbar.LengthIndefinite).SetAction("Ok",
                    v =>
                    {
                        currentActivity.RequestPermissions(permissions, REQUEST_BLUETOOTH);
                    }).Show();
            }
            else
            {
                currentActivity.RequestPermissions(permissions, REQUEST_BLUETOOTH);
            }
        }

        public void OnRequestPermissionsResult(bool isGranted)
        {
            tcsPermissions.TrySetResult(isGranted);
        }
    }
}