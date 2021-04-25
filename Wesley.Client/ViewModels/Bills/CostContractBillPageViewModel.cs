using Wesley.Client.Enums;
using Wesley.Client.Models.Finances;
using Wesley.Client.Models.Products;
using Wesley.Client.Models.Terminals;
using Wesley.Client.Services;
using Wesley.Infrastructure.Helpers;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Wesley.Client.ViewModels
{
    public class CostContractBillPageViewModel : ViewModelBaseCutom<CostContractBillModel>
    {
        private readonly ICostContractService _costContractService;

        public ReactiveCommand<object, Unit> AccountingSelected { get; }
        [Reactive] public CostContractItemModel Selecter { get; set; }

        public CostContractBillPageViewModel(INavigationService navigationService,
                  IProductService productService,
                  IUserService userService,
                  ITerminalService terminalService,
                  IWareHousesService wareHousesService,
                  IAccountingService accountingService,
                  ICostContractService costContractService,
                  IDialogService dialogService) : base(navigationService, productService, terminalService, userService, wareHousesService, accountingService, dialogService)
        {
            Title = "费用合同";

            _costContractService = costContractService;
            this.BillType = BillTypeEnum.CostContractBill;

            InitBill();

            //验证
            var valid_TerminalId = this.ValidationRule(x => x.Bill.CustomerId, _isZero, "客户未指定");
            var valid_AccountingOptionId = this.ValidationRule(x => x.Bill.AccountingOptionId, _isZero, "费用类型未指定");
            var valid_ItemCount = this.ValidationRule(x => x.Bill.Items.Count, _isZero, "合同项目为空，请先添加");
            var valid_IsVieweBill = this.ValidationRule(x => x.Bill.AuditedStatus, _isBool, "已审核单据不能操作");


            //编辑
            this.WhenAnyValue(x => x.Selecter).Throttle(TimeSpan.FromMilliseconds(500))
             .Skip(1)
             .Where(x => x != null)
             .SubOnMainThread(async x =>
            {
                if (x != null)
                {
                    var product = ProductSeries.Select(p => p).Where(p => p.ProductId == x.ProductId).FirstOrDefault();
                    if (product != null)
                    {
                        product.UnitId = x.UnitId ?? 0;
                        product.Quantity = x.TotalQuantity;
                        product.Remark = x.Remark;
                        product.UnitName = x.UnitName;

                        if (x.BigUnitId > 0)
                        {
                            product.Id = x.Id;
                            product.bigOption.Name = x.UnitName;
                            product.BigPriceUnit.Quantity = x.TotalQuantity;
                            product.BigPriceUnit.Remark = x.Remark;
                            product.BigPriceUnit.UnitId = x.BigUnitId ?? 0;
                        }

                        if (x.SmallUnitId > 0)
                        {
                            product.Id = x.Id;
                            product.bigOption.Name = x.UnitName;
                            product.SmallPriceUnit.Quantity = x.TotalQuantity;
                            product.SmallPriceUnit.Remark = x.Remark;
                            product.SmallPriceUnit.UnitId = x.SmallUnitId ?? 0;
                        }

                        await this.NavigateAsync("EditAllocationProductPage", ("Product", product));
                    }
                }

                this.Selecter = null;
            }, ex => _dialogService.ShortAlert(ex.ToString()));

            //提交单据
            this.SubmitDataCommand = ReactiveCommand.CreateFromTask<object, Unit>(async _ =>
            {
                var postData = new CostContractUpdateModel()
                {
                    EmployeeId = Settings.UserId,
                    CustomerId = Bill.CustomerId,
                    AccountingOptionId = Bill.AccountingOptionId,
                    Year = DateTime.Now.Year,
                    Month = DateTime.Now.Month,
                    ContractType = 1, //按单位量总计兑付
                    SaleRemark = Bill.SaleRemark,
                    Remark = Bill.Remark,
                    Items = Bill.Items
                };

                return await SubmitAsync(postData, Bill.Id, _costContractService.CreateOrUpdateAsync, (result) =>
                {
                    Bill = new CostContractBillModel();
                }, token: cts.Token);
            },
            this.IsValid());

            //存储记录
            this.SaveCommand = ReactiveCommand.Create<object>(async e =>
            {
                if (!this.Bill.AuditedStatus && this.Bill.CustomerId != 0 && this.Bill.CustomerId != Settings.CostContractBill?.CustomerId)
                {
                    var ok = await _dialogService.ShowConfirmAsync("你是否要保存单据？", "提示", "确定", "取消");
                    if (ok)
                    {
                        if (!this.Bill.AuditedStatus)
                            Settings.CostContractBill = this.Bill;
                    }
                }
                await _navigationService.GoBackAsync();
            });

            //费用选择
            this.AccountingSelected = ReactiveCommand.Create<object>(async e =>
            {
                if (!valid_TerminalId.IsValid) { this.Alert(valid_TerminalId.Message[0]); return; }
                await SelectCostAccounting((data) =>
                 {
                     Bill.AccountingOptionId = data.AccountingOptionId;
                     Bill.AccountingOptionName = data.Name;
                 }, BillTypeEnum.CostContractBill);
            });

            //添加商品
            this.AddProductCommand = ReactiveCommand.Create<object>(async e =>
           {
               if (!valid_TerminalId.IsValid) { this.Alert(valid_TerminalId.Message[0]); return; }
               if (!valid_AccountingOptionId.IsValid) { this.Alert(valid_AccountingOptionId.Message[0]); return; }
               if (!valid_IsVieweBill.IsValid) { this.Alert(valid_IsVieweBill.Message[0]); return; }

               await this.NavigateAsync("SelectProductPage", ("Reference", this.PageName), ("SerchKey", Filter.SerchKey));
           });

            //审核
            this.AuditingDataCommand = ReactiveCommand.Create<object>(async _ =>
            {
                //是否具有审核权限
                await this.Access(AccessGranularityEnum.CostContractApproved);
                await SubmitAsync(Bill.Id, _costContractService.AuditingAsync, async (result) =>
                {
                    var db = Shiny.ShinyHost.Resolve<LocalDatabase>();
                    await db.SetPending(SelecterMessage.Id, true);
                });
            }, this.WhenAny(x => x.Bill.Id, (x) => x.GetValue() > 0));

            //菜单选择
            this.SetMenus((x) =>
            {
                switch (x)
                {
                    case Enums.MenuEnum.REMARK: //整单备注
                        AllRemak((result) => { Bill.Remark = result; }, Bill.Remark);
                        break;
                    case Enums.MenuEnum.CLEAR: //清空单据
                        {
                            ClearBill<CostContractBillModel, CostContractItemModel>(Bill, DoClear);
                        }
                        break;
                    case Enums.MenuEnum.SALEREMARK: //销售备注
                        AllRemak((result) => { Bill.SaleRemark = result; }, Bill.SaleRemark);
                        break;
                }
            }, 3, 20, 4);

            this.AccountingSelected.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.AddProductCommand.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));
            this.ExceptionsSubscribe();
        }

        private void InitBill(bool clear = false)
        {
            this.BillType = BillTypeEnum.CostContractBill;
            var bill = Settings.CostContractBill;
            if (bill != null && !clear)
            {
                this.Bill = bill;
                this.Bill.Items = new ObservableCollection<CostContractItemModel>(bill?.Items);
                this.Selecter = bill?.Items?.FirstOrDefault();
            }
            else
            {
                this.Bill = new CostContractBillModel()
                {
                    BillTypeId = (int)this.BillType,
                    MakeUserId = Settings.UserId,
                    MakeUserName = Settings.UserRealName,
                    CreatedOnUtc = DateTime.Now,
                    BillNumber = CommonHelper.GetBillNumber(CommonHelper.GetEnumDescription(BillType).Split(',')[1], Settings.StoreId)
                };
            }
        }
        private void DoClear()
        {
            InitBill(true);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            try
            {
                //删除商品
                if (parameters.ContainsKey("DelProduct"))
                {
                    parameters.TryGetValue("DelProduct", out ProductModel p);
                    if (p != null)
                    {
                        var bigProduct = Bill.Items.Where(b => b.Id == p.Id && b.BigUnitId == p.UnitId).FirstOrDefault();
                        var smalProduct = Bill.Items.Where(b => b.Id == p.Id && b.SmallUnitId == p.UnitId).FirstOrDefault();
                        if (bigProduct != null)
                        {
                            Bill.Items.Remove(bigProduct);
                        }

                        if (smalProduct != null)
                        {
                            Bill.Items.Remove(smalProduct);
                        }
                    }
                }

                //编辑商品更新
                if (parameters.ContainsKey("UpdateProduct"))
                {
                    parameters.TryGetValue("UpdateProduct", out ProductModel p);
                    if (p != null)
                    {
                        var bigProduct = Bill.Items.Where(b => b.ProductId == p.ProductId && b.BigUnitId == p.UnitId).FirstOrDefault();
                        var smalProduct = Bill.Items.Where(b => b.ProductId == p.ProductId && b.SmallUnitId == p.UnitId).FirstOrDefault();
                        if (bigProduct != null)
                        {
                            bigProduct.UnitName = p.UnitName;
                            bigProduct.BigUnitQuantity = p.Quantity;
                            bigProduct.Total = p.Quantity;
                            bigProduct.TotalQuantity = p.Quantity;
                            bigProduct.Remark = p.Remark;
                            bigProduct.BigUnitId = p.UnitId;
                        }
                        if (smalProduct != null)
                        {
                            smalProduct.UnitName = p.UnitName;
                            smalProduct.SmallUnitQuantity = p.Quantity;
                            smalProduct.Total = p.Quantity;
                            smalProduct.TotalQuantity = p.Quantity;
                            smalProduct.Remark = p.Remark;
                            smalProduct.BigUnitId = p.UnitId;
                        }

                        Bill.AllNum = Bill.Items.Count;
                        Bill.BigNum = Bill.Items.Select(s => s.BigUnitQuantity).Sum() ?? 0;
                        Bill.SmallNum = Bill.Items.Select(s => s.SmallUnitQuantity).Sum() ?? 0;
                    }
                }

                //选择客户
                if (parameters.ContainsKey("Terminaler"))
                {
                    parameters.TryGetValue("Terminaler", out TerminalModel terminaler);
                    Bill.CustomerId = terminaler != null ? terminaler.Id : 0;
                    Bill.CustomerName = terminaler != null ? terminaler.Name : "";
                }

                //选择商品序列
                if (parameters.ContainsKey("ProductSeries"))
                {
                    parameters.TryGetValue("ProductSeries", out List<ProductModel> productSeries);
                    this.ProductSeries = new ObservableCollection<ProductModel>(productSeries);

                    foreach (var p in productSeries)
                    {
                        var bigItem = new CostContractItemModel()
                        {
                            //Id = p.Id,
                            UnitId = p.BigPriceUnit.UnitId,
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            UnitName = p.bigOption.Name,
                            BigUnitQuantity = p.BigPriceUnit.Quantity,
                            TotalQuantity = p.BigPriceUnit.Quantity,
                            Total = p.BigPriceUnit.Quantity,
                            Remark = p.BigPriceUnit.Remark,
                            BigUnitId = p.BigPriceUnit.UnitId,
                        };

                        var smallItem = new CostContractItemModel()
                        {
                            //Id = p.Id,
                            UnitId = p.SmallPriceUnit.UnitId,
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            UnitName = p.smallOption.Name,
                            SmallUnitQuantity = p.SmallPriceUnit.Quantity,
                            TotalQuantity = p.SmallPriceUnit.Quantity,
                            Total = p.SmallPriceUnit.Quantity,
                            Remark = p.SmallPriceUnit.Remark,
                            SmallUnitId = p.SmallPriceUnit.UnitId,
                        };

                        if (bigItem.TotalQuantity > 0 && Bill.Items.Where(s => s.ProductId == bigItem.ProductId && s.UnitId == bigItem.UnitId && s.TotalQuantity == bigItem.TotalQuantity).Count() == 0)
                            Bill.Items.Add(bigItem);

                        if (smallItem.TotalQuantity > 0 && Bill.Items.Where(s => s.ProductId == smallItem.ProductId && s.UnitId == smallItem.UnitId && s.TotalQuantity == smallItem.TotalQuantity).Count() == 0)
                            Bill.Items.Add(smallItem);
                    }

                    Bill.AllNum = Bill.Items.Count;
                    Bill.BigNum = Bill.Items.Select(s => s.BigUnitQuantity).Sum() ?? 0;
                    Bill.SmallNum = Bill.Items.Select(s => s.SmallUnitQuantity).Sum() ?? 0;
                }

                //预览单据
                if (parameters.ContainsKey("Bill"))
                {
                    parameters.TryGetValue("Bill", out CostContractBillModel bill);
                    if (bill != null)
                    {
                        Bill = bill;
                        if (bill.AuditedStatus)
                        {

                        }
                        else
                        {
                            this.SetMenus((x) =>
                            {
                                switch (x)
                                {
                                    case Enums.MenuEnum.SHENGHE://审核
                                        ((ICommand)AuditingDataCommand)?.Execute(null);
                                        break;
                                }
                            }, 34);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}
