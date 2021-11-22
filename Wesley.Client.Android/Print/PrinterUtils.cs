using Android.App;
//using Android.Content.SharedPreferences;
using Android.Bluetooth;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Java.Lang;
using System.Collections.Generic;
using System.Text;

namespace Wesley.Client.Droid
{

    /// <summary>
    /// ESC-POS指令集
    /// </summary>
    [SuppressWarnings(Value = new string[] { "all" })]
    public class PrinterUtils
    {
        private static string hexStr = "0123456789ABCDEF";
        private static string[] binaryArray = {"0000", "0001", "0010", "0011",
            "0100", "0101", "0110", "0111", "1000", "1001", "1010", "1011",
            "1100", "1101", "1110", "1111"};

        public static byte ESC = 27;//换码
        public static byte FS = 28;//文本分隔符
        public static byte GS = 29;//组分隔符

        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte DLE = 16;//数据连接换码

        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte EOT = 4;//传输结束

        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte ENQ = 5;//询问字符

        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte SP = 32;//空格
        public static byte HT = 9;//横向列表
        public static byte LF = 10;//打印并换行（水平定位）

        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte CR = 13;//归位键

        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte FF = 12;//走纸控制（打印并回到标准模式（在页模式下） ）

        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte CAN = 24;//作废（页模式下取消打印数据 ）


        [SuppressWarnings(Value = new string[] { "unused" })]
        public static class CodePage
        {
            public static byte PC437 = 0;
            public static byte KATAKANA = 1;
            public static byte PC850 = 2;
            public static byte PC860 = 3;
            public static byte PC863 = 4;
            public static byte PC865 = 5;
            public static byte WPC1252 = 16;
            public static byte PC866 = 17;
            public static byte PC852 = 18;
            public static byte PC858 = 19;
        }


        [SuppressWarnings(Value = new string[] { "unused" })]
        public static class BarCode
        {
            public static byte UPC_A = 0;
            public static byte UPC_E = 1;
            public static byte EAN13 = 2;
            public static byte EAN8 = 3;
            public static byte CODE39 = 4;
            public static byte ITF = 5;
            public static byte NW7 = 6;
            public static byte CODE93 = 72;
            public static byte CODE128 = 73;
        }

        /// <summary>
        /// 初始化打印机
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] InitPrinter()
        {
            byte[] result = new byte[2];
            result[0] = ESC;
            result[1] = 64;
            return result;
        }

