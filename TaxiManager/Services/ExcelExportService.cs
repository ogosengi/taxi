using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
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
        static ExcelExportService()
        {
            // EPPlus 라이센스 설정 (비상업적 사용)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public ExcelExportService()
        {
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
        /// 엑셀 작업용 통합 리포트 내보내기 (편집 가능한 형태)
        /// </summary>
        public void ExportWorkableReportToExcel(List<TaxiWorkShift> workShifts, List<DailySettlement> settlements,
            TaxiOperationStats stats, DateTime startDate, DateTime endDate, string filePath)
        {
            using var package = new ExcelPackage();

            // 1. 근무시간 데이터 시트 (편집 가능)
            CreateEditableWorkShiftsSheet(package, workShifts);

            // 2. 일별마감 데이터 시트 (편집 가능)
            CreateEditableSettlementsSheet(package, settlements);

            // 3. 시간대별 효율성 데이터 시트
            CreateEditableEfficiencySheet(package, stats);

            // 4. 통계 대시보드 시트 (수식 기반)
            CreateStatsDashboardSheet(package, startDate, endDate);

            // 5. 차트 시트
            CreateChartsSheet(package);

            // 6. 월별 요약 시트 (피벗 테이블 스타일)
            CreateMonthlySummarySheet(package);

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

        /// <summary>
        /// 편집 가능한 근무시간 시트 생성 (테이블 형태)
        /// </summary>
        private void CreateEditableWorkShiftsSheet(ExcelPackage package, List<TaxiWorkShift> workShifts)
        {
            var worksheet = package.Workbook.Worksheets.Add("근무시간데이터");

            // 헤더
            var headers = new string[] { "날짜", "시작시간", "종료시간", "야간근무", "근무시간", "근무타입", "메모" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                worksheet.Cells[1, i + 1].Style.Font.Color.SetColor(Color.White);
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

            // 테이블로 변환
            var range = worksheet.Cells[1, 1, workShifts.Count + 1, headers.Length];
            var table = worksheet.Tables.Add(range, "근무시간테이블");
            table.ShowHeader = true;

            worksheet.Cells.AutoFitColumns();
        }

        /// <summary>
        /// 편집 가능한 일별마감 시트 생성 (테이블 형태)
        /// </summary>
        private void CreateEditableSettlementsSheet(ExcelPackage package, List<DailySettlement> settlements)
        {
            var worksheet = package.Workbook.Worksheets.Add("일별마감데이터");

            // 헤더
            var headers = new string[] { "마감일", "총매출", "총근무시간", "시간당평균매출", "메모" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.DarkGreen);
                worksheet.Cells[1, i + 1].Style.Font.Color.SetColor(Color.White);
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

                // 시간당 평균매출 수식으로 계산
                worksheet.Cells[row, 4].Formula = $"=B{row}/C{row}";
                worksheet.Cells[row, 4].Style.Numberformat.Format = "#,##0";

                worksheet.Cells[row, 5].Value = settlement.Notes;
            }

            // 테이블로 변환
            var range = worksheet.Cells[1, 1, settlements.Count + 1, headers.Length];
            var table = worksheet.Tables.Add(range, "일별마감테이블");
            table.ShowHeader = true;

            worksheet.Cells.AutoFitColumns();
        }

        /// <summary>
        /// 편집 가능한 시간대별 효율성 시트 생성
        /// </summary>
        private void CreateEditableEfficiencySheet(ExcelPackage package, TaxiOperationStats stats)
        {
            var worksheet = package.Workbook.Worksheets.Add("시간대별효율성");

            // 헤더
            worksheet.Cells[1, 1].Value = "시작시간";
            worksheet.Cells[1, 2].Value = "시간당평균매출";
            worksheet.Cells[1, 3].Value = "효율성순위";

            for (int i = 1; i <= 3; i++)
            {
                worksheet.Cells[1, i].Style.Font.Bold = true;
                worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.Purple);
                worksheet.Cells[1, i].Style.Font.Color.SetColor(Color.White);
            }

            // 데이터 (효율성 순으로 정렬)
            var sortedEfficiency = stats.HourlyEfficiency.OrderByDescending(x => x.Value).ToList();
            for (int i = 0; i < sortedEfficiency.Count; i++)
            {
                var row = i + 2;
                worksheet.Cells[row, 1].Value = sortedEfficiency[i].Key;
                worksheet.Cells[row, 2].Value = sortedEfficiency[i].Value;
                worksheet.Cells[row, 2].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[row, 3].Value = i + 1; // 순위

                // 1위 강조
                if (i == 0)
                {
                    for (int j = 1; j <= 3; j++)
                    {
                        worksheet.Cells[row, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, j].Style.Fill.BackgroundColor.SetColor(Color.Gold);
                        worksheet.Cells[row, j].Style.Font.Bold = true;
                    }
                }
            }

            // 테이블로 변환
            if (sortedEfficiency.Count > 0)
            {
                var range = worksheet.Cells[1, 1, sortedEfficiency.Count + 1, 3];
                var table = worksheet.Tables.Add(range, "효율성테이블");
                table.ShowHeader = true;
            }

            worksheet.Cells.AutoFitColumns();
        }

        /// <summary>
        /// 통계 대시보드 시트 생성 (수식 기반)
        /// </summary>
        private void CreateStatsDashboardSheet(ExcelPackage package, DateTime startDate, DateTime endDate)
        {
            var worksheet = package.Workbook.Worksheets.Add("통계대시보드");

            // 제목
            worksheet.Cells[1, 1].Value = "택시 운행 통계 대시보드";
            worksheet.Cells[1, 1].Style.Font.Size = 18;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1, 1, 4].Merge = true;

            // 기간
            worksheet.Cells[3, 1].Value = "조회 기간:";
            worksheet.Cells[3, 2].Value = $"{startDate:yyyy-MM-dd} ~ {endDate:yyyy-MM-dd}";
            worksheet.Cells[3, 1].Style.Font.Bold = true;

            // 통계 항목들 (수식으로 계산)
            var statsLabels = new string[]
            {
                "총 근무 일수", "총 매출", "총 근무시간", "평균 시간당 매출",
                "최고 일 매출", "최저 일 매출", "평균 일 매출",
                "가장 효율적인 시간대", "최고 시간당 매출"
            };

            var formulas = new string[]
            {
                "=COUNTA(일별마감데이터[마감일])", // 총 근무 일수
                "=SUM(일별마감데이터[총매출])", // 총 매출
                "=SUM(일별마감데이터[총근무시간])", // 총 근무시간
                "=B8/B9", // 평균 시간당 매출
                "=MAX(일별마감데이터[총매출])", // 최고 일 매출
                "=MIN(일별마감데이터[총매출])", // 최저 일 매출
                "=AVERAGE(일별마감데이터[총매출])", // 평균 일 매출
                "=INDEX(시간대별효율성[시작시간],1)", // 가장 효율적인 시간대
                "=MAX(시간대별효율성[시간당평균매출])" // 최고 시간당 매출
            };

            for (int i = 0; i < statsLabels.Length; i++)
            {
                var row = i + 6;
                worksheet.Cells[row, 1].Value = statsLabels[i];
                worksheet.Cells[row, 1].Style.Font.Bold = true;

                if (i < formulas.Length)
                {
                    worksheet.Cells[row, 2].Formula = formulas[i];
                }

                // 숫자 포맷 설정
                if (i == 1 || i == 4 || i == 5 || i == 6 || i == 8) // 매출 관련
                {
                    worksheet.Cells[row, 2].Style.Numberformat.Format = "#,##0";
                }
                else if (i == 2 || i == 3) // 시간 관련
                {
                    worksheet.Cells[row, 2].Style.Numberformat.Format = "0.0";
                }
            }

            // 월별 통계 섹션
            worksheet.Cells[16, 1].Value = "월별 매출 분석";
            worksheet.Cells[16, 1].Style.Font.Size = 14;
            worksheet.Cells[16, 1].Style.Font.Bold = true;

            // 월별 헤더
            worksheet.Cells[18, 1].Value = "월";
            worksheet.Cells[18, 2].Value = "매출";
            worksheet.Cells[18, 3].Value = "근무시간";
            worksheet.Cells[18, 4].Value = "평균시간당매출";

            for (int i = 1; i <= 4; i++)
            {
                worksheet.Cells[18, i].Style.Font.Bold = true;
                worksheet.Cells[18, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[18, i].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
            }

            worksheet.Cells.AutoFitColumns();
        }

        /// <summary>
        /// 차트 시트 생성
        /// </summary>
        private void CreateChartsSheet(ExcelPackage package)
        {
            var worksheet = package.Workbook.Worksheets.Add("차트분석");

            worksheet.Cells[1, 1].Value = "시각적 분석 차트";
            worksheet.Cells[1, 1].Style.Font.Size = 16;
            worksheet.Cells[1, 1].Style.Font.Bold = true;

            // 차트 생성을 위한 안내
            worksheet.Cells[3, 1].Value = "차트 생성 가이드:";
            worksheet.Cells[3, 1].Style.Font.Bold = true;

            var instructions = new string[]
            {
                "1. 일별매출 추이: 일별마감데이터 시트에서 날짜와 총매출로 선 차트 생성",
                "2. 시간대별 효율성: 시간대별효율성 시트에서 막대 차트 생성",
                "3. 월별 매출 비교: 통계대시보드의 월별 데이터로 파이 차트 생성",
                "4. 근무시간 분포: 근무시간데이터에서 히스토그램 생성"
            };

            for (int i = 0; i < instructions.Length; i++)
            {
                worksheet.Cells[i + 5, 1].Value = instructions[i];
            }

            worksheet.Cells.AutoFitColumns();
        }

        /// <summary>
        /// 월별 요약 시트 생성
        /// </summary>
        private void CreateMonthlySummarySheet(ExcelPackage package)
        {
            var worksheet = package.Workbook.Worksheets.Add("월별요약");

            worksheet.Cells[1, 1].Value = "월별 운행 요약";
            worksheet.Cells[1, 1].Style.Font.Size = 16;
            worksheet.Cells[1, 1].Style.Font.Bold = true;

            // 헤더
            var headers = new string[] { "연월", "근무일수", "총매출", "총근무시간", "평균시간당매출", "최고일매출", "목표달성률" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[3, i + 1].Value = headers[i];
                worksheet.Cells[3, i + 1].Style.Font.Bold = true;
                worksheet.Cells[3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(Color.Navy);
                worksheet.Cells[3, i + 1].Style.Font.Color.SetColor(Color.White);
            }

            // 수식으로 월별 데이터 계산하는 템플릿 제공
            worksheet.Cells[5, 1].Value = "사용법: 피벗 테이블을 사용하여 일별마감데이터에서 월별 요약을 생성하세요.";
            worksheet.Cells[6, 1].Value = "삽입 > 피벗테이블 > 일별마감데이터 선택 > 행: 월, 값: 총매출(합계), 총근무시간(합계)";

            worksheet.Cells.AutoFitColumns();
        }
    }
}