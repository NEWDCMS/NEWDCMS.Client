using Wesley.Client.Models;
using Wesley.Client.Models.Finances;
using Wesley.Client.Models.Purchases;
using Wesley.Client.Models.Sales;
using Wesley.Client.Models.WareHouses;
using Newtonsoft.Json;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;

namespace Wesley.Client
{
    public static class Settings
    {
        private static ISettings AppSettings => CrossSettings.Current;

        /// <summary>
        /// Token
        /// </summary>
        public static string AccessToken
        {
            get => AppSettings.GetValueOrDefault(nameof(AccessToken), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(AccessToken), value);
        }
        public static DateTime LastUpdatedRefreshTokenTime
        {
            get => AppSettings.GetValueOrDefault(nameof(LastUpdatedRefreshTokenTime), DateTime.MinValue);
            set => AppSettings.AddOrUpdateValue(nameof(LastUpdatedRefreshTokenTime), value);
        }

        /// <summary>
        /// 更新
        /// </summary>
        public static bool IsNextTimeUpdate
        {
            get => AppSettings.GetValueOrDefault(nameof(IsNextTimeUpdate), false);
            set => AppSettings.AddOrUpdateValue(nameof(IsNextTimeUpdate), value);
        }
        public static bool ShowBoarding
        {
            get => AppSettings.GetValueOrDefault(nameof(ShowBoarding), false);
            set => AppSettings.AddOrUpdateValue(nameof(ShowBoarding), value);
        }

        public static bool IsInitData
        {
            get => AppSettings.GetValueOrDefault(nameof(IsInitData), false);
            set => AppSettings.AddOrUpdateValue(nameof(IsInitData), value);
        }

        public static int StoreId
        {
            get => AppSettings.GetValueOrDefault(nameof(StoreId), 0);
            set => AppSettings.AddOrUpdateValue(nameof(StoreId), value);
        }
        public static string StoreName
        {
            get => AppSettings.GetValueOrDefault(nameof(StoreName), "");
            set => AppSettings.AddOrUpdateValue(nameof(StoreName), value);
        }
        public static string DefaultRole
        {
            get => AppSettings.GetValueOrDefault(nameof(DefaultRole), "");
            set => AppSettings.AddOrUpdateValue(nameof(DefaultRole), value);
        }
        public static int UserId
        {
            get => AppSettings.GetValueOrDefault(nameof(UserId), 0);
            set => AppSettings.AddOrUpdateValue(nameof(UserId), value);
        }
        public static string UserRealName
        {
            get => AppSettings.GetValueOrDefault(nameof(UserRealName), "");
            set => AppSettings.AddOrUpdateValue(nameof(UserRealName), value);
        }

        public static string UserEmall
        {
            get => AppSettings.GetValueOrDefault(nameof(UserEmall), "");
            set => AppSettings.AddOrUpdateValue(nameof(UserEmall), value);
        }

        public static string FaceImage
        {
            get => AppSettings.GetValueOrDefault(nameof(FaceImage), "");
            set => AppSettings.AddOrUpdateValue(nameof(FaceImage), value);
        }
        public static string UUID
        {
            get => AppSettings.GetValueOrDefault(nameof(UUID), "");
            set => AppSettings.AddOrUpdateValue(nameof(UUID), value);
        }
        public static string UserMobile
        {
            get => AppSettings.GetValueOrDefault(nameof(UserMobile), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(UserMobile), value);
        }
        public static string UserName
        {
            get => AppSettings.GetValueOrDefault(nameof(UserName), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(UserName), value);
        }
        public static string Password
        {
            get => AppSettings.GetValueOrDefault(nameof(Password), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(Password), value);
        }

        public static string DealerNumber
        {
            get => AppSettings.GetValueOrDefault(nameof(DealerNumber), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(DealerNumber), value);
        }
        public static string MarketingCenter
        {
            get => AppSettings.GetValueOrDefault(nameof(MarketingCenter), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(MarketingCenter), value);
        }

        public static string MarketingCenterCode
        {
            get => AppSettings.GetValueOrDefault(nameof(MarketingCenterCode), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(MarketingCenterCode), value);
        }

        public static string SalesArea
        {
            get => AppSettings.GetValueOrDefault(nameof(SalesArea), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(SalesArea), value);
        }

