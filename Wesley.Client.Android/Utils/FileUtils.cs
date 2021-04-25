using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Java.IO;
using Java.Lang;
using Java.Nio.Channels;
using Java.Text;
using Java.Util;
using Java.Util.Zip;
using System;
using System.Collections.Generic;
using System.Text;
using StringBuilder = System.Text.StringBuilder;


namespace Wesley.Client.Droid.Utils
{
    /// <summary>
    /// （实验性：待提交到Wesley.logger里程碑)）
    /// </summary>
    public class Constant
    {
        public static System.String PATH_DATA = FileUtils.CreateRootPath(AppUtils.GetAppContext()) + "/cache";

        public static System.String PATH_COLLECT = FileUtils.CreateRootPath(AppUtils.GetAppContext()) + "/collect";

        public static System.String PATH_TXT = PATH_DATA + "/dcms/";

        public static System.String BASE_PATH = AppUtils.GetAppContext().CacheDir.Path;

        public const System.String SUFFIX_TXT = ".txt";
        public const System.String SUFFIX_PDF = ".pdf";
        public const System.String SUFFIX_EPUB = ".epub";
        public const System.String SUFFIX_ZIP = ".zip";
        public const System.String SUFFIX_CHM = ".chm";
    }

    /// <summary>
    /// （实验性：待提交到Wesley.logger里程碑)）
    /// </summary>
    public class FileUtils
    {
        static string BASEDIR;

        public static string GetPathOPF(string unzipDir)
        {
            string mPathOPF = "";
            try
            {
                BufferedReader br = new BufferedReader(new InputStreamReader(new System.IO.FileStream(unzipDir
                        + "/META-INF/container.xml", System.IO.FileMode.Open), "UTF-8"));
                string line;
                while ((line = br.ReadLine()) != null)
                {
                    if (line.Contains("full-path"))
                    {
                        int start = line.IndexOf("full-path");
                        int start2 = line.IndexOf('\"', start);
                        int stop2 = line.IndexOf('\"', start2 + 1);
                        if (start2 > -1 && stop2 > start2)
                        {
                            mPathOPF = line.Substring(start2 + 1, stop2).Trim();
                            break;
                        }
                    }
                }
                br.Close();

                if (!mPathOPF.Contains("/"))
                {
                    return null;
                }

                int last = mPathOPF.LastIndexOf('/');
                if (last > -1)
                {
                    mPathOPF = mPathOPF.Substring(0, last);
                }

                return mPathOPF;
            }
            catch (Java.Lang.Exception )
            {

            }
            return mPathOPF;
        }

        public static bool CheckOPFInRootDirectory(string unzipDir)
        {
            string mPathOPF = "";
            bool status = false;
            try
            {
                BufferedReader br = new BufferedReader(new InputStreamReader(new System.IO.FileStream(unzipDir
                        + "/META-INF/container.xml", System.IO.FileMode.Open), "UTF-8"));
                string line;
                while ((line = br.ReadLine()) != null)
                {
                    if (line.Contains("full-path"))
                    {
                        int start = line.IndexOf("full-path");
                        int start2 = line.IndexOf('\"', start);
                        int stop2 = line.IndexOf('\"', start2 + 1);
                        if (start2 > -1 && stop2 > start2)
                        {
                            mPathOPF = line.Substring(start2 + 1, stop2).Trim();
                            break;
                        }
                    }
                }
                br.Close();

                if (!mPathOPF.Contains("/"))
                {
                    status = true;
                }
                else
                {
                    status = false;
                }
            }
            catch (Java.Lang.Exception)
            {

            }
            return status;
        }

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="inputZip"></param>
        /// <param name="destinationDirectory"></param>
        public static void UnzipFile(string inputZip, string destinationDirectory)
        {

            int buffer = 2048;
            List<string> zipFiles = new List<string>();
            File sourceZipFile = new File(inputZip);
            File unzipDirectory = new File(destinationDirectory);

            CreateDir(unzipDirectory.AbsolutePath);

            ZipFile zipFile;
            zipFile = new ZipFile(sourceZipFile, ZipFile.OpenRead);
            IEnumeration zipFileEntries = zipFile.Entries();

            while (zipFileEntries.HasMoreElements)
            {

                ZipEntry entry = (ZipEntry)zipFileEntries.NextElement();
                string currentEntry = entry.Name;
                File destFile = new File(unzipDirectory, currentEntry);

                if (currentEntry.EndsWith(Constant.SUFFIX_ZIP))
                {
                    zipFiles.Add(destFile.AbsolutePath);
                }

                File destinationParent = destFile.ParentFile;
                CreateDir(destinationParent.AbsolutePath);

                if (!entry.IsDirectory)
                {

                    if (destFile != null && destFile.Exists())
                    {
                        continue;
                    }

                    BufferedInputStream inputStream = new BufferedInputStream(zipFile.GetInputStream(entry));
                    int currentByte;
                    // buffer for writing file
                    byte[] data = new byte[buffer];

                    var fos = new System.IO.FileStream(destFile.AbsolutePath, System.IO.FileMode.OpenOrCreate);
                    BufferedOutputStream dest = new BufferedOutputStream(fos, buffer);

                    while ((currentByte = inputStream.Read(data, 0, buffer)) != -1)
                    {
                        dest.Write(data, 0, currentByte);
                    }
                    dest.Flush();
                    dest.Close();
                    inputStream.Close();
                }
            }
            zipFile.Close();

            foreach (var zipName in zipFiles)
            {
                UnzipFile(zipName, destinationDirectory + File.SeparatorChar
                        + zipName.Substring(0, zipName.LastIndexOf(Constant.SUFFIX_ZIP)));
            }
        }

