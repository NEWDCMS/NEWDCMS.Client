using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.Content;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Environment = Android.OS.Environment;
using Exception = Java.Lang.Exception;
using File = Java.IO.File;
using Uri = Android.Net.Uri;
using System.Reflection;

namespace DCMS.Client.Droid.AutoUpdater
{

    /// <summary>
    /// 比较接收版本
    /// </summary>
    public static class CompareVersionReceiver
    {
        public static int VersionCompare(string oldVersion, string newVersion)
        {
            try
            {
                if (!string.IsNullOrEmpty(oldVersion) && !string.IsNullOrEmpty(newVersion))
                {
                    Version oldv = new Version(oldVersion);
                    Version newv = new Version(newVersion);
                    var cc = newv.CompareTo(oldv);
                    return cc;
                }
                return -1;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"{ex.Message}");
                return -1;
            }
        }
    }

    public static class AutoUpdate
    {
        public static Activity Context { get; set; }
        public static string Authority { get; set; }
        public static void Init(Activity activity, string fileProviderAuthority)
        {
            Context = activity;
            Authority = fileProviderAuthority;
        }
    }

    public static class UpdateConfig
    {
        public static Action<Activity, string> AlertFunc = null;
        internal static void Alert(Activity activity, string msg)
        {
            AlertFunc?.Invoke(activity, msg);
        }
    }

    public class UpdateUtils
    {
        public static string GetVersion(Activity activity)
        {
            try
            {
                PackageManager manager = activity.PackageManager;
                PackageInfo info = manager.GetPackageInfo(activity.PackageName, 0);
                string version = info.VersionName;
                return version;
            }
            catch (Exception)
            {
                return "";
            }
        }


        public bool CheckNewVersion(Activity activity, UpdateInfo updateInfo)
        {
            string installedVersion = GetVersion(activity);
            if (CompareVersionReceiver.VersionCompare(installedVersion, updateInfo.Version) > 0)
                return true;
            else
                return false;
        }

        public void Update(Activity activity, UpdateInfo updateInfo)
        {
            try
            {
                string installedVersion = GetVersion(activity);
                if (!CheckNewVersion(AutoUpdate.Context, updateInfo))
                {
                    UserDialogs.Instance.Alert("当前已经是最新版本了。", "", "确定");
                }
                else
                {
                    //更新新版本
                    StarNewVersionUpdateV2(activity, updateInfo);
                }
            }
            catch (Exception e)
            {
                UpdateConfig.Alert(activity, e.ToString());
            }
        }

        //public void NewVersionUpdateV2(Activity activity, UpdateInfo updateInfo)
        //{
        //    string version = GetVersion(activity);
        //    var sb = new System.Text.StringBuilder();
        //    sb.AppendFormat("当前版本:{0}, 发现新版本:{1}, 是否更新?", version, updateInfo.Version);


        //    Dialog dialog = new AlertDialog.Builder(activity)
        //        .SetTitle("软件更新")
        //        .SetMessage(sb.ToString())
        //        //确定
        //        .SetPositiveButton("更新下载", async (s, e) =>
        //        {
        //            var cancelled = false;
        //            using (var dlg = UserDialogs.Instance.Progress(null, () => cancelled = true, cancelText: "取消"))
        //            {
        //                //下载任务
        //                DownloadFileV2(activity, dlg, updateInfo);
        //                while (!cancelled && dlg.PercentComplete < 100)
        //                {
        //                    await Task.Delay(TimeSpan.FromMilliseconds(500));
        //                }
        //            }
        //        })
        //        //取消
        //        .SetNegativeButton("以后更新", (s, e) =>
        //        {

        //        }).Create();

        //    dialog.Show();
        //}


        /// <summary>
        /// 直接下载更新
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="updateInfo"></param>
        public async void StarNewVersionUpdateV2(Activity activity, UpdateInfo updateInfo)
        {
            var cancelled = false;
            using (var dlg = UserDialogs.Instance.Progress("下载中", () => cancelled = true, cancelText: "取消"))
            {
                //下载任务
                DownloadFileV2(activity, dlg, updateInfo);
                while (!cancelled && dlg.PercentComplete < 100)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(500));
                }
            }
        }


        /// <summary>
        /// 下载更新并安装
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="updateInfo"></param>
        private void DownloadFileV2(Activity activity, IProgressDialog pDialog, UpdateInfo updateInfo)
        {
            try
            {
                //string directory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                //完整的最新应用程序文件名
                string flaName;
                bool downloadedOK = false;

                //判断文件目录是否存在
                //==  Download/apk/

                // "/storage/emulated/0/Android/data/com.dcms.clientv3/files/Download/apk"
                // "/storage/emulated/0/Android/data/com.dcms.clientv3/files/Download"
                using (var tempFile = new File(Application.Context.GetExternalFilesDir(Environment.DirectoryDownloads), "apk"))
                {
                    if (!tempFile.Exists())
                    {
                        //创建目录
                        try
                        {
                            tempFile.Mkdir();
                        }
                        catch (Exception e)
                        {
                            e.PrintStackTrace();
                        }
                    }

                    flaName = tempFile.Path + "/" + updateInfo.Version + ".apk";
                }


                //外部存储路径下的apk文件
                File file = new File(flaName);
                //防止文件过多
                if (file.Exists())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (Exception ex)
                    {
                        ex.PrintStackTrace();
                    }
                }

                //默认：http://storage.jsdcms.com:5000/api/version/updater/app/download
                using (var webClient = new WebClient())
                {
                    //进度
                    webClient.DownloadProgressChanged += (s, e) =>
                    {
                        var bytesIn = double.Parse(e.BytesReceived.ToString());
                        var totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                        if (totalBytes == 0) totalBytes = 1;
                        var percentage = (int)(bytesIn / totalBytes * 100);
                        if (percentage == 100)
                        {
                            downloadedOK = true;
                        }
                        activity.RunOnUiThread(() =>
                        {
                            if (pDialog != null)
                                pDialog.PercentComplete = percentage;
                        });
                    };

                    //完成
                    webClient.DownloadDataCompleted += (s, e) =>
                    {
                        activity.RunOnUiThread(() =>
                        {
                            try
                            {
                                //关闭ProgressDialog
                                if (pDialog != null)
                                    pDialog.Hide();

                                //写入流
                                if (e.Result.Length > 0 && e.Error == null && e.Cancelled == false)
                                {
                                    byte[] buffer = e.Result;
                                    using (var sfs = new FileStream(flaName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                                    {
                                        using (MemoryStream ms = new MemoryStream(buffer))
                                        {
                                            ms.CopyTo(sfs);
                                        }
                                    }
                                }

                                //外部存储路径下的apk文件
                                if (!file.Exists())
                                {
                                    return;
                                }
                                else
                                {
                                    //通过在代码中写入linux指令修改此apk文件的权限，改为全局可读可写可执行
                                    string[] command = { "chmod", "777", file.Path };
                                    Java.Lang.ProcessBuilder builder = new Java.Lang.ProcessBuilder(command);
                                    try
                                    {
                                        builder.Start();
                                    }
                                    catch (Java.IO.IOException ex)
                                    {
                                        ex.PrintStackTrace();
                                    }
                                }

                                //是否下载
                                if (downloadedOK)
                                {
                                    var context = Application.Context;
                                    Uri uri;

                                    try
                                    {
                                        //安装
                                        var installApk = new Intent(Intent.ActionView);
                                        installApk.SetFlags(ActivityFlags.NewTask);

                                        if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                                        {
                                            uri = FileProvider.GetUriForFile(context, AutoUpdate.Authority, file);
                                            //Intent.ActionInstallPackage
                                            installApk.SetAction("android.intent.action.INSTALL_PACKAGE");
                                            installApk.SetDataAndType(uri, "application/vnd.android.package-archive");
                                            installApk.AddFlags(ActivityFlags.GrantPersistableUriPermission);
                                            installApk.AddFlags(ActivityFlags.GrantPrefixUriPermission);
                                            installApk.AddFlags(ActivityFlags.GrantWriteUriPermission);
                                            installApk.AddFlags(ActivityFlags.GrantReadUriPermission);
                                        }
                                        else
                                        {
                                            uri = Uri.FromFile(file);
                                            installApk.SetDataAndType(uri, "application/vnd.android.package-archive");
                                        }

                                        context.StartActivity(installApk);
                                    }
                                    catch (ActivityNotFoundException)
                                    {
                                        var errorInstalled = new AlertDialog.Builder(context).Create();
                                        errorInstalled.SetTitle("出了点问题");
                                        errorInstalled.SetMessage(string.Format("{0} 不能被安装,请重试", "DCMS " + updateInfo.Version));
                                        errorInstalled.Show();
                                    }
                                    downloadedOK = false;
                                }
                                else
                                {
                                    try
                                    {  //删除
                                        System.IO.File.Delete(flaName);
                                    }
                                    catch (ActivityNotFoundException ex)
                                    {
                                        ex.PrintStackTrace();
                                    }
                                }

                            }
                            catch (TargetInvocationException) { }
                            catch (WebException) { }
                            catch (Exception) { }
                            finally
                            {
                                if (file != null)
                                    file.Dispose();
                            }
                        });

                    };

                    //开始异步下载
                    //application/octet-stream
                    webClient.DownloadDataAsync(new System.Uri(updateInfo.DownLoadUrl), flaName);
                };
            }
            catch (System.Net.Sockets.SocketException ex)
            { }
            catch (Exception ex)
            { }
        }

        public async void StarInit()
        {
            var cancelled = false;
            using (var dlg = UserDialogs.Instance.Progress("初始化", () => cancelled = true, cancelText: "取消"))
            {
                while (!cancelled && dlg.PercentComplete < 100)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(500));
                }
            }
        }
    }
}
