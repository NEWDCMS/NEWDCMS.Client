using System.Collections.Generic;

namespace Wesley.Client.Models
{
    /// <summary>
    /// 用于分页
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedResult<T> where T : class
    {
        public int Page { get; set; }
        public int PerPage { get; set; }
        public int Total { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
