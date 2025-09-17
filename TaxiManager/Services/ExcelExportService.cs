using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using TaxiManager.Models;

namespace TaxiManager.Services
{
    /// <summary>
    /// 택시 운행 데이터를 엑셀로 내보내는 서비스
    /// </summary>
    public class ExcelExportService
    {
        /// <summary>
        /// ExcelExportService 생성자
        /// </summary>
        public ExcelExportService()
        {
            // EPPlus 라이센스 설정 (비상업적 사용)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }
        /// <summary>
        /// 근무시간 데이터를 엑셀로 내보내기
        /// </summary>
        public void ExportWorkShiftsToExcel(List<TaxiWorkShift> workShifts, string filePath)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("근무시간");

            // 헤더 설정
            var headers = new string[] { "날짜", "시작시간", "종료시간", "야간근무", "근무시간", "근무타입", "메모" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                worksheet.Cells[1, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            // 데이터 입력
            for (int i = 0; i < workShifts.Count; i++)
            {
                var shift = workShifts[i];
                var row = i + 2;

                worksheet.Cells[row, 1].Value = shift.Date.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 2].Value = shift.StartTime.ToString("HH:mm");
                worksheet.Cells[row, 3].Value = shift.EndTime.ToString("HH:mm");
                worksheet.Cells[row, 4].Value = shift.IsNightShift ? "야간" : "주간";
                worksheet.Cells[row, 5].Value = $"{shift.WorkingHours:F1}시간";
                worksheet.Cells[row, 6].Value = shift.ShiftType;
                worksheet.Cells[row, 7].Value = shift.Notes;

                // 테두리 설정
                for (int j = 1; j <= headers.Length; j++)
                {
                    worksheet.Cells[row, j].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }
            }

            // 컬럼 너비 자동 조정
            worksheet.Cells.AutoFitColumns();

            // 파일 저장
            var fileInfo = new FileInfo(filePath);
            package.SaveAs(fileInfo);
        }

        /// <summary>
        /// 일별 마감 데이터를 엑셀로 내보내기
        /// </summary>
        public void ExportDailySettlementsToExcel(List<DailySettlement> settlements, string filePath)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("일별마감");

            // 헤더 설정
            var headers = new string[] { "마감일", "총매출", "총근무시간", "시간당평균매출", "메모" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                worksheet.Cells[1, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            // 데이터 입력
            for (int i = 0; i < settlements.Count; i++)
            {
                var settlement = settlements[i];
                var row = i + 2;

                worksheet.Cells[row, 1].Value = settlement.Date.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 2].Value = settlement.TotalRevenue;
                worksheet.Cells[row, 2].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[row, 3].Value = $"{settlement.TotalWorkingHours:F1}시간";
                worksheet.Cells[row, 4].Value = settlement.AverageRevenuePerHour;
                worksheet.Cells[row, 4].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[row, 5].Value = settlement.Notes;

                // 테두리 설정
                for (int j = 1; j <= headers.Length; j++)
                {
                    worksheet.Cells[row, j].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }
            }

            // 컬럼 너비 자동 조정
            worksheet.Cells.AutoFitColumns();

            // 파일 저장
            var fileInfo = new FileInfo(filePath);
            package.SaveAs(fileInfo);
        }

        /// <summary>
        /// 통합 운행 리포트를 엑셀로 내보내기 (근무시간 + 일별마감 + 통계)
        /// </summary>
        public void ExportFullReportToExcel(List<TaxiWorkShift> workShifts, List<DailySettlement> settlements,
            TaxiOperationStats stats, DateTime startDate, DateTime endDate, string filePath)
        {
            using var package = new ExcelPackage();

            // 1. 요약 통계 시트
            CreateStatsSheet(package, stats, startDate, endDate);

            // 2. 근무시간 시트
            CreateWorkShiftsSheet(package, workShifts);

            // 3. 일별마감 시트
            CreateSettlementsSheet(package, settlements);

            // 4. 시간대별 효율성 시트
            CreateEfficiencySheet(package, stats);

            // 파일 저장
            var fileInfo = new FileInfo(filePath);
            package.SaveAs(fileInfo);
        }

        /// <summary>
        /// 요약 통계 시트 생성
        /// </summary>
        private void CreateStatsSheet(ExcelPackage package, TaxiOperationStats stats, DateTime startDate, DateTime endDate)
        {
            var worksheet = package.Workbook.Worksheets.Add("요약통계");

            // 제목
            worksheet.Cells[1, 1].Value = "택시 운행 요약 통계";
            worksheet.Cells[1, 1].Style.Font.Size = 16;
            worksheet.Cells[1, 1].Style.Font.Bold = true;

            // 기간
            worksheet.Cells[3, 1].Value = "조회 기간:";
            worksheet.Cells[3, 2].Value = $"{startDate:yyyy-MM-dd} ~ {endDate:yyyy-MM-dd}";

            // 통계 데이터
            var statsData = new object[,]
            {
                { "총 근무 일수", $"{stats.TotalWorkDays}일" },
                { "총 매출", $"{stats.TotalRevenue:C}" },
                { "총 근무시간", $"{stats.TotalWorkingHours:F1}시간" },
                { "시간당 평균 매출", $"{stats.AverageRevenuePerHour:C}" },
                { "가장 효율적인 근무시간", stats.MostEfficientStartTime },
                { "최고 시간당 매출", $"{stats.MostEfficientHourlyRevenue:C}" }
            };

            for (int i = 0; i < statsData.GetLength(0); i++)
            {
                worksheet.Cells[i + 5, 1].Value = statsData[i, 0];
                worksheet.Cells[i + 5, 2].Value = statsData[i, 1];
                worksheet.Cells[i + 5, 1].Style.Font.Bold = true;
            }

            worksheet.Cells.AutoFitColumns();
        }

        /// <summary>
        /// 근무시간 시트 생성
        /// </summary>
        private void CreateWorkShiftsSheet(ExcelPackage package, List<TaxiWorkShift> workShifts)
        {
            var worksheet = package.Workbook.Worksheets.Add("근무시간");

            // 헤더
            var headers = new string[] { "날짜", "시작시간", "종료시간", "야간근무", "근무시간", "근무타입", "메모" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            }

            // 데이터
            for (int i = 0; i < workShifts.Count; i++)
            {
                var shift = workShifts[i];
                var row = i + 2;

                worksheet.Cells[row, 1].Value = shift.Date.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 2].Value = shift.StartTime.ToString("HH:mm");
                worksheet.Cells[row, 3].Value = shift.EndTime.ToString("HH:mm");
                worksheet.Cells[row, 4].Value = shift.IsNightShift ? "야간" : "주간";
                worksheet.Cells[row, 5].Value = shift.WorkingHours;
                worksheet.Cells[row, 5].Style.Numberformat.Format = "0.0";
                worksheet.Cells[row, 6].Value = shift.ShiftType;
                worksheet.Cells[row, 7].Value = shift.Notes;
            }

            worksheet.Cells.AutoFitColumns();
        }

