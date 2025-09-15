namespace TaxiManager.Models
{
    /// <summary>
    /// 택시 운행 통계 정보
    /// </summary>
    public class TaxiOperationStats
    {
        /// <summary>
        /// 총 근무 횟수
        /// </summary>
        public int TotalShifts { get; set; }

        /// <summary>
        /// 완료된 근무 횟수
        /// </summary>
        public int CompletedShifts { get; set; }

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

        /// <summary>
        /// 근무 완료율 (%)
        /// </summary>
        public double CompletionRate
        {
            get
            {
                return TotalShifts > 0 ? (double)CompletedShifts / TotalShifts * 100 : 0;
            }
        }
    }
}