        public static string SalesAreaCode
        {
            get => AppSettings.GetValueOrDefault(nameof(SalesAreaCode), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(SalesAreaCode), value);
        }

        public static string BusinessDepartment
        {
            get => AppSettings.GetValueOrDefault(nameof(BusinessDepartment), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(BusinessDepartment), value);
        }

        public static string BusinessDepartmentCode
        {
            get => AppSettings.GetValueOrDefault(nameof(BusinessDepartmentCode), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(BusinessDepartmentCode), value);
        }


        /// <summary>
        /// 角色
        /// </summary>
        public static string AvailableUserRoles
        {
            get => AppSettings.GetValueOrDefault(nameof(AvailableUserRoles), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(AvailableUserRoles), value);
        }
        public static string AvailablePermissionRecords
        {
            get => AppSettings.GetValueOrDefault(nameof(AvailablePermissionRecords), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(AvailablePermissionRecords), value);
        }
        public static string CompanySetting
        {
            get => AppSettings.GetValueOrDefault(nameof(CompanySetting), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(CompanySetting), value);
        }
        /// <summary>
        /// 记录最近一次签到客户
        /// </summary>
        public static int LastSigninId
        {
            get => AppSettings.GetValueOrDefault(nameof(LastSigninId), 0);
            set => AppSettings.AddOrUpdateValue(nameof(LastSigninId), value);
        }
        public static int LastSigninCoustmerId
        {
            get => AppSettings.GetValueOrDefault(nameof(LastSigninCoustmerId), 0);
            set => AppSettings.AddOrUpdateValue(nameof(LastSigninCoustmerId), value);
        }
        public static string LastSigninCoustmerName
        {
            get => AppSettings.GetValueOrDefault(nameof(LastSigninCoustmerName), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(LastSigninCoustmerName), value);
        }

        public static string SelectedDeviceName
        {
            get => AppSettings.GetValueOrDefault(nameof(SelectedDeviceName), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(SelectedDeviceName), value);
        }

        public static string ReportsDatas
        {
            get => AppSettings.GetValueOrDefault(nameof(ReportsDatas), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(ReportsDatas), value);
        }
        public static string AppDatas
        {
            get => AppSettings.GetValueOrDefault(nameof(AppDatas), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(AppDatas), value);
        }
        public static string SubscribeDatas
        {
            get => AppSettings.GetValueOrDefault(nameof(SubscribeDatas), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(SubscribeDatas), value);
        }
        public static string PusherMQEndpoint
        {
            get => AppSettings.GetValueOrDefault(nameof(PusherMQEndpoint), GlobalSettings.PushServerEndpoint);
            set => AppSettings.AddOrUpdateValue(nameof(PusherMQEndpoint), value);
        }
        public static int WareHouseId
        {
            get => AppSettings.GetValueOrDefault(nameof(WareHouseId), 0);
            set => AppSettings.AddOrUpdateValue(nameof(WareHouseId), value);
        }
        public static DateTime LastUpdateTime
        {
            get => AppSettings.GetValueOrDefault(nameof(LastUpdateTime), DateTime.MinValue);
            set => AppSettings.AddOrUpdateValue(nameof(LastUpdateTime), value);
        }
        public static void Remove(string key) => AppSettings.Remove(key);
        public static void Remove(string key, string name) => AppSettings.Remove(key, name);
        public static void ClearEverything()
        {
            AppSettings.Clear();
        }
        /// <summary>
        /// 销售单（订单）默认售价（层次价格+价格方案）
        /// </summary>
        public static string DefaultPricePlan
        {
            get => AppSettings.GetValueOrDefault(nameof(DefaultPricePlan), "0_0");
            set => AppSettings.AddOrUpdateValue(nameof(DefaultPricePlan), value);
        }

        public static string DisplayPhotos
        {
            get => AppSettings.GetValueOrDefault(nameof(DisplayPhotos), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(DisplayPhotos), value);
        }
        public static string DoorheadPhotos
        {
            get => AppSettings.GetValueOrDefault(nameof(DoorheadPhotos), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(DoorheadPhotos), value);
        }

        public static string TempAddCustomer
        {
            get => AppSettings.GetValueOrDefault(nameof(TempAddCustomer), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(TempAddCustomer), value);
        }

