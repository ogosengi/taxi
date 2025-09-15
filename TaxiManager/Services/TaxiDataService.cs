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
                CREATE TABLE IF NOT EXISTS WorkShifts (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Date TEXT NOT NULL,
                    StartTime TEXT NOT NULL,
                    EndTime TEXT NOT NULL,
                    IsNightShift INTEGER NOT NULL,
                    Revenue REAL NOT NULL,
                    Notes TEXT,
                    IsCompleted INTEGER NOT NULL
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
                INSERT INTO WorkShifts (Date, StartTime, EndTime, IsNightShift, Revenue, Notes, IsCompleted)
                VALUES (@Date, @StartTime, @EndTime, @IsNightShift, @Revenue, @Notes, @IsCompleted)";

            command.Parameters.AddWithValue("@Date", workShift.Date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@StartTime", workShift.StartTime.ToString("HH:mm"));
            command.Parameters.AddWithValue("@EndTime", workShift.EndTime.ToString("HH:mm"));
            command.Parameters.AddWithValue("@IsNightShift", workShift.IsNightShift);
            command.Parameters.AddWithValue("@Revenue", workShift.Revenue);
            command.Parameters.AddWithValue("@Notes", workShift.Notes ?? string.Empty);
            command.Parameters.AddWithValue("@IsCompleted", workShift.IsCompleted);

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
                UPDATE WorkShifts
                SET Date = @Date, StartTime = @StartTime, EndTime = @EndTime,
                    IsNightShift = @IsNightShift, Revenue = @Revenue, Notes = @Notes, IsCompleted = @IsCompleted
                WHERE Id = @Id";

            command.Parameters.AddWithValue("@Id", workShift.Id);
            command.Parameters.AddWithValue("@Date", workShift.Date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@StartTime", workShift.StartTime.ToString("HH:mm"));
            command.Parameters.AddWithValue("@EndTime", workShift.EndTime.ToString("HH:mm"));
            command.Parameters.AddWithValue("@IsNightShift", workShift.IsNightShift);
            command.Parameters.AddWithValue("@Revenue", workShift.Revenue);
            command.Parameters.AddWithValue("@Notes", workShift.Notes ?? string.Empty);
            command.Parameters.AddWithValue("@IsCompleted", workShift.IsCompleted);

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
        /// 특정 날짜의 총 매출을 계산
        /// </summary>
        public decimal GetDailyRevenue(DateTime date)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT SUM(Revenue) FROM WorkShifts
                WHERE Date = @Date AND IsCompleted = 1";
            command.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));

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
                SELECT SUM(Revenue) FROM WorkShifts
                WHERE strftime('%Y', Date) = @Year
                AND strftime('%m', Date) = @Month
                AND IsCompleted = 1";
            command.Parameters.AddWithValue("@Year", year.ToString());
            command.Parameters.AddWithValue("@Month", month.ToString("D2"));

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
                SELECT SUM(Revenue) FROM WorkShifts
                WHERE Date >= @StartDate AND Date <= @EndDate AND IsCompleted = 1";
            command.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyy-MM-dd"));

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