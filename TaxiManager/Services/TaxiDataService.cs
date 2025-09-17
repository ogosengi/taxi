using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;
using TaxiManager.Models;

namespace TaxiManager.Services
{
    /// <summary>
    /// 택시 운행 데이터를 관리하는 서비스 클래스 (SQLite 사용)
    /// </summary>
    public class TaxiDataService
    {
        private readonly string _connectionString;

        public TaxiDataService()
        {
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "taxidata.db");
            _connectionString = $"Data Source={dbPath}";

            // 디버그: 데이터베이스 경로 출력
            System.Diagnostics.Debug.WriteLine($"Database path: {dbPath}");

            InitializeDatabase();
        }

        /// <summary>
        /// 데이터베이스 초기화 (테이블이 없을 때만 생성)
        /// </summary>
        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();

            // WorkShifts 테이블 확인 및 Revenue 컬럼 제거 마이그레이션
            command.CommandText = "PRAGMA table_info(WorkShifts)";
            var workShiftsReader = command.ExecuteReader();
            var workShiftsColumns = new List<string>();
            while (workShiftsReader.Read())
            {
                workShiftsColumns.Add(workShiftsReader.GetString(1)); // name 컬럼은 인덱스 1
            }
            workShiftsReader.Close();

            if (workShiftsColumns.Contains("Revenue"))
            {
                // Revenue 컬럼이 있으면 마이그레이션 수행
                command.CommandText = @"
                    CREATE TABLE WorkShifts_new (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date TEXT NOT NULL,
                        StartTime TEXT NOT NULL,
                        EndTime TEXT NOT NULL,
                        IsNightShift INTEGER NOT NULL,
                        Notes TEXT
                    )";
                command.ExecuteNonQuery();

                // 기존 데이터 복사 (Revenue 제외)
                command.CommandText = @"
                    INSERT INTO WorkShifts_new (Id, Date, StartTime, EndTime, IsNightShift, Notes)
                    SELECT Id, Date, StartTime, EndTime, IsNightShift, Notes FROM WorkShifts";
                command.ExecuteNonQuery();

                // 기존 테이블 삭제하고 새 테이블로 이름 변경
                command.CommandText = "DROP TABLE WorkShifts";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE WorkShifts_new RENAME TO WorkShifts";
                command.ExecuteNonQuery();
            }
            else
            {
                // 새 테이블 생성 (Revenue 컬럼 없이)
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS WorkShifts (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date TEXT NOT NULL,
                        StartTime TEXT NOT NULL,
                        EndTime TEXT NOT NULL,
                        IsNightShift INTEGER NOT NULL,
                        Notes TEXT
                    )";
                command.ExecuteNonQuery();
            }

            // 기존 DailySettlements 테이블 확인 및 마이그레이션
            command.CommandText = "PRAGMA table_info(DailySettlements)";
            var reader = command.ExecuteReader();
            var columns = new List<string>();
            while (reader.Read())
            {
                columns.Add(reader.GetString(1)); // name 컬럼은 인덱스 1
            }
            reader.Close();

            if (columns.Contains("SettlementDateTime"))
            {
                // 기존 테이블에 SettlementDateTime이 있으면 마이그레이션 수행
                command.CommandText = @"
                    CREATE TABLE DailySettlements_new (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date TEXT NOT NULL UNIQUE,
                        TotalRevenue REAL NOT NULL,
                        TotalWorkingHours REAL NOT NULL,
                        Notes TEXT
                    )";
                command.ExecuteNonQuery();

                // 기존 데이터 복사 (SettlementDateTime 제외)
                command.CommandText = @"
                    INSERT INTO DailySettlements_new (Id, Date, TotalRevenue, TotalWorkingHours, Notes)
                    SELECT Id, Date, TotalRevenue, TotalWorkingHours, Notes FROM DailySettlements";
                command.ExecuteNonQuery();

                // 기존 테이블 삭제하고 새 테이블로 이름 변경
                command.CommandText = "DROP TABLE DailySettlements";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE DailySettlements_new RENAME TO DailySettlements";
                command.ExecuteNonQuery();
            }
            else
            {
                // 새 테이블 생성
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS DailySettlements (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date TEXT NOT NULL UNIQUE,
                        TotalRevenue REAL NOT NULL,
                        TotalWorkingHours REAL NOT NULL,
                        Notes TEXT
                    )";
                command.ExecuteNonQuery();
            }