        public static bool EnableBluetooth
        {
            get => AppSettings.GetValueOrDefault(nameof(EnableBluetooth), false);
            set => AppSettings.AddOrUpdateValue(nameof(EnableBluetooth), value);
        }


        #region 单据共享存储

        private static string AdvanceReceiptBills
        {
            get => AppSettings.GetValueOrDefault(nameof(AdvanceReceiptBills), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(AdvanceReceiptBills), value);
        }
        private static string AllocationBills
        {
            get => AppSettings.GetValueOrDefault(nameof(AllocationBills), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(AllocationBills), value);
        }
        private static string BackStockBills
        {
            get => AppSettings.GetValueOrDefault(nameof(BackStockBills), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(BackStockBills), value);
        }
        private static string CostContractBills
        {
            get => AppSettings.GetValueOrDefault(nameof(CostContractBills), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(CostContractBills), value);
        }
        private static string CostExpenditureBills
        {
            get => AppSettings.GetValueOrDefault(nameof(CostExpenditureBills), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(CostExpenditureBills), value);
        }
        private static string ExchangeBills
        {
            get => AppSettings.GetValueOrDefault(nameof(ExchangeBills), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(ExchangeBills), value);
        }
        private static string InventoryPartTaskBills
        {
            get => AppSettings.GetValueOrDefault(nameof(InventoryPartTaskBills), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(InventoryPartTaskBills), value);
        }
        private static string PurchaseBills
        {
            get => AppSettings.GetValueOrDefault(nameof(AdvanceReceiptBills), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(AdvanceReceiptBills), value);
        }
        private static string CashReceiptBills
        {
            get => AppSettings.GetValueOrDefault(nameof(CashReceiptBills), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(CashReceiptBills), value);
        }
        private static string ReturnBills
        {
            get => AppSettings.GetValueOrDefault(nameof(ReturnBills), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(ReturnBills), value);
        }
        private static string ReturnReservationBills
        {
            get => AppSettings.GetValueOrDefault(nameof(ReturnReservationBills), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(ReturnReservationBills), value);
        }
        private static string SaleBills
        {
            get => AppSettings.GetValueOrDefault(nameof(SaleBills), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(SaleBills), value);
        }
        private static string SaleReservationBills
        {
            get => AppSettings.GetValueOrDefault(nameof(SaleReservationBills), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(SaleReservationBills), value);
        }

