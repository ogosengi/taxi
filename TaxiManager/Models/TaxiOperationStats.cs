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

        /// <summary>
        /// 가장 효율적인 근무 시간대 (시작 시간)
        /// </summary>
        public string MostEfficientStartTime { get; set; } = string.Empty;

        /// <summary>
        /// 가장 효율적인 시간대의 시간당 평균 매출
        /// </summary>
        public decimal MostEfficientHourlyRevenue { get; set; }

        /// <summary>
        /// 시간대별 효율성 상세 정보
        /// </summary>
        public Dictionary<string, decimal> HourlyEfficiency { get; set; } = new Dictionary<string, decimal>();

        /// <summary>
        /// 2시간 블록별 수익률 분석
        /// </summary>
        public Dictionary<string, decimal> TwoHourBlockEfficiency { get; set; } = new Dictionary<string, decimal>();

        /// <summary>
        /// 4시간 블록별 수익률 분석
        /// </summary>
        public Dictionary<string, decimal> FourHourBlockEfficiency { get; set; } = new Dictionary<string, decimal>();

        /// <summary>
        /// 근무시간 길이별 효율성 (짧은/중간/긴 근무)
        /// </summary>
        public Dictionary<string, decimal> WorkDurationEfficiency { get; set; } = new Dictionary<string, decimal>();

        /// <summary>
        /// 야간/주간 근무 비교 분석
        /// </summary>
        public Dictionary<string, decimal> DayNightComparison { get; set; } = new Dictionary<string, decimal>();

        /// <summary>
        /// 요일별 수익률 분석
        /// </summary>
        public Dictionary<string, decimal> DayOfWeekEfficiency { get; set; } = new Dictionary<string, decimal>();

        /// <summary>
        /// 최고 수익률 시간대 정보
        /// </summary>
        public string BestRevenueTimeBlock { get; set; } = string.Empty;

        /// <summary>
        /// 최고 수익률 근무 길이
        /// </summary>
        public string BestWorkDuration { get; set; } = string.Empty;

    }
}