            // 디버그: 테이블 존재 확인
            command.CommandText = "SELECT COUNT(*) FROM WorkShifts";
            var workShiftCount = command.ExecuteScalar();
            System.Diagnostics.Debug.WriteLine($"WorkShifts table record count: {workShiftCount}");

            command.CommandText = "SELECT COUNT(*) FROM DailySettlements";
            var settlementCount = command.ExecuteScalar();
            System.Diagnostics.Debug.WriteLine($"DailySettlements table record count: {settlementCount}");
        }

        /// <summary>
        /// 모든 근무 시간 정보를 반환
        /// </summary>
        public List<TaxiWorkShift> GetAllWorkShifts()
        {
            var workShifts = new List<TaxiWorkShift>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM WorkShifts ORDER BY Date DESC";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                workShifts.Add(new TaxiWorkShift
                {
                    Id = reader.GetInt32(0),
                    Date = DateTime.Parse(reader.GetString(1)),
                    StartTime = TimeOnly.Parse(reader.GetString(2)),
                    EndTime = TimeOnly.Parse(reader.GetString(3)),
                    IsNightShift = reader.GetInt32(4) == 1,
                    Notes = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
                });
            }

            return workShifts;
        }

        /// <summary>
        /// 새로운 근무 시간 정보를 추가
        /// </summary>
        public void AddWorkShift(TaxiWorkShift workShift)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO WorkShifts (Date, StartTime, EndTime, IsNightShift, Notes)
                VALUES (@Date, @StartTime, @EndTime, @IsNightShift, @Notes)";

            command.Parameters.AddWithValue("@Date", workShift.Date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@StartTime", workShift.StartTime.ToString("HH:mm"));
            command.Parameters.AddWithValue("@EndTime", workShift.EndTime.ToString("HH:mm"));
            command.Parameters.AddWithValue("@IsNightShift", workShift.IsNightShift);
            command.Parameters.AddWithValue("@Notes", workShift.Notes ?? string.Empty);

            // 디버그: 추가하려는 데이터 출력
            System.Diagnostics.Debug.WriteLine($"Adding WorkShift: {workShift.Date:yyyy-MM-dd} {workShift.StartTime}-{workShift.EndTime}");

            var rowsAffected = command.ExecuteNonQuery();
            System.Diagnostics.Debug.WriteLine($"Rows affected: {rowsAffected}");
        }

        /// <summary>
        /// 근무 시간 정보를 업데이트
        /// </summary>
        public void UpdateWorkShift(TaxiWorkShift workShift)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE WorkShifts
                SET Date = @Date, StartTime = @StartTime, EndTime = @EndTime,
                    IsNightShift = @IsNightShift, Notes = @Notes
                WHERE Id = @Id";

            command.Parameters.AddWithValue("@Id", workShift.Id);
            command.Parameters.AddWithValue("@Date", workShift.Date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@StartTime", workShift.StartTime.ToString("HH:mm"));
            command.Parameters.AddWithValue("@EndTime", workShift.EndTime.ToString("HH:mm"));
            command.Parameters.AddWithValue("@IsNightShift", workShift.IsNightShift);
            command.Parameters.AddWithValue("@Notes", workShift.Notes ?? string.Empty);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 근무 시간 정보를 삭제
        /// </summary>
        public void DeleteWorkShift(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM WorkShifts WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }


        /// <summary>
        /// 특정 월의 총 매출을 계산 (마감된 날짜만)
        /// </summary>
        public decimal GetMonthlyRevenue(int year, int month)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT SUM(TotalRevenue) FROM DailySettlements
                WHERE strftime('%Y', Date) = @Year
                AND strftime('%m', Date) = @Month";
            command.Parameters.AddWithValue("@Year", year.ToString());
            command.Parameters.AddWithValue("@Month", month.ToString("D2"));

            var result = command.ExecuteScalar();
            return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
        }

        /// <summary>
        /// 특정 기간의 총 매출을 계산 (마감된 날짜만)
        /// </summary>
        public decimal GetPeriodRevenue(DateTime startDate, DateTime endDate)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT SUM(TotalRevenue) FROM DailySettlements
                WHERE Date >= @StartDate AND Date <= @EndDate";
            command.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyy-MM-dd"));

            var result = command.ExecuteScalar();
            return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
        }

        /// <summary>
        /// 특정 기간의 총 근무시간을 계산 (마감된 날짜만)
        /// </summary>
        public double GetPeriodWorkingHours(DateTime startDate, DateTime endDate)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT SUM(TotalWorkingHours) FROM DailySettlements
                WHERE Date >= @StartDate AND Date <= @EndDate";
            command.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyy-MM-dd"));

            var result = command.ExecuteScalar();
            return result == DBNull.Value ? 0 : Convert.ToDouble(result);
        }

        /// <summary>
        /// 운행 현황 통계 조회 (마감된 날짜 기준)
        /// </summary>
        public TaxiOperationStats GetOperationStats(DateTime startDate, DateTime endDate)
        {
            var settlements = GetDailySettlements(startDate, endDate);
            var hourlyEfficiency = CalculateHourlyEfficiency(startDate, endDate);

            var mostEfficientTime = hourlyEfficiency.Count > 0
                ? hourlyEfficiency.OrderByDescending(x => x.Value).First()
                : new KeyValuePair<string, decimal>("정보없음", 0);

            var stats = new TaxiOperationStats
            {
                TotalWorkDays = settlements.Count,
                TotalRevenue = settlements.Sum(x => x.TotalRevenue),
                TotalWorkingHours = settlements.Sum(x => x.TotalWorkingHours),
                AverageRevenuePerHour = settlements.Sum(x => x.TotalWorkingHours) > 0
                    ? settlements.Sum(x => x.TotalRevenue) / (decimal)settlements.Sum(x => x.TotalWorkingHours)
                    : 0,
                MostEfficientStartTime = mostEfficientTime.Key,
                MostEfficientHourlyRevenue = mostEfficientTime.Value,
                HourlyEfficiency = hourlyEfficiency
            };

            return stats;
        }

        /// <summary>
        /// 시간대별 효율성 계산 (시작 시간 기준)
        /// 계산식: 각 시간대별로 (총 매출 / 총 근무시간) 계산
        /// </summary>
        private Dictionary<string, decimal> CalculateHourlyEfficiency(DateTime startDate, DateTime endDate)
        {
            var hourlyData = new Dictionary<string, (decimal totalRevenue, double totalHours)>();

            // 해당 기간의 모든 마감자료와 근무시간 가져오기
            var settlements = GetDailySettlements(startDate, endDate);
            var allWorkShifts = GetAllWorkShifts()
                .Where(ws => ws.Date >= startDate.Date && ws.Date <= endDate.Date)
                .ToList();

            // 각 근무시간에 대해 해당 날짜의 마감 매출을 비례 배분
            foreach (var workShift in allWorkShifts)
            {
                var settlement = settlements.FirstOrDefault(s => s.Date.Date == workShift.Date.Date);
                if (settlement == null || settlement.TotalWorkingHours == 0) continue;

                // 이 근무시간의 매출 비율 계산 (해당 날짜 총 근무시간 대비)
                var dailyTotalHours = allWorkShifts
                    .Where(ws => ws.Date.Date == workShift.Date.Date)
                    .Sum(ws => ws.WorkingHours);

                if (dailyTotalHours == 0) continue;

                var revenueRatio = workShift.WorkingHours / dailyTotalHours;
                var allocatedRevenue = settlement.TotalRevenue * (decimal)revenueRatio;

                // 시작 시간대별로 집계
                var startTimeKey = $"{workShift.StartTime:HH}시";

                if (!hourlyData.ContainsKey(startTimeKey))
                {
                    hourlyData[startTimeKey] = (0, 0);
                }

                hourlyData[startTimeKey] = (
                    hourlyData[startTimeKey].totalRevenue + allocatedRevenue,
                    hourlyData[startTimeKey].totalHours + workShift.WorkingHours
                );
            }

            // 시간당 평균 매출 계산
            var efficiency = new Dictionary<string, decimal>();
            foreach (var item in hourlyData)
            {
                if (item.Value.totalHours > 0)
                {
                    efficiency[item.Key] = item.Value.totalRevenue / (decimal)item.Value.totalHours;
                }
            }

            return efficiency.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// 일별 마감 처리
        /// </summary>
        public void CreateDailySettlement(DateTime date, string notes = "")
        {
            // 해당 날짜의 모든 근무시간 조회
            var dailyShifts = GetAllWorkShifts()
                .Where(x => x.Date.Date == date.Date)
                .ToList();

            if (!dailyShifts.Any())
            {
                throw new InvalidOperationException("해당 날짜에 근무 기록이 없습니다.");
            }

            var totalWorkingHours = dailyShifts.Sum(x => x.WorkingHours);

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT OR REPLACE INTO DailySettlements (Date, TotalRevenue, TotalWorkingHours, Notes)
                VALUES (@Date, @TotalRevenue, @TotalWorkingHours, @Notes)";

            command.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@TotalRevenue", 0); // 매출은 별도로 입력받음
            command.Parameters.AddWithValue("@TotalWorkingHours", totalWorkingHours);
            command.Parameters.AddWithValue("@Notes", notes ?? string.Empty);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 매출과 함께 일별 마감 처리
        /// </summary>
        public void CreateDailySettlementWithRevenue(DateTime date, decimal revenue, string notes = "")
        {
            // 해당 날짜의 모든 근무시간 조회
            var dailyShifts = GetAllWorkShifts()
                .Where(x => x.Date.Date == date.Date)
                .ToList();

            if (!dailyShifts.Any())
            {
                throw new InvalidOperationException("해당 날짜에 근무 기록이 없습니다.");
            }

            var totalWorkingHours = dailyShifts.Sum(x => x.WorkingHours);

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT OR REPLACE INTO DailySettlements (Date, TotalRevenue, TotalWorkingHours, Notes)
                VALUES (@Date, @TotalRevenue, @TotalWorkingHours, @Notes)";

            command.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@TotalRevenue", revenue);
            command.Parameters.AddWithValue("@TotalWorkingHours", totalWorkingHours);
            command.Parameters.AddWithValue("@Notes", notes ?? string.Empty);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 특정 날짜의 마감 정보 조회
        /// </summary>
        public DailySettlement? GetDailySettlement(DateTime date)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM DailySettlements WHERE Date = @Date";
            command.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new DailySettlement
                {
                    Id = reader.GetInt32(0),
                    Date = DateTime.Parse(reader.GetString(1)),
                    TotalRevenue = reader.GetDecimal(2),
                    TotalWorkingHours = reader.GetDouble(3),
                    Notes = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                };
            }

            return null;
        }

        /// <summary>
        /// 기간별 일별마감 목록 조회
        /// </summary>
        public List<DailySettlement> GetDailySettlements(DateTime startDate, DateTime endDate)
        {
            var settlements = new List<DailySettlement>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT * FROM DailySettlements
                WHERE Date >= @StartDate AND Date <= @EndDate
                ORDER BY Date DESC";
            command.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyy-MM-dd"));

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                settlements.Add(new DailySettlement
                {
                    Id = reader.GetInt32(0),
                    Date = DateTime.Parse(reader.GetString(1)),
                    TotalRevenue = reader.GetDecimal(2),
                    TotalWorkingHours = reader.GetDouble(3),
                    Notes = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                });
            }

            return settlements;
        }

        /// <summary>
        /// 특정 날짜가 마감되었는지 확인
        /// </summary>
        public bool IsDateSettled(DateTime date)
        {
            return GetDailySettlement(date) != null;
        }

        /// <summary>
        /// 모든 일별 마감 정보를 반환
        /// </summary>
        public List<DailySettlement> GetAllDailySettlements()
        {
            var settlements = new List<DailySettlement>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM DailySettlements ORDER BY Date ASC";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                settlements.Add(new DailySettlement
                {
                    Id = reader.GetInt32(0),
                    Date = DateTime.Parse(reader.GetString(1)),
                    TotalRevenue = reader.GetDecimal(2),
                    TotalWorkingHours = reader.GetDouble(3),
                    Notes = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                });
            }

            return settlements;
        }

        /// <summary>
        /// 일별 마감 정보를 업데이트
        /// </summary>
        public void UpdateDailySettlement(DailySettlement settlement)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE DailySettlements
                SET TotalRevenue = @TotalRevenue, TotalWorkingHours = @TotalWorkingHours,
                    Notes = @Notes
                WHERE Id = @Id";

            command.Parameters.AddWithValue("@Id", settlement.Id);
            command.Parameters.AddWithValue("@TotalRevenue", settlement.TotalRevenue);
            command.Parameters.AddWithValue("@TotalWorkingHours", settlement.TotalWorkingHours);
            command.Parameters.AddWithValue("@Notes", settlement.Notes ?? string.Empty);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 일별 마감 정보를 삭제
        /// </summary>
        public void DeleteDailySettlement(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM DailySettlements WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }
    }
}