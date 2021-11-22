using Android.Content;
using Wesley.Client.Models;
using Wesley.Client.Models.Finances;
using Wesley.Client.Models.Purchases;
using Wesley.Client.Models.Sales;
using Wesley.Client.Models.WareHouses;
using Wesley.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Wesley.Client.Droid
{
    public interface IPrintDataMaker
    {
        List<byte[]> GetPrintData(int type);
        List<byte[]> GetBillPrintData(int type, AbstractBill bill);
    }


    /// <summary>
    /// 单据打印
    /// </summary>
    public class PrintOrderDataMaker : IPrintDataMaker
    {
        private string qr;
        private int width;
        private int height;
        private Context btService;
        private string remark = "备注：本店出售商品质量合格，如果有误请在三日内联系，过期不负责任！";

        public PrintOrderDataMaker(Context btService, String qr, int width, int height)
        {
            this.qr = qr;
            this.width = width;
            this.height = height;
            this.btService = btService;
        }

        /// <summary>
        /// 测试数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<byte[]> GetPrintData(int type)
        {
            try
            {
                PrinterWriter printer;
                List<byte[]> data = new List<byte[]>();

                var height = PrinterWriter.HEIGHT_PARTING_DEFAULT;

                //默认80
                width = PrinterWriter80mm.TYPE_80;
                printer = new PrinterWriter80mm(height, width);

                if (type == 76)
                {
                    width = PrinterWriter76mm.TYPE_76;
                    printer = new PrinterWriter76mm(height, width);
                }
                else if (type == 58)
                {
                    var width = PrinterWriter58mm.TYPE_58;
                    printer = new PrinterWriter58mm(height, width);
                }

                printer.SetAlignCenter();
                data.Add(printer.GetDataAndReset());

                printer.PrintLineFeed();
                printer.SetAlignCenter();
                printer.SetEmphasizedOn();
                printer.SetFontSize(1);
                printer.Print("陕西玖鉴商贸有限公司");
                printer.PrintLineFeed();
                printer.SetEmphasizedOff();
                printer.PrintLineFeed();

                printer.PrintLineFeed();
                printer.SetFontSize(0);
                printer.SetAlignLeft();
                printer.Print("订单编号：" + "546545645465456454");
                printer.PrintLineFeed();

                printer.SetAlignLeft();
                printer.Print("打印时间：" + $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                printer.PrintLineFeed();
                printer.PrintLine();

                printer.PrintLineFeed();
                printer.SetAlignLeft();
                printer.Print("订单状态: " + "已接单");
                printer.PrintLineFeed();
                printer.Print("用户昵称: " + "周末先生");
                printer.PrintLineFeed();
                printer.Print("用餐人数: " + "10人");
                printer.PrintLineFeed();
                printer.Print("用餐桌号: " + "A3" + "号桌");
                printer.PrintLineFeed();
                printer.Print("预定时间：" + "2017-10-1 17：00");
                printer.PrintLineFeed();
                printer.Print("预留时间：30分钟");
                printer.PrintLineFeed();
                printer.Print("联系方式：" + "18094111545454");
                printer.PrintLineFeed();
                printer.PrintLine();
                printer.PrintLineFeed();

                printer.SetAlignLeft();
                printer.Print("备注：" + "记得留位置");
                printer.PrintLineFeed();
                printer.PrintLine();

                printer.PrintLineFeed();

                printer.SetAlignLeft();
                printer.Print("菜品信息");
                printer.PrintLineFeed();
                printer.SetAlignLeft();
                printer.PrintInOneLine("菜名", "数量", "单价", 0);
                printer.PrintLineFeed();
                for (int i = 0; i < 3; i++)
                {
                    printer.PrintInOneLine("干锅包菜", "X" + 3, "￥" + 30, 0);
                    printer.PrintLineFeed();
                }
                printer.PrintLineFeed();
                printer.PrintLine();
                printer.PrintLineFeed();
                printer.SetAlignLeft();
                printer.PrintInOneLine("菜品总额：", "￥" + 100, 0);
                printer.PrintLineFeed();

                printer.SetAlignLeft();
                printer.PrintInOneLine("优惠金额：", "￥" + "0.00", 0);
                printer.PrintLineFeed();

                printer.SetAlignLeft();
                printer.PrintInOneLine("订金/退款：", "￥" + "0.00", 0);
                printer.PrintLineFeed();

                printer.SetAlignLeft();
                printer.PrintInOneLine("总计金额：", "￥" + 90, 0);
                printer.PrintLineFeed();

                printer.PrintLine();
                printer.PrintLineFeed();
                printer.SetAlignLeft();
                printer.Print(remark);
                printer.PrintLineFeed();
                printer.PrintLineFeed();
                printer.FeedPaperCutPartial();

                data.Add(printer.GetDataAndClose());

                return data;
            }
            catch (Exception e)
            {
                return null;
            }

        }


        /// <summary>
        /// 单据数据
        /// </summary>
        /// <param name="bill"></param>
        /// <returns></returns>
        public List<byte[]> GetBillPrintData(int type, AbstractBill bill)
        {

            PrinterWriter printer;
            List<byte[]> data = new List<byte[]>();

            var height = PrinterWriter.HEIGHT_PARTING_DEFAULT;

            try
            {
                //默认80
                width = PrinterWriter80mm.TYPE_80;
                printer = new PrinterWriter80mm(height, width);

                if (type == 76)
                {
                    width = PrinterWriter76mm.TYPE_76;
                    printer = new PrinterWriter76mm(height, width);
                }
                else if (type == 58)
                {
                    var width = PrinterWriter58mm.TYPE_58;
                    printer = new PrinterWriter58mm(height, width);
                }

                printer.SetAlignCenter();
                data.Add(printer.GetDataAndReset());

                //调拨凭证
                if (bill is AllocationBillModel ab)
                {
                    #region

                    //经销商名称
                    printer.PrintLineFeed();
                    printer.SetAlignCenter();
                    printer.SetEmphasizedOn();
                    printer.SetFontSize(1);

                    printer.Print("调拨凭证");
                    printer.PrintLineFeed();
                    printer.SetEmphasizedOff();
                    printer.PrintLineFeed();

                    printer.SetAlignCenter();
                    printer.SetFontSize(0);
                    printer.Print($"--**{Settings.StoreName}**--");
                    printer.SetEmphasizedOff();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("订单编号：" + $"{ab.BillNumber}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("单据类型：" + $"调拨单");
                    printer.PrintLineFeed();


                    printer.SetAlignLeft();
                    printer.Print("操作时间：" + $"{ab.CreatedOnUtc:yyyy-MM-dd HH:mm:ss}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("打印时间：" + $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    printer.PrintLineFeed();


                    printer.PrintLine();
                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.PrintInOneLine(0, new string[] { "商品", "数量", "单位", "金额" }, new int[] { 2, 2, 1, 2 }, new bool[] { false, true, true, true });
                    printer.PrintLineFeed();
                    foreach (var item in ab.Items)
                    {
                        string subTotalStr = $"￥{item.Subtotal:#.00}";
                        if (item.Subtotal == 0)
                        {
                            subTotalStr = "0";
                        }
                        printer.PrintInOneLine(0, new string[] { item?.ProductName == null ? "" : item?.ProductName, $"*{item.Quantity}", item?.UnitName == null ? "" : item?.UnitName, subTotalStr }, new int[] { 2, 2, 1, 2 }, new bool[] { false, true, true, true });
                        printer.PrintLineFeed();
                    }
                    printer.PrintLine();
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("出货仓库：" + $"{ab.ShipmentWareHouseName}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("入货仓库：" + $"{ab.IncomeWareHouseName}");
                    printer.PrintLineFeed();

                    printer.PrintLine();
                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.Print("开 单 人：" + $"{ab.MakeUserName}");
                    printer.PrintLineFeed();
                    printer.PrintLine();

                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.Print(remark);
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.FeedPaperCutPartial();

                    data.Add(printer.GetDataAndClose());



                    #endregion
                }
                else if (bill is SaleReservationBillModel srb)
                {
                    #region
                    //经销商名称
                    printer.PrintLineFeed();
                    printer.SetAlignCenter();
                    printer.SetEmphasizedOn();
                    printer.SetFontSize(1);
                    printer.Print("销售订单凭证");
                    printer.PrintLineFeed();
                    printer.SetEmphasizedOff();
                    printer.PrintLineFeed();

                    printer.SetAlignCenter();
                    printer.SetFontSize(0);

                    printer.Print($"--**{Settings.StoreName}**--");
                    printer.SetEmphasizedOff();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("订单编号：" + $"{srb.BillNumber}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("单据类型：" + $"销售订单");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("客    户：" + $"{srb.TerminalName}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("操作时间：" + $"{srb.CreatedOnUtc:yyyy-MM-dd HH:mm:ss}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("打印时间：" + $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    printer.PrintLineFeed();


                    printer.PrintLine();
                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.PrintInOneLine(0, new string[] { "商品", "数量", "单位", "金额" }, new int[] { 2, 2, 1, 2 }, new bool[] { false, true, true, true });
                    printer.PrintLineFeed();
                    foreach (var item in srb.Items)
                    {
                        string subTotalStr = $"￥{item.Subtotal:#.00}";
                        if (item.Subtotal == 0 && item.IsGifts)
                        {
                            subTotalStr = string.IsNullOrEmpty(item.Remark) ? "赠品" : item.Remark;
                        }
                        printer.PrintInOneLine(0, new string[] { item?.ProductName == null ? "" : item?.ProductName, $"*{item.Quantity}", item?.UnitName == null ? "" : item?.UnitName, subTotalStr }, new int[] { 2, 2, 1, 2 }, new bool[] { false, true, true, true });
                        printer.PrintLineFeed();
                    }
                    printer.PrintLine();
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("合计：" + $"￥{srb.SumAmount}");
                    printer.PrintLineFeed();
                    printer.Print($"大写：{NumberUtils.MoneyToUpper(srb.SumAmount)}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("优惠：" + $"￥{srb.PreferentialAmount}");
                    printer.PrintLineFeed();
                    printer.Print($"大写：{NumberUtils.MoneyToUpper(srb.PreferentialAmount)}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("欠款：" + $"￥{srb.OweCash}");
                    printer.PrintLineFeed();
                    printer.Print($"大写：{NumberUtils.MoneyToUpper(srb.OweCash)}");
                    printer.PrintLineFeed();

                    printer.PrintLine();
                    printer.SetAlignLeft();
                    printer.Print($"备注：{srb.Remark}");
                    printer.PrintLineFeed();

                    printer.PrintLine();
                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.Print("业 务 员：" + $"{srb.BusinessUserName}  电话：{Settings.UserMobile}");
                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.Print("签 名 栏：");
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.PrintLine();

                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.Print(remark);
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.FeedPaperCutPartial();

                    data.Add(printer.GetDataAndClose());

                    #endregion
                }
                else if (bill is SaleBillModel sb)
                {
                    #region
                    //经销商名称
                    printer.PrintLineFeed();
                    printer.SetAlignCenter();
                    printer.SetEmphasizedOn();
                    printer.SetFontSize(1);

                    printer.Print("销售单凭证");
                    printer.PrintLineFeed();
                    printer.SetEmphasizedOff();
                    printer.PrintLineFeed();

                    printer.SetAlignCenter();
                    printer.SetFontSize(0);
                    printer.Print($"--**{Settings.StoreName}**--");
                    printer.SetEmphasizedOff();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("订单编号：" + $"{sb.BillNumber}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("单据类型：" + $"销售单");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("客    户：" + $"{sb.TerminalName}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("操作时间：" + $"{sb.CreatedOnUtc:yyyy-MM-dd HH:mm:ss}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("打印时间：" + $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    printer.PrintLineFeed();


                    printer.PrintLine();
                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.PrintInOneLine(0, new string[] { "商品", "数量", "单位", "金额" }, new int[] { 2, 2, 1, 2 }, new bool[] { false, true, true, true });
                    printer.PrintLineFeed();
                    foreach (var item in sb.Items)
                    {
                        string subTotalStr = $"￥{item.Subtotal:#.00}";
                        if (item.Subtotal == 0 && item.IsGifts)
                        {
                            subTotalStr = string.IsNullOrEmpty(item.Remark) ? "赠品" : item.Remark;
                        }
                        printer.PrintInOneLine(0, new string[] { item?.ProductName == null ? "" : item?.ProductName, $"*{item.Quantity}", item?.UnitName == null ? "" : item?.UnitName, subTotalStr }, new int[] { 2, 2, 1, 2 }, new bool[] { false, true, true, true });
                        printer.PrintLineFeed();
                    }
                    printer.PrintLine();
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("合计：" + $"￥{sb.SumAmount}");
                    printer.PrintLineFeed();
                    printer.Print($"大写：{NumberUtils.MoneyToUpper(sb.SumAmount)}");
                    printer.PrintLineFeed();


                    printer.SetAlignLeft();
                    printer.Print("优惠：" + $"￥{sb.PreferentialAmount}");
                    printer.PrintLineFeed();
                    printer.Print($"大写：{NumberUtils.MoneyToUpper(sb.PreferentialAmount)}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("欠款：" + $"￥{sb.OweCash}");
                    printer.PrintLineFeed();
                    printer.Print($"大写：{NumberUtils.MoneyToUpper(sb.OweCash)}");
                    printer.PrintLineFeed();

                    printer.PrintLine();
                    printer.SetAlignLeft();
                    printer.Print($"备注：{sb.Remark}");
                    printer.PrintLineFeed();

                    printer.PrintLine();
                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.Print($"业 务 员：{sb.BusinessUserName}  电话：{Settings.UserMobile}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("签 名 栏：");
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.PrintLine();

                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.Print(remark);
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.FeedPaperCutPartial();

                    data.Add(printer.GetDataAndClose());



                    #endregion
                }
                else if (bill is ReturnReservationBillModel rrb)
                {
                    #region

                    //经销商名称
                    printer.PrintLineFeed();
                    printer.SetAlignCenter();
                    printer.SetEmphasizedOn();
                    printer.SetFontSize(1);

                    printer.Print("退货订单凭证");
                    printer.PrintLineFeed();
                    printer.SetEmphasizedOff();
                    printer.PrintLineFeed();

                    printer.SetAlignCenter();
                    printer.SetFontSize(0);
                    printer.Print($"--**{Settings.StoreName}**--");
                    printer.SetEmphasizedOff();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();


                    printer.SetAlignLeft();
                    printer.Print("订单编号：" + $"{rrb.BillNumber}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("单据类型：" + $"退货订单");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("客    户：" + $"{rrb.TerminalName}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("操作时间：" + $"{rrb.CreatedOnUtc:yyyy-MM-dd HH:mm:ss}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("打印时间：" + $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    printer.PrintLineFeed();


                    printer.PrintLine();
                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.PrintInOneLine(0, new string[] { "商品", "数量", "单位", "金额" }, new int[] { 2, 2, 1, 2 }, new bool[] { false, true, true, true });
                    printer.PrintLineFeed();
                    foreach (var item in rrb.Items)
                    {
                        string subTotalStr = $"￥{item.Subtotal:#.00}";
                        if (item.Subtotal == 0 && item.IsGifts)
                        {
                            subTotalStr = string.IsNullOrEmpty(item.Remark) ? "赠品" : item.Remark;
                        }
                        printer.PrintInOneLine(0, new string[] { item?.ProductName == null ? "" : item?.ProductName, $"*{item.Quantity}", item?.UnitName == null ? "" : item?.UnitName, subTotalStr }, new int[] { 2, 2, 1, 2 }, new bool[] { false, true, true, true });
                        printer.PrintLineFeed();
                    }
                    printer.PrintLine();
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("合计：" + $"￥{rrb.SumAmount}");
                    printer.PrintLineFeed();
                    printer.Print($"大写：{NumberUtils.MoneyToUpper(rrb.SumAmount)}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("优惠：" + $"￥{rrb.PreferentialAmount}");
                    printer.PrintLineFeed();
                    printer.Print($"大写：{NumberUtils.MoneyToUpper(rrb.PreferentialAmount)}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("欠款：" + $"￥{rrb.OweCash}");
                    printer.PrintLineFeed();
                    printer.Print($"大写：{NumberUtils.MoneyToUpper(rrb.OweCash)}");
                    printer.PrintLineFeed();

                    printer.PrintLine();
                    printer.SetAlignLeft();
                    printer.Print($"备注：{rrb.Remark}");
                    printer.PrintLineFeed();

                    printer.PrintLine();
                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.Print("业 务 员：" + $"{rrb.BusinessUserName}  电话：{Settings.UserMobile}");
                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.Print("签 名 栏：");
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.PrintLine();

                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.Print(remark);
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.FeedPaperCutPartial();

                    data.Add(printer.GetDataAndClose());


                    #endregion
                }
                else if (bill is ReturnBillModel rb)
                {
                    #region

                    //经销商名称
                    printer.PrintLineFeed();
                    printer.SetAlignCenter();
                    printer.SetEmphasizedOn();
                    printer.SetFontSize(1);

                    printer.Print("退货单凭证");
                    printer.PrintLineFeed();
                    printer.SetEmphasizedOff();
                    printer.PrintLineFeed();

                    printer.SetAlignCenter();
                    printer.SetFontSize(0);
                    printer.Print($"--**{Settings.StoreName}**--");
                    printer.SetEmphasizedOff();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("订单编号：" + $"{rb.BillNumber}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("单据类型：" + $"退货单");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("客    户：" + $"{rb.TerminalName}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("操作时间：" + $"{rb.CreatedOnUtc:yyyy-MM-dd HH:mm:ss}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("打印时间：" + $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    printer.PrintLineFeed();


                    printer.PrintLine();
                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.PrintInOneLine(0, new string[] { "商品", "数量", "单位", "金额" }, new int[] { 2, 2, 1, 2 }, new bool[] { false, true, true, true });
                    printer.PrintLineFeed();
                    foreach (var item in rb.Items)
                    {
                        string subTotalStr = $"￥{item.Subtotal:#.00}";
                        if (item.Subtotal == 0 && item.IsGifts)
                        {
                            subTotalStr = string.IsNullOrEmpty(item.Remark) ? "赠品" : item.Remark;
                        }
                        printer.PrintInOneLine(0, new string[] { item?.ProductName == null ? "" : item?.ProductName, $"*{item.Quantity}", item?.UnitName == null ? "" : item?.UnitName, subTotalStr }, new int[] { 2, 2, 1, 2 }, new bool[] { false, true, true, true });
                        printer.PrintLineFeed();
                    }
                    printer.PrintLine();
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("合计：" + $"￥{rb.SumAmount}");
                    printer.PrintLineFeed();
                    printer.Print($"大写：{NumberUtils.MoneyToUpper(rb.SumAmount)}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("优惠：" + $"￥{rb.PreferentialAmount}");
                    printer.PrintLineFeed();
                    printer.Print($"大写：{NumberUtils.MoneyToUpper(rb.PreferentialAmount)}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("欠款：" + $"￥{rb.OweCash}");
                    printer.PrintLineFeed();
                    printer.Print($"大写：{NumberUtils.MoneyToUpper(rb.OweCash)}");
                    printer.PrintLineFeed();

                    printer.PrintLine();
                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.Print("业 务 员：" + $"{rb.BusinessUserName}  电话：{Settings.UserMobile}");
                    printer.PrintLineFeed();
                    printer.Print("签 名 栏：");
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.PrintLine();

                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.Print(remark);
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.FeedPaperCutPartial();

                    data.Add(printer.GetDataAndClose());


                    #endregion
                }
                else if (bill is CashReceiptBillModel rcp)
                {
                    #region
                    rcp.SumAmount = rcp.Items.Sum(c => c.ReceivableAmountOnce) ?? 0; //本次收款合计
                    rcp.PreferentialAmount = rcp.Items.Sum(c => c.DiscountAmountOnce) ?? 0; //本次优惠合计

                    //经销商名称
                    printer.PrintLineFeed();
                    printer.SetAlignCenter();
                    printer.SetEmphasizedOn();
                    printer.SetFontSize(1);

                    printer.Print("收款凭证");
                    printer.PrintLineFeed();
                    printer.SetEmphasizedOff();
                    printer.PrintLineFeed();

                    printer.SetAlignCenter();
                    printer.SetFontSize(0);
                    printer.Print($"--**{Settings.StoreName}**--");
                    printer.SetEmphasizedOff();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("订单编号：" + $"{rcp.BillNumber}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("单据类型：" + $"收款单");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("客    户：" + $"{rcp.TerminalName}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("操作时间：" + $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("打印时间：" + $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    printer.PrintLineFeed();


                    printer.PrintLine();
                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.PrintInOneLine("单据号", "本次优惠", "本次收款", 0);
                    printer.PrintLineFeed();
                    foreach (var item in rcp.Items)
                    {
                        printer.PrintInOneLine(item.BillNumber, "￥" + item.DiscountAmountOnce, "￥" + $"{item.ReceivableAmountOnce:#.00}", 0);
                        printer.PrintLineFeed();
                    }
                    printer.PrintLine();
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("合计：" + $"￥{rcp.SumAmount}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("优惠：" + $"￥{rcp.PreferentialAmount}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("欠款：" + $"￥{rcp.OweCash}");
                    printer.PrintLineFeed();

                    printer.PrintLine();
                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.Print("业 务 员：" + $"{rcp.BusinessUserName}");
                    printer.PrintLineFeed();
                    printer.PrintLine();

                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.Print(remark);
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.FeedPaperCutPartial();

                    data.Add(printer.GetDataAndClose());


                    #endregion
                }
                else if (bill is PurchaseBillModel pb)
                {
                    #region

                    //经销商名称
                    printer.PrintLineFeed();
                    printer.SetAlignCenter();
                    printer.SetEmphasizedOn();
                    printer.SetFontSize(1);

                    printer.Print("采购单凭证");
                    printer.PrintLineFeed();
                    printer.SetEmphasizedOff();
                    printer.PrintLineFeed();

                    printer.SetAlignCenter();
                    printer.SetFontSize(0);
                    printer.Print($"--**{Settings.StoreName}**--");
                    printer.SetEmphasizedOff();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("订单编号：" + $"{pb.BillNumber}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("单据类型：" + $"采购单");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("客    户：" + $"{pb.TerminalName}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("操作时间：" + $"{pb.CreatedOnUtc:yyyy-MM-dd HH:mm:ss}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("打印时间：" + $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    printer.PrintLineFeed();


                    printer.PrintLine();
                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.PrintInOneLine(0, new string[] { "商品", "数量", "单位", "金额" }, new int[] { 2, 2, 1, 2 }, new bool[] { false, true, true, true });
                    printer.PrintLineFeed();
                    foreach (var item in pb.Items)
                    {
                        string subTotalStr = $"￥{item.Subtotal:#.00}";
                        if (item.Subtotal == 0)
                        {
                            subTotalStr = "0";
                        }
                        printer.PrintInOneLine(0, new string[] { item?.ProductName == null ? "" : item?.ProductName, $"*{item.Quantity}", item?.UnitName == null ? "" : item?.UnitName, subTotalStr }, new int[] { 2, 2, 1, 2 }, new bool[] { false, true, true, true });
                        printer.PrintLineFeed();
                    }
                    printer.PrintLine();
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("合计：" + $"￥{pb.SumAmount}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("优惠：" + $"￥{pb.PreferentialAmount}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("欠款：" + $"￥{pb.OweCash}");
                    printer.PrintLineFeed();

                    printer.PrintLine();
                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.Print("业 务 员：" + $"{pb.BusinessUserName}");
                    printer.PrintLineFeed();
                    printer.PrintLine();

                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.Print(remark);
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.FeedPaperCutPartial();

                    data.Add(printer.GetDataAndClose());


                    #endregion
                }
                else if (bill is InventoryPartTaskBillModel ipt)
                {
                    #region

                    //经销商名称
                    printer.PrintLineFeed();
                    printer.SetAlignCenter();
                    printer.SetEmphasizedOn();
                    printer.SetFontSize(1);

                    printer.Print("盘点单凭证");
                    printer.PrintLineFeed();
                    printer.SetEmphasizedOff();
                    printer.PrintLineFeed();

                    printer.SetAlignCenter();
                    printer.SetFontSize(0);
                    printer.Print($"--**{Settings.StoreName}**--");
                    printer.SetEmphasizedOff();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("订单编号：" + $"{ipt.BillNumber}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("单据类型：" + $"盘点单");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("客    户：" + $"{ipt.TerminalName}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("操作时间：" + $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("打印时间：" + $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    printer.PrintLineFeed();


                    printer.PrintLine();
                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.PrintInOneLine("商品", "盘盈", "盘亏", 0);
                    printer.PrintLineFeed();
                    foreach (var item in ipt.Items)
                    {
                        printer.PrintInOneLine(item.ProductName, "X" + item.VolumeQuantity, "X" + $"{item.LossesQuantity}", 0);
                        printer.PrintLineFeed();
                    }
                    printer.PrintLine();
                    printer.PrintLineFeed();

                    printer.PrintLine();
                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.Print("业 务 员：" + $"{ipt.BusinessUserName}");
                    printer.PrintLineFeed();
                    printer.PrintLine();

                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.Print(remark);
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.FeedPaperCutPartial();

                    data.Add(printer.GetDataAndClose());

                    #endregion
                }
                else if (bill is CostExpenditureBillModel ceb)
                {
                    #region

                    //经销商名称
                    printer.PrintLineFeed();
                    printer.SetAlignCenter();
                    printer.SetEmphasizedOn();
                    printer.SetFontSize(1);

                    printer.Print("费用支出凭证");
                    printer.PrintLineFeed();
                    printer.SetEmphasizedOff();
                    printer.PrintLineFeed();

                    printer.SetAlignCenter();
                    printer.SetFontSize(0);
                    printer.Print($"--**{Settings.StoreName}**--");
                    printer.SetEmphasizedOff();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("订单编号：" + $"{ceb.BillNumber}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("单据类型：" + $"费用支出");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("客    户：" + $"{ceb.TerminalName}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("操作时间：" + $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("打印时间：" + $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    printer.PrintLineFeed();


                    printer.PrintLine();
                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.PrintInOneLine("科目", "客户", "金额", 0);
                    printer.PrintLineFeed();
                    foreach (var item in ceb.Items)
                    {
                        printer.PrintInOneLine(item.AccountingOptionName, $"{item.CustomerName}", "￥" + item.Amount, 0);
                        printer.PrintLineFeed();
                    }
                    printer.PrintLine();
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("合计：" + $"￥{ceb.SumAmount}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("优惠：" + $"￥{ceb.PreferentialAmount}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("欠款：" + $"￥{ceb.OweCash}");
                    printer.PrintLineFeed();

                    printer.PrintLine();
                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.Print("业 务 员：" + $"{ceb.BusinessUserName}");
                    printer.PrintLineFeed();
                    printer.PrintLine();

                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.Print(remark);
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.FeedPaperCutPartial();

                    data.Add(printer.GetDataAndClose());


                    #endregion
                }
                else if (bill is CostContractBillModel ccb)
                {
                    #region

                    //经销商名称
                    printer.PrintLineFeed();
                    printer.SetAlignCenter();
                    printer.SetEmphasizedOn();
                    printer.SetFontSize(1);

                    printer.Print("费用合同凭证");
                    printer.PrintLineFeed();
                    printer.SetEmphasizedOff();
                    printer.PrintLineFeed();

                    printer.SetAlignCenter();
                    printer.SetFontSize(0);
                    printer.Print($"--**{Settings.StoreName}**--");
                    printer.SetEmphasizedOff();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("订单编号：" + $"{ccb.BillNumber}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("单据类型：" + $"费用合同单");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("客    户：" + $"{ccb.TerminalName}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("操作时间：" + $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("打印时间：" + $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    printer.PrintLineFeed();


                    printer.PrintLine();
                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.PrintInOneLine("商品/现金", "数量", "金额", 0);
                    printer.PrintLineFeed();
                    foreach (var item in ccb.Items)
                    {
                        printer.PrintInOneLine(item.ProductName, "X" + item.TotalQuantity, $"￥{item.TotalQuantity}", 0);
                        printer.PrintLineFeed();
                    }
                    printer.PrintLine();
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("合计：" + $"￥{ccb.SumAmount}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("优惠：" + $"￥{ccb.PreferentialAmount}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("欠款：" + $"￥{ccb.OweCash}");
                    printer.PrintLineFeed();

                    printer.PrintLine();
                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.Print("业 务 员：" + $"{ccb.BusinessUserName}");
                    printer.PrintLineFeed();
                    printer.PrintLine();

                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.Print(remark);
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.FeedPaperCutPartial();

                    data.Add(printer.GetDataAndClose());


                    #endregion
                }
                else if (bill is AdvanceReceiptBillModel arb)
                {
                    #region

                    arb.Items.Add(new AccountMaping()
                    {
                        AccountingOptionName = arb.AccountingOptionName,
                        CollectionAmount = arb.AdvanceAmount ?? 0
                    });

                    //经销商名称
                    printer.PrintLineFeed();
                    printer.SetAlignCenter();
                    printer.SetEmphasizedOn();
                    printer.SetFontSize(1);
                    printer.Print("预收款凭证");
                    printer.PrintLineFeed();
                    printer.SetEmphasizedOff();
                    printer.PrintLineFeed();

                    printer.SetAlignCenter();
                    printer.SetFontSize(0);
                    printer.Print($"--**{Settings.StoreName}**--");
                    printer.SetEmphasizedOff();
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("订单编号：" + $"{arb.BillNumber}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("单据类型：" + $"预收款单");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("客    户：" + $"{arb.TerminalName}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("操作时间：" + $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("打印时间：" + $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    printer.PrintLineFeed();


                    printer.PrintLine();
                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.PrintInOneLine("科目", "金额", "备注", 0);
                    printer.PrintLineFeed();
                    foreach (var item in arb.Items)
                    {
                        printer.PrintInOneLine(item.AccountingOptionName, "￥" + item.CollectionAmount, $"", 0);
                        printer.PrintLineFeed();
                    }
                    printer.PrintLine();
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("合计：" + $"￥{arb.SumAmount}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("优惠：" + $"￥{arb.PreferentialAmount}");
                    printer.PrintLineFeed();

                    printer.SetAlignLeft();
                    printer.Print("欠款：" + $"￥{arb.OweCash}");
                    printer.PrintLineFeed();

                    printer.PrintLine();
                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.Print("业 务 员：" + $"{arb.BusinessUserName}");
                    printer.PrintLineFeed();
                    printer.PrintLine();

                    printer.PrintLineFeed();
                    printer.SetAlignLeft();
                    printer.Print(remark);
                    printer.PrintLineFeed();
                    printer.PrintLineFeed();
                    printer.FeedPaperCutPartial();

                    data.Add(printer.GetDataAndClose());


                    #endregion
                }

                return data;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}