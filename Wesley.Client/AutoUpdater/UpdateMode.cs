using Wesley.Client.Models;
using System;
namespace Wesley.Client
{
    public enum UpdateMode
    {
        MissingNo = -1,
        AutoInstall,
        OpenAppStore
    }

    /// <summary>
    /// 更新信息
    /// </summary>
    public class UpdateInfo : EntityBase
    {
        public int FileId { get; set; }
        public string Name { get; set; }
        public bool Enable { get; set; }
        public string Version { get; set; }
        /// <summary>
        /// http://storage.jsdcms.com:5000/api/version/updater/app/download
        /// </summary>
        public string DownLoadUrl { get; set; }
        public string UpgradeDescription { get; set; }
        public DateTime CreatedOn { get; set; }
    }

}
