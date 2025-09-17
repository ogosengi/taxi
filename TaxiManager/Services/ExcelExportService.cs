using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.Util;
using TaxiManager.Models;

namespace TaxiManager.Services
{
    /// <summary>
    /// 택시 운행 데이터를 엑셀로 내보내는 서비스 (NPOI 사용)
    /// </summary>
    public class ExcelExportService
    {
        /// <summary>
        /// ExcelExportService 생성자
        /// </summary>
        public ExcelExportService()
        {
        }

        /// <summary>
        /// 근무시간 데이터를 엑셀로 내보내기
        /// </summary>
        public void ExportWorkShiftsToExcel(List<TaxiWorkShift> workShifts, string filePath)
        {
            var workbook = new XSSFWorkbook();
            var worksheet = workbook.CreateSheet("근무시간");

            // 헤더 설정
            var headerRow = worksheet.CreateRow(0);
            var headers = new string[] { "날짜", "시작시간", "종료시간", "야간근무", "근무시간", "근무타입", "메모" };

            // 헤더 스타일
            var headerStyle = CreateHeaderStyle(workbook, IndexedColors.Grey25Percent);

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = headerRow.CreateCell(i);
                cell.SetCellValue(headers[i]);
                cell.CellStyle = headerStyle;
            }

            // 데이터 입력
            for (int i = 0; i < workShifts.Count; i++)
            {
                var shift = workShifts[i];
                var row = worksheet.CreateRow(i + 1);

                row.CreateCell(0).SetCellValue(shift.Date.ToString("yyyy-MM-dd"));
                row.CreateCell(1).SetCellValue(shift.StartTime.ToString("HH:mm"));
                row.CreateCell(2).SetCellValue(shift.EndTime.ToString("HH:mm"));
                row.CreateCell(3).SetCellValue(shift.IsNightShift ? "야간" : "주간");
                row.CreateCell(4).SetCellValue($"{shift.WorkingHours:F1}시간");
                row.CreateCell(5).SetCellValue(shift.ShiftType ?? "");
                row.CreateCell(6).SetCellValue(shift.Notes ?? "");
            }

            // 컬럼 너비 자동 조정
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.AutoSizeColumn(i);
            }