        /// <summary>
        /// 读取Assets文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static byte[] ReadAssets(string fileName)
        {
            if (fileName == null || fileName.Length <= 0)
            {
                return null;
            }
            byte[] buffer = null;
            try
            {
                var fin = AppUtils.GetAppContext().Assets.Open("uploader" + fileName);
                buffer = new byte[fin.Length];
                fin.Read(buffer, 0, (int)fin.Length);
                fin.Close();
            }
            catch (Java.Lang.Exception e)
            {
                e.PrintStackTrace();
            }
            return buffer;
        }

        /// <summary>
        /// 创建根缓存目录
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string CreateRootPath(Context context)
        {
            string cacheRootPath = "";
            if (IsSdCardAvailable())
            {
                // /sdcard/Android/data/<application package>/cache
                cacheRootPath = context.ExternalCacheDir.Path;
            }
            else
            {
                // /data/data/<application package>/cache
                cacheRootPath = context.CacheDir.Path;
            }
            return cacheRootPath;
        }

        /// <summary>
        /// SdCard是否可用
        /// </summary>
        /// <returns></returns>
        public static bool IsSdCardAvailable()
        {
            return Android.OS.Environment.MediaMounted.Equals(Android.OS.Environment.ExternalStorageState);
        }

        /// <summary>
        /// 递归创建文件夹
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static string CreateDir(string dirPath)
        {
            try
            {
                File file = new File(dirPath);
                if (file.ParentFile.Exists())
                {
                    file.Mkdir();
                    return file.AbsolutePath;
                }
                else
                {
                    CreateDir(file.ParentFile.AbsolutePath);
                    file.Mkdir();
                }
            }
            catch (Java.Lang.Exception e)
            {
                e.PrintStackTrace();
            }
            return dirPath;
        }