        public static AdvanceReceiptBillModel AdvanceReceiptBill
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(Settings.AdvanceReceiptBills))
                        return JsonConvert.DeserializeObject<AdvanceReceiptBillModel>(Settings.AdvanceReceiptBills);
                    else
                        return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    var bill = JsonConvert.SerializeObject(value);
                    Settings.AdvanceReceiptBills = bill;
                }
                catch (Exception)
                {
                    Settings.AdvanceReceiptBills = "";
                }
            }
        }
        public static AllocationBillModel AllocationBill
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(Settings.AllocationBills))
                        return JsonConvert.DeserializeObject<AllocationBillModel>(Settings.AllocationBills);
                    else
                        return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    var bill = JsonConvert.SerializeObject(value);
                    Settings.AllocationBills = bill;
                }
                catch (Exception)
                {
                    Settings.AllocationBills = "";
                }
            }
        }
        public static AllocationBillModel BackStockBill
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(Settings.BackStockBills))
                        return JsonConvert.DeserializeObject<AllocationBillModel>(Settings.BackStockBills);
                    else
                        return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    var bill = JsonConvert.SerializeObject(value);
                    Settings.BackStockBills = bill;
                }
                catch (Exception)
                {
                    Settings.BackStockBills = "";
                }
            }
        }
        public static CostContractBillModel CostContractBill
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(Settings.CostContractBills))
                        return JsonConvert.DeserializeObject<CostContractBillModel>(Settings.CostContractBills);
                    else
                        return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    var bill = JsonConvert.SerializeObject(value);
                    Settings.CostContractBills = bill;
                }
                catch (Exception)
                {
                    Settings.CostContractBills = "";
                }
            }
        }
        public static CostExpenditureBillModel CostExpenditureBill
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(Settings.CostExpenditureBills))
                        return JsonConvert.DeserializeObject<CostExpenditureBillModel>(Settings.CostExpenditureBills);
                    else
                        return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    var bill = JsonConvert.SerializeObject(value);
                    Settings.CostExpenditureBills = bill;
                }
                catch (Exception)
                {
                    Settings.CostExpenditureBills = "";
                }
            }
        }
        public static ExchangeBillModel ExchangeBill
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(Settings.ExchangeBills))
                        return JsonConvert.DeserializeObject<ExchangeBillModel>(Settings.ExchangeBills);
                    else
                        return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    var bill = JsonConvert.SerializeObject(value);
                    Settings.ExchangeBills = bill;
                }
                catch (Exception)
                {
                    Settings.ExchangeBills = "";
                }
            }
        }
        public static InventoryPartTaskBillModel InventoryPartTaskBill
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(Settings.InventoryPartTaskBills))
                        return JsonConvert.DeserializeObject<InventoryPartTaskBillModel>(Settings.InventoryPartTaskBills);
                    else
                        return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    var bill = JsonConvert.SerializeObject(value);
                    Settings.InventoryPartTaskBills = bill;
                }
                catch (Exception)
                {
                    Settings.InventoryPartTaskBills = "";
                }
            }
        }
        public static PurchaseBillModel PurchaseBill
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(Settings.PurchaseBills))
                        return JsonConvert.DeserializeObject<PurchaseBillModel>(Settings.PurchaseBills);
                    else
                        return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    var bill = JsonConvert.SerializeObject(value);
                    Settings.PurchaseBills = bill;
                }
                catch (Exception)
                {
                    Settings.PurchaseBills = "";
                }
            }
        }
        public static CashReceiptBillModel CashReceiptBill
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(Settings.CashReceiptBills))
                        return JsonConvert.DeserializeObject<CashReceiptBillModel>(Settings.CashReceiptBills);
                    else
                        return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    var bill = JsonConvert.SerializeObject(value);
                    Settings.CashReceiptBills = bill;
                }
                catch (Exception)
                {
                    Settings.CashReceiptBills = "";
                }
            }
        }
        public static ReturnBillModel ReturnBill
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(Settings.ReturnBills))
                        return JsonConvert.DeserializeObject<ReturnBillModel>(Settings.ReturnBills);
                    else
                        return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    var bill = JsonConvert.SerializeObject(value);
                    Settings.ReturnBills = bill;
                }
                catch (Exception)
                {
                    Settings.ReturnBills = "";
                }
            }
        }
        public static ReturnReservationBillModel ReturnReservationBill
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(Settings.ReturnReservationBills))
                        return JsonConvert.DeserializeObject<ReturnReservationBillModel>(Settings.ReturnReservationBills);
                    else
                        return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    var bill = JsonConvert.SerializeObject(value);
                    Settings.ReturnReservationBills = bill;
                }
                catch (Exception)
                {
                    Settings.ReturnReservationBills = "";
                }
            }
        }
        public static SaleBillModel SaleBill
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(Settings.SaleBills))
                        return JsonConvert.DeserializeObject<SaleBillModel>(Settings.SaleBills);
                    else
                        return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    var bill = JsonConvert.SerializeObject(value);
                    Settings.SaleBills = bill;
                }
                catch (Exception)
                {
                    Settings.SaleBills = "";
                }
            }
        }
        public static SaleReservationBillModel SaleReservationBill
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(Settings.SaleReservationBills))
                        return JsonConvert.DeserializeObject<SaleReservationBillModel>(Settings.SaleReservationBills);
                    else
                        return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    var bill = JsonConvert.SerializeObject(value);
                    Settings.SaleReservationBills = bill;
                }
                catch (Exception)
                {
                    Settings.SaleReservationBills = "";
                }
            }
        }



        private static string Abnormals
        {
            get => AppSettings.GetValueOrDefault(nameof(Abnormals), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(Abnormals), value);
        }
        public static AbnormalNum Abnormal
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(Settings.Abnormals))
                        return JsonConvert.DeserializeObject<AbnormalNum>(Settings.Abnormals);
                    else
                        return null;
                }
                catch (Exception)
                {
                    return new AbnormalNum { Id = 0, Counter = 0 };
                }
            }
            set
            {
                try
                {
                    var bill = JsonConvert.SerializeObject(value);
                    Settings.Abnormals = bill;
                }
                catch (Exception)
                {
                    Settings.Abnormals = "";
                }
            }
        }

        #endregion
    }

}