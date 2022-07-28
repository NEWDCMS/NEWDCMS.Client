namespace Wesley.Client.Models.Report
{

    //员工报表

    #region 业务员业绩
    /// <summary>
    /// 业务员业绩
    /// </summary>
    public class StaffReportBusinessUserAchievement
    {

        /// <summary>
        /// 业务员Id
        /// </summary>
        public int? BusinessUserId { get; set; }

        /// <summary>
        /// 业务员名称
        /// </summary>
        public string BusinessUserName { get; set; }

        /// <summary>
        /// 销售金额
        /// </summary>
        public decimal? SaleAmount { get; set; }

        /// <summary>
        /// 退货金额
        /// </summary>
        public decimal? ReturnAmount { get; set; }

        /// <summary>
        /// 销售净额
        /// </summary>
        public decimal? NetAmount { get; set; }

    }
    #endregion


}