        /// <summary>
        /// 递归创建文件夹
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string CreateFile(File file)
        {
            try
            {
                if (file.ParentFile.Exists())
                {
                    file.CreateNewFile();
                    return file.AbsolutePath;
                }
                else
                {
                    CreateDir(file.ParentFile.AbsolutePath);
                    file.CreateNewFile();
                }
            }
            catch (Java.Lang.Exception e)
            {
                e.PrintStackTrace();
            }
            return "";
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        /// <param name="isAppend"></param>
        public static void WriteFile(string filePath, string content, bool isAppend)
        {
            try
            {
                FileOutputStream fout = new FileOutputStream(filePath, isAppend);
                byte[] bytes = Encoding.UTF8.GetBytes(content);
                fout.Write(bytes);
                fout.Close();
            }
            catch (Java.Lang.Exception e)
            {
                e.PrintStackTrace();
            }
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="filePathAndName"></param>
        /// <param name="fileContent"></param>
        public static void WriteFile(string filePathAndName, string fileContent)
        {
            try
            {
                var outstream = new System.IO.FileStream(filePathAndName, System.IO.FileMode.OpenOrCreate);
                OutputStreamWriter outSW = new OutputStreamWriter(outstream);
                outSW.Write(fileContent);
                outSW.Close();
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
            }
        }

        /// <summary>
        /// 获取Raw下的文件内容
        /// </summary>
        /// <param name="context"></param>
        /// <param name="resId"></param>
        /// <returns></returns>
        public static string GetFileFromRaw(Context context, int resId)
        {
            if (context == null)
            {
                return null;
            }

            StringBuilder s = new StringBuilder();
            try
            {
                InputStreamReader inStream = new InputStreamReader(context.Resources.OpenRawResource(resId));
                BufferedReader br = new BufferedReader(inStream);
                string line;
                while ((line = br.ReadLine()) != null)
                {
                    s.Append(line);
                }
                return s.ToString();
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
                return null;
            }
        }

        /// <summary>
        /// 获取字节
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static byte[] GetBytesFromFile(File f)
        {
            if (f == null)
            {
                return null;
            }
            try
            {
                FileInputStream stream = new FileInputStream(f);
                ByteArrayOutputStream outStream = new ByteArrayOutputStream(1000);
                byte[] b = new byte[1000];
                for (int n; (n = stream.Read(b)) != -1;)
                {
                    outStream.Write(b, 0, n);
                }
                stream.Close();
                outStream.Close();
                return outStream.ToByteArray();
            }
            catch (IOException)
            {
            }
            return null;
        }

        /// <summary>
        /// 文件拷贝
        /// </summary>
        /// <param name="src">源</param>
        /// <param name="desc">目标</param>
        public static void FileChannelCopy(File src, File desc)
        {
            //createFile(src);
            CreateFile(desc);
            FileInputStream fi = null;
            FileOutputStream fo = null;
            try
            {
                fi = new FileInputStream(src);
                fo = new FileOutputStream(desc);
                FileChannel inStream = fi.Channel;//得到对应的文件通道
                FileChannel outStream = fo.Channel;//得到对应的文件通道
                inStream.TransferTo(0, inStream.Size(), outStream);//连接两个通道，并且从in通道读取，然后写入out通道
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
            }
            finally
            {
                try
                {
                    if (fo != null) fo.Close();
                    if (fi != null) fi.Close();
                }
                catch (IOException e)
                {
                    e.PrintStackTrace();
                }
            }
        }

        /// <summary>
        /// 转换文件大小
        /// </summary>
        /// <param name="fileLen"></param>
        /// <returns></returns>
        public static string FormatFileSizeToString(long fileLen)
        {
            DecimalFormat df = new DecimalFormat("0.00");
            string fileSizestring = "";
            if (fileLen < 1024)
            {
                fileSizestring = df.Format((double)fileLen) + "B";
            }
            else if (fileLen < 1048576)
            {
                fileSizestring = df.Format((double)fileLen / 1024) + "K";
            }
            else if (fileLen < 1073741824)
            {
                fileSizestring = df.Format((double)fileLen / 1048576) + "M";
            }
            else
            {
                fileSizestring = df.Format((double)fileLen / 1073741824) + "G";
            }
            return fileSizestring;
        }

        /// <summary>
        /// 删除指定文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool DeleteFile(File file)
        {
            return DeleteFileOrDirectory(file);
        }

        /// <summary>
        /// 删除指定文件，如果是文件夹，则递归删除
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool DeleteFileOrDirectory(File file)
        {
            try
            {
                if (file != null && file.IsFile)
                {
                    return file.Delete();
                }
                if (file != null && file.IsDirectory)
                {
                    File[] childFiles = file.ListFiles();
                    // 删除空文件夹
                    if (childFiles == null || childFiles.Length == 0)
                    {
                        return file.Delete();
                    }
                    // 递归删除文件夹下的子文件
                    for (int i = 0; i < childFiles.Length; i++)
                    {
                        DeleteFileOrDirectory(childFiles[i]);
                    }
                    return file.Delete();
                }
            }
            catch (Java.Lang.Exception e)
            {
                e.PrintStackTrace();
            }
            return false;
        }

        /// <summary>
        /// 获取文件夹大小
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static long GetFolderSize(string dir)
        {
            File file = new File(dir);
            long size = 0;
            try
            {
                File[] fileList = file.ListFiles();
                for (int i = 0; i < fileList.Length; i++)
                {
                    // 如果下面还有文件
                    if (fileList[i].IsDirectory)
                    {
                        size += GetFolderSize(fileList[i].AbsolutePath);
                    }
                    else
                    {
                        size += fileList[i].Length();
                    }
                }
            }
            catch (Java.Lang.Exception e)
            {
                e.PrintStackTrace();
            }
            return size;
        }

        /// <summary>
        /// 获取文件扩展名
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetExtensionName(string filename)
        {
            if ((filename != null) && (filename.Length > 0))
            {
                int dot = filename.LastIndexOf('.');
                if ((dot > -1) && (dot < (filename.Length - 1)))
                {
                    return filename.Substring(dot + 1);
                }
            }
            return filename;
        }

        /// <summary>
        /// 获取文件内容
        /// </summary>
        /// <param name="path"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static string GetFileOutputString(string path, string charset)
        {
            try
            {
                File file = new File(path);
                BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(new System.IO.FileStream(file.AbsolutePath, System.IO.FileMode.Open), charset), 8192);
                StringBuilder sb = new StringBuilder();
                string line = null;
                while ((line = bufferedReader.ReadLine()) != null)
                {
                    sb.Append("\n").Append(line);
                }
                bufferedReader.Close();
                return sb.ToString();
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
            }
            return null;
        }

        /// <summary>
        /// 递归获取所有文件
        /// </summary>
        /// <param name="root"></param>
        /// <param name="ext"></param>
        private /*synchronized*/ void GetAllFiles(File root, string ext)
        {
            List<File> list = new List<File>();
            File[] files = root.ListFiles();
            if (files != null)
            {
                foreach (File f in files)
                {
                    if (f.IsDirectory)
                    {
                        GetAllFiles(f, ext);
                    }
                    else
                    {
                        if (f.Name.EndsWith(ext) && f.Length() > 50)
                            list.Add(f);
                    }
                }
            }
        }

        /// <summary>
        /// 获取字符集
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetCharset(string fileName)
        {
            BufferedInputStream bis = null;
            string charset = "GBK";
            byte[] first3Bytes = new byte[3];
            try
            {
                bool check = false;
                bis = new BufferedInputStream(new System.IO.FileStream(fileName, System.IO.FileMode.OpenOrCreate));
                bis.Mark(0);
                int read = bis.Read(first3Bytes, 0, 3);
                if (read == -1)
                    return charset;
                if (first3Bytes[0] == (byte)0xFF && first3Bytes[1] == (byte)0xFE)
                {
                    charset = "UTF-16LE";
                    check = true;
                }
                else if (first3Bytes[0] == (byte)0xFE
                      && first3Bytes[1] == (byte)0xFF)
                {
                    charset = "UTF-16BE";
                    check = true;
                }
                else if (first3Bytes[0] == (byte)0xEF
                      && first3Bytes[1] == (byte)0xBB
                      && first3Bytes[2] == (byte)0xBF)
                {
                    charset = "UTF-8";
                    check = true;
                }
                bis.Mark(0);
                if (!check)
                {
                    while ((read = bis.Read()) != -1)
                    {
                        if (read >= 0xF0)
                            break;
                        if (0x80 <= read && read <= 0xBF) // 单独出现BF以下的，也算是GBK
                            break;
                        if (0xC0 <= read && read <= 0xDF)
                        {
                            read = bis.Read();
                            if (0x80 <= read && read <= 0xBF) // 双字节 (0xC0 - 0xDF)
                                                              // (0x80 - 0xBF),也可能在GB编码内
                                continue;
                            else
                                break;
                        }
                        else if (0xE0 <= read && read <= 0xEF)
                        {// 也有可能出错，但是几率较小
                            read = bis.Read();
                            if (0x80 <= read && read <= 0xBF)
                            {
                                read = bis.Read();
                                if (0x80 <= read && read <= 0xBF)
                                {
                                    charset = "UTF-8";
                                    break;
                                }
                                else
                                    break;
                            }
                            else
                                break;
                        }
                    }
                }
            }
            catch (Java.Lang.Exception e)
            {
                e.PrintStackTrace();
            }
            finally
            {
                if (bis != null)
                {
                    try
                    {
                        bis.Close();
                    }
                    catch (IOException e)
                    {
                        e.PrintStackTrace();
                    }
                }
            }

            return charset;
        }

        public static string GetCharset1(string fileName)
        {
            BufferedInputStream bin = new BufferedInputStream(new System.IO.FileStream(fileName, System.IO.FileMode.OpenOrCreate));
            int p = (bin.Read() << 8) + bin.Read();
            string code = p switch
            {
                0xefbb => "UTF-8",
                0xfffe => "Unicode",
                0xfeff => "UTF-16BE",
                _ => "GBK",
            };
            return code;
        }

        /// <summary>
        /// 保存Wifi文本
        /// </summary>
        /// <param name="src"></param>
        /// <param name="desc"></param>
        public static void SaveWifiTxt(string src, string desc)
        {
            byte[] LINE_END = Encoding.ASCII.GetBytes("\n");
            try
            {
                InputStreamReader isr = new InputStreamReader(new System.IO.FileStream(src, System.IO.FileMode.OpenOrCreate), GetCharset(src));
                BufferedReader br = new BufferedReader(isr);

                FileOutputStream fout = new FileOutputStream(desc, true);
                string temp;
                while ((temp = br.ReadLine()) != null)
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(temp);
                    fout.Write(bytes);
                    fout.Write(LINE_END);
                }
                br.Close();
                fout.Close();
            }
            catch (FileNotFoundException e)
            {
                e.PrintStackTrace();
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
            }
        }

