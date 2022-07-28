using Wesley.Client.Enums;
using LiteDB;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Wesley.Client.Models.Census
{


    /// <summary>
    /// 用于门店拜访记录表
    /// </summary>
    public class VisitStore : EntityBase
    {

        /// <summary>
        /// 客户Id
        /// </summary>
        [Reactive] public int TerminalId { get; set; }
        [Reactive] public string TerminalName { get; set; }

        /// <summary>
        /// 片区Id
        /// </summary>
        public int? DistrictId { get; set; }

        /// <summary>
        /// 渠道Id
        /// </summary>
        public int? ChannelId { get; set; }


        /// <summary>
        /// 业务员
        /// </summary>
        [Reactive] public int BusinessUserId { get; set; }
        [Reactive] public string BusinessUserName { get; set; }
        [Reactive] public string FaceImage { get; set; }


        /// <summary>
        /// 客户编号
        /// </summary>
        public string CodeNumber { get; set; }


        /// <summary>
        /// 状态
        /// </summary>
        public int VisitTypeId { get; set; }
        public VisitTypeEnum VisitType
        {
            get { return (VisitTypeEnum)VisitTypeId; }
            set { VisitTypeId = (int)value; }
        }


        public int SignTypeId { get; set; }
        public SignEnum SignType
        {
            get { return (SignEnum)SignTypeId; }
            set { SignTypeId = (int)value; }
        }

        /// <summary>
        /// 签到时间
        /// </summary>
        [Reactive] public DateTime SigninDateTime { get; set; }
        [Reactive] public bool SigninDateTimeEnable { get; set; }

        /// <summary>
        /// 签退时间
        /// </summary>
        [Reactive] public DateTime SignOutDateTime { get; set; }

        /// <summary>
        /// 预计下次订货时间
        /// </summary>
        public int NextOrderDays { get; set; }

        /// <summary>
        /// 上次签到时间
        /// </summary>
        public int LastSigninDateTime { get; set; }

        [Reactive] public string LastSigninDateTimeName { get; set; } = "0天";



        /// <summary>
        /// 上次采购时间
        /// </summary>
        public int LastPurchaseDateTime { get; set; }
        [Reactive] public string LastPurchaseDateTimeName { get; set; } = "0天";

        /// <summary>
        /// 上次采购时间
        /// </summary>
        [Reactive] public DateTime LastPurchaseDate { get; set; }


        /// <summary>
        /// 在店时间 （秒）
        /// </summary>
        public int? OnStoreStopSeconds { get; set; }
        public string OnStoreStopSecondsFMT { get; set; }

        /// <summary>
        /// 线路
        /// </summary>
        public int? LineId { get; set; }

        /// <summary>
        /// 纬度坐标
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// 经度坐标
        /// </summary>
        public double? Longitude { get; set; }



        /// <summary>
        /// 销售单
        /// </summary>
        public int? SaleBillId { get; set; }
        /// <summary>
        /// 销订单
        /// </summary>
        public int? SaleReservationBillId { get; set; }
        /// <summary>
        /// 退货单
        /// </summary>
        public int? ReturnBillId { get; set; }
        /// <summary>
        /// 退订单
        /// </summary>
        public int? ReturnReservationBillId { get; set; }


        /// <summary>
        /// 销售金额
        /// </summary>
        public decimal? SaleAmount { get; set; }
        /// <summary>
        /// 销订金额
        /// </summary>
        public decimal? SaleOrderAmount { get; set; }
        /// <summary>
        /// 退货金额
        /// </summary>
        public decimal? ReturnAmount { get; set; }
        /// <summary>
        /// 退订金额
        /// </summary>
        public decimal? ReturnOrderAmount { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        [Reactive] public string Remark { get; set; }

        /// <summary>
        /// 门头照片
        /// </summary>
        [BsonIgnore]
        [Reactive] public ObservableCollection<DoorheadPhoto> DoorheadPhotos { get; set; } = new ObservableCollection<DoorheadPhoto>();
        [Reactive] public string DoorheadPhoto { get; set; }

        /// <summary>
        /// 陈列照片
        /// </summary>
        [BsonIgnore]
        [Reactive] public ObservableCollection<DisplayPhoto> DisplayPhotos { get; set; } = new ObservableCollection<DisplayPhoto>();

        public double? Distance { get; set; }
        public bool Abnormal { get; set; }
    }

    /// <summary>
    /// 用于分组
    /// </summary>
    public class VisitStoreGroup : List<VisitStore>
    {
        public string BusinessUserName { get; set; }
        public string FaceImage { get; set; }
        public VisitStoreGroup(string businessUserName, string faceImage, List<VisitStore> visits) : base(visits)
        {
            BusinessUserName = businessUserName;
            FaceImage = faceImage;
        }
    }


    /// <summary>
    /// 业务员拜访列表
    /// </summary>
    public partial class BusinessVisitList : EntityBase
    {

        [Reactive] public bool Selected { get; set; }

        [Reactive] public string BgColor { get; set; } = "#ffffff";

        [Reactive] public string TxtColor { get; set; } = "#333333";

        public int ColumnIndex { get; set; }
        [Reactive] public int BusinessUserId { get; set; }

        [Reactive] public string BusinessUserName { get; set; }


        /// <summary>
        /// 拜访数
        /// </summary>
        [Reactive] public int? VisitedCount { get; set; }

        /// <summary>
        /// 未拜访数
        /// </summary>
        [Reactive] public int? NoVisitedCount { get; set; }


        /// <summary>
        /// 异常拜访数
        /// </summary>
        [Reactive] public int? ExceptVisitCount { get; set; }

        /// <summary>
        /// 拜访记录
        /// </summary>
        public List<VisitStore> VisitRecords { get; set; } = new List<VisitStore>();

        /// <summary>
        /// 跟踪记录
        /// </summary>
        public List<QueryVisitStoreAndTracking> RealTimeTracks { get; set; } = new List<QueryVisitStoreAndTracking>();

    }



    /// <summary>
    /// 业务员拜访排行
    /// </summary>
    public partial class BusinessVisitRank : EntityBase
    {
        [Reactive] public int BusinessUserId { get; set; }
        public string BusinessUserName { get; set; }
        /// <summary>
        /// 拜访数
        /// </summary>
        public int? VisitedCount { get; set; }

        /// <summary>
        /// 客户数
        /// </summary>
        public int? CustomerCount { get; set; }

        /// <summary>
        /// 未拜访数
        /// </summary>
        public int? NoVisitedCount { get; set; }

        /// <summary>
        /// 异常拜访数
        /// </summary>
        public int? ExceptVisitCount { get; set; }


    }


    /// <summary>
    /// 用于轨迹实时上报
    /// </summary>
    public class TrackingModel : EntityBase
    {

        /// <summary>
        /// 业务员
        /// </summary>
        [Reactive] public int BusinessUserId { get; set; }

        /// <summary>
        /// 业务员
        /// </summary>
        public string BusinessUserName { get; set; }

        /// <summary>
        /// 纬度坐标
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// 经度坐标
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }
        public string Town { get; set; }
        /// <summary>
        /// 县区
        /// </summary>
        public string County { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 上报时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }


        public int Retries { get; set; }
        public DateTime DateLastAttempt { get; set; }
        public DateTime? DateSync { get; set; }
        public DateTime DateCreated { get; set; }

    }

    public partial class RealTimeTrackings : EntityBase
    {
        public IList<QueryVisitStoreAndTracking> Items { get; set; } = new List<QueryVisitStoreAndTracking>();
    }

    public class QueryVisitStoreAndTracking : EntityBase
    {

        /// <summary>
        /// 业务员
        /// </summary>
        [Reactive] public int BusinessUserId { get; set; }

        /// <summary>
        /// 业务员
        /// </summary>
        public string BusinessUserName { get; set; }

        public string Ctype { get; set; }

        public string TerminalName { get; set; }

        /// <summary>
        /// 纬度坐标
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// 经度坐标
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// 上报时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }
    }
}