        /// <summary>
        /// 일별마감 시트 생성
        /// </summary>
        private void CreateSettlementsSheet(ExcelPackage package, List<DailySettlement> settlements)
        {
            var worksheet = package.Workbook.Worksheets.Add("일별마감");

            // 헤더
            var headers = new string[] { "마감일", "총매출", "총근무시간", "시간당평균매출", "메모" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
            }

            // 데이터
            for (int i = 0; i < settlements.Count; i++)
            {
                var settlement = settlements[i];
                var row = i + 2;

                worksheet.Cells[row, 1].Value = settlement.Date.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 2].Value = settlement.TotalRevenue;
                worksheet.Cells[row, 2].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[row, 3].Value = settlement.TotalWorkingHours;
                worksheet.Cells[row, 3].Style.Numberformat.Format = "0.0";
                worksheet.Cells[row, 4].Value = settlement.AverageRevenuePerHour;
                worksheet.Cells[row, 4].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[row, 5].Value = settlement.Notes;
            }

            worksheet.Cells.AutoFitColumns();
        }

        /// <summary>
        /// 시간대별 효율성 시트 생성
        /// </summary>
        private void CreateEfficiencySheet(ExcelPackage package, TaxiOperationStats stats)
        {
            var worksheet = package.Workbook.Worksheets.Add("시간대별효율성");

            // 헤더
            worksheet.Cells[1, 1].Value = "시작시간";
            worksheet.Cells[1, 2].Value = "시간당평균매출";
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 2].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
            worksheet.Cells[1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[1, 2].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);

            // 데이터 (효율성 순으로 정렬)
            var sortedEfficiency = stats.HourlyEfficiency.OrderByDescending(x => x.Value).ToList();
            for (int i = 0; i < sortedEfficiency.Count; i++)
            {
                var row = i + 2;
                worksheet.Cells[row, 1].Value = sortedEfficiency[i].Key;
                worksheet.Cells[row, 2].Value = sortedEfficiency[i].Value;
                worksheet.Cells[row, 2].Style.Numberformat.Format = "#,##0";

                // 가장 효율적인 시간대 강조
                if (i == 0)
                {
                    worksheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                    worksheet.Cells[row, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 2].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                }
            }

            worksheet.Cells.AutoFitColumns();
        }
    }
}