        public static void WriteLog(System.Exception ex)
        {
            SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd");
            string fileName = format.Format(new Date()) + ".log";
            File file = new File(CreateRootPath(MainApplication.Context) + "/log/dcms_" + fileName);
            CreateFile(file);
            WriteFile(file.AbsolutePath, ex.ToFormattedString());
        }


        public static bool CreateFolder()
        {
            string path = GetBaseFolderPath();
            BASEDIR = System.IO.Path.Combine(path, "WorkManager", "Logs");

            try
            {
                if (!System.IO.Directory.Exists(BASEDIR))
                {
                    System.IO.Directory.CreateDirectory(BASEDIR);
                }
                return true;
            }
            catch (Java.Lang.Exception ex)
            {
                Android.Util.Log.Debug("HATA", ex.Message);
                return false;
            }
        }

        public static void WriteLog(string log)
        {
            string fileName = DateTime.Now.ToString("yyyyMMdd") + "-report.log";
            string filePath = System.IO.Path.Combine(BASEDIR, fileName);

            System.IO.StreamWriter writer = null;
            try
            {
                writer = new System.IO.StreamWriter(filePath, true);
                //string data = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                writer.WriteLineAsync(log);
            }
            catch (Java.Lang.Exception ex)
            {
                Android.Util.Log.Debug("HATA", ex.Message);
            }
            finally
            {
                if (writer != null)
                    writer.Dispose();
            }
        }

        public static string GetBaseFolderPath(bool getSDPath = false)
        {
            string baseFolderPath = "";
            try
            {
                Context context = Application.Context;
                Java.IO.File[] dirs = context.GetExternalFilesDirs(null);
                foreach (Java.IO.File folder in dirs)
                {
                    baseFolderPath = folder.Path;
                    bool IsRemovable = Android.OS.Environment.InvokeIsExternalStorageRemovable(folder);
                    bool IsEmulated = Android.OS.Environment.InvokeIsExternalStorageEmulated(folder);

                    if (getSDPath ? IsRemovable && !IsEmulated : !IsRemovable && IsEmulated)
                        baseFolderPath = folder.Path;
                }
            }
            catch (Java.Lang.Exception) { }
            return baseFolderPath;
        }
    }
}