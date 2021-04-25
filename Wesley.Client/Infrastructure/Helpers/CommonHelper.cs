using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Wesley.Infrastructure.Helpers
{
    public class Utils
    {
        // 两次点击按钮之间的点击间隔不能少于1000毫秒
        private static readonly int MIN_CLICK_DELAY_TIME = 1000;
        private static long lastClickTime;
        public static bool IsFastClick()
        {
            bool flag = false;
            long curClickTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            if ((curClickTime - lastClickTime) >= MIN_CLICK_DELAY_TIME)
            {
                flag = true;
            }
            lastClickTime = curClickTime;
            return flag;
        }
    }

    /// <summary>
    /// 公共辅助方法
    /// </summary>
    public partial class CommonHelper
    {
        /// <summary>
        /// 格式化json字符串
        /// </summary>
        /// <param name="sourJsonStr"></param>
        /// <returns></returns>
        public static string FormatJsonStr(string sourJsonStr)
        {
            var serializer = new JsonSerializer();
            using (var tr = new StringReader(sourJsonStr))
            {
                using (var jtr = new JsonTextReader(tr))
                {
                    object obj = serializer.Deserialize(jtr);
                    return obj != null ? obj.ToString() : sourJsonStr;
                }
            }
        }


        public static string ConvetToSeconds(int duration)
        {
            TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(duration));
            string str = "";
            if (ts.Days > 0)
            {
                str = ts.Days.ToString() + "天" + ts.Hours.ToString() + "时" + ts.Minutes.ToString() + "分" + ts.Seconds + "秒";
            }
            else if (ts.Hours > 0)
            {
                str = ts.Hours.ToString() + "时" + ts.Minutes.ToString() + "分" + ts.Seconds + "秒";
            }
            else if (ts.Hours == 0 && ts.Minutes > 0)
            {
                str = ts.Minutes.ToString() + "分" + ts.Seconds + "秒";
            }
            else if (ts.Hours == 0 && ts.Minutes == 0)
            {
                str = ts.Seconds + "秒";
            }
            return str;
        }

        /// <summary>
        /// 166，189，199 网络
        /// </summary>
        /// <param name="str_handset"></param>
        /// <param name="stand"></param>
        /// <returns></returns>
        public static bool IsPhoneNo(string str_handset, bool stand)
        {
            if (stand)
            {
                return Regex.IsMatch(str_handset, "^(0\\d{2,3}\\d{7,8}(\\d{3,5}){0,1})|(((19[0-9])|(18[0-9])|(16[0-9])|(13[0-9])|(15([0-3]|[5-9]))|(18[0-9])|(17[0-9])|(14[0-9]))\\d{8})$");
            }
            else
            {
                return Regex.IsMatch(str_handset, "^(0\\d{2,3}-?\\d{7,8}(-\\d{3,5}){0,1})|(((19[0-9])|(18[0-9])|(16[0-9])|(13[0-9])|(15([0-3]|[5-9]))|(18[0-9])|(17[0-9])|(14[0-9]))\\d{8})$");
            }
        }

        /// <summary>
        /// 将 Stream 转成 byte[]
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        /// <summary>
        /// 将 byte[] 转成 Stream
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }


        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp(System.DateTime time, int length = 13)
        {
            long ts = ConvertDateTimeToInt(time);
            return ts.ToString().Substring(0, length);
        }
        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static long ConvertDateTimeToInt(System.DateTime time)
        {
            System.DateTime startTime = System.TimeZoneInfo.ConvertTimeToUtc(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            //TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }




        #region 生成单据号
        /// <summary>
        /// 生成单据号
        /// </summary>
        /// <param name="billType">单据类型</param>
        /// <param name="storeId">经销商id</param>
        /// <returns>类型+7+12</returns>
        public static string GetBillNumber(string billType, int storeId)
        {
            string stamp = GetTimeStamp(DateTime.Now, 12);
            int realCount = 0;
            var str = storeId.ToString();
            //7位经销商编号，支持百万家
            if (str.Length > 7)
            {
                var start = int.Parse(str.Substring(0, 7));
                var end = int.Parse(str.Substring(7, str.Length - 7));
                storeId = start + end;
            }
            var numArry = GetNumHash(storeId, ref realCount);
            int[] arry = new int[7];
            for (var i = 0; i < 7; i++)
            {
                if (realCount > i)
                {
                    arry[i] = numArry[i];
                }
                else
                {
                    arry[i] = 0;
                }
            }
            string billNumber = billType + "" + string.Join("", arry) + "" + stamp;
            return billNumber;
        }

        /// <summary>
        ///  将整型转成整型数组
        /// </summary>
        /// <example>10 转成 num[0]=1 num[1]=0 </example>
        /// <param name="showNumber">整型数字</param>
        /// <param name="realCount">返回的实际大小，即数组长度</param>
        /// <returns>整型数组</returns>
        public static int[] GetNumHash(int showNumber, ref int realCount)
        {
            int[] num_hash = new int[10];
            int index = 0;
            while (showNumber / 10 != 0)
            {
                num_hash[index] = (showNumber % 10);
                showNumber /= 10;
                index++;
            }
            num_hash[index] = showNumber;
            realCount = index + 1;
            return num_hash;
        }

        #endregion


        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum enumValue)
        {
            try
            {
                string value = enumValue.ToString();
                FieldInfo field = enumValue.GetType().GetField(value);
                //获取描述属性
                object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                //当描述属性没有时，直接返回名称
                if (objs == null || objs.Length == 0)
                {
                    return value;
                }

                DescriptionAttribute descriptionAttribute = (DescriptionAttribute)objs[0];
                return descriptionAttribute.Description;
            }
            catch (Exception)
            {
                return enumValue.ToString();
            }
        }


        /// 获取枚举描述(泛型支持)
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetEnumDescription<TEnum>(TEnum enumValue)
        {
            try
            {
                string value = enumValue.ToString();
                FieldInfo field = enumValue.GetType().GetField(value);
                //获取描述属性
                object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                //当描述属性没有时，直接返回名称
                if (objs == null || objs.Length == 0)
                {
                    return value;
                }

                DescriptionAttribute descriptionAttribute = (DescriptionAttribute)objs[0];
                return descriptionAttribute.Description;
            }
            catch (Exception)
            {
                return enumValue.ToString();
            }
        }

        /// <summary>
        /// 获取枚举描述(泛型支持)
        /// </summary>
        /// <typeparam name="TEnum">枚举</typeparam>
        /// <param name="i">枚举具体值</param>
        /// <returns></returns>
        public static string GetEnumDescription<TEnum>(int i)
        {
            try
            {
                foreach (var item in Enum.GetValues(typeof(TEnum)))
                {
                    if ((int)item == i)
                    {
                        return GetEnumDescription<TEnum>((TEnum)item);
                    }
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }


        ///// <summary>
        ///// 获取最小单位转换
        ///// </summary>
        ///// <param name="bigUnitIdint">大单位</param>
        ///// <param name="strokeUnitId">中单位</param>
        ///// <param name="smallUnitId">小单位</param>
        ///// <param name="bigQuantity">大转小数量</param>
        ///// <param name="strokeQuantity">中转小数量</param>
        ///// <param name="thisUnitId">当前单位</param>
        ///// <returns></returns>
        //public static int GetSmallConversionQuantity(int bigUnitId, int strokeUnitId, int smallUnitId, int bigQuantity, int strokeQuantity, int thisUnitId)
        //{
        //    int result;
        //    //大
        //    if (thisUnitId == bigUnitId)
        //    {
        //        result = bigQuantity;
        //    }
        //    else
        //    {
        //        //中
        //        if (thisUnitId == strokeQuantity)
        //        {
        //            result = strokeQuantity;
        //        }
        //        //小
        //        else
        //        {
        //            result = 1;
        //        }
        //    }

        //    if (result == 0)
        //    {
        //        result = 1;
        //    }
        //    return result;
        //}


        /// <summary>
        /// 生成随机纯字母随机数
        /// </summary>
        /// <param name="IntStr">生成长度</param>
        /// <returns></returns>
        public static string GenerateStrchar(int Length)
        {
            return Str_char(Length, false);
        }

        /// <summary>
        /// 生成随机纯字母随机数
        /// </summary>
        /// <param name="Length">生成长度</param>
        /// <param name="Sleep">是否要在生成前将当前线程阻止以避免重复</param>
        /// <returns></returns>
        public static string Str_char(int Length, bool Sleep)
        {
            if (Sleep)
            {
                System.Threading.Thread.Sleep(3);
            }

            char[] Pattern = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            string result = "";
            int n = Pattern.Length;
            System.Random random = new Random(~unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < Length; i++)
            {
                int rnd = random.Next(0, n);
                result += Pattern[rnd];
            }
            return result;
        }

        /// <summary>
        /// 从字符串的指定位置开始截取到字符串结尾的了符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <returns>子字符串</returns>
        public static string CutString(string str, int startIndex)
        {
            return CutString(str.Replace(" ", "").Trim(), startIndex, str.Length);
        }

        /// <summary>
        /// 从字符串的指定位置截取指定长度的子字符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <param name="length">子字符串的长度</param>
        /// <returns>子字符串</returns>
        public static string CutString(string str, int startIndex, int length)
        {
            if (startIndex >= 0)
            {
                if (length < 0)
                {
                    length *= -1;
                    if (startIndex - length < 0)
                    {
                        length = startIndex;
                        startIndex = 0;
                    }
                    else
                        startIndex -= length;
                }

                if (startIndex > str.Length)
                    return "";
            }
            else
            {
                if (length < 0)
                    return "";
                else
                {
                    if (length + startIndex > 0)
                    {
                        length += startIndex;
                        startIndex = 0;
                    }
                    else
                        return "";
                }
            }

            if (str.Length - startIndex < length)
                length = str.Length - startIndex;

            return str.Substring(startIndex, length);
        }


    }

    public static class TypeExtensions
    {
        // The Trim method only trims 0x0009, 0x000a, 0x000b, 0x000c, 0x000d, 0x0085, 0x2028, and 0x2029.
        // This array adds in control characters.
        public static readonly char[] WhiteSpaceChars = new char[] { (char)0x00, (char)0x01, (char)0x02, (char)0x03, (char)0x04, (char)0x05,
            (char)0x06, (char)0x07, (char)0x08, (char)0x09, (char)0x0a, (char)0x0b, (char)0x0c, (char)0x0d, (char)0x0e, (char)0x0f,
            (char)0x10, (char)0x11, (char)0x12, (char)0x13, (char)0x14, (char)0x15, (char)0x16, (char)0x17, (char)0x18, (char)0x19, (char)0x20,
            (char)0x1a, (char)0x1b, (char)0x1c, (char)0x1d, (char)0x1e, (char)0x1f, (char)0x7f, (char)0x85, (char)0x2028, (char)0x2029 };

        /// <summary> 
        /// Gets a value that indicates whether or not the collection is empty. 
        /// </summary> 
        public static bool IsNullOrBlank(this string s)
        {
            if (s == null || s.Trim(WhiteSpaceChars).Length == 0)
            {
                return true;
            }

            return false;
        }

        public static bool NotNullOrBlank(this string s)
        {
            if (s == null || s.Trim(WhiteSpaceChars).Length == 0)
            {
                return false;
            }

            return true;
        }
    }

    public class ToolsClass
    {
        /// <summary>
        /// 判断一个字符串是否为url
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsUrl(string str)
        {
            try
            {
                string Url = @"^http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$";
                return Regex.IsMatch(str, Url);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
