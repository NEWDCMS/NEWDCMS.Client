using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace Wesley.Infrastructure.Helpers
{


    public static class ReflectionUtil
    {

        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            var formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }


        #region 根据对象名返回类实例

        /// <summary>
        /// 根据对象名返回类实例
        /// </summary>
        /// <param name="parObjectName">对象名称</param>
        /// <returns>对象实例（可强制转换为对象实例）</returns>
        public static object GetObjectByObjectName(string parObjectName)
        {
            Type t = Type.GetType(parObjectName); //找到对象
            return System.Activator.CreateInstance(t);         //实例化对象
        }

        #endregion

        #region 判断对象是否为空

        /// <summary>
        /// 判断对象是否为空
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        static public bool ObjectIsNull(Object obj)
        {
            //如果对象引用为null 或者 对象值为null 或者对象值为空
            if (obj == null || obj == DBNull.Value || obj.ToString().Equals("") || obj.ToString() == "")
            {
                return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// 通用（调用对象方法前先new一遍对象，故对象的状态无法保留；无用有无参构造函数，并调用无参方法），
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodName"></param>
        public static void InvokeMethod<T>(string methodName, object[] param = null) where T : new()
        {
            T instance = new T();
            MethodInfo method = typeof(T).GetMethod(methodName, new Type[] { });
            method.Invoke(instance, param);
        }


        /// <summary>
        /// 调用一个具体实例对象的方法，会保留对象状态
        /// </summary>
        /// <param name="o"></param>
        /// <param name="methodName"></param>
        public static void InvokeMethod(object o, string methodName, object[] param = null)
        {
            o.GetType().GetMethod(methodName, new Type[] { }).Invoke(o, param);
        }

    }
}
