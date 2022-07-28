using Wesley.Client.Enums;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Refit;
using System.Collections.Generic;
using System.Reactive;
using System.Runtime.Serialization;

namespace Wesley.Client.Models
{
    public class GridTable
    {
        public int RowIndex { get; set; }
        public Dictionary<string, string> Columns = new Dictionary<string, string>();
    }


    public class PopData : ReactiveObject
    {
        public int Id { get; set; }
        [Reactive] public string Column { get; set; }
        [Reactive] public bool ColumnEnable { get; set; }

        [Reactive] public string Column1 { get; set; }
        [Reactive] public bool Column1Enable { get; set; }

        [Reactive] public bool Selected { get; set; }
        public object Data { get; set; }

        //public ReactiveCommand<int, Unit> SelectedCommand { get; set; }
    }



    /// <summary>
    /// 表示接入模块功能应用
    /// </summary>
    public class Module : Base
    {
        public ReactiveCommand<Module, Unit> OnItemTappedCommand { get; set; }

        [DataMember]
        [Reactive] public bool Selected { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int AType { get; set; }
        public string ATypeName { get; set; }
        [DataMember]
        public int Count { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Icon { get; set; }
        [DataMember]
        public string Color { get; set; } = "#ffffff";
        [DataMember]
        public string BgColor { get; set; } = "#53a245";
        [DataMember]
        public string Navigation { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public BillTypeEnum BillType { get; set; } = BillTypeEnum.None;
        [DataMember]
        public ChartTemplate ChartType { get; set; }
        [Reactive] public bool Enable { get; set; }
        [DataMember]
        public List<AccessGranularityEnum> PermissionCodes { get; set; } = new List<AccessGranularityEnum>();
    }

    public class ModuleGroup : List<Module>
    {
        public string ATypeName { get; set; }

        public ModuleGroup(string typeName, List<Module> modules) : base(modules)
        {
            ATypeName = typeName;
        }
    }

    public class ModuleQuery
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
    public class Copyright : Base
    {
        public string Name { get; set; }
        public string Licence { get; set; }
        public string Version { get; set; }
        public string IsChenged { get; set; }
    }

    public class APPFeatures
    {
        /// <summary>
        /// 报表功能
        /// </summary>
        public List<Module> ReportsDatas { get; set; }
        /// <summary>
        /// 应用功能
        /// </summary>
        public List<Module> AppDatas { get; set; }
        /// <summary>
        /// 订阅频道
        /// </summary>
        public List<MessageInfo> SubscribeDatas { get; set; }
    }

    public class SMSParams
    {
        [AliasAs("storeId")]
        public int StoreId { get; set; }

        /// <summary>
        /// default : 0
        /// </summary>
        [AliasAs("rType")]
        public int RType { get; set; }

        [AliasAs("mobile")]
        public string Mobile { get; set; }

        [AliasAs("id")]
        public int Id { get; set; }
    }
}
