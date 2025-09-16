namespace TaxiManager.Models
{
    /// <summary>
    /// 택시 운행 통계 정보
    /// </summary>
    public class TaxiOperationStats
    {
        /// <summary>
        /// 총 근무 일수
        /// </summary>
        public int TotalWorkDays { get; set; }

        /// <summary>
        /// 총 매출
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// 총 근무시간
        /// </summary>
        public double TotalWorkingHours { get; set; }

        /// <summary>
        /// 시간당 평균 매출
        /// </summary>
        public decimal AverageRevenuePerHour { get; set; }

    }
}