        /// <summary>
        /// 打印并换行
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] PrintLineFeed()
        {
            byte[] result = new byte[1];
            result[0] = LF;
            return result;
        }

        /// <summary>
        /// 下划线
        /// </summary>
        /// <param name="cn">是否为中文</param>
        /// <param name="dot">dot 线宽 （0表示关闭）</param>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] UnderLine(bool cn, int dot)
        {
            byte[] result = new byte[3];
            result[0] = cn ? FS : ESC;
            result[1] = 45;
            switch (dot)
            {
                default:
                case 0:
                    result[2] = 0;
                    break;
                case 1:
                    result[2] = 1;
                    break;
                case 2:
                    result[2] = 2;
                    break;
            }
            return result;
        }

        /// <summary>
        /// 开启着重强调(加粗)
        /// </summary>
        // //[SuppressWarnings("unused")]
        public static byte[] EmphasizedOn()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 69;
            result[2] = 0xF;
            return result;
        }

        /// <summary>
        /// 关闭着重强调(加粗)
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] EmphasizedOff()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 69;
            result[2] = 0;
            return result;
        }


        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] OverlappingOn()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 47;
            result[2] = 0xF;
            return result;
        }

        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] OverlappingOff()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 47;
            result[2] = 0;
            return result;
        }

        /// <summary>
        ///  开启 double-strike 模式
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] DoubleStrikeOn()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 71;
            result[2] = 0xF;
            return result;
        }

        /// <summary>
        /// 关闭 double-strike 模式
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] DoubleStrikeOff()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 71;
            result[2] = 0;
            return result;
        }

        /// <summary>
        /// Select Font A
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] SelectFontA()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 77;
            result[2] = 0;
            return result;
        }

        /// <summary>
        /// Select Font B
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] SelectFontB()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 77;
            result[2] = 1;
            return result;
        }

        /// <summary>
        /// Select Font C ( some printers don't have font C )
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] SelectFontC()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 77;
            result[2] = 2;
            return result;
        }

        /// <summary>
        /// Select Font A
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] SelectCNFontA()
        {
            byte[] result = new byte[3];
            result[0] = FS;
            result[1] = 33;
            result[2] = 0;
            return result;
        }

        /// <summary>
        /// Select Font B
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] SelectCNFontB()
        {
            byte[] result = new byte[3];
            result[0] = FS;
            result[1] = 33;
            result[2] = 1;
            return result;
        }

        /// <summary>
        /// 关闭双倍字高
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] DoubleHeightWidthOff()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 33;
            result[2] = 0;
            return result;
        }

        /// <summary>
        /// 双倍字高（仅英文字体有效）
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] DoubleHeightOn()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 33;
            result[2] = 16;
            return result;
        }

        /// <summary>
        /// 双倍字体高宽（仅英文字体有效）
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] DoubleHeightWidthOn()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 33;
            result[2] = 56;
            return result;
        }


        /// <summary>
        /// 左对齐
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] AlignLeft()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 97;
            result[2] = 0;
            return result;
        }

        /// <summary>
        /// 居中对齐
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] AlignCenter()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 97;
            result[2] = 1;
            return result;
        }

        /// <summary>
        /// 右对齐
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] AlignRight()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 97;
            result[2] = 2;
            return result;
        }


        /// <summary>
        /// 打印并走纸n行
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] PrintAndFeedLines(byte n)
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 100;
            result[2] = n;
            return result;
        }

        /// <summary>
        /// 打印并反向走纸n行（不一定有效）
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] PrintAndReverseFeedLines(byte n)
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 101;
            result[2] = n;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] PrintHorizontalTab()
        {
            byte[] result = new byte[5];
            result[0] = ESC;
            result[1] = 44;
            result[2] = 20;
            result[3] = 28;
            result[4] = 0;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] PrintHTNext()
        {
            byte[] result = new byte[1];
            result[0] = HT;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] PrintLineNormalHeight()
        {
            byte[] result = new byte[2];
            result[0] = ESC;
            result[1] = 50;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] PrintLineHeight(byte height)
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 51;
            result[2] = height;
            return result;
        }

        /// <summary>
        /// Select character code table
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] SelectCodeTab(byte cp)
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 116;
            result[2] = cp;
            return result;
        }

        /// <summary>
        /// 弹开纸箱
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] DrawerKick()
        {
            byte[] result = new byte[5];
            result[0] = ESC;
            result[1] = 112;
            result[2] = 0;
            result[3] = 60;
            result[4] = 120;
            return result;
        }


        /// <summary>
        /// 选择打印颜色1（不一定有效）
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] SelectColor1()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 114;
            result[2] = 0;
            return result;
        }

        /// <summary>
        /// 选择打印颜色2（不一定有效）
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] SelectColor2()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 114;
            result[2] = 1;
            return result;
        }


        /// <summary>
        /// white printing mode on (不一定有效) Turn white/black reverse printing mode on
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] WitePrintingOn()
        {
            byte[] result = new byte[3];
            result[0] = GS;
            result[1] = 66;
            result[2] = (byte)128;
            return result;
        }

        /// <summary>
        /// white printing mode off (不一定有效) Turn white/black reverse printing mode off
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] WhitePrintingOff()
        {
            byte[] result = new byte[3];
            result[0] = GS;
            result[1] = 66;
            result[2] = 0;
            return result;
        }


        /// <summary>
        /// select bar code height  Select the height of the bar code as n dots default dots = 162
        /// </summary>
        /// <param name="dots"></param>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] Barcode_height(byte dots)
        {
            byte[] result = new byte[3];
            result[0] = GS;
            result[1] = 104;
            result[2] = dots;
            return result;
        }

        /// <summary>
        /// select font hri
        /// Selects a font for the Human Readable Interpretation(HRI) characters when printing a barcode,
        /// using n as follows:
        /// @param n Font
        /// 0, 48 Font A
        /// 1, 49 Font B
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] Select_font_hri(byte n)
        {
            byte[] result = new byte[3];
            result[0] = GS;
            result[1] = 102;
            result[2] = n;
            return result;
        }

        /**
         * select position_hri
         * Selects the print position of Human Readable Interpretation (HRI) characters when printing a barcode, using n as follows:
         *
         * @param n Print position
         *          0, 48 Not printed
         *          1, 49 Above the barcode
         *          2, 50 Below the barcode
         *          3, 51 Both above and below the barcode
         * @return bytes for this command
         */
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] Select_position_hri(byte n)
        {
            byte[] result = new byte[3];
            result[0] = GS;
            result[1] = 72;
            result[2] = n;
            return result;
        }

        /// <summary>
        /// print bar code
        /// </summary>
        /// <param name="barcode_typ"></param>
        /// <param name="barcode2print"></param>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] Print_bar_code(byte barcode_typ, string barcode2print)
        {
            byte[] barcodeBytes = Encoding.Default.GetBytes(barcode2print);
            byte[] result = new byte[3 + barcodeBytes.Length + 1];
            result[0] = GS;
            result[1] = 107;
            result[2] = barcode_typ;
            int idx = 3;
            foreach (byte b in barcodeBytes)
            {
                result[idx] = b;
                idx++;
            }
            result[idx] = 0;
            return result;
        }

        /// <summary>
        /// Set horizontal tab positions
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] Set_HT_position(byte col)
        {
            byte[] result = new byte[4];
            result[0] = ESC;
            result[1] = 68;
            result[2] = col;
            result[3] = 0;
            return result;
        }

        /// <summary>
        /// 字体变大为标准的n倍
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] FontSizeSetBig(int num)
        {
            byte realSize = 0;
            switch (num)
            {
                case 0:
                    realSize = 0;
                    break;
                case 1:
                    realSize = 17;
                    break;
                case 2:
                    realSize = 34;
                    break;
                case 3:
                    realSize = 51;
                    break;
                case 4:
                    realSize = 68;
                    break;
                case 5:
                    realSize = 85;
                    break;
                case 6:
                    realSize = 102;
                    break;
                case 7:
                    realSize = 119;
                    break;
            }
            byte[] result = new byte[3];
            result[0] = GS;
            result[1] = 33;
            result[2] = realSize;
            return result;
        }

        /// <summary>
        /// 进纸切割
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] FeedPaperCut()
        {
            byte[] result = new byte[4];
            result[0] = GS;
            result[1] = 86;
            result[2] = 65;
            result[3] = 0;
            return result;
        }

        /// <summary>
        /// 进纸切割（留部分）
        /// </summary>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] FeedPaperCutPartial()
        {
            byte[] result = new byte[4];
            result[0] = GS;
            result[1] = 86;
            result[2] = 66;
            result[3] = 0;
            return result;
        }

        /**
         * 解码图片
         *
         * @param image   图片
         * @param parting 高度分割值
         * @return 数据流
         */
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static List<byte[]> DecodeBitmapToDataList(Bitmap image, int parting)
        {
            if (parting <= 0 || parting > 255)
                parting = 255;

            if (image == null)
                return null;

            int width = image.Width;
            int height = image.Height;

            if (width <= 0 || height <= 0)
                return null;

            if (width > 2040)
            {
                // 8位9针，宽度限制2040像素（但一般纸张都没法打印那么宽，但并不影响打印）
                float scale = 2040 / (float)width;
                Matrix matrix = new Matrix();
                matrix.PostScale(scale, scale);
                Bitmap resizeImage;
                try
                {
                    resizeImage = Bitmap.CreateBitmap(image, 0, 0, width, height, matrix, true);

                    var datas = DecodeBitmapToDataList(resizeImage, parting);

                    resizeImage.Recycle();

                    return datas;
                }
                catch (OutOfMemoryError)
                {
                    return null;
                }
            }

            // 宽命令
            string widthHexString = Integer.ToHexString(width % 8 == 0 ? width / 8 : (width / 8 + 1));
            if (widthHexString.Length > 2)
            {
                // 超过2040像素才会到达这里
                return null;
            }
            else if (widthHexString.Length == 1)
            {
                widthHexString = "0" + widthHexString;
            }
            widthHexString += "00";

            // 每行字节数(除以8，不足补0)
            string zeroStr = "";
            int zeroCount = width % 8;
            if (zeroCount > 0)
            {
                for (int i = 0; i < (8 - zeroCount); i++)
                {
                    zeroStr += "0";
                }
            }
            List<string> commandList = new List<string>();
            // 高度每parting像素进行一次分割
            int time = height % parting == 0 ? height / parting : (height / parting + 1);// 循环打印次数
            for (int t = 0; t < time; t++)
            {
                int partHeight = t == time - 1 ? height % parting : parting;// 分段高度

                // 高命令
                string heightHexString = Integer.ToHexString(partHeight);
                if (heightHexString.Length > 2)
                {
                    // 超过255像素才会到达这里
                    return null;
                }
                else if (heightHexString.Length == 1)
                {
                    heightHexString = "0" + heightHexString;
                }
                heightHexString += "00";

                // 宽高指令
                string commandHexString = "1D763000";
                commandList.Add(commandHexString + widthHexString + heightHexString);

                List<string> list = new List<string>(); //binaryString list
                Java.Lang.StringBuilder sb = new Java.Lang.StringBuilder();
                // 像素二值化，非黑即白
                for (int i = 0; i < partHeight; i++)
                {
                    sb.Delete(0, sb.Length());
                    for (int j = 0; j < width; j++)
                    {
                        // 实际在图片中的高度
                        int startHeight = t * parting + i;
                        //得到当前像素的值
                        int color = image.GetPixel(j, startHeight);
                        int red, green, blue;
                        if (image.HasAlpha)
                        {
                            //得到alpha通道的值
                            int alpha = Color.GetAlphaComponent(color);
                            //得到图像的像素RGB的值
                            red = Color.GetRedComponent(color);
                            green = Color.GetGreenComponent(color);
                            blue = Color.GetBlueComponent(color);
                            float offset = alpha / 255.0f;
                            // 根据透明度将白色与原色叠加
                            red = 0xFF + (int)Java.Lang.Math.Ceil((red - 0xFF) * offset);
                            green = 0xFF + (int)Java.Lang.Math.Ceil((green - 0xFF) * offset);
                            blue = 0xFF + (int)Java.Lang.Math.Ceil((blue - 0xFF) * offset);
                        }
                        else
                        {
                            //得到图像的像素RGB的值
                            red = Color.GetRedComponent(color);
                            green = Color.GetGreenComponent(color);
                            blue = Color.GetBlueComponent(color);
                        }
                        // 接近白色改为白色。其余黑色
                        if (red > 160 && green > 160 && blue > 160)
                            sb.Append("0");
                        else
                            sb.Append("1");
                    }
                    // 每一行结束时，补充剩余的0
                    if (zeroCount > 0)
                    {
                        sb.Append(zeroStr);
                    }
                    list.Add(sb.ToString());
                }
                // binaryStr每8位调用一次转换方法，再拼合
                List<string> bmpHexList = new List<string>();
                foreach (string binaryStr in list)
                {
                    sb.Delete(0, sb.Length());
                    for (int i = 0; i < binaryStr.Length; i += 8)
                    {
                        string str = binaryStr.Substring(i, i + 8);
                        // 2进制转成16进制
                        string hexString = BinaryStrToHexString(str);
                        sb.Append(hexString);
                    }
                    bmpHexList.Add(sb.ToString());
                }

                // 数据指令
                commandList.AddRange(bmpHexList);
            }
            List<byte[]> data = new List<byte[]>();
            foreach (string hexStr in commandList)
            {
                data.Add(HexStringToBytes(hexStr));
            }
            return data;
        }

        /**
         * 解码图片
         *
         * @param image   图片
         * @param parting 高度分割值
         * @return 数据流
         */
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] DecodeBitmap(Bitmap image, int parting)
        {
            List<byte[]> data = DecodeBitmapToDataList(image, parting);
            int len = 0;
            foreach (byte[] srcArray in data)
            {
                len += srcArray.Length;
            }
            byte[] destArray = new byte[len];
            int destLen = 0;
            foreach (byte[] srcArray in data)
            {
                JavaSystem.Arraycopy(srcArray, 0, destArray, destLen, srcArray.Length);
                destLen += srcArray.Length;
            }
            return destArray;
        }

        /// <summary>
        /// 解码图片
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] DecodeBitmap(Bitmap image)
        {
            return DecodeBitmap(image, PrinterWriter.HEIGHT_PARTING_DEFAULT);
        }

        /// <summary>
        /// 合并byte数组
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] MergerByteArray(List<byte[]> byteArray)
        {

            int length = 0;
            foreach (byte[] item in byteArray)
            {
                length += item.Length;
            }
            byte[] result = new byte[length];
            int index = 0;
            foreach (byte[] item in byteArray)
            {
                foreach (byte b in item)
                {
                    result[index] = b;
                    index++;
                }
            }
            return result;
        }


        /// <summary>
        /// 2进制转成16进制
        /// </summary>
        /// <param name="binaryStr"></param>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static string BinaryStrToHexString(string binaryStr)
        {
            string hex = "";
            string f4 = binaryStr.Substring(0, 4);
            string b4 = binaryStr.Substring(4, 8);
            for (int i = 0; i < binaryArray.Length; i++)
            {
                if (f4.Equals(binaryArray[i]))
                    hex += hexStr.Substring(i, i + 1);
            }
            for (int i = 0; i < binaryArray.Length; i++)
            {
                if (b4.Equals(binaryArray[i]))
                    hex += hexStr.Substring(i, i + 1);
            }
            return hex;
        }

        /// <summary>
        /// 16进制指令list转换为byte[]指令
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] HexListToByte(List<string> list)
        {
            List<byte[]> commandList = new List<byte[]>();
            foreach (var hexStr in list)
            {
                commandList.Add(HexStringToBytes(hexStr));
            }
            int len = 0;
            foreach (byte[] srcArray in commandList)
            {
                len += srcArray.Length;
            }
            byte[] destArray = new byte[len];
            int destLen = 0;
            foreach (byte[] srcArray in commandList)
            {
                JavaSystem.Arraycopy(srcArray, 0, destArray, destLen, srcArray.Length);
                destLen += srcArray.Length;
            }
            return destArray;
        }

        /// <summary>
        /// 16进制串转byte数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        [SuppressWarnings(Value = new string[] { "unused" })]
        public static byte[] HexStringToBytes(string hexString)
        {
            if (hexString == null || hexString.Equals(""))
            {
                return null;
            }
            hexString = hexString.ToUpper();
            int length = hexString.Length / 2;
            char[] hexChars = hexString.ToCharArray();
            byte[] d = new byte[length];
            for (int i = 0; i < length; i++)
            {
                int pos = i * 2;
                d[i] = (byte)(CharToByte(hexChars[pos]) << 4 | CharToByte(hexChars[pos + 1]));
            }
            return d;
        }

        /// <summary>
        /// 16进制char 转 byte
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static byte CharToByte(char c)
        {
            return (byte)hexStr.IndexOf(c);
        }
    }

    public class PrintUtil
    {
        private static string FILENAME = "bt";
        //蓝牙设备地址
        private static string DEFAULT_BLUETOOTH_DEVICE_ADDRESS = "default_bluetooth_device_address";
        //蓝牙设备名称
        private static string DEFAULT_BLUETOOTH_DEVICE_NAME = "default_bluetooth_device_name";

        public static string ACTION_PRINT_TEST = "action_print_test";
        public static string ACTION_PRINT_TEST_TWO = "action_print_test_two";
        public static string ACTION_PRINT = "action_print";
        public static string ACTION_PRINT_TICKET = "action_print_ticket";
        public static string ACTION_PRINT_BITMAP = "action_print_bitmap";
        public static string ACTION_PRINT_PAINTING = "action_print_painting";

        public static string PRINT_EXTRA = "print_extra";

        public static void SetDefaultBluetoothDeviceAddress(Context mContext, string value)
        {
            var sharedPreferences = mContext.GetSharedPreferences(FILENAME, FileCreationMode.Private);
            var editor = sharedPreferences.Edit();
            editor.PutString(DEFAULT_BLUETOOTH_DEVICE_ADDRESS, value);
            editor.Apply();
            App.BtAddress = value;
        }

        public static void SetDefaultBluetoothDeviceName(Context mContext, string value)
        {
            var sharedPreferences = mContext.GetSharedPreferences(FILENAME, FileCreationMode.Private);
            var editor = sharedPreferences.Edit();
            editor.PutString(DEFAULT_BLUETOOTH_DEVICE_NAME, value);
            editor.Apply();
            App.BtName = value;
        }
        /// <summary>
        /// 是否绑定了打印机设备
        /// </summary>
        /// <param name="mContext"></param>
        /// <param name="bluetoothAdapter"></param>
        /// <returns></returns>
        public static bool IsBondPrinter(Context mContext, BluetoothAdapter bluetoothAdapter)
        {
            //蓝牙适配器打开
            if (!BtUtil.IsOpen(bluetoothAdapter))
            {
                return false;
            }
            string defaultBluetoothDeviceAddress = GetDefaultBluethoothDeviceAddress(mContext);
            if (TextUtils.IsEmpty(defaultBluetoothDeviceAddress))
            {
                return false;
            }

            var deviceSet = bluetoothAdapter.BondedDevices;
            if (deviceSet == null || deviceSet.Count == 0)
            {
                return false;
            }

            foreach (BluetoothDevice bluetoothDevice in deviceSet)
            {
                if (bluetoothDevice.Address.Equals(defaultBluetoothDeviceAddress))
                {
                    return true;
                }
            }
            return false;
        }

        public static string GetDefaultBluethoothDeviceAddress(Context mContext)
        {
            var sharedPreferences = mContext.GetSharedPreferences(FILENAME, FileCreationMode.Private);
            return sharedPreferences.GetString(DEFAULT_BLUETOOTH_DEVICE_ADDRESS, "");
        }

        public static bool IsBondPrinterIgnoreBluetooth(Context mContext)
        {
            string defaultBluetoothDeviceAddress = GetDefaultBluethoothDeviceAddress(mContext);
            return !(TextUtils.IsEmpty(defaultBluetoothDeviceAddress)
                    || TextUtils.IsEmpty(GetDefaultBluetoothDeviceName(mContext)));
        }

        /// <summary>
        /// 绑定设备的蓝牙名称
        /// </summary>
        /// <param name="mContext"></param>
        /// <returns></returns>
        public static string GetDefaultBluetoothDeviceName(Context mContext)
        {
            var sharedPreferences = mContext.GetSharedPreferences(FILENAME, FileCreationMode.Private);
            return sharedPreferences.GetString(DEFAULT_BLUETOOTH_DEVICE_NAME, "");
        }

        public static void Apply(ISharedPreferencesEditor editor)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Gingerbread)
            {
                editor.Apply();
            }
            else
            {
                editor.Commit();
            }
        }
    }

    public class BtUtil
    {
        /// <summary>
        /// 判断蓝牙是否打开
        /// </summary>
        /// <param name="adapter"></param>
        /// <returns></returns>
        public static bool IsOpen(BluetoothAdapter adapter)
        {
            if (null != adapter)
            {
                return adapter.IsEnabled;
            }
            return false;
        }

        /// <summary>
        /// 搜索蓝牙设备
        /// </summary>
        /// <param name="adapter"></param>
        public static void SearchDevices(BluetoothAdapter adapter)
        {
            // 寻找蓝牙设备，android会将查找到的设备以广播形式发出去
            if (null != adapter)
            {
                adapter.StartDiscovery();
            }
        }

        /// <summary>
        /// 取消搜索蓝牙设备
        /// </summary>
        /// <param name="adapter"></param>
        public static void CancelDiscovery(BluetoothAdapter adapter)
        {
            if (null != adapter)
            {
                adapter.CancelDiscovery();
            }
        }

        /// <summary>
        /// 注册 bluetooth receiver
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="activity"></param>
        public static void RegisterBluetoothReceiver(BroadcastReceiver receiver, Activity activity)
        {
            if (null == receiver || null == activity)
            {
                return;
            }
            IntentFilter intentFilter = new IntentFilter();
            //start discovery
            intentFilter.AddAction(BluetoothAdapter.ActionDiscoveryStarted);
            //finish discovery
            intentFilter.AddAction(BluetoothAdapter.ActionDiscoveryFinished);
            //bluetooth status change
            intentFilter.AddAction(BluetoothAdapter.ActionStateChanged);
            //found device
            intentFilter.AddAction(BluetoothDevice.ActionFound);
            //bond status change
            intentFilter.AddAction(BluetoothDevice.ActionBondStateChanged);
            //pairing device
            intentFilter.AddAction("android.bluetooth.device.action.PAIRING_REQUEST");

            activity.RegisterReceiver(receiver, intentFilter);
        }

        /// <summary>
        /// 卸载 bluetooth receiver
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="activity"></param>
        public static void UnregisterBluetoothReceiver(BroadcastReceiver receiver, Activity activity)
        {
            if (null == receiver || null == activity)
            {
                return;
            }
            activity.UnregisterReceiver(receiver);
        }
    }
}