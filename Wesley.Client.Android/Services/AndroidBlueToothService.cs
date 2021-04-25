using Acr.UserDialogs;
using Android;
using Android.Annotation;
using Android.Bluetooth;
using Android.OS;
using AndroidX.Core.Content;
using Wesley.Client.Droid.Services;
using Wesley.Client.Models;
using Wesley.Client.Models.Finances;
using Wesley.Client.Models.Global;
using Wesley.Client.Models.Purchases;
using Wesley.Client.Models.Sales;
using Wesley.Client.Models.WareHouses;
using Wesley.Client.Services;
using Google.Android.Material.Snackbar;
using Java.Util;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidBlueToothService))]
namespace Wesley.Client.Droid.Services
{
    public class AndroidBlueToothService : IBlueToothService
    {
        private CancellationTokenSource _cancellationToken { get; set; }

        public AbstractBill Data { get; set; } = null;

        #region 蓝牙

        public void Connect(string name)
        {
            Task.Run(async () => await ConnectDevice(name));
        }

        /// <summary>
        /// 连接到设备
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private async Task ConnectDevice(string name)
        {
            BluetoothDevice device = null;
            BluetoothSocket bthSocket = null;

            _cancellationToken = new CancellationTokenSource();


            while (_cancellationToken.IsCancellationRequested == false)
            {
                BluetoothAdapter adapter;
                try
                {
                    Thread.Sleep(250);

                    adapter = BluetoothAdapter.DefaultAdapter;

                    if (adapter == null)
                        System.Diagnostics.Debug.Write("No bluetooth adapter found!");
                    else
                        System.Diagnostics.Debug.Write("Adapter found!");

                    if (!adapter.IsEnabled)
                        System.Diagnostics.Debug.Write("Bluetooth adapter is not enabled.");
                    else
                        System.Diagnostics.Debug.Write("Adapter found!");

                    System.Diagnostics.Debug.Write("Try to connect to " + name);

                    foreach (var bondedDevice in adapter.BondedDevices)
                    {
                        //Debug.Write("Paired devices found: " + bondedDevice.Name.ToUpper());
                        if (bondedDevice.Name.ToUpper().IndexOf(name.ToUpper()) >= 0)
                        {
                            device = bondedDevice;
                            break;
                        }
                    }

                    if (device == null)
                        await UserDialogs.Instance.AlertAsync("Named device not found.");
                    else
                    {
                        bthSocket = device?.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
                        //bthSocket = bthServerSocket.Accept();

                        adapter.CancelDiscovery();

                        if (bthSocket != null)
                        {
                            bthSocket?.Connect();

                            if (bthSocket.IsConnected)
                            {
                                //while (_cancellationToken.IsCancellationRequested == false)
                                //{
                                //    if (MessageToSend != null)
                                //    {
                                //        //byte[] temp = Encoding.UTF8.GetBytes(MessageToSend);
                                //        //byte[] buffer = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(936), temp);
                                //        //await bthSocket.OutputStream.WriteAsync(buffer, 0, buffer.Length);

                                //        var outputStreamr = bthSocket.OutputStream;
                                //        PrintUtils.SetOutputStream(outputStreamr);
                                //        Print();

                                //        MessageToSend = null;
                                //    }
                                //}


                                var outputStreamr = bthSocket.OutputStream;
                                PrintUtils.SetOutputStream(outputStreamr);

                                while (_cancellationToken.IsCancellationRequested == false)
                                {
                                    if (Data != null)
                                    {
                                        ParsingPrint(Data);
                                        Data = null;
                                        _cancellationToken.Cancel();
                                    }
                                }
                            }
                        }
                        else
                        {
                            await UserDialogs.Instance.AlertAsync("适配不到设备，重新尝试");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _cancellationToken.Cancel();
                    await UserDialogs.Instance.AlertAsync(ex.Message, okText: "取消", cancelToken: _cancellationToken.Token);
                }
                finally
                {
                    if (bthSocket != null)
                        bthSocket.Close();

                    device = null;
                }
            }
        }

        /// <summary>
        /// 打印示例
        /// </summary>
        public void PrintSample()
        {
            PrintUtils.SelectCommand(PrintUtils.RESET);
            PrintUtils.SelectCommand(PrintUtils.LINE_SPACING_DEFAULT);
            PrintUtils.SelectCommand(PrintUtils.ALIGN_CENTER);
            PrintUtils.PrintText("陕西玖鉴商贸有限公司\n\n");
            PrintUtils.SelectCommand(PrintUtils.DOUBLE_HEIGHT_WIDTH);
            PrintUtils.PrintText("退货凭证（台账）\n\n");
            PrintUtils.SelectCommand(PrintUtils.NORMAL);
            PrintUtils.SelectCommand(PrintUtils.ALIGN_LEFT);
            PrintUtils.PrintText(PrintUtils.PrintTwoData("订单编号", "XS7970000159189439220\n"));
            PrintUtils.PrintText(PrintUtils.PrintTwoData("单据类型", "销售单订单\n"));
            //PrintUtils.PrintText(PrintUtils.PrintTwoData("客户地址", "陕西省西安市雁塔区xx路\n"));
            PrintUtils.PrintText(PrintUtils.PrintTwoData("操作时间", $"{DateTime.Now:yyyy - MM - dd HH: mm:ss}\n"));
            PrintUtils.PrintText(PrintUtils.PrintTwoData("打印时间", $"{DateTime.Now:yyyy - MM - dd HH: mm:ss}\n"));

            PrintUtils.PrintText("------------------------------------------------\n");
            PrintUtils.SelectCommand(PrintUtils.BOLD);
            PrintUtils.PrintText(PrintUtils.PrintThreeData("商品", "数量", "金额\n"));
            PrintUtils.PrintText("------------------------------------------------\n");
            PrintUtils.SelectCommand(PrintUtils.BOLD_CANCEL);
            PrintUtils.PrintText(PrintUtils.PrintThreeData("雪花超醇9度580ml晋陕专用绿瓶1*9塑封含瓶347白膜", "1", "0.00\n"));
            //PrintUtils.PrintText(PrintUtils.PrintThreeData("雪花超醇9度580ml晋陕专用绿瓶1*9塑封含瓶347白膜", "1", "6.00\n"));
            //PrintUtils.PrintText(PrintUtils.PrintThreeData("雪花超醇9度580ml晋陕专用绿瓶1*9塑封含瓶347白膜", "1", "26.00\n"));
            //PrintUtils.PrintText(PrintUtils.PrintThreeData("雪花超醇9度580ml晋陕专用绿瓶1*9塑封含瓶347白膜", "1", "226.00\n"));
            //PrintUtils.PrintText(PrintUtils.PrintThreeData("雪花超醇9度580ml晋陕专用绿瓶1*9塑封含瓶347白膜", "1", "2226.00\n"));
            //PrintUtils.PrintText(PrintUtils.PrintThreeData("雪花超醇9度580ml晋陕专用绿瓶1*9塑封含瓶347白膜", "1", "0.00\n"));
            //PrintUtils.PrintText(PrintUtils.PrintThreeData("雪花超醇9度580ml晋陕专用绿瓶1*9塑封含瓶347白膜", "1", "6.00\n"));
            //PrintUtils.PrintText(PrintUtils.PrintThreeData("雪花超醇9度580ml晋陕专用绿瓶1*9塑封含瓶347白膜", "1", "26.00\n"));
            //PrintUtils.PrintText(PrintUtils.PrintThreeData("雪花超醇9度580ml晋陕专用绿瓶1*9塑封含瓶347白膜", "1", "226.00\n"));
            //PrintUtils.PrintText(PrintUtils.PrintThreeData("雪花超醇9度580ml晋陕专用绿瓶1*9塑封含瓶347白膜", "1", "2226.00\n"));
            //PrintUtils.PrintText(PrintUtils.PrintThreeData("雪花超醇9度580ml晋陕专用绿瓶1*9塑封含瓶347白膜", "888", "98886.00\n"));

            PrintUtils.PrintText("------------------------------------------------\n");
            PrintUtils.PrintText(PrintUtils.PrintTwoData("合计", "￥0.00\n"));
            PrintUtils.PrintText(PrintUtils.PrintTwoData("优惠", "￥3.50\n"));
            PrintUtils.PrintText("------------------------------------------------\n");
            PrintUtils.PrintText(PrintUtils.PrintTwoData("欠款", "￥50.00\n"));
            PrintUtils.PrintText("------------------------------------------------\n");
            PrintUtils.PrintText(PrintUtils.PrintTwoData("业务员", "小陈\n"));
            PrintUtils.PrintText("------------------------------------------------\n");

            PrintUtils.SelectCommand(PrintUtils.ALIGN_LEFT);
            PrintUtils.PrintText("备注：本店出售商品质量合格，如果有误请在三日内联系，过期不负责任。");
            PrintUtils.PrintText("\n\n\n\n\n");
        }


        /// <summary>
        /// 解析打印单据
        /// </summary>
        /// <param name="bill"></param>
        /// <returns></returns>
        public void ParsingPrint(AbstractBill bill)
        {
            if (bill is AllocationBillModel ab)
            {
                #region

                PrintUtils.SelectCommand(PrintUtils.RESET);
                PrintUtils.SelectCommand(PrintUtils.LINE_SPACING_DEFAULT);
                PrintUtils.SelectCommand(PrintUtils.ALIGN_CENTER);
                PrintUtils.PrintText($"{Settings.StoreName}\n\n");
                PrintUtils.SelectCommand(PrintUtils.DOUBLE_HEIGHT_WIDTH);
                PrintUtils.PrintText("调拨凭证\n\n");
                PrintUtils.SelectCommand(PrintUtils.NORMAL);
                PrintUtils.SelectCommand(PrintUtils.ALIGN_LEFT);
                PrintUtils.PrintText(PrintUtils.PrintTwoData("订单编号", $"{ab.BillNumber}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("单据类型", "调拨单\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("客    户", $"{ab.TerminalName}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("操作时间", $"{DateTime.Now:yyyy - MM - dd HH: mm:ss}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("打印时间", $"{DateTime.Now:yyyy - MM - dd HH: mm:ss}\n"));

                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.SelectCommand(PrintUtils.BOLD);
                PrintUtils.PrintText(PrintUtils.PrintThreeData("商品", "数量", "金额\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.SelectCommand(PrintUtils.BOLD_CANCEL);

                foreach (var item in ab.Items)
                {
                    PrintUtils.PrintText(PrintUtils.PrintThreeData(item.ProductName, $"{item.Quantity}", $"{item.Subtotal:#.00}\n"));
                }

                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("出货仓库", $"{ab.ShipmentWareHouseName}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("入货仓库", $"{ab.IncomeWareHouseName}\n"));
                //PrintUtils.PrintText("------------------------------------------------\n");
                //PrintUtils.PrintText(PrintUtils.PrintTwoData("欠款", $"￥{ab.OweCash}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("业务员", $"{ab.BusinessUserName}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");

                PrintUtils.SelectCommand(PrintUtils.ALIGN_LEFT);
                PrintUtils.PrintText("备注：本店出售商品质量合格，如果有误请在三日内联系，过期不负责任。");
                PrintUtils.PrintText("\n\n\n\n\n");

                #endregion
            }
            else if (bill is SaleReservationBillModel srb)
            {
                #region

                PrintUtils.SelectCommand(PrintUtils.RESET);
                PrintUtils.SelectCommand(PrintUtils.LINE_SPACING_DEFAULT);
                PrintUtils.SelectCommand(PrintUtils.ALIGN_CENTER);
                PrintUtils.PrintText($"{Settings.StoreName}\n\n");
                PrintUtils.SelectCommand(PrintUtils.DOUBLE_HEIGHT_WIDTH);
                PrintUtils.PrintText("销售订单凭证\n\n");
                PrintUtils.SelectCommand(PrintUtils.NORMAL);
                PrintUtils.SelectCommand(PrintUtils.ALIGN_LEFT);
                PrintUtils.PrintText(PrintUtils.PrintTwoData("订单编号", $"{srb.BillNumber}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("单据类型", "销售订单\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("客    户", $"{srb.TerminalName}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("操作时间", $"{DateTime.Now:yyyy - MM - dd HH: mm:ss}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("打印时间", $"{DateTime.Now:yyyy - MM - dd HH: mm:ss}\n"));

                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.SelectCommand(PrintUtils.BOLD);
                PrintUtils.PrintText(PrintUtils.PrintThreeData("商品", "数量", "金额\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.SelectCommand(PrintUtils.BOLD_CANCEL);

                foreach (var item in srb.Items)
                {
                    PrintUtils.PrintText(PrintUtils.PrintThreeData(item.ProductName, $"{item.Quantity}", $"{item.Subtotal:#.00}\n"));
                }

                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("合计", $"￥{srb.SumAmount}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("优惠", $"￥{srb.PreferentialAmount}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("欠款", $"￥{srb.OweCash}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("业务员", $"{srb.BusinessUserName}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");

                PrintUtils.SelectCommand(PrintUtils.ALIGN_LEFT);
                PrintUtils.PrintText("备注：本店出售商品质量合格，如果有误请在三日内联系，过期不负责任。");
                PrintUtils.PrintText("\n\n\n\n\n");

                #endregion
            }
            else if (bill is SaleBillModel sb)
            {
                #region

                PrintUtils.SelectCommand(PrintUtils.RESET);
                PrintUtils.SelectCommand(PrintUtils.LINE_SPACING_DEFAULT);
                PrintUtils.SelectCommand(PrintUtils.ALIGN_CENTER);
                PrintUtils.PrintText($"{Settings.StoreName}\n\n");
                PrintUtils.SelectCommand(PrintUtils.DOUBLE_HEIGHT_WIDTH);
                PrintUtils.PrintText("销售凭证\n\n");
                PrintUtils.SelectCommand(PrintUtils.NORMAL);
                PrintUtils.SelectCommand(PrintUtils.ALIGN_LEFT);
                PrintUtils.PrintText(PrintUtils.PrintTwoData("订单编号", $"{sb.BillNumber}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("单据类型", "销售单\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("客    户", $"{sb.TerminalName}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("操作时间", $"{DateTime.Now:yyyy - MM - dd HH: mm:ss}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("打印时间", $"{DateTime.Now:yyyy - MM - dd HH: mm:ss}\n"));

                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.SelectCommand(PrintUtils.BOLD);
                PrintUtils.PrintText(PrintUtils.PrintThreeData("商品", "数量", "金额\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.SelectCommand(PrintUtils.BOLD_CANCEL);

                foreach (var item in sb.Items)
                {
                    PrintUtils.PrintText(PrintUtils.PrintThreeData(item.ProductName, $"{item.Quantity}", $"{item.Subtotal:#.00}\n"));
                }

                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("合计", $"￥{sb.SumAmount}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("优惠", $"￥{sb.PreferentialAmount}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("欠款", $"￥{sb.OweCash}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("业务员", $"{sb.BusinessUserName}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");

                PrintUtils.SelectCommand(PrintUtils.ALIGN_LEFT);
                PrintUtils.PrintText("备注：本店出售商品质量合格，如果有误请在三日内联系，过期不负责任。");
                PrintUtils.PrintText("\n\n\n\n\n");

                #endregion
            }
            else if (bill is ReturnReservationBillModel rrb)
            {
                #region

                PrintUtils.SelectCommand(PrintUtils.RESET);
                PrintUtils.SelectCommand(PrintUtils.LINE_SPACING_DEFAULT);
                PrintUtils.SelectCommand(PrintUtils.ALIGN_CENTER);
                PrintUtils.PrintText($"{Settings.StoreName}\n\n");
                PrintUtils.SelectCommand(PrintUtils.DOUBLE_HEIGHT_WIDTH);
                PrintUtils.PrintText("退货订单凭证\n\n");
                PrintUtils.SelectCommand(PrintUtils.NORMAL);
                PrintUtils.SelectCommand(PrintUtils.ALIGN_LEFT);
                PrintUtils.PrintText(PrintUtils.PrintTwoData("订单编号", $"{rrb.BillNumber}\n")); //打印两列
                PrintUtils.PrintText(PrintUtils.PrintTwoData("单据类型", "退货订单\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("客    户", $"{rrb.TerminalName}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("操作时间", $"{DateTime.Now:yyyy - MM - dd HH: mm:ss}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("打印时间", $"{DateTime.Now:yyyy - MM - dd HH: mm:ss}\n"));

                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.SelectCommand(PrintUtils.BOLD);
                PrintUtils.PrintText(PrintUtils.PrintThreeData("商品", "数量", "金额\n")); //打印三列
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.SelectCommand(PrintUtils.BOLD_CANCEL);

                foreach (var item in rrb.Items)
                {
                    PrintUtils.PrintText(PrintUtils.PrintThreeData(item.ProductName, $"{item.Quantity}", $"{item.Subtotal:#.00}\n"));
                }

                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("合计", $"￥{rrb.SumAmount}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("优惠", $"￥{rrb.PreferentialAmount}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("欠款", $"￥{rrb.OweCash}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("业务员", $"{rrb.BusinessUserName}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");

                PrintUtils.SelectCommand(PrintUtils.ALIGN_LEFT);
                PrintUtils.PrintText("备注：本店出售商品质量合格，如果有误请在三日内联系，过期不负责任。");
                PrintUtils.PrintText("\n\n\n\n\n");

                #endregion
            }
            else if (bill is ReturnBillModel rb)
            {
                #region

                PrintUtils.SelectCommand(PrintUtils.RESET);
                PrintUtils.SelectCommand(PrintUtils.LINE_SPACING_DEFAULT);
                PrintUtils.SelectCommand(PrintUtils.ALIGN_CENTER);
                PrintUtils.PrintText($"{Settings.StoreName}\n\n");
                PrintUtils.SelectCommand(PrintUtils.DOUBLE_HEIGHT_WIDTH);
                PrintUtils.PrintText("退货凭证\n\n");
                PrintUtils.SelectCommand(PrintUtils.NORMAL);
                PrintUtils.SelectCommand(PrintUtils.ALIGN_LEFT);
                PrintUtils.PrintText(PrintUtils.PrintTwoData("订单编号", $"{rb.BillNumber}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("单据类型", "退货单\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("客    户", $"{rb.TerminalName}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("操作时间", $"{DateTime.Now:yyyy - MM - dd HH: mm:ss}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("打印时间", $"{DateTime.Now:yyyy - MM - dd HH: mm:ss}\n"));

                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.SelectCommand(PrintUtils.BOLD);
                PrintUtils.PrintText(PrintUtils.PrintThreeData("商品", "数量", "金额\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.SelectCommand(PrintUtils.BOLD_CANCEL);

                foreach (var item in rb.Items)
                {
                    PrintUtils.PrintText(PrintUtils.PrintThreeData(item.ProductName, $"{item.Quantity}", $"{item.Subtotal:#.00}\n"));
                }

                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("合计", $"￥{rb.SumAmount}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("优惠", $"￥{rb.PreferentialAmount}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("欠款", $"￥{rb.OweCash}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("业务员", $"{rb.BusinessUserName}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");

                PrintUtils.SelectCommand(PrintUtils.ALIGN_LEFT);
                PrintUtils.PrintText("备注：本店出售商品质量合格，如果有误请在三日内联系，过期不负责任。");
                PrintUtils.PrintText("\n\n\n\n\n");

                #endregion
            }
            else if (bill is CashReceiptBillModel rcp)
            {
                #region
                rcp.SumAmount = rcp.Items.Sum(c => c.ReceivableAmountOnce)??0; //本次收款合计
                rcp.PreferentialAmount= rcp.Items.Sum(c => c.DiscountAmountOnce) ?? 0; //本次优惠合计

                PrintUtils.SelectCommand(PrintUtils.RESET);
                PrintUtils.SelectCommand(PrintUtils.LINE_SPACING_DEFAULT);
                PrintUtils.SelectCommand(PrintUtils.ALIGN_CENTER);
                PrintUtils.PrintText($"{Settings.StoreName}\n\n");
                PrintUtils.SelectCommand(PrintUtils.DOUBLE_HEIGHT_WIDTH);
                PrintUtils.PrintText("收款凭证\n\n");
                PrintUtils.SelectCommand(PrintUtils.NORMAL);
                PrintUtils.SelectCommand(PrintUtils.ALIGN_LEFT);
                PrintUtils.PrintText(PrintUtils.PrintTwoData("收款单编号", $"{rcp.BillNumber}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("单据类型", "收款单\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("客    户", $"{rcp.TerminalName}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("操作时间", $"{DateTime.Now:yyyy - MM - dd HH: mm:ss}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("打印时间", $"{DateTime.Now:yyyy - MM - dd HH: mm:ss}\n"));

                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.SelectCommand(PrintUtils.BOLD);
                PrintUtils.PrintText(PrintUtils.PrintThreeData("单据号", "本次优惠", "本次收款\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.SelectCommand(PrintUtils.BOLD_CANCEL);

                foreach (var item in rcp.Items)
                {
                    PrintUtils.PrintText(PrintUtils.PrintThreeData(item.BillNumber, $"{item.DiscountAmountOnce}", $"{item.ReceivableAmountOnce}\n"));
                }

                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("合计", $"￥{rcp.SumAmount}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("优惠", $"￥{rcp.PreferentialAmount}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("欠款", $"￥{rcp.OweCash}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("业务员", $"{rcp.BusinessUserName}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");

                PrintUtils.SelectCommand(PrintUtils.ALIGN_LEFT);
                PrintUtils.PrintText("备注：本店出售商品质量合格，如果有误请在三日内联系，过期不负责任。");
                PrintUtils.PrintText("\n\n\n\n\n");

                #endregion
            }
            else if (bill is PurchaseBillModel pb)
            {
                #region

                PrintUtils.SelectCommand(PrintUtils.RESET);
                PrintUtils.SelectCommand(PrintUtils.LINE_SPACING_DEFAULT);
                PrintUtils.SelectCommand(PrintUtils.ALIGN_CENTER);
                PrintUtils.PrintText($"{Settings.StoreName}\n\n");
                PrintUtils.SelectCommand(PrintUtils.DOUBLE_HEIGHT_WIDTH);
                PrintUtils.PrintText("采购凭证\n\n");
                PrintUtils.SelectCommand(PrintUtils.NORMAL);
                PrintUtils.SelectCommand(PrintUtils.ALIGN_LEFT);
                PrintUtils.PrintText(PrintUtils.PrintTwoData("订单编号", $"{pb.BillNumber}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("单据类型", "采购单\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("客    户", $"{pb.TerminalName}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("操作时间", $"{DateTime.Now:yyyy - MM - dd HH: mm:ss}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("打印时间", $"{DateTime.Now:yyyy - MM - dd HH: mm:ss}\n"));

                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.SelectCommand(PrintUtils.BOLD);
                PrintUtils.PrintText(PrintUtils.PrintThreeData("商品", "数量", "金额\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.SelectCommand(PrintUtils.BOLD_CANCEL);

                foreach (var item in pb.Items)
                {
                    PrintUtils.PrintText(PrintUtils.PrintThreeData(item.ProductName, $"{item.Quantity}", $"{item.Subtotal:#.00}\n"));
                }

                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("合计", $"￥{pb.SumAmount}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("优惠", $"￥{pb.PreferentialAmount}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("欠款", $"￥{pb.OweCash}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("业务员", $"{pb.BusinessUserName}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");

                PrintUtils.SelectCommand(PrintUtils.ALIGN_LEFT);
                PrintUtils.PrintText("备注：本店出售商品质量合格，如果有误请在三日内联系，过期不负责任。");
                PrintUtils.PrintText("\n\n\n\n\n");

                #endregion
            }
            else if (bill is InventoryPartTaskBillModel ipt)
            {
                #region

                PrintUtils.SelectCommand(PrintUtils.RESET);
                PrintUtils.SelectCommand(PrintUtils.LINE_SPACING_DEFAULT);
                PrintUtils.SelectCommand(PrintUtils.ALIGN_CENTER);
                PrintUtils.PrintText($"{Settings.StoreName}\n\n");
                PrintUtils.SelectCommand(PrintUtils.DOUBLE_HEIGHT_WIDTH);
                PrintUtils.PrintText("盘点凭证\n\n");
                PrintUtils.SelectCommand(PrintUtils.NORMAL);
                PrintUtils.SelectCommand(PrintUtils.ALIGN_LEFT);
                PrintUtils.PrintText(PrintUtils.PrintTwoData("盘点单编号", $"{ipt.BillNumber}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("单据类型", "盘点单\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("客    户", $"{ipt.TerminalName}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("操作时间", $"{DateTime.Now:yyyy - MM - dd HH: mm:ss}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("打印时间", $"{DateTime.Now:yyyy - MM - dd HH: mm:ss}\n"));

                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.SelectCommand(PrintUtils.BOLD);
                PrintUtils.PrintText(PrintUtils.PrintThreeData("商品", "盘盈", "盘亏\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.SelectCommand(PrintUtils.BOLD_CANCEL);

                foreach (var item in ipt.Items)
                {
                    PrintUtils.PrintText(PrintUtils.PrintThreeData(item.ProductName, $"{item.VolumeQuantity}", $"{item.LossesQuantity}\n"));
                }

                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("业务员", $"{ipt.BusinessUserName}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");

                PrintUtils.SelectCommand(PrintUtils.ALIGN_LEFT);
                PrintUtils.PrintText("备注：本店出售商品质量合格，如果有误请在三日内联系，过期不负责任。");
                PrintUtils.PrintText("\n\n\n\n\n");

                #endregion
            }
            else if (bill is CostExpenditureBillModel ceb)
            {
                #region

                PrintUtils.SelectCommand(PrintUtils.RESET);
                PrintUtils.SelectCommand(PrintUtils.LINE_SPACING_DEFAULT);
                PrintUtils.SelectCommand(PrintUtils.ALIGN_CENTER);
                PrintUtils.PrintText($"{Settings.StoreName}\n\n");
                PrintUtils.SelectCommand(PrintUtils.DOUBLE_HEIGHT_WIDTH);
                PrintUtils.PrintText("费用支出凭证\n\n");
                PrintUtils.SelectCommand(PrintUtils.NORMAL);
                PrintUtils.SelectCommand(PrintUtils.ALIGN_LEFT);
                PrintUtils.PrintText(PrintUtils.PrintTwoData("单据编号", $"{ceb.BillNumber}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("单据类型", "费用支出单\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("客    户", $"{ceb.TerminalName}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("操作时间", $"{DateTime.Now:yyyy - MM - dd HH: mm:ss}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("打印时间", $"{DateTime.Now:yyyy - MM - dd HH: mm:ss}\n"));

                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.SelectCommand(PrintUtils.BOLD);
                PrintUtils.PrintText(PrintUtils.PrintThreeData("科目", "客户", "金额\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.SelectCommand(PrintUtils.BOLD_CANCEL);

                foreach (var item in ceb.Items)
                {
                    PrintUtils.PrintText(PrintUtils.PrintThreeData(item.AccountingOptionName, $"{item.CustomerName}", $"{item.Amount}\n"));
                }

                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("合计", $"￥{ceb.SumAmount}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("优惠", $"￥{ceb.PreferentialAmount}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("欠款", $"￥{ceb.OweCash}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("业务员", $"{ceb.BusinessUserName}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");

                PrintUtils.SelectCommand(PrintUtils.ALIGN_LEFT);
                PrintUtils.PrintText("备注：本店出售商品质量合格，如果有误请在三日内联系，过期不负责任。");
                PrintUtils.PrintText("\n\n\n\n\n");

                #endregion
            }
            else if (bill is CostContractBillModel ccb)
            {
                #region

                PrintUtils.SelectCommand(PrintUtils.RESET);
                PrintUtils.SelectCommand(PrintUtils.LINE_SPACING_DEFAULT);
                PrintUtils.SelectCommand(PrintUtils.ALIGN_CENTER);
                PrintUtils.PrintText($"{Settings.StoreName}\n\n");
                PrintUtils.SelectCommand(PrintUtils.DOUBLE_HEIGHT_WIDTH);
                PrintUtils.PrintText("费用合同凭证\n\n");
                PrintUtils.SelectCommand(PrintUtils.NORMAL);
                PrintUtils.SelectCommand(PrintUtils.ALIGN_LEFT);
                PrintUtils.PrintText(PrintUtils.PrintTwoData("单据编号", $"{ccb.BillNumber}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("单据类型", "费用合同单\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("客    户", $"{ccb.TerminalName}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("操作时间", $"{DateTime.Now:yyyy - MM - dd HH: mm:ss}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("打印时间", $"{DateTime.Now:yyyy - MM - dd HH: mm:ss}\n"));

                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.SelectCommand(PrintUtils.BOLD);
                PrintUtils.PrintText(PrintUtils.PrintThreeData("商品", "数量", "金额\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.SelectCommand(PrintUtils.BOLD_CANCEL);

                foreach (var item in ccb.Items)
                {
                    PrintUtils.PrintText(PrintUtils.PrintThreeData(item.ProductName, $"{item.TotalQuantity}", $"{item.Total}\n"));
                }

                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("合计", $"￥{ccb.SumAmount}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("优惠", $"￥{ccb.PreferentialAmount}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("欠款", $"￥{ccb.OweCash}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("业务员", $"{ccb.BusinessUserName}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");

                PrintUtils.SelectCommand(PrintUtils.ALIGN_LEFT);
                PrintUtils.PrintText("备注：本店出售商品质量合格，如果有误请在三日内联系，过期不负责任。");
                PrintUtils.PrintText("\n\n\n\n\n");

                #endregion
            }
            else if (bill is AdvanceReceiptBillModel arb)
            {
                #region

                arb.Items.Add(new AccountMaping()
                {
                    AccountingOptionName= arb.AccountingOptionName,
                    CollectionAmount= arb.AdvanceAmount??0
                });

                PrintUtils.SelectCommand(PrintUtils.RESET);
                PrintUtils.SelectCommand(PrintUtils.LINE_SPACING_DEFAULT);
                PrintUtils.SelectCommand(PrintUtils.ALIGN_CENTER);
                PrintUtils.PrintText($"{Settings.StoreName}\n\n");
                PrintUtils.SelectCommand(PrintUtils.DOUBLE_HEIGHT_WIDTH);
                PrintUtils.PrintText("预收款凭证\n\n");
                PrintUtils.SelectCommand(PrintUtils.NORMAL);
                PrintUtils.SelectCommand(PrintUtils.ALIGN_LEFT);
                PrintUtils.PrintText(PrintUtils.PrintTwoData("单据编号", $"{arb.BillNumber}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("单据类型", "预收款单\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("客    户", $"{arb.TerminalName}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("操作时间", $"{DateTime.Now:yyyy - MM - dd HH: mm:ss}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("打印时间", $"{DateTime.Now:yyyy - MM - dd HH: mm:ss}\n"));

                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.SelectCommand(PrintUtils.BOLD);
                PrintUtils.PrintText(PrintUtils.PrintThreeData("科目", "金额", "备注\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.SelectCommand(PrintUtils.BOLD_CANCEL);

                foreach (var item in arb.Items)
                {
                    PrintUtils.PrintText(PrintUtils.PrintThreeData(item.AccountingOptionName, $"{item.CollectionAmount}", $"\n"));
                }

                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("合计", $"￥{arb.SumAmount}\n"));
                PrintUtils.PrintText(PrintUtils.PrintTwoData("优惠", $"￥{arb.PreferentialAmount}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("欠款", $"￥{arb.OweCash}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");
                PrintUtils.PrintText(PrintUtils.PrintTwoData("业务员", $"{arb.BusinessUserName}\n"));
                PrintUtils.PrintText("------------------------------------------------\n");

                PrintUtils.SelectCommand(PrintUtils.ALIGN_LEFT);
                PrintUtils.PrintText("备注：本店出售商品质量合格，如果有误请在三日内联系，过期不负责任。");
                PrintUtils.PrintText("\n\n\n\n\n");

                #endregion
            }
        }


        /// <summary>
        /// 断开
        /// </summary>
        public void Disconnect()
        {
            if (_cancellationToken != null)
            {
                _cancellationToken.Cancel();
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="printData"></param>
        public void SendData(AbstractBill printData)
        {
            if (Data == null)
                Data = printData;
        }

        /// <summary>
        /// 配对设备
        /// </summary>
        /// <returns></returns>
        public ObservableRangeCollection<Printer> PairedDevices()
        {
            using (var bluetoothAdapter = BluetoothAdapter.DefaultAdapter)
            {
                //判断蓝牙是否开启
                if (!bluetoothAdapter.IsEnabled) 
                    bluetoothAdapter.Enable();

                if (!bluetoothAdapter.IsDiscovering)
                    bluetoothAdapter.StartDiscovery();

                var devices = new ObservableRangeCollection<Printer>();
                var btdevices = bluetoothAdapter?.BondedDevices.ToList();
                if (btdevices != null)
                {
                    foreach (var bd in btdevices)
                    {
                        devices.Add(new Printer()
                        {
                            LocalName= bd.Name,
                            Address = bd.Address,
                            Selected = false,
                            Name = bd.Name
                        });
                    }
                }
                return devices;
            }
        }


        /// <summary>
        /// 开始搜索
        /// </summary>
        public void StartDiscovery()
        {
            using (BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter)
            {
                if (!bluetoothAdapter.IsDiscovering)
                    bluetoothAdapter.StartDiscovery();
            }
        }

        #endregion

        public const int REQUEST_BLUETOOTH = 1;
        private readonly string[] permissions = { Manifest.Permission.Bluetooth };
        private TaskCompletionSource<bool> tcsPermissions;
        public Task<bool> GetPermissionsAsync()
        {
            tcsPermissions = new TaskCompletionSource<bool>();
            if ((int)Build.VERSION.SdkInt < 23)
            {
                tcsPermissions.TrySetResult(true);
            }
            else
            {
                var currentActivity = MainActivity.Instance;

                if (ContextCompat.CheckSelfPermission(currentActivity, Manifest.Permission.Bluetooth) != (int)Android.Content.PM.Permission.Granted)
                {
                    RequestBluetoothPermission();
                }
                else
                {
                    tcsPermissions.TrySetResult(true);
                }
            }

            return tcsPermissions.Task;
        }


        private void RequestBluetoothPermission()
        {
            var currentActivity = MainActivity.Instance;
            if (currentActivity.ShouldShowRequestPermissionRationale(Manifest.Permission.Bluetooth))
            {
                Snackbar.Make(currentActivity.FindViewById(Android.Resource.Id.Content),
                    "App requires Bluetooth permission.",
                    Snackbar.LengthIndefinite).SetAction("Ok",
                    v =>
                    {
                        currentActivity.RequestPermissions(permissions, REQUEST_BLUETOOTH);
                    }).Show();
            }
            else
            {
                currentActivity.RequestPermissions(permissions, REQUEST_BLUETOOTH);
            }
        }

        public void OnRequestPermissionsResult(bool isGranted)
        {
            tcsPermissions.TrySetResult(isGranted);
        }
    }


    /// <summary>
    /// 用于票据打印辅助类
    /// </summary>
    public class PrintUtils
    {
        #region  打印

        private static System.IO.Stream _outputStream = null;

        /// <summary>
        /// 打印纸一行最大的字节
        /// </summary>
        private static readonly int LINE_BYTE_SIZE = 48;
        private static readonly int LEFT_LENGTH = 28;
        private static readonly int RIGHT_LENGTH = 20;

        /// <summary>
        /// 左侧汉字最多显示几个文字
        /// </summary>
        private static readonly int LEFT_TEXT_MAX_LENGTH = 16;

        /// <summary>
        /// 小票打印菜品的名称，上限调到8个字
        /// </summary>
        public static readonly int MEAL_NAME_MAX_LENGTH = 12;

        /// <summary>
        /// 复位打印机
        /// </summary>
        public static byte[] RESET = { 0x1b, 0x40 };

        /// <summary>
        /// 左对齐
        /// </summary>
        public static byte[] ALIGN_LEFT = { 0x1b, 0x61, 0x00 };

        /// <summary>
        /// 中间对齐
        /// </summary>
        public static byte[] ALIGN_CENTER = { 0x1b, 0x61, 0x01 };

        /// <summary>
        /// 右对齐
        /// </summary>
        public static byte[] ALIGN_RIGHT = { 0x1b, 0x61, 0x02 };

        /// <summary>
        /// 选择加粗模式
        /// </summary>
        public static byte[] BOLD = { 0x1b, 0x45, 0x01 };

        /// <summary>
        /// 取消加粗模式
        /// </summary>
        public static byte[] BOLD_CANCEL = { 0x1b, 0x45, 0x00 };

        /// <summary>
        /// 宽高加倍
        /// </summary>
        public static byte[] DOUBLE_HEIGHT_WIDTH = { 0x1d, 0x21, 0x11 };

        /// <summary>
        /// 宽加倍
        /// </summary>
        public static byte[] DOUBLE_WIDTH = { 0x1d, 0x21, 0x10 };

        /// <summary>
        /// 高加倍
        /// </summary>
        public static byte[] DOUBLE_HEIGHT = { 0x1d, 0x21, 0x01 };

        /// <summary>
        /// 字体不放大
        /// </summary>
        public static byte[] NORMAL = { 0x1d, 0x21, 0x00 };

        /// <summary>
        /// 设置默认行间距
        /// </summary>
        public static byte[] LINE_SPACING_DEFAULT = { 0x1b, 0x32 };

        /// <summary>
        /// 获取输出流
        /// </summary>
        /// <returns></returns>
        public static Stream GetOutputStream()
        {
            return _outputStream;
        }

        /// <summary>
        /// 设置输出流
        /// </summary>
        /// <param name="outputStream"></param>
        public static void SetOutputStream(System.IO.Stream outputStream)
        {
            _outputStream = outputStream;
        }

        /// <summary>
        /// 打印文本
        /// </summary>
        /// <param name="text"></param>
        public static void PrintText(string text)
        {
            try
            {
                byte[] data = Encoding.GetEncoding(936).GetBytes(text);
                _outputStream.Write(data, 0, data.Length);
                _outputStream.Flush();
            }
            catch (Java.Lang.Exception e)
            {
                e.PrintStackTrace();
            }
        }

        /// <summary>
        /// 设置打印格式
        /// </summary>
        /// <param name="command"></param>
        public static void SelectCommand(byte[] command)
        {
            try
            {
                _outputStream.Write(command);
                _outputStream.Flush();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 打印两列
        /// </summary>
        /// <param name="leftText"></param>
        /// <param name="rightText"></param>
        /// <returns></returns>
        [SuppressLint()]
        public static String PrintTwoData(String leftText, String rightText)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                int leftTextLength = GetBytesLength(leftText);
                int rightTextLength = GetBytesLength(rightText);
                sb.Append(leftText);
                // 计算两侧文字中间的空格
                int marginBetweenMiddleAndRight = LINE_BYTE_SIZE - leftTextLength - rightTextLength;
                for (int i = 0; i < marginBetweenMiddleAndRight; i++)
                {
                    sb.Append(" ");
                }
                sb.Append(rightText);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return "";
            }
        }

        /// <summary>
        /// 打印三列
        /// </summary>
        /// <param name="leftText"></param>
        /// <param name="middleText"></param>
        /// <param name="rightText"></param>
        /// <returns></returns>
        public static String PrintThreeData(String leftText, String middleText, String rightText)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                // 左边最多显示 LEFT_TEXT_MAX_LENGTH 个汉字 + 两个点
                if (leftText.Length > LEFT_TEXT_MAX_LENGTH)
                {
                    leftText = leftText.Substring(0, LEFT_TEXT_MAX_LENGTH) + "..";
                }
                int leftTextLength = GetBytesLength(leftText);
                int middleTextLength = GetBytesLength(middleText);
                int rightTextLength = GetBytesLength(rightText);

                sb.Append(leftText);
                // 计算左侧文字和中间文字的空格长度
                int marginBetweenLeftAndMiddle = LEFT_LENGTH - leftTextLength - middleTextLength / 2;

                for (int i = 0; i < marginBetweenLeftAndMiddle; i++)
                {
                    sb.Append(" ");
                }
                sb.Append(middleText);

                // 计算右侧文字和中间文字的空格长度
                int marginBetweenMiddleAndRight = RIGHT_LENGTH - middleTextLength / 2 - rightTextLength;

                for (int i = 0; i < marginBetweenMiddleAndRight; i++)
                {
                    sb.Append(" ");
                }

                // 打印的时候发现，最右边的文字总是偏右一个字符，所以需要删除一个空格
                sb.Remove(sb.Length - 1, 1).Append(rightText);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return "";
            }
        }


        /// <summary>
        /// 获取数据长度
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [SuppressLint()]
        private static int GetBytesLength(string msg)
        {
            return Encoding.GetEncoding(936).GetBytes(msg).Length;
        }

        /// <summary>
        /// 格式化菜品名称，最多显示MEAL_NAME_MAX_LENGTH个数
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string FormatMealName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }
            if (name.Length > MEAL_NAME_MAX_LENGTH)
            {
                return name.Substring(0, 8) + "..";
            }
            return name;
        }

        #endregion
    }
}