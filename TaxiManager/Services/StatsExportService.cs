using System.Text;
using TaxiManager.Models;

namespace TaxiManager.Services
{
    /// <summary>
    /// 통계 정보를 텍스트 파일로 내보내는 서비스
    /// </summary>
    public class StatsExportService
    {
        /// <summary>
        /// 통계 정보를 텍스트 파일로 내보내기
        /// </summary>
        public void ExportStatsToTextFile(TaxiOperationStats stats, DateTime startDate, DateTime endDate, string filePath)
        {
            var sb = new StringBuilder();

            // 제목 및 기간 정보
            sb.AppendLine("=".PadRight(60, '='));
            sb.AppendLine("📊 택시 운행 통계 리포트");
            sb.AppendLine("=".PadRight(60, '='));
            sb.AppendLine();
            sb.AppendLine($"📅 분석 기간: {startDate:yyyy-MM-dd} ~ {endDate:yyyy-MM-dd}");
            sb.AppendLine($"📄 생성 일시: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine();

            // 기본 통계 정보
            sb.AppendLine("📈 기본 운행 통계");
            sb.AppendLine("-".PadRight(40, '-'));
            sb.AppendLine($"• 총 근무 일수: {stats.TotalWorkDays:N0}일");
            sb.AppendLine($"• 총 매출: {stats.TotalRevenue:C}");
            sb.AppendLine($"• 총 근무시간: {stats.TotalWorkingHours:F1}시간");
            sb.AppendLine($"• 시간당 평균 매출: {stats.AverageRevenuePerHour:C}");
            sb.AppendLine();

            // 최고 효율성 분석
            sb.AppendLine("🎯 최고 효율성 분석");
            sb.AppendLine("-".PadRight(40, '-'));
            sb.AppendLine($"• 가장 효율적인 1시간: {stats.MostEfficientStartTime} ({stats.MostEfficientHourlyRevenue:C}/시간)");
            sb.AppendLine($"• 가장 효율적인 시간대: {stats.BestRevenueTimeBlock}");
            sb.AppendLine($"• 가장 효율적인 근무길이: {stats.BestWorkDuration}");
            sb.AppendLine();

            // 시간대별 효율성 (상위 10개만)
            if (stats.HourlyEfficiency.Count > 0)
            {
                sb.AppendLine("⏰ 시간대별 효율성 분석 (상위 10개)");
                sb.AppendLine("-".PadRight(40, '-'));
                var topEfficiency = stats.HourlyEfficiency
                    .Where(x => x.Value > 0)
                    .OrderByDescending(x => x.Value)
                    .Take(10);

                foreach (var item in topEfficiency)
                {
                    sb.AppendLine($"• {item.Key}: {item.Value:C}/시간");
                }
                sb.AppendLine();
            }

            // 야간/주간 근무 비교
            if (stats.DayNightComparison.Count > 0)
            {
                sb.AppendLine("🌙 주간/야간 근무 비교");
                sb.AppendLine("-".PadRight(40, '-'));
                foreach (var item in stats.DayNightComparison)
                {
                    sb.AppendLine($"• {item.Key}: {item.Value:C}/시간");
                }
                sb.AppendLine();
            }

            // 요일별 효율성
            if (stats.DayOfWeekEfficiency.Count > 0)
            {
                sb.AppendLine("📅 요일별 효율성 분석");
                sb.AppendLine("-".PadRight(40, '-'));
                var dayOrder = new string[] { "월요일", "화요일", "수요일", "목요일", "금요일", "토요일", "일요일" };
                foreach (var day in dayOrder)
                {
                    if (stats.DayOfWeekEfficiency.ContainsKey(day))
                    {
                        sb.AppendLine($"• {day}: {stats.DayOfWeekEfficiency[day]:C}/시간");
                    }
                }
                sb.AppendLine();
            }

            // 근무시간 길이별 효율성
            if (stats.WorkDurationEfficiency.Count > 0)
            {
                sb.AppendLine("⏱️ 근무시간 길이별 효율성");
                sb.AppendLine("-".PadRight(40, '-'));
                foreach (var item in stats.WorkDurationEfficiency.OrderByDescending(x => x.Value))
                {
                    sb.AppendLine($"• {item.Key}: {item.Value:C}/시간");
                }
                sb.AppendLine();
            }

            // 2시간 블록별 효율성 (상위 10개)
            if (stats.TwoHourBlockEfficiency.Count > 0)
            {
                sb.AppendLine("🕐 2시간 블록별 효율성 (상위 10개)");
                sb.AppendLine("-".PadRight(40, '-'));
                var topBlocks = stats.TwoHourBlockEfficiency
                    .Where(x => x.Value > 0)
                    .OrderByDescending(x => x.Value)
                    .Take(10);

                foreach (var item in topBlocks)
                {
                    sb.AppendLine($"• {item.Key}: {item.Value:C}/시간");
                }
                sb.AppendLine();
            }

            // 4시간 블록별 효율성 (상위 5개)
            if (stats.FourHourBlockEfficiency.Count > 0)
            {
                sb.AppendLine("🕓 4시간 블록별 효율성 (상위 5개)");
                sb.AppendLine("-".PadRight(40, '-'));
                var topBlocks = stats.FourHourBlockEfficiency
                    .Where(x => x.Value > 0)
                    .OrderByDescending(x => x.Value)
                    .Take(5);

                foreach (var item in topBlocks)
                {
                    sb.AppendLine($"• {item.Key}: {item.Value:C}/시간");
                }
                sb.AppendLine();
            }

            // 운행 최적화 제안
            sb.AppendLine("💡 운행 최적화 제안");
            sb.AppendLine("-".PadRight(40, '-'));
            sb.AppendLine($"• 가장 수익성 높은 시간대: {stats.MostEfficientStartTime} 시간대 집중 운행");
            sb.AppendLine($"• 추천 근무 패턴: {stats.BestWorkDuration} 근무");
            sb.AppendLine($"• 추천 시간대: {stats.BestRevenueTimeBlock}");

            if (stats.DayNightComparison.Count > 0)
            {
                var bestShift = stats.DayNightComparison.OrderByDescending(x => x.Value).First();
                sb.AppendLine($"• 추천 근무 시간: {bestShift.Key} 우선 고려");
            }
            sb.AppendLine();

            // 파일 정보
            sb.AppendLine("=".PadRight(60, '='));
            sb.AppendLine("📋 파일 정보");
            sb.AppendLine($"• 파일명: {Path.GetFileName(filePath)}");
            sb.AppendLine($"• 생성 프로그램: TaxiManager v1.0");
            sb.AppendLine($"• 분석 방식: 실제 근무시각 기준 시간대별 효율성 분석");
            sb.AppendLine("=".PadRight(60, '='));

            // 파일로 저장
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }
    }
}