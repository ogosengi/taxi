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
                // 종료시간이 00:00인 경우 다음날 자정으로 처리
                // 또는 야간근무 체크가 되어있거나 종료시간이 시작시간보다 작은 경우
                if (IsNightShift || EndTime < StartTime || (EndTime.Hour == 0 && EndTime.Minute == 0))
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