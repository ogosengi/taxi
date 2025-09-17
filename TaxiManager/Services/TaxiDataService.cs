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

            // 근무시간 테이블 생성 (존재하지 않을 경우에만)
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS WorkShifts (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Date TEXT NOT NULL,
                    StartTime TEXT NOT NULL,
                    EndTime TEXT NOT NULL,
                    IsNightShift INTEGER NOT NULL,
                    Revenue REAL NOT NULL,
                    Notes TEXT
                )";
            command.ExecuteNonQuery();

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
                    Revenue = reader.GetDecimal(5),
                    Notes = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
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
                INSERT INTO WorkShifts (Date, StartTime, EndTime, IsNightShift, Revenue, Notes)
                VALUES (@Date, @StartTime, @EndTime, @IsNightShift, @Revenue, @Notes)";

            command.Parameters.AddWithValue("@Date", workShift.Date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@StartTime", workShift.StartTime.ToString("HH:mm"));
            command.Parameters.AddWithValue("@EndTime", workShift.EndTime.ToString("HH:mm"));
            command.Parameters.AddWithValue("@IsNightShift", workShift.IsNightShift);
            command.Parameters.AddWithValue("@Revenue", workShift.Revenue);
            command.Parameters.AddWithValue("@Notes", workShift.Notes ?? string.Empty);

            // 디버그: 추가하려는 데이터 출력
            System.Diagnostics.Debug.WriteLine($"Adding WorkShift: {workShift.Date:yyyy-MM-dd} {workShift.StartTime}-{workShift.EndTime}, Revenue: {workShift.Revenue}");

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
                    IsNightShift = @IsNightShift, Revenue = @Revenue, Notes = @Notes
                WHERE Id = @Id";

            command.Parameters.AddWithValue("@Id", workShift.Id);
            command.Parameters.AddWithValue("@Date", workShift.Date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@StartTime", workShift.StartTime.ToString("HH:mm"));
            command.Parameters.AddWithValue("@EndTime", workShift.EndTime.ToString("HH:mm"));
            command.Parameters.AddWithValue("@IsNightShift", workShift.IsNightShift);
            command.Parameters.AddWithValue("@Revenue", workShift.Revenue);
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
        /// 특정 날짜의 총 매출을 계산 (마감 여부와 관계없이)
        /// </summary>
        public decimal GetDailyRevenue(DateTime date)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT SUM(Revenue) FROM WorkShifts
                WHERE Date = @Date";
            command.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));

            var result = command.ExecuteScalar();
            return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
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

            return new TaxiOperationStats
            {
                TotalWorkDays = settlements.Count,
                TotalRevenue = settlements.Sum(x => x.TotalRevenue),
                TotalWorkingHours = settlements.Sum(x => x.TotalWorkingHours),
                AverageRevenuePerHour = settlements.Sum(x => x.TotalWorkingHours) > 0
                    ? settlements.Sum(x => x.TotalRevenue) / (decimal)settlements.Sum(x => x.TotalWorkingHours)
                    : 0
            };
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

            var totalRevenue = dailyShifts.Sum(x => x.Revenue);
            var totalWorkingHours = dailyShifts.Sum(x => x.WorkingHours);

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT OR REPLACE INTO DailySettlements (Date, TotalRevenue, TotalWorkingHours, Notes)
                VALUES (@Date, @TotalRevenue, @TotalWorkingHours, @Notes)";

            command.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@TotalRevenue", totalRevenue);
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