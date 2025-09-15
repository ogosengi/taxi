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

        /// <summary>
        /// 근무 시간대 타입을 반환 (사용자 정의 시간 지원)
        /// </summary>
        public string ShiftType
        {
            get
            {
                return $"근무시간 ({StartTime:HH:mm}-{EndTime:HH:mm})";
            }
        }

        /// <summary>
        /// 근무 시간을 계산 (시간 단위) - 다음날까지 이어지는 근무 지원
        /// </summary>
        public double WorkingHours
        {
            get
            {
                if (IsNightShift || EndTime < StartTime)
                {
                    // 다음날까지 이어지는 근무인 경우
                    var startDateTime = Date.Date.Add(StartTime.ToTimeSpan());
                    var endDateTime = Date.Date.AddDays(1).Add(EndTime.ToTimeSpan());
                    return (endDateTime - startDateTime).TotalHours;
                }
                else
                {
                    // 같은 날 내에서 끝나는 근무
                    return (EndTime.ToTimeSpan() - StartTime.ToTimeSpan()).TotalHours;
                }
            }
        }

    }
}