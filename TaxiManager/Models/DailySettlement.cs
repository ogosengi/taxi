using System;

namespace TaxiManager.Models
{
    /// <summary>
    /// 일별 마감 정보를 관리하는 클래스
    /// </summary>
    public class DailySettlement
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } // 마감 날짜
        public DateTime SettlementDateTime { get; set; } // 마감 처리 일시
        public decimal TotalRevenue { get; set; } // 총 매출
        public double TotalWorkingHours { get; set; } // 총 근무시간
        public string Notes { get; set; } = string.Empty; // 메모

        /// <summary>
        /// 시간당 평균 매출
        /// </summary>
        public decimal AverageRevenuePerHour
        {
            get
            {
                return TotalWorkingHours > 0 ? TotalRevenue / (decimal)TotalWorkingHours : 0;
            }
        }
    }
}