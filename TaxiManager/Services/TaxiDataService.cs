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
            InitializeDatabase();
        }

        /// <summary>
        /// 데이터베이스 초기화
        /// </summary>
        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS 근무시간 (
                    아이디 INTEGER PRIMARY KEY AUTOINCREMENT,
                    날짜 TEXT NOT NULL,
                    시작시간 TEXT NOT NULL,
                    종료시간 TEXT NOT NULL,
                    야간근무여부 INTEGER NOT NULL,
                    매출 REAL NOT NULL,
                    메모 TEXT,
                    마감여부 INTEGER NOT NULL
                )";
            command.ExecuteNonQuery();
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
            command.CommandText = "SELECT * FROM 근무시간 ORDER BY 날짜 DESC";

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
                    Notes = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    IsCompleted = reader.GetInt32(7) == 1
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
                INSERT INTO 근무시간 (날짜, 시작시간, 종료시간, 야간근무여부, 매출, 메모, 마감여부)
                VALUES (@날짜, @시작시간, @종료시간, @야간근무여부, @매출, @메모, @마감여부)";

            command.Parameters.AddWithValue("@날짜", workShift.Date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@시작시간", workShift.StartTime.ToString("HH:mm"));
            command.Parameters.AddWithValue("@종료시간", workShift.EndTime.ToString("HH:mm"));
            command.Parameters.AddWithValue("@야간근무여부", workShift.IsNightShift);
            command.Parameters.AddWithValue("@매출", workShift.Revenue);
            command.Parameters.AddWithValue("@메모", workShift.Notes ?? string.Empty);
            command.Parameters.AddWithValue("@마감여부", workShift.IsCompleted);

            command.ExecuteNonQuery();
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
                UPDATE 근무시간
                SET 날짜 = @날짜, 시작시간 = @시작시간, 종료시간 = @종료시간,
                    야간근무여부 = @야간근무여부, 매출 = @매출, 메모 = @메모, 마감여부 = @마감여부
                WHERE 아이디 = @아이디";

            command.Parameters.AddWithValue("@아이디", workShift.Id);
            command.Parameters.AddWithValue("@날짜", workShift.Date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@시작시간", workShift.StartTime.ToString("HH:mm"));
            command.Parameters.AddWithValue("@종료시간", workShift.EndTime.ToString("HH:mm"));
            command.Parameters.AddWithValue("@야간근무여부", workShift.IsNightShift);
            command.Parameters.AddWithValue("@매출", workShift.Revenue);
            command.Parameters.AddWithValue("@메모", workShift.Notes ?? string.Empty);
            command.Parameters.AddWithValue("@마감여부", workShift.IsCompleted);

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
            command.CommandText = "DELETE FROM 근무시간 WHERE 아이디 = @아이디";
            command.Parameters.AddWithValue("@아이디", id);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 특정 날짜의 총 매출을 계산
        /// </summary>
        public decimal GetDailyRevenue(DateTime date)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT SUM(매출) FROM 근무시간
                WHERE 날짜 = @날짜 AND 마감여부 = 1";
            command.Parameters.AddWithValue("@날짜", date.ToString("yyyy-MM-dd"));

            var result = command.ExecuteScalar();
            return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
        }

        /// <summary>
        /// 특정 월의 총 매출을 계산
        /// </summary>
        public decimal GetMonthlyRevenue(int year, int month)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT SUM(매출) FROM 근무시간
                WHERE strftime('%Y', 날짜) = @년도
                AND strftime('%m', 날짜) = @월
                AND 마감여부 = 1";
            command.Parameters.AddWithValue("@년도", year.ToString());
            command.Parameters.AddWithValue("@월", month.ToString("D2"));

            var result = command.ExecuteScalar();
            return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
        }

        /// <summary>
        /// 특정 기간의 총 매출을 계산
        /// </summary>
        public decimal GetPeriodRevenue(DateTime startDate, DateTime endDate)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT SUM(매출) FROM 근무시간
                WHERE 날짜 >= @시작날짜 AND 날짜 <= @종료날짜 AND 마감여부 = 1";
            command.Parameters.AddWithValue("@시작날짜", startDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@종료날짜", endDate.ToString("yyyy-MM-dd"));

            var result = command.ExecuteScalar();
            return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
        }

        /// <summary>
        /// 특정 기간의 총 근무시간을 계산
        /// </summary>
        public double GetPeriodWorkingHours(DateTime startDate, DateTime endDate)
        {
            var shifts = GetAllWorkShifts()
                .Where(x => x.Date.Date >= startDate.Date && x.Date.Date <= endDate.Date && x.IsCompleted)
                .ToList();

            return shifts.Sum(x => x.WorkingHours);
        }

        /// <summary>
        /// 운행 현황 통계 조회
        /// </summary>
        public TaxiOperationStats GetOperationStats(DateTime startDate, DateTime endDate)
        {
            var shifts = GetAllWorkShifts()
                .Where(x => x.Date.Date >= startDate.Date && x.Date.Date <= endDate.Date)
                .ToList();

            return new TaxiOperationStats
            {
                TotalShifts = shifts.Count,
                CompletedShifts = shifts.Count(x => x.IsCompleted),
                TotalRevenue = shifts.Where(x => x.IsCompleted).Sum(x => x.Revenue),
                TotalWorkingHours = shifts.Where(x => x.IsCompleted).Sum(x => x.WorkingHours),
                AverageRevenuePerHour = shifts.Where(x => x.IsCompleted && x.WorkingHours > 0)
                    .Select(x => x.Revenue / (decimal)x.WorkingHours)
                    .DefaultIfEmpty(0)
                    .Average()
            };
        }
    }
}