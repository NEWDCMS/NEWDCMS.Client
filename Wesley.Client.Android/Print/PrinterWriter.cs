using Android.Graphics;
using Java.IO;
using Java.Lang;
using System.Collections.Generic;
using System.Text;

namespace Wesley.Client.Droid
{
    /// <summary>
    /// 打印写入器
    /// </summary>
    public abstract class PrinterWriter
    {

        /// <summary>
        /// 默认高度
        /// </summary>
        public readonly static int HEIGHT_PARTING_DEFAULT = 255;
        public readonly static string CHARSET = "gb2312";
        private ByteArrayOutputStream bos;
        private int heightParting;

        public PrinterWriter() : this(HEIGHT_PARTING_DEFAULT)
        {
        }

        public PrinterWriter(int parting)
        {
            try
            {
                if (parting <= 0 || parting > HEIGHT_PARTING_DEFAULT)
                    heightParting = HEIGHT_PARTING_DEFAULT;
                else
                    heightParting = parting;

                Init();

            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        /// <summary>
        /// 重置 使用 init 替代
        /// </summary>
        [Deprecated]
        public void Reset()
        {
            try
            {
                Init();
            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            try
            {
                bos = new ByteArrayOutputStream();
                Write(PrinterUtils.InitPrinter());
            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        /// <summary>
        /// 获取预打印数据并关闭流
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        [Deprecated]
        public byte[] GetData()
        {
            try
            {
                return GetDataAndClose();
            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        /// <summary>
        /// 获取预打印数据并重置流
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public byte[] GetDataAndReset()
        {
            try
            {
                byte[] data;
                bos.Flush();
                data = bos.ToByteArray();
                bos.Reset();
                return data;
            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        /// <summary>
        /// 获取预打印数据并关闭流
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public byte[] GetDataAndClose()
        {
            try
            {
                byte[] data;
                bos.Flush();
                data = bos.ToByteArray();
                bos.Close();
                bos = null;
                return data;
            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="data"></param>
        public void Write(byte[] data)
        {
            try
            {
                if (bos == null)
                    Reset();

                bos.Write(data);
            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        /// <summary>
        /// 设置居中
        /// </summary>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public void SetAlignCenter()
        {
            try
            {
                Write(PrinterUtils.AlignCenter());
            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        /// <summary>
        /// 设置左对齐
        /// </summary>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public void SetAlignLeft()
        {
            try
            {
                Write(PrinterUtils.AlignLeft());
            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        /// <summary>
        /// 设置右对齐
        /// </summary>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public void SetAlignRight()
        {
            try
            {
                Write(PrinterUtils.AlignRight());
            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        /// <summary>
        /// 开启着重
        /// </summary>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public void SetEmphasizedOn()
        {
            try
            {
                Write(PrinterUtils.EmphasizedOn());
            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        /// <summary>
        /// 关闭着重
        /// </summary>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public void SetEmphasizedOff()
        {
            try
            {
                Write(PrinterUtils.EmphasizedOff());
            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        /// <summary>
        /// 设置文字大小 
        /// </summary>
        /// <param name="size">文字大小 （0～7）（默认0）</param>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public void SetFontSize(int size)
        {
            try
            {
                Write(PrinterUtils.FontSizeSetBig(size));
            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        /// <summary>
        /// 设置行高度
        /// </summary>
        /// <param name="height"></param>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public void SetLineHeight(int height)
        {
            try
            {
                if (height >= 0 && height <= 255)
                    Write(PrinterUtils.PrintLineHeight((byte)height));
            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        /// <summary>
        /// 写入字符串
        /// </summary>
        /// <param name="str"></param>
        public void Print(string str)
        {
            try
            {
                Print(str, CHARSET);
            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        /// <summary>
        /// 写入字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="charsetName"></param>
        public void Print(string str, string charsetName)
        {
            try
            {
                if (str == null)
                    return;

                Write(Encoding.GetEncoding(charsetName).GetBytes(str));
            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        /// <summary>
        /// 写入一条横线
        /// </summary>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public void PrintLine()
        {
            try
            {
                int length = GetLineWidth();
                string line = "";
                while (length > 0)
                {
                    line += "- ";
                    length--;
                }

                Print(line);
            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        /// <summary>
        /// 获取横线线宽
        /// </summary>
        /// <returns></returns>
        protected abstract int GetLineWidth();

        /// <summary>
        /// 一行输出
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <param name="textSize"></param>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public void PrintInOneLine(string str1, string str2, int textSize)
        {
            try
            {
                PrintInOneLine(str1, str2, textSize, CHARSET);
            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        public void PrintInOneLine(string str1, string str2, string str3, int textSize)
        {
            try
            {
                PrintInOneLine(str1, str2, str3, CHARSET);
            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        public void PrintInOneLine(string str1, string str2, string str3, string str4, int textSize)
        {
            try
            {
                PrintInOneLine(str1, str2, str3, str4, CHARSET);
            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }
        public void PrintInOneLine(int textSize, string[] printStr, int[] strWeight, bool[] padLeft)
        {
            try
            {
                PrintInOneLine(CHARSET, printStr, strWeight, padLeft);
            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        /// <summary>
        /// 一行输出
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <param name="textSize"></param>
        /// <param name="charsetName"></param>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public void PrintInOneLine(string str1, string str2, int textSize, string charsetName)
        {
            try
            {
                int lineLength = GetLineStringWidth(textSize);
                int needEmpty = lineLength - (GetStringWidth(str1) + GetStringWidth(str2)) % lineLength;
                string empty = "";
                while (needEmpty > 0)
                {
                    empty += " ";
                    needEmpty--;
                }
                Print(str1 + empty + str2, charsetName);
            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        /// <summary>
        /// 一行输出
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <param name="str3"></param>
        /// <param name="charsetName"></param>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public void PrintInOneLine(string str1, string str2, string str3, string charsetName)
        {
            try
            {
                int lineLength = GetLineStringWidth(0);
                int needEmpty = (lineLength - (GetStringWidth(str1) + GetStringWidth(str2) + GetStringWidth(str3)) % lineLength) / 2;
                string empty = "";
                while (needEmpty > 0)
                {
                    empty += " ";
                    needEmpty--;
                }

                Print(str1 + empty + str2 + empty + str3, charsetName);
            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        [SuppressWarnings(Value = new string[] { "unused" })]
        public void PrintInOneLine(string str1, string str2, string str3, string str4, string charsetName)
        {
            try
            {
                int lineLength = GetLineStringWidth(0);
                int needEmpty = (lineLength - (GetStringWidth(str1) + GetStringWidth(str2) + GetStringWidth(str3) + GetStringWidth(str4)) % lineLength) / 2;
                string empty = "";
                while (needEmpty > 0)
                {
                    empty += " ";
                    needEmpty--;
                }
                Print(str1 + empty + str2 + empty + str3 + str4, charsetName);
            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        public void PrintInOneLine(string charsetName, string[] printStr, int[] strWeight, bool[] padLeft)
        {
            try
            {
                if (printStr == null || strWeight == null || padLeft == null)
                {
                    return;
                }
                if (printStr.Length != strWeight.Length || printStr.Length != padLeft.Length)
                {
                    return;
                }
                int lineLength = GetLineStringWidth(0);
                Dictionary<int, string> printContent = new Dictionary<int, string>();

                int countWeights = 0;
                foreach (var item in strWeight)
                {
                    countWeights += item;
                }
                double lineCell = (double)lineLength / (double)countWeights;
                double frontWidth = 0;
                for (int i = 0; i < printStr.Length; i++)
                {
                    double cellWidth = lineCell * strWeight[i];

                    double needEmpty = cellWidth - GetStringWidth(printStr[i]);
                    if (needEmpty < 0)
                    {
                        needEmpty = Math.Abs(needEmpty);
                        int subIndex = 0;
                        int warpIndex = 0;
                        bool breakFlag = false;
                        for (int j = 0; j < printStr[i].Length;)
                        {
                            string appendValue = printStr[i].Substring(subIndex, printStr[i].Length - j);
                            needEmpty = cellWidth - GetStringWidth(appendValue);
                            if (needEmpty >= 0)
                            {
                                if (needEmpty > 0)
                                {
                                    if (padLeft[i])
                                    {
                                        appendValue = appendValue.PadLeft((int)Math.Floor(needEmpty) + appendValue.Length);
                                    }
                                    else
                                    {
                                        appendValue = appendValue.PadRight((int)Math.Floor(needEmpty) + appendValue.Length);
                                    }
                                }
                                if (printContent.TryGetValue(warpIndex, out string value))
                                {
                                    double frontNeedEmpty = frontWidth - GetStringWidth(printContent[warpIndex]);
                                    if (frontNeedEmpty > 1)
                                    {
                                        if (padLeft[i])
                                        {
                                            appendValue = appendValue.PadLeft((int)Math.Floor(frontNeedEmpty) + appendValue.Length);
                                        }
                                        else
                                        {
                                            appendValue = appendValue.PadRight((int)Math.Floor(frontNeedEmpty) + appendValue.Length);
                                        }
                                    }

                                    printContent[warpIndex] += appendValue;
                                }
                                else
                                {
                                    double frontNeedEmpty = frontWidth - GetStringWidth("");
                                    if (frontNeedEmpty > 1)
                                    {
                                        if (padLeft[i])
                                        {
                                            appendValue = appendValue.PadLeft((int)Math.Floor(frontNeedEmpty) + appendValue.Length);
                                        }
                                        else
                                        {
                                            appendValue = appendValue.PadRight((int)Math.Floor(frontNeedEmpty) + appendValue.Length);
                                        }
                                    }
                                    printContent.Add(warpIndex, appendValue);
                                }
                                if (breakFlag)
                                {
                                    break;
                                }
                                j = (subIndex + printStr[i].Length - j);
                                subIndex = j;
                                warpIndex++;
                                continue;
                            }
                            j++;
                        }
                    }
                    else
                    {
                        string appendValue = printStr[i];
                        if (needEmpty > 0)
                        {
                            if (padLeft[i])
                            {
                                appendValue = appendValue.PadLeft((int)Math.Floor(needEmpty) + appendValue.Length);
                            }
                            else
                            {

                                appendValue = appendValue.PadRight((int)Math.Floor(needEmpty) + appendValue.Length);
                            }
                        }
                        if (printContent.TryGetValue(0, out string value))
                        {
                            printContent[0] += appendValue;
                        }
                        else
                        {
                            printContent.Add(0, appendValue);
                        }

                    }
                    frontWidth += cellWidth;
                }
                List<int> printKeys = new List<int>(printContent.Keys);
                for (int i = 0; i < printKeys.Count; i++)
                {
                    Print(printContent[printKeys[i]], charsetName);
                    if (i < printKeys.Count - 1)
                    {
                        PrintLineFeed();
                    }
                }

            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }


        /// <summary>
        /// 打印 Drawable 图片
        /// </summary>
        /// <param name="textSize"></param>
        /// <returns></returns>
        protected abstract int GetLineStringWidth(int textSize);

        private int GetStringWidth(string str)
        {
            try
            {
                int width = 0;
                foreach (char c in str.ToCharArray())
                {
                    width += IsChinese(c) ? 2 : 1;
                }
                return width;
            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        /// <summary>
        /// 打印 Drawable 图片
        /// </summary>
        /// <param name="res"></param>
        /// <param name="id"></param>
        [SuppressWarnings(Value = new string[] { "unused" })]
        [Deprecated]
        public void PrintDrawable(Android.Content.Res.Resources res, int id)
        {
            try
            {
                int maxWidth = GetDrawableMaxWidth();
                Bitmap image = ScalingBitmap(res, id, maxWidth);

                if (image == null)
                    return;

                byte[] command = PrinterUtils.DecodeBitmap(image, heightParting);
                image.Recycle();

                try
                {
                    if (command != null)
                    {
                        Write(command);
                    }
                }
                catch (IOException e)
                {
                    throw new IOException(e.Message);
                }

            }
            catch (System.Exception ex)
            {
                throw new IOException(ex.Message);
            }

        }

        /// <summary>
        /// 获取图片数据流
        /// </summary>
        /// <param name="res"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<byte[]> GetImageByte(Android.Content.Res.Resources res, int id)
        {
            int maxWidth = GetDrawableMaxWidth();
            Bitmap image = ScalingBitmap(res, id, maxWidth);

            if (image == null)
                return null;

            List<byte[]> data = PrinterUtils.DecodeBitmapToDataList(image, heightParting);
            image.Recycle();
            return data;
        }

        /// <summary>
        /// 获取图片最大宽度
        /// </summary>
        /// <returns></returns>
        protected abstract int GetDrawableMaxWidth();

        /// <summary>
        /// 缩放图片
        /// </summary>
        /// <param name="res"></param>
        /// <param name="id"></param>
        /// <param name="maxWidth"></param>
        /// <returns></returns>
        private Bitmap ScalingBitmap(Android.Content.Res.Resources res, int id, int maxWidth)
        {
            if (res == null)
                return null;
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;// 设置只量取宽高
            BitmapFactory.DecodeResource(res, id, options);// 量取宽高
            options.InJustDecodeBounds = false;
            // 粗略缩放
            if (maxWidth > 0 && options.OutWidth > maxWidth)
            {
                // 超过限定宽
                double ratio = options.OutWidth / (double)maxWidth;// 计算缩放比
                int sampleSize = (int)Java.Lang.Math.Floor(ratio);// 向下取整，保证缩放后不会低于最大宽高
                if (sampleSize > 1)
                {
                    options.InSampleSize = sampleSize;// 设置缩放比，原图的几分之一
                }
            }
            try
            {
                Bitmap image = BitmapFactory.DecodeResource(res, id, options);
                int width = image.Width;
                int height = image.Height;
                // 精确缩放
                if (maxWidth <= 0 || width <= maxWidth)
                {
                    return image;
                }
                float scale = maxWidth / (float)width;
                Matrix matrix = new Matrix();
                matrix.PostScale(scale, scale);
                Bitmap resizeImage = Bitmap.CreateBitmap(image, 0, 0, width, height, matrix, true);
                image.Recycle();
                return resizeImage;
            }
            catch (OutOfMemoryError e)
            {
                return null;
            }
        }

        /// <summary>
        /// 打印 Drawable 图片
        /// </summary>
        /// <param name="drawable"></param>
        [SuppressWarnings(Value = new string[] { "unused" })]
        [Deprecated]
        public void PrintDrawable(Android.Graphics.Drawables.Drawable drawable)
        {

            int maxWidth = GetDrawableMaxWidth();
            Bitmap image = ScalingDrawable(drawable, maxWidth);
            if (image == null)
                return;
            byte[]
        command = PrinterUtils.DecodeBitmap(image, heightParting);
            image.Recycle();
            try
            {
                if (command != null)
                {
                    Write(command);
                }
            }
            catch (IOException e)
            {
                throw new IOException(e.Message);
            }

        }

        /// <summary>
        /// 获取图片数据流
        /// </summary>
        /// <param name="drawable"></param>
        /// <returns></returns>
        public List<byte[]> GetImageByte(Android.Graphics.Drawables.Drawable drawable)
        {
            int maxWidth = GetDrawableMaxWidth();
            Bitmap image = ScalingDrawable(drawable, maxWidth);
            if (image == null)
                return null;
            List<byte[]> data = PrinterUtils.DecodeBitmapToDataList(image, heightParting);
            image.Recycle();
            return data;
        }

        /// <summary>
        /// 缩放图片
        /// </summary>
        /// <param name="drawable"></param>
        /// <param name="maxWidth"></param>
        /// <returns></returns>
        private Bitmap ScalingDrawable(Android.Graphics.Drawables.Drawable drawable, int maxWidth)
        {
            if (drawable == null || drawable.IntrinsicWidth == 0 || drawable.IntrinsicHeight == 0)
                return null;

            int width = drawable.IntrinsicWidth;
            int height = drawable.IntrinsicHeight;
            try
            {
                Bitmap image = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
                Canvas canvas = new Canvas(image);
                drawable.SetBounds(0, 0, width, height);
                drawable.Draw(canvas);
                // 精确缩放
                if (maxWidth <= 0 || width <= maxWidth)
                {
                    return image;
                }

                float scale = maxWidth / (float)width;
                Matrix matrix = new Matrix();
                matrix.PostScale(scale, scale);
                Bitmap resizeImage = Bitmap.CreateBitmap(image, 0, 0, width, height, matrix, true);
                image.Recycle();
                return resizeImage;
            }
            catch (OutOfMemoryError)
            {
                return null;
            }
        }

        /// <summary>
        /// 打印 Bitmap 图片
        /// </summary>
        /// <param name="image"></param>
        [SuppressWarnings(Value = new string[] { "unused" })]
        [Deprecated]
        public void PrintBitmap(Bitmap image)
        {

            int maxWidth = GetDrawableMaxWidth();

            Bitmap scalingImage = ScalingBitmap(image, maxWidth);

            if (scalingImage == null)
                return;

            byte[] command = PrinterUtils.DecodeBitmap(scalingImage, heightParting);

            scalingImage.Recycle();

            try
            {
                if (command != null)
                {
                    Write(command);
                }
            }
            catch (IOException e)
            {
                throw new IOException(e.Message);
            }
        }

        /// <summary>
        /// 获取图片数据流
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public List<byte[]> GetImageByte(Bitmap image)
        {
            int maxWidth = GetDrawableMaxWidth();
            Bitmap scalingImage = ScalingBitmap(image, maxWidth);
            if (scalingImage == null)
                return null;
            List<byte[]> data = PrinterUtils.DecodeBitmapToDataList(image, heightParting);
            image.Recycle();
            return data;
        }

        /// <summary>
        /// 缩放图片
        /// </summary>
        /// <param name="image"></param>
        /// <param name="maxWidth"></param>
        /// <returns></returns>
        private Bitmap ScalingBitmap(Bitmap image, int maxWidth)
        {
            if (image == null || image.Width <= 0 || image.Height <= 0)
                return null;
            try
            {
                int width = image.Width;
                int height = image.Height;
                // 精确缩放
                float scale = 1;
                if (maxWidth <= 0 || width <= maxWidth)
                {
                    scale = maxWidth / (float)width;
                }
                Matrix matrix = new Matrix();
                matrix.PostScale(scale, scale);
                return Bitmap.CreateBitmap(image, 0, 0, width, height, matrix, true);
            }
            catch (OutOfMemoryError)
            {
                return null;
            }
        }

        /// <summary>
        /// 打印图片文件
        /// </summary>
        /// <param name="filePath"></param>
        [SuppressWarnings(Value = new string[] { "unused" })]
        [Deprecated]
        public void PrintImageFile(string filePath)
        {
            Bitmap image;
            try
            {
                int width;
                int height;
                BitmapFactory.Options options = new BitmapFactory.Options();
                options.InJustDecodeBounds = true;
                BitmapFactory.DecodeFile(filePath, options);
                width = options.OutWidth;
                height = options.OutHeight;
                if (width <= 0 || height <= 0)
                    return;
                options.InJustDecodeBounds = false;
                options.InPreferredConfig = Bitmap.Config.Argb8888;
                image = BitmapFactory.DecodeFile(filePath, options);
            }
            catch (OutOfMemoryError)
            {
                return;
            }

            PrintBitmap(image);
        }

        /// <summary>
        /// 打印图片文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public List<byte[]> GetImageByte(string filePath)
        {
            Bitmap image;
            try
            {
                int width;
                int height;
                BitmapFactory.Options options = new BitmapFactory.Options();
                options.InJustDecodeBounds = true;
                BitmapFactory.DecodeFile(filePath, options);
                width = options.OutWidth;
                height = options.OutHeight;
                if (width <= 0 || height <= 0)
                    return null;
                options.InJustDecodeBounds = false;
                options.InPreferredConfig = Bitmap.Config.Argb8888;
                image = BitmapFactory.DecodeFile(filePath, options);
            }
            catch (OutOfMemoryError)
            {
                return null;
            }
            return GetImageByte(image);
        }

        /// <summary>
        /// 输出并换行
        /// </summary>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public void PrintLineFeed()
        {
            Write(PrinterUtils.PrintLineFeed());
        }

        /// <summary>
        /// 进纸切割
        /// </summary>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public void FeedPaperCut()
        {
            Write(PrinterUtils.FeedPaperCut());
        }

        /// <summary>
        /// 进纸切割（留部分）
        /// </summary>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public void FeedPaperCutPartial()
        {
            Write(PrinterUtils.FeedPaperCutPartial());
        }

        /// <summary>
        /// 设置图片打印高度分割值 最大允许255像素
        /// </summary>
        /// <param name="parting"></param>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public void SetHeightParting(int parting)
        {
            if (parting <= 0 || parting > HEIGHT_PARTING_DEFAULT)
                return;
            heightParting = parting;
        }

        /// <summary>
        /// 获取图片打印高度分割值
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public int GetHeightParting()
        {
            return heightParting;
        }

        /**
         * 判断是否中文
         * GENERAL_PUNCTUATION 判断中文的“号
         * CJK_SYMBOLS_AND_PUNCTUATION 判断中文的。号
         * HALFWIDTH_AND_FULLWIDTH_FORMS 判断中文的，号
         * @param c 字符
         * @return 是否中文
         */
        public static bool IsChinese(char c)
        {
            Character.UnicodeBlock ub = Character.UnicodeBlock.Of(c);
            return ub == Character.UnicodeBlock.CjkUnifiedIdeographs
                    || ub == Character.UnicodeBlock.CjkCompatibilityIdeographs
                    || ub == Character.UnicodeBlock.CjkUnifiedIdeographsExtensionA
                    || ub == Character.UnicodeBlock.CjkUnifiedIdeographsExtensionB
                    || ub == Character.UnicodeBlock.CjkSymbolsAndPunctuation
                    || ub == Character.UnicodeBlock.HalfwidthAndFullwidthForms
                    || ub == Character.UnicodeBlock.GeneralPunctuation;
        }
    }
}