            // 파일 저장
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            workbook.Write(fileStream);
            workbook.Close();
        }

        /// <summary>
        /// 일별 마감 데이터를 엑셀로 내보내기
        /// </summary>
        public void ExportDailySettlementsToExcel(List<DailySettlement> settlements, string filePath)
        {
            var workbook = new XSSFWorkbook();
            var worksheet = workbook.CreateSheet("일별마감");

            // 헤더 설정
            var headerRow = worksheet.CreateRow(0);
            var headers = new string[] { "마감일", "총매출", "총근무시간", "시간당평균매출", "메모" };

            // 헤더 스타일
            var headerStyle = CreateHeaderStyle(workbook, IndexedColors.LightBlue);

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = headerRow.CreateCell(i);
                cell.SetCellValue(headers[i]);
                cell.CellStyle = headerStyle;
            }

            // 숫자 스타일
            var numberStyle = CreateNumberStyle(workbook, "#,##0");
            var decimalStyle = CreateNumberStyle(workbook, "0.0");

            // 데이터 입력
            for (int i = 0; i < settlements.Count; i++)
            {
                var settlement = settlements[i];
                var row = worksheet.CreateRow(i + 1);

                row.CreateCell(0).SetCellValue(settlement.Date.ToString("yyyy-MM-dd"));

                var revenueCell = row.CreateCell(1);
                revenueCell.SetCellValue((double)settlement.TotalRevenue);
                revenueCell.CellStyle = numberStyle;

                var hoursCell = row.CreateCell(2);
                hoursCell.SetCellValue(settlement.TotalWorkingHours);
                hoursCell.CellStyle = decimalStyle;

                var avgCell = row.CreateCell(3);
                avgCell.SetCellValue((double)settlement.AverageRevenuePerHour);
                avgCell.CellStyle = numberStyle;

                row.CreateCell(4).SetCellValue(settlement.Notes ?? "");
            }

            // 컬럼 너비 자동 조정
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.AutoSizeColumn(i);
            }

            // 파일 저장
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            workbook.Write(fileStream);
            workbook.Close();
        }

        /// <summary>
        /// 엑셀 작업용 통합 리포트 내보내기 (편집 가능한 형태)
        /// </summary>
        public void ExportWorkableReportToExcel(List<TaxiWorkShift> workShifts, List<DailySettlement> settlements,
            TaxiOperationStats stats, DateTime startDate, DateTime endDate, string filePath)
        {
            var workbook = new XSSFWorkbook();

            // 1. 근무시간 데이터 시트 (편집 가능)
            CreateEditableWorkShiftsSheet(workbook, workShifts);

            // 2. 일별마감 데이터 시트 (편집 가능)
            CreateEditableSettlementsSheet(workbook, settlements);

            // 3. 시간대별 효율성 데이터 시트
            CreateEditableEfficiencySheet(workbook, stats);

            // 4. 통계 대시보드 시트 (수식 기반)
            CreateStatsDashboardSheet(workbook, startDate, endDate);

            // 5. 차트 시트
            CreateChartsSheet(workbook);

            // 6. 월별 요약 시트 (피벗 테이블 스타일)
            CreateMonthlySummarySheet(workbook);

            // 파일 저장
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            workbook.Write(fileStream);
            workbook.Close();
        }

        /// <summary>
        /// 통합 운행 리포트를 엑셀로 내보내기 (근무시간 + 일별마감 + 통계)
        /// </summary>
        public void ExportFullReportToExcel(List<TaxiWorkShift> workShifts, List<DailySettlement> settlements,
            TaxiOperationStats stats, DateTime startDate, DateTime endDate, string filePath)
        {
            var workbook = new XSSFWorkbook();

            // 1. 요약 통계 시트
            CreateStatsSheet(workbook, stats, startDate, endDate);

            // 2. 근무시간 시트
            CreateWorkShiftsSheet(workbook, workShifts);

            // 3. 일별마감 시트
            CreateSettlementsSheet(workbook, settlements);

            // 4. 시간대별 효율성 시트
            CreateEfficiencySheet(workbook, stats);

            // 파일 저장
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            workbook.Write(fileStream);
            workbook.Close();
        }

        /// <summary>
        /// 헤더 스타일 생성
        /// </summary>
        private ICellStyle CreateHeaderStyle(IWorkbook workbook, IndexedColors backgroundColor)
        {
            var style = workbook.CreateCellStyle();
            style.FillForegroundColor = backgroundColor.Index;
            style.FillPattern = FillPattern.SolidForeground;

            var font = workbook.CreateFont();
            font.IsBold = true;
            style.SetFont(font);

            style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;

            return style;
        }

        /// <summary>
        /// 숫자 스타일 생성
        /// </summary>
        private ICellStyle CreateNumberStyle(IWorkbook workbook, string format)
        {
            var style = workbook.CreateCellStyle();
            style.DataFormat = workbook.CreateDataFormat().GetFormat(format);
            return style;
        }

        /// <summary>
        /// 요약 통계 시트 생성
        /// </summary>
        private void CreateStatsSheet(IWorkbook workbook, TaxiOperationStats stats, DateTime startDate, DateTime endDate)
        {
            var worksheet = workbook.CreateSheet("요약통계");

            // 제목
            var titleRow = worksheet.CreateRow(0);
            var titleCell = titleRow.CreateCell(0);
            titleCell.SetCellValue("택시 운행 요약 통계");

            var titleStyle = workbook.CreateCellStyle();
            var titleFont = workbook.CreateFont();
            titleFont.FontHeightInPoints = 16;
            titleFont.IsBold = true;
            titleStyle.SetFont(titleFont);
            titleCell.CellStyle = titleStyle;

            // 기간
            var periodRow = worksheet.CreateRow(2);
            periodRow.CreateCell(0).SetCellValue("조회 기간:");
            periodRow.CreateCell(1).SetCellValue($"{startDate:yyyy-MM-dd} ~ {endDate:yyyy-MM-dd}");

            // 통계 데이터
            var statsData = new object[,]
            {
                { "총 근무 일수", $"{stats.TotalWorkDays}일" },
                { "총 매출", $"{stats.TotalRevenue:C}" },
                { "총 근무시간", $"{stats.TotalWorkingHours:F1}시간" },
                { "시간당 평균 매출", $"{stats.AverageRevenuePerHour:C}" },
                { "가장 효율적인 근무시간", stats.MostEfficientStartTime ?? "" },
                { "최고 시간당 매출", $"{stats.MostEfficientHourlyRevenue:C}" }
            };

            var boldStyle = workbook.CreateCellStyle();
            var boldFont = workbook.CreateFont();
            boldFont.IsBold = true;
            boldStyle.SetFont(boldFont);

            for (int i = 0; i < statsData.GetLength(0); i++)
            {
                var row = worksheet.CreateRow(i + 4);
                var labelCell = row.CreateCell(0);
                labelCell.SetCellValue(statsData[i, 0].ToString());
                labelCell.CellStyle = boldStyle;
                row.CreateCell(1).SetCellValue(statsData[i, 1].ToString());
            }

            // 컬럼 너비 자동 조정
            worksheet.AutoSizeColumn(0);
            worksheet.AutoSizeColumn(1);
        }

        /// <summary>
        /// 근무시간 시트 생성
        /// </summary>
        private void CreateWorkShiftsSheet(IWorkbook workbook, List<TaxiWorkShift> workShifts)
        {
            var worksheet = workbook.CreateSheet("근무시간");

            // 헤더
            var headerRow = worksheet.CreateRow(0);
            var headers = new string[] { "날짜", "시작시간", "종료시간", "야간근무", "근무시간", "근무타입", "메모" };
            var headerStyle = CreateHeaderStyle(workbook, IndexedColors.Grey25Percent);

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = headerRow.CreateCell(i);
                cell.SetCellValue(headers[i]);
                cell.CellStyle = headerStyle;
            }

            var decimalStyle = CreateNumberStyle(workbook, "0.0");

            // 데이터
            for (int i = 0; i < workShifts.Count; i++)
            {
                var shift = workShifts[i];
                var row = worksheet.CreateRow(i + 1);

                row.CreateCell(0).SetCellValue(shift.Date.ToString("yyyy-MM-dd"));
                row.CreateCell(1).SetCellValue(shift.StartTime.ToString("HH:mm"));
                row.CreateCell(2).SetCellValue(shift.EndTime.ToString("HH:mm"));
                row.CreateCell(3).SetCellValue(shift.IsNightShift ? "야간" : "주간");

                var hoursCell = row.CreateCell(4);
                hoursCell.SetCellValue(shift.WorkingHours);
                hoursCell.CellStyle = decimalStyle;

                row.CreateCell(5).SetCellValue(shift.ShiftType ?? "");
                row.CreateCell(6).SetCellValue(shift.Notes ?? "");
            }

            // 컬럼 너비 자동 조정
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.AutoSizeColumn(i);
            }
        }

        /// <summary>
        /// 일별마감 시트 생성
        /// </summary>
        private void CreateSettlementsSheet(IWorkbook workbook, List<DailySettlement> settlements)
        {
            var worksheet = workbook.CreateSheet("일별마감");

            // 헤더
            var headerRow = worksheet.CreateRow(0);
            var headers = new string[] { "마감일", "총매출", "총근무시간", "시간당평균매출", "메모" };
            var headerStyle = CreateHeaderStyle(workbook, IndexedColors.LightBlue);

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = headerRow.CreateCell(i);
                cell.SetCellValue(headers[i]);
                cell.CellStyle = headerStyle;
            }

            var numberStyle = CreateNumberStyle(workbook, "#,##0");
            var decimalStyle = CreateNumberStyle(workbook, "0.0");

            // 데이터
            for (int i = 0; i < settlements.Count; i++)
            {
                var settlement = settlements[i];
                var row = worksheet.CreateRow(i + 1);

                row.CreateCell(0).SetCellValue(settlement.Date.ToString("yyyy-MM-dd"));

                var revenueCell = row.CreateCell(1);
                revenueCell.SetCellValue((double)settlement.TotalRevenue);
                revenueCell.CellStyle = numberStyle;

                var hoursCell = row.CreateCell(2);
                hoursCell.SetCellValue(settlement.TotalWorkingHours);
                hoursCell.CellStyle = decimalStyle;

                var avgCell = row.CreateCell(3);
                avgCell.SetCellValue((double)settlement.AverageRevenuePerHour);
                avgCell.CellStyle = numberStyle;

                row.CreateCell(4).SetCellValue(settlement.Notes ?? "");
            }

            // 컬럼 너비 자동 조정
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.AutoSizeColumn(i);
            }
        }

        /// <summary>
        /// 시간대별 효율성 시트 생성
        /// </summary>
        private void CreateEfficiencySheet(IWorkbook workbook, TaxiOperationStats stats)
        {
            var worksheet = workbook.CreateSheet("시간대별효율성");

            // 헤더
            var headerRow = worksheet.CreateRow(0);
            headerRow.CreateCell(0).SetCellValue("시작시간");
            headerRow.CreateCell(1).SetCellValue("시간당평균매출");

            var headerStyle = CreateHeaderStyle(workbook, IndexedColors.LightGreen);
            headerRow.GetCell(0).CellStyle = headerStyle;
            headerRow.GetCell(1).CellStyle = headerStyle;

            var numberStyle = CreateNumberStyle(workbook, "#,##0");
            var highlightStyle = workbook.CreateCellStyle();
            highlightStyle.FillForegroundColor = IndexedColors.Yellow.Index;
            highlightStyle.FillPattern = FillPattern.SolidForeground;

            // 데이터 (효율성 순으로 정렬)
            var sortedEfficiency = stats.HourlyEfficiency.OrderByDescending(x => x.Value).ToList();
            for (int i = 0; i < sortedEfficiency.Count; i++)
            {
                var row = worksheet.CreateRow(i + 1);
                var timeCell = row.CreateCell(0);
                var revenueCell = row.CreateCell(1);

                timeCell.SetCellValue(sortedEfficiency[i].Key);
                revenueCell.SetCellValue((double)sortedEfficiency[i].Value);
                revenueCell.CellStyle = numberStyle;

                // 가장 효율적인 시간대 강조
                if (i == 0)
                {
                    timeCell.CellStyle = highlightStyle;
                    var highlightNumberStyle = workbook.CreateCellStyle();
                    highlightNumberStyle.CloneStyleFrom(numberStyle);
                    highlightNumberStyle.FillForegroundColor = IndexedColors.Yellow.Index;
                    highlightNumberStyle.FillPattern = FillPattern.SolidForeground;
                    revenueCell.CellStyle = highlightNumberStyle;
                }
            }

            worksheet.AutoSizeColumn(0);
            worksheet.AutoSizeColumn(1);
        }

        /// <summary>
        /// 편집 가능한 근무시간 시트 생성
        /// </summary>
        private void CreateEditableWorkShiftsSheet(IWorkbook workbook, List<TaxiWorkShift> workShifts)
        {
            var worksheet = workbook.CreateSheet("근무시간데이터");

            // 헤더
            var headerRow = worksheet.CreateRow(0);
            var headers = new string[] { "날짜", "시작시간", "종료시간", "야간근무", "근무시간", "근무타입", "메모" };

            var headerStyle = workbook.CreateCellStyle();
            headerStyle.FillForegroundColor = IndexedColors.DarkBlue.Index;
            headerStyle.FillPattern = FillPattern.SolidForeground;
            var headerFont = workbook.CreateFont();
            headerFont.IsBold = true;
            headerFont.Color = IndexedColors.White.Index;
            headerStyle.SetFont(headerFont);

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = headerRow.CreateCell(i);
                cell.SetCellValue(headers[i]);
                cell.CellStyle = headerStyle;
            }

            var decimalStyle = CreateNumberStyle(workbook, "0.0");

            // 데이터
            for (int i = 0; i < workShifts.Count; i++)
            {
                var shift = workShifts[i];
                var row = worksheet.CreateRow(i + 1);

                row.CreateCell(0).SetCellValue(shift.Date.ToString("yyyy-MM-dd"));
                row.CreateCell(1).SetCellValue(shift.StartTime.ToString("HH:mm"));
                row.CreateCell(2).SetCellValue(shift.EndTime.ToString("HH:mm"));
                row.CreateCell(3).SetCellValue(shift.IsNightShift ? "야간" : "주간");

                var hoursCell = row.CreateCell(4);
                hoursCell.SetCellValue(shift.WorkingHours);
                hoursCell.CellStyle = decimalStyle;

                row.CreateCell(5).SetCellValue(shift.ShiftType ?? "");
                row.CreateCell(6).SetCellValue(shift.Notes ?? "");
            }

            // 컬럼 너비 자동 조정
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.AutoSizeColumn(i);
            }
        }

        /// <summary>
        /// 편집 가능한 일별마감 시트 생성
        /// </summary>
        private void CreateEditableSettlementsSheet(IWorkbook workbook, List<DailySettlement> settlements)
        {
            var worksheet = workbook.CreateSheet("일별마감데이터");

            // 헤더
            var headerRow = worksheet.CreateRow(0);
            var headers = new string[] { "마감일", "총매출", "총근무시간", "시간당평균매출", "메모" };

            var headerStyle = workbook.CreateCellStyle();
            headerStyle.FillForegroundColor = IndexedColors.DarkGreen.Index;
            headerStyle.FillPattern = FillPattern.SolidForeground;
            var headerFont = workbook.CreateFont();
            headerFont.IsBold = true;
            headerFont.Color = IndexedColors.White.Index;
            headerStyle.SetFont(headerFont);

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = headerRow.CreateCell(i);
                cell.SetCellValue(headers[i]);
                cell.CellStyle = headerStyle;
            }

            var numberStyle = CreateNumberStyle(workbook, "#,##0");
            var decimalStyle = CreateNumberStyle(workbook, "0.0");

            // 데이터
            for (int i = 0; i < settlements.Count; i++)
            {
                var settlement = settlements[i];
                var row = worksheet.CreateRow(i + 1);

                row.CreateCell(0).SetCellValue(settlement.Date.ToString("yyyy-MM-dd"));

                var revenueCell = row.CreateCell(1);
                revenueCell.SetCellValue((double)settlement.TotalRevenue);
                revenueCell.CellStyle = numberStyle;

                var hoursCell = row.CreateCell(2);
                hoursCell.SetCellValue(settlement.TotalWorkingHours);
                hoursCell.CellStyle = decimalStyle;

                // 시간당 평균매출 수식으로 계산
                var avgCell = row.CreateCell(3);
                avgCell.SetCellFormula($"B{i+2}/C{i+2}");
                avgCell.CellStyle = numberStyle;

                row.CreateCell(4).SetCellValue(settlement.Notes ?? "");
            }

            // 컬럼 너비 자동 조정
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.AutoSizeColumn(i);
            }
        }

        /// <summary>
        /// 편집 가능한 시간대별 효율성 시트 생성
        /// </summary>
        private void CreateEditableEfficiencySheet(IWorkbook workbook, TaxiOperationStats stats)
        {
            var worksheet = workbook.CreateSheet("시간대별효율성");

            // 헤더
            var headerRow = worksheet.CreateRow(0);
            headerRow.CreateCell(0).SetCellValue("시작시간");
            headerRow.CreateCell(1).SetCellValue("시간당평균매출");
            headerRow.CreateCell(2).SetCellValue("효율성순위");

            var headerStyle = workbook.CreateCellStyle();
            headerStyle.FillForegroundColor = IndexedColors.Violet.Index;
            headerStyle.FillPattern = FillPattern.SolidForeground;
            var headerFont = workbook.CreateFont();
            headerFont.IsBold = true;
            headerFont.Color = IndexedColors.White.Index;
            headerStyle.SetFont(headerFont);

            for (int i = 0; i < 3; i++)
            {
                headerRow.GetCell(i).CellStyle = headerStyle;
            }

            var numberStyle = CreateNumberStyle(workbook, "#,##0");
            var goldStyle = workbook.CreateCellStyle();
            goldStyle.FillForegroundColor = IndexedColors.Gold.Index;
            goldStyle.FillPattern = FillPattern.SolidForeground;
            var goldFont = workbook.CreateFont();
            goldFont.IsBold = true;
            goldStyle.SetFont(goldFont);

            // 데이터 (효율성 순으로 정렬)
            var sortedEfficiency = stats.HourlyEfficiency.OrderByDescending(x => x.Value).ToList();
            for (int i = 0; i < sortedEfficiency.Count; i++)
            {
                var row = worksheet.CreateRow(i + 1);
                var timeCell = row.CreateCell(0);
                var revenueCell = row.CreateCell(1);
                var rankCell = row.CreateCell(2);

                timeCell.SetCellValue(sortedEfficiency[i].Key);
                revenueCell.SetCellValue((double)sortedEfficiency[i].Value);
                revenueCell.CellStyle = numberStyle;
                rankCell.SetCellValue(i + 1); // 순위

                // 1위 강조
                if (i == 0)
                {
                    timeCell.CellStyle = goldStyle;
                    var goldNumberStyle = workbook.CreateCellStyle();
                    goldNumberStyle.CloneStyleFrom(numberStyle);
                    goldNumberStyle.FillForegroundColor = IndexedColors.Gold.Index;
                    goldNumberStyle.FillPattern = FillPattern.SolidForeground;
                    goldNumberStyle.SetFont(goldFont);
                    revenueCell.CellStyle = goldNumberStyle;
                    rankCell.CellStyle = goldStyle;
                }
            }

            // 컬럼 너비 자동 조정
            for (int i = 0; i < 3; i++)
            {
                worksheet.AutoSizeColumn(i);
            }
        }

        /// <summary>
        /// 통계 대시보드 시트 생성 (수식 기반)
        /// </summary>
        private void CreateStatsDashboardSheet(IWorkbook workbook, DateTime startDate, DateTime endDate)
        {
            var worksheet = workbook.CreateSheet("통계대시보드");

            // 제목
            var titleRow = worksheet.CreateRow(0);
            var titleCell = titleRow.CreateCell(0);
            titleCell.SetCellValue("택시 운행 통계 대시보드");

            var titleStyle = workbook.CreateCellStyle();
            var titleFont = workbook.CreateFont();
            titleFont.FontHeightInPoints = 18;
            titleFont.IsBold = true;
            titleStyle.SetFont(titleFont);
            titleCell.CellStyle = titleStyle;

            // 제목 셀 병합
            worksheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 3));

            // 기간
            var periodRow = worksheet.CreateRow(2);
            var periodLabelCell = periodRow.CreateCell(0);
            periodLabelCell.SetCellValue("조회 기간:");

            var boldStyle = workbook.CreateCellStyle();
            var boldFont = workbook.CreateFont();
            boldFont.IsBold = true;
            boldStyle.SetFont(boldFont);
            periodLabelCell.CellStyle = boldStyle;

            periodRow.CreateCell(1).SetCellValue($"{startDate:yyyy-MM-dd} ~ {endDate:yyyy-MM-dd}");

            // 통계 항목들
            var statsLabels = new string[]
            {
                "총 근무 일수", "총 매출", "총 근무시간", "평균 시간당 매출",
                "최고 일 매출", "최저 일 매출", "평균 일 매출"
            };

            var numberStyle = CreateNumberStyle(workbook, "#,##0");
            var decimalStyle = CreateNumberStyle(workbook, "0.0");

            for (int i = 0; i < statsLabels.Length; i++)
            {
                var row = worksheet.CreateRow(i + 5);
                var labelCell = row.CreateCell(0);
                labelCell.SetCellValue(statsLabels[i]);
                labelCell.CellStyle = boldStyle;

                var valueCell = row.CreateCell(1);

                // 간단한 수식 예시 (실제 데이터 테이블과 연결하려면 더 복잡한 수식 필요)
                switch (i)
                {
                    case 0: // 총 근무 일수
                        valueCell.SetCellValue("=COUNTA(일별마감데이터.A:A)-1");
                        break;
                    case 1: // 총 매출
                        valueCell.SetCellValue("=SUM(일별마감데이터.B:B)");
                        valueCell.CellStyle = numberStyle;
                        break;
                    case 2: // 총 근무시간
                        valueCell.SetCellValue("=SUM(일별마감데이터.C:C)");
                        valueCell.CellStyle = decimalStyle;
                        break;
                    case 3: // 평균 시간당 매출
                        valueCell.SetCellValue("=B8/B9");
                        valueCell.CellStyle = numberStyle;
                        break;
                    case 4: // 최고 일 매출
                        valueCell.SetCellValue("=MAX(일별마감데이터.B:B)");
                        valueCell.CellStyle = numberStyle;
                        break;
                    case 5: // 최저 일 매출
                        valueCell.SetCellValue("=MIN(일별마감데이터.B:B)");
                        valueCell.CellStyle = numberStyle;
                        break;
                    case 6: // 평균 일 매출
                        valueCell.SetCellValue("=AVERAGE(일별마감데이터.B:B)");
                        valueCell.CellStyle = numberStyle;
                        break;
                }
            }

            // 월별 통계 섹션
            var monthlyTitleRow = worksheet.CreateRow(14);
            var monthlyTitleCell = monthlyTitleRow.CreateCell(0);
            monthlyTitleCell.SetCellValue("월별 매출 분석");

            var subtitleStyle = workbook.CreateCellStyle();
            var subtitleFont = workbook.CreateFont();
            subtitleFont.FontHeightInPoints = 14;
            subtitleFont.IsBold = true;
            subtitleStyle.SetFont(subtitleFont);
            monthlyTitleCell.CellStyle = subtitleStyle;

            // 월별 헤더
            var monthlyHeaderRow = worksheet.CreateRow(16);
            var monthlyHeaders = new string[] { "월", "매출", "근무시간", "평균시간당매출" };
            var monthlyHeaderStyle = CreateHeaderStyle(workbook, IndexedColors.LightBlue);

            for (int i = 0; i < monthlyHeaders.Length; i++)
            {
                var cell = monthlyHeaderRow.CreateCell(i);
                cell.SetCellValue(monthlyHeaders[i]);
                cell.CellStyle = monthlyHeaderStyle;
            }

            // 컬럼 너비 자동 조정
            for (int i = 0; i < 4; i++)
            {
                worksheet.AutoSizeColumn(i);
            }
        }

        /// <summary>
        /// 차트 시트 생성
        /// </summary>
        private void CreateChartsSheet(IWorkbook workbook)
        {
            var worksheet = workbook.CreateSheet("차트분석");

            var titleRow = worksheet.CreateRow(0);
            var titleCell = titleRow.CreateCell(0);
            titleCell.SetCellValue("시각적 분석 차트");

            var titleStyle = workbook.CreateCellStyle();
            var titleFont = workbook.CreateFont();
            titleFont.FontHeightInPoints = 16;
            titleFont.IsBold = true;
            titleStyle.SetFont(titleFont);
            titleCell.CellStyle = titleStyle;

            // 차트 생성 안내
            var guideRow = worksheet.CreateRow(2);
            var guideCell = guideRow.CreateCell(0);
            guideCell.SetCellValue("차트 생성 가이드:");

            var boldStyle = workbook.CreateCellStyle();
            var boldFont = workbook.CreateFont();
            boldFont.IsBold = true;
            boldStyle.SetFont(boldFont);
            guideCell.CellStyle = boldStyle;

            var instructions = new string[]
            {
                "1. 일별매출 추이: 일별마감데이터 시트에서 날짜와 총매출로 선 차트 생성",
                "2. 시간대별 효율성: 시간대별효율성 시트에서 막대 차트 생성",
                "3. 월별 매출 비교: 통계대시보드의 월별 데이터로 파이 차트 생성",
                "4. 근무시간 분포: 근무시간데이터에서 히스토그램 생성"
            };

            for (int i = 0; i < instructions.Length; i++)
            {
                var row = worksheet.CreateRow(i + 4);
                row.CreateCell(0).SetCellValue(instructions[i]);
            }

            worksheet.AutoSizeColumn(0);
        }

        /// <summary>
        /// 월별 요약 시트 생성
        /// </summary>
        private void CreateMonthlySummarySheet(IWorkbook workbook)
        {
            var worksheet = workbook.CreateSheet("월별요약");

            var titleRow = worksheet.CreateRow(0);
            var titleCell = titleRow.CreateCell(0);
            titleCell.SetCellValue("월별 운행 요약");

            var titleStyle = workbook.CreateCellStyle();
            var titleFont = workbook.CreateFont();
            titleFont.FontHeightInPoints = 16;
            titleFont.IsBold = true;
            titleStyle.SetFont(titleFont);
            titleCell.CellStyle = titleStyle;

            // 헤더
            var headerRow = worksheet.CreateRow(2);
            var headers = new string[] { "연월", "근무일수", "총매출", "총근무시간", "평균시간당매출", "최고일매출", "목표달성률" };

            var headerStyle = workbook.CreateCellStyle();
            headerStyle.FillForegroundColor = IndexedColors.DarkBlue.Index;
            headerStyle.FillPattern = FillPattern.SolidForeground;
            var headerFont = workbook.CreateFont();
            headerFont.IsBold = true;
            headerFont.Color = IndexedColors.White.Index;
            headerStyle.SetFont(headerFont);

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = headerRow.CreateCell(i);
                cell.SetCellValue(headers[i]);
                cell.CellStyle = headerStyle;
            }

            // 사용법 안내
            var instructionRow1 = worksheet.CreateRow(4);
            instructionRow1.CreateCell(0).SetCellValue("사용법: 피벗 테이블을 사용하여 일별마감데이터에서 월별 요약을 생성하세요.");

            var instructionRow2 = worksheet.CreateRow(5);
            instructionRow2.CreateCell(0).SetCellValue("삽입 > 피벗테이블 > 일별마감데이터 선택 > 행: 월, 값: 총매출(합계), 총근무시간(합계)");

            // 컬럼 너비 자동 조정
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.AutoSizeColumn(i);
            }
        }
    }
}