using Wesley.Client.Enums;
using Wesley.Infrastructure.Helpers;
using LiteDB;
using Newtonsoft.Json;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Wesley.Client.Models
{
    [JsonObject(MemberSerialization.OptOut)]
    public class EntityBase : Base
    {
        [DataMember]
        [BsonId]
        public int Id { get; set; }
        public int StoreId { get; set; }

        [BsonIgnore]
        public string GUID { get; set; } = Guid.NewGuid().ToString();
    }


    [JsonObject(MemberSerialization.OptOut)]
    public class CacheBillData : EntityBase
    {
        public int TypeId { get; set; }
        public string DataValue { get; set; }
    }


    [JsonObject(MemberSerialization.OptOut)]
    public class CachePaymentMethod : EntityBase
    {
        public int BillTypeId { get; set; }
        public string BillGuid { get; set; }
        public string DataValue { get; set; }
    }

    public interface IBCollection<T>
    {
        /// <summary>
        /// 单据项目
        /// </summary>
        public ObservableCollection<T> Items { get; set; }
    }

    /// <summary>
    /// 用于表示单据抽象基类
    /// </summary>
    public abstract class AbstractBill : EntityBase, ICloneable
    {
        /// <summary>
        /// 单据类型Id
        /// </summary>
        public int BillTypeId { get; set; }
        [Reactive] public int PrintNum { get; set; }
        public string BillTypeName { get; set; }
        public string Navigation { get; set; }
        public BillTypeEnum BillType
        {
            get { return (BillTypeEnum)BillTypeId; }
            set { BillTypeId = (int)value; }
        }

        [Reactive] public string BillNumber { get; set; }

        [Reactive] public int WareHouseId { get; set; }
        [Reactive] public string WareHouseName { get; set; }

        [Reactive] public int ShipmentWareHouseId { get; set; }
        [Reactive] public string ShipmentWareHouseName { get; set; }

        [Reactive] public int IncomeWareHouseId { get; set; }
        [Reactive] public string IncomeWareHouseName { get; set; }


        [Reactive] public int DistrictId { get; set; }
        [Reactive] public string DistrictName { get; set; }

        [Reactive] public int DeliveryUserId { get; set; }
        [Reactive] public string DeliveryUserName { get; set; }

        [Reactive] public int DepartmentId { get; set; }
        [Reactive] public string DepartmentName { get; set; }

        [Reactive] public int BusinessUserId { get; set; }
        [Reactive] public string BusinessUserName { get; set; }

        [Reactive] public int TerminalId { get; set; }
        [Reactive] public string TerminalName { get; set; }

        [Reactive] public string Remark { get; set; }
        public int MakeUserId { get; set; }
        [Reactive] public string MakeUserName { get; set; }
        public DateTime CreatedOnUtc { get; set; } = DateTime.Now;
        public int? AuditedUserId { get; set; }

        [Reactive] public bool AuditedStatus { get; set; }
        [Reactive] public bool IsSubmitBill { get; set; }
        public DateTime? AuditedDate { get; set; }
        public int? ReversedUserId { get; set; }
        [Reactive] public bool ReversedStatus { get; set; }
        public DateTime? ReversedDate { get; set; }

        [Reactive] public decimal SumAmount { get; set; }
        [Reactive] public decimal PreferentialAmount { get; set; }
        [Reactive] public decimal OweCash { get; set; }

        [Reactive] public DateTime? ManufactureDete { get; set; }

        /// <summary>
        /// 生成单号
        /// </summary>
        /// <returns></returns>
        public string GenerateNumber()
        {
            var number = CommonHelper.GetBillNumber(CommonHelper.GetEnumDescription(this.BillType).Split(',')[1], this.StoreId);
            this.BillNumber = number;
            return number;
        }

        public bool IsLast { get; set; }


        public object Clone()
        {
            var clone = (AbstractBill)this.MemberwiseClone();
            //HandleCloned(clone);
            return clone;
        }

        public int CompareTo(object obj)
        {
            return 0;
        }

        [Reactive] public PaymentMethodBaseModel PaymentMethods { get; set; } = new PaymentMethodBaseModel();

    }
    public class AccountMaping : EntityBase
    {
        [Reactive] public string Name { get; set; }
        [Reactive] public int AccountingTypeId { get; set; }
        [Reactive] public int AccountCodeTypeId { get; set; }
        [Reactive] public int AccountingOptionId { get; set; }
        [Reactive] public string AccountingOptionName { get; set; }
        [Reactive] public decimal CollectionAmount { get; set; }
        [Reactive] public int BillId { get; set; }
        [Reactive] public int Number { get; set; }
    }
    public class AbnormalNum
    {
        public int Id { get; set; }
        public int Counter { get; set; }
    }

}
