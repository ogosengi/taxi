using System;

namespace TaxiManager.Models
{
    /// <summary>
    /// 택시 근무 시간 정보를 관리하는 클래스
    /// </summary>
    public class TaxiWorkShift
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } // 근무 날짜
        public TimeOnly StartTime { get; set; } // 근무 시작 시간
        public TimeOnly EndTime { get; set; } // 근무 종료 시간
        public bool IsNightShift { get; set; } // 야간 근무 여부 (다음날까지 이어지는 경우)
        public decimal Revenue { get; set; } // 매출
        public string Notes { get; set; } = string.Empty; // 메모
        public bool IsCompleted { get; set; } // 마감 여부

        /// <summary>
        /// 근무 시간대 타입을 반환
        /// </summary>
        public string ShiftType
        {
            get
            {
                if (StartTime.Hour == 10 && EndTime.Hour == 15)
                    return "오전 근무 (10:00-15:00)";
                else if (StartTime.Hour == 19)
                    return "야간 근무 (19:00-02:00)";
                else
                    return "사용자 정의 근무";
            }
        }

        /// <summary>
        /// 근무 시간을 계산 (시간 단위)
        /// </summary>
        public double WorkingHours
        {
            get
            {
                if (IsNightShift)
                {
                    // 야간 근무의 경우 다음날 2시까지
                    var endDateTime = new DateTime(Date.Year, Date.Month, Date.Day, 23, 59, 59).AddDays(1).Date.AddHours(2);
                    var startDateTime = Date.Date.Add(StartTime.ToTimeSpan());
                    return (endDateTime - startDateTime).TotalHours;
                }
                else
                {
                    return (EndTime.ToTimeSpan() - StartTime.ToTimeSpan()).TotalHours;
                }
            }
        }
    }
}