using Wesley.Client.Enums;
using Wesley.Client.Models;
using Wesley.Client.Models.Terminals;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Wesley.Client
{
    /// <summary>
    /// 定义全局配置设置
    /// </summary>
    public static class GlobalSettings
    {
        public static bool IsNotConnected { get; set; }
        public static bool IsSleeping { get; set; }

        public const string BaseEndpoint = "";
        public const string StorageEndpoint = "";
        public const string FileCenterEndpoint = "";


        public const string PushServerEndpoint = "";
        public static string GetNavigation(BillTypeEnum type)
        {
            var app = AppDatas.Where(a => a.BillType == type).FirstOrDefault();
            return app != null ? app.Navigation : "";
        }

        /// <summary>
        /// 报表功能
        /// </summary>
        public static List<Module> ReportsDatas
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(Settings.ReportsDatas))
                        return JsonConvert.DeserializeObject<List<Module>>(Settings.ReportsDatas);
                    else
                        return new List<Module>();
                }
                catch (Exception)
                {
                    return new List<Module>();
                }
            }
        }

        /// <summary>
        /// 应用功能
        /// </summary>
        public static List<Module> AppDatas
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(Settings.AppDatas))
                    {
                        var apps = JsonConvert.DeserializeObject<List<Module>>(Settings.AppDatas);
                        return apps.ToList();
                    }
                    else
                        return new List<Module>();
                }
                catch (Exception)
                {
                    return new List<Module>();
                }
            }
        }


        /// <summary>
        /// 订阅频道
        /// </summary>
        public static List<MessageInfo> SubscribeDatas
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(Settings.SubscribeDatas))
                        return JsonConvert.DeserializeObject<List<MessageInfo>>(Settings.SubscribeDatas);
                    else
                        return new List<MessageInfo>();
                }
                catch (Exception)
                {
                    return new List<MessageInfo>();
                }
            }
        }

        /// <summary>
        /// 工具栏菜单
        /// </summary>
        public static List<SubMenu> ToolBarMenus => new List<SubMenu>()
        {
            new SubMenu { Id = 0, Text = "\uf10b 支付方式"},
            new SubMenu { Id = 1, Text = "\uf0d6 欠款"},
            new SubMenu { Id = 2, Text = "\uf195 优惠"},
            new SubMenu { Id = 3, Text = "\uf044 整单备注"},
            new SubMenu { Id = 4, Text = "\uf1f8 清空单据"},
            new SubMenu { Id = 5, Text = "\uf02f 打印"},
            new SubMenu { Id = 6, Text = "\uf1da 历史单据"},
            new SubMenu { Id = 7, Text = "\uf140 默认售价"},
            new SubMenu { Id = 8, Text = "\uf017 今日" },
            new SubMenu { Id = 9, Text = "\uf272 昨日" },
            new SubMenu { Id = 10, Text = "\uf017 本月" },
            new SubMenu { Id = 11, Text = "\uf017 本年" },
            new SubMenu { Id = 12, Text = "\uf017 星期" },
            new SubMenu { Id = 13, Text = "\uf017 本周" },
            new SubMenu { Id = 14, Text = "\uf017 其它" },
            new SubMenu { Id = 15, Text = "\uf06e 显示零库存" },
            new SubMenu { Id = 16, Text = "\uf070 显示停用" },
            new SubMenu { Id = 17, Text = "\uf029 扫一扫" },
            new SubMenu { Id = 18, Text = "\uf279 附近客户" },
            new SubMenu { Id = 19, Text = "\uf041 线路选择" },
            new SubMenu { Id = 20, Text = "\uf07c 销售备注" },
            new SubMenu { Id = 21, Text = "\uf1f8 清空历史" },
            new SubMenu { Id = 22, Text = "\uf160 按销补货" },
            new SubMenu { Id = 23, Text = "\uf161 按退调拨" },
            new SubMenu { Id = 24, Text = "\uf15e 按库调拨" },
            new SubMenu { Id = 25, Text = "\uf1de 全部盘点" },
            new SubMenu { Id = 26, Text = "\uf0ed 未盘点" },
            new SubMenu { Id = 27, Text = "\uf0ee 已盘点" },
            new SubMenu { Id = 28, Text = "\uf233 全部" },
            new SubMenu { Id = 29, Text = "\uf064 上月" },
            new SubMenu { Id = 30, Text = "\uf1da 30天未交" },
            new SubMenu { Id = 31, Text = "\uf158 结算方式"},
            new SubMenu { Id = 32, Text = "\uf0c9 拜访记录"},//
            new SubMenu { Id = 33, Text = "\uf2d3 拒签"},
            new SubMenu { Id = 34, Text = "\uf14a 审核"},
            new SubMenu { Id = 35, Text = "\uf0e2 冲改"},
            new SubMenu { Id = 36, Text = "\uf0e2 红冲"},
            new SubMenu { Id = 37, Text = "\uf0d1 配送"},
        };

        #region Map

        public static double? Latitude { get; set; } = 34.35564;
        public static double? Longitude { get; set; } = 108.954696;
        public static List<PoiOInfo> PoiOInfos => new List<PoiOInfo>();

        public static void UpdatePoi(double? lat, double? lon)
        {
            if (lat.HasValue && lat > 0) Latitude = lat;
            if (lon.HasValue && lon > 0) Longitude = lon;
        }

        #endregion
        public static string AgreementText => "<b>1.服务终止</b><br/>" +
            " 1)您同意,在本公司未向您收费的情况下,本公司可自行全权决定以任何理由终止服务。<br/>" +
            " 2)您同意,在本公司向您收费的情况下,服务到期后,服务自动终<br/>" +
            " 3)您同意,无论本公司是否向您收费,只要您有违反本协议条款的行为,或者是本公司基于合理的理由怀疑您违反本协议的行为。<br/>" +
            " 本公司可自行全权决定,在发出通知或不发出通知的情况下,随时停止提供\"服务\"或其任何部份。<br/>" +
            " 4)您同意,服务终止后,本公司没有义务为您原账号中或与之相关的任何信息提供保留或者导出服务。<br/>" +
            " 5)您同意,本公司不会就终止对您的服务而对您或任何第三者承担任何责任。<br/>" +

            " <b>2.服务付费、续费、服务变更</b><br/>" +
            " 1)在服务期内,您可以增加授权账号,但是不能减少授权账号。如需添加授权账号,添加的授权账号截至日期与已有的账号组的授权使用截至日期保持一致。" +
            "具体换算,请查看本公司官方网站。在 " +
            "重新签订新的服务期的时候,可以根据需要增减授权账号。<br/>" +
            " 2)服务到期后,如果未续费,则视为服务终止。本公司没有义务替您保留账号及与之相关的数据,如果您超过90日没有支付相关费用,这些数据有可能被永久删除,本公司对此不承担任何责任。如果您延期续费,则需补全拖欠费用期间的与之相对应的服务费用。" +

            "<b>3.责任免除</b>。<br/>" +
            " 本公司对下列事项不作任何陈述与保证。<br/>" +
            "1)\"服务\"与任何其它硬件、软件、系统或数据结合时的,安全性、及时性、不受干扰或不发生错误; 。<br/>" +
            "2)服务”符合您的要求或经验进行修改;。<br/>" +
            "3)缺陷将会被更正;。<br/>" +
            "4) nternet延迟。本公司提供的“服务”可能因 Int ernet和电子通信固有的缺陷而产生限制、延迟和其它问题,本公司对此不承担责<br/>" +

            "<b>4.责任</b><br/>" +
            "您要对您账号下所发生的任何行为负责并遵守地方、国家及国外所有与适用本服务有关的适用法律、法规。。<br/>" +

            "<b>5.知识产权</b><br/>" +
            " 本公司完全拥有Wesley云管家系统的全部知识产权, Wesley云管家的名字、标志、LOGO、商标,文档,未经授权不能使用。<br/>" +

            "<b>6.第三方产品说明</b><br/>" +
            " 本公司只基于本协议各项条款向您提供服务,任何第三方产品在您使用服务过程中向您发出的各种活动广告等均与本公司无关。本公司对第三方的产品没有任何责任和义务提供任何服务。<br/>" +

            " <b>7.注册、保密</b><br/>" +
            " 1)您在注册成为Wesley云管家用户时,您提供关于您或贵公司的资料是真实、完整的。倘若您提供任何不真实、不完整或过时的资料,或Wesley云管家有合理理由怀疑该等资料不真实、不完整、Wesley云管家有权暂停或终止对您的服务。<br/>" +
            "  2)您有义务维持并及时更新用户资料,使其保持真实、完整和反映当前的情况。<br/>" +
            "  3)在登记过程中,您将选择用户注册名和密码您须自行负责对您的用户注册名和密码保密,且须对您在用户注册名和密码下发生的所有活动承担责任。<br/>" +
            "  4)您同意: 如发现任何人未经授权使用您的用户注册名或密码,您会立即通知Wesley云管家;<br/>" +

            " <b>8 关于您的资料</b><br/>" +
            " 您同意,“您的资料”和您提供在XXX云管家网站上交易的任何“物品”。<br/>" +
            "  1)不发布具有欺诈或者是任何会违反任何法律法规、条例或规章的资料,物品。<br/>" +
            "  2)不会含有蓄意毁坏、恶意干扰、秘密地截取或侵占任何系统、数据或个人资料的任何病毒、伪装破坏程序、电脑蠕虫、定时程序";

        public static string GetWeek(string dt)
        {
            string week = "";
            //根据取得的英文单词返回汉字
            switch (dt)
            {
                case "Monday":
                    week = "星期一";
                    break;
                case "Tuesday":
                    week = "星期二";
                    break;
                case "Wednesday":
                    week = "星期三";
                    break;
                case "Thursday":
                    week = "星期四";
                    break;
                case "Friday":
                    week = "星期五";
                    break;
                case "Saturday":
                    week = "星期六";
                    break;
                case "Sunday":
                    week = "星期日";
                    break;
            }
            return week;
        }


        private static TerminalModel terminalModel;
        public static TerminalModel TempAddCustomerStore
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(Settings.TempAddCustomer))
                    {
                        terminalModel = JsonConvert.DeserializeObject<TerminalModel>(Settings.TempAddCustomer);
                        return terminalModel;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set
            {
                terminalModel = value;
                if (terminalModel == null)
                    Settings.TempAddCustomer = "";
                else
                    Settings.TempAddCustomer = JsonConvert.SerializeObject(terminalModel);
            }
        }
    }
}
