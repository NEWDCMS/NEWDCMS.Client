
using System.Collections.Generic;
using System.ComponentModel;

namespace Wesley.Mvvm
{
    /// <summary>
    /// 为<see cref=“PropertyChangedEventArgs”/>实例提供缓存
    /// </summary>
    public sealed class PropertyChangedEventArgsCache
    {
        /// <summary>
        /// 底层字典。这个实例是它自己的互斥体
        /// </summary>
        private readonly Dictionary<string, PropertyChangedEventArgs> _cache = new Dictionary<string, PropertyChangedEventArgs>();

        /// <summary>
        /// 私有构造函数来阻止其他实例
        /// </summary>
        private PropertyChangedEventArgsCache()
        {
        }

        /// <summary>
        /// 缓存的全局实例
        /// </summary>
        public static PropertyChangedEventArgsCache Instance { get; } = new PropertyChangedEventArgsCache();

        /// <summary>
        /// 检索指定属性的<see cref=“PropertyChangedEventArgs”/>实例，创建该实例并在必要时将其添加到缓存中。
        /// </summary>
        /// <param name="propertyName">更改的属性的名称.</param>
        public PropertyChangedEventArgs Get(string propertyName)
        {
            lock (_cache)
            {
                if (_cache.TryGetValue(propertyName, out var result))
                    return result;
                result = new PropertyChangedEventArgs(propertyName);
                _cache.Add(propertyName, result);
                return result;
            }
        }
    }
}