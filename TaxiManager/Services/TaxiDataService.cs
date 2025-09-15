using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TaxiManager.Models;

namespace TaxiManager.Services
{
    /// <summary>
    /// 택시 운행 데이터를 관리하는 서비스 클래스
    /// </summary>
    public class TaxiDataService
    {
        private readonly string _dataFilePath;
        private List<TaxiWorkShift> _workShifts;

        public TaxiDataService()
        {
            _dataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "taxidata.json");
            _workShifts = LoadData();
        }

        /// <summary>
        /// 모든 근무 시간 정보를 반환
        /// </summary>
        public List<TaxiWorkShift> GetAllWorkShifts()
        {
            return _workShifts.OrderByDescending(x => x.Date).ToList();
        }

        /// <summary>
        /// 새로운 근무 시간 정보를 추가
        /// </summary>
        public void AddWorkShift(TaxiWorkShift workShift)
        {
            workShift.Id = _workShifts.Count > 0 ? _workShifts.Max(x => x.Id) + 1 : 1;
            _workShifts.Add(workShift);
            SaveData();
        }

        /// <summary>
        /// 근무 시간 정보를 업데이트
        /// </summary>
        public void UpdateWorkShift(TaxiWorkShift workShift)
        {
            var existing = _workShifts.FirstOrDefault(x => x.Id == workShift.Id);
            if (existing != null)
            {
                var index = _workShifts.IndexOf(existing);
                _workShifts[index] = workShift;
                SaveData();
            }
        }

        /// <summary>
        /// 근무 시간 정보를 삭제
        /// </summary>
        public void DeleteWorkShift(int id)
        {
            var workShift = _workShifts.FirstOrDefault(x => x.Id == id);
            if (workShift != null)
            {
                _workShifts.Remove(workShift);
                SaveData();
            }
        }

        /// <summary>
        /// 특정 날짜의 총 매출을 계산
        /// </summary>
        public decimal GetDailyRevenue(DateTime date)
        {
            return _workShifts
                .Where(x => x.Date.Date == date.Date && x.IsCompleted)
                .Sum(x => x.Revenue);
        }

        /// <summary>
        /// 특정 월의 총 매출을 계산
        /// </summary>
        public decimal GetMonthlyRevenue(int year, int month)
        {
            return _workShifts
                .Where(x => x.Date.Year == year && x.Date.Month == month && x.IsCompleted)
                .Sum(x => x.Revenue);
        }

        /// <summary>
        /// 데이터를 파일에서 로드
        /// </summary>
        private List<TaxiWorkShift> LoadData()
        {
            if (!File.Exists(_dataFilePath))
                return new List<TaxiWorkShift>();

            try
            {
                var json = File.ReadAllText(_dataFilePath);
                return JsonSerializer.Deserialize<List<TaxiWorkShift>>(json) ?? new List<TaxiWorkShift>();
            }
            catch
            {
                return new List<TaxiWorkShift>();
            }
        }

        /// <summary>
        /// 데이터를 파일에 저장
        /// </summary>
        private void SaveData()
        {
            try
            {
                var json = JsonSerializer.Serialize(_workShifts, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_dataFilePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"데이터 저장 중 오류가 발생했습니다: {ex.Message}");
            }
        }
    }
}