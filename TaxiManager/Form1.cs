using TaxiManager.Models;
using TaxiManager.Services;

namespace TaxiManager;

public partial class Form1 : Form
{
    private TaxiDataService _dataService;

    public Form1()
    {
        InitializeComponent();
        _dataService = new TaxiDataService();
        LoadWorkShifts();
    }

    /// <summary>
    /// 근무 시간 목록을 로드
    /// </summary>
    private void LoadWorkShifts()
    {
        var workShifts = _dataService.GetAllWorkShifts();
        dataGridView1.DataSource = workShifts;

        // 컬럼 헤더를 한글로 설정
        if (dataGridView1.Columns.Count > 0)
        {
            dataGridView1.Columns["Id"].HeaderText = "ID";
            dataGridView1.Columns["Date"].HeaderText = "날짜";
            dataGridView1.Columns["StartTime"].HeaderText = "시작시간";
            dataGridView1.Columns["EndTime"].HeaderText = "종료시간";
            dataGridView1.Columns["IsNightShift"].HeaderText = "야간근무";
            dataGridView1.Columns["Revenue"].HeaderText = "매출";
            dataGridView1.Columns["Notes"].HeaderText = "메모";
            dataGridView1.Columns["ShiftType"].HeaderText = "근무타입";
            dataGridView1.Columns["WorkingHours"].HeaderText = "근무시간";

            // 컬럼 너비 조정
            dataGridView1.Columns["Id"].Width = 50;
            dataGridView1.Columns["Date"].Width = 100;
            dataGridView1.Columns["StartTime"].Width = 80;
            dataGridView1.Columns["EndTime"].Width = 80;
            dataGridView1.Columns["IsNightShift"].Width = 80;
            dataGridView1.Columns["Revenue"].Width = 100;
            dataGridView1.Columns["Notes"].Width = 150;
            dataGridView1.Columns["ShiftType"].Width = 150;
            dataGridView1.Columns["WorkingHours"].Width = 100;
        }
    }

    /// <summary>
    /// 새 근무 시간 추가 버튼 클릭
    /// </summary>
    private void btnAddShift_Click(object sender, EventArgs e)
    {
        try
        {
            var workShift = new TaxiWorkShift
            {
                Date = dateTimePicker1.Value.Date,
                StartTime = TimeOnly.FromDateTime(dateTimePickerStart.Value),
                EndTime = TimeOnly.FromDateTime(dateTimePickerEnd.Value),
                IsNightShift = checkBoxNightShift.Checked,
                Revenue = numericUpDownRevenue.Value,
                Notes = textBoxNotes.Text
            };

            _dataService.AddWorkShift(workShift);
            LoadWorkShifts();
            ClearInputs();
            MessageBox.Show("근무 시간이 추가되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// 입력 필드를 초기화
    /// </summary>
    private void ClearInputs()
    {
        dateTimePicker1.Value = DateTime.Now;
        dateTimePickerStart.Value = DateTime.Today.AddHours(10);
        dateTimePickerEnd.Value = DateTime.Today.AddHours(15);
        checkBoxNightShift.Checked = false;
        numericUpDownRevenue.Value = 0;
        textBoxNotes.Text = "";
    }


    /// <summary>
    /// 선택된 근무 시간 삭제
    /// </summary>
    private void btnDeleteShift_Click(object sender, EventArgs e)
    {
        if (dataGridView1.SelectedRows.Count > 0)
        {
            var selectedShift = dataGridView1.SelectedRows[0].DataBoundItem as TaxiWorkShift;
            if (selectedShift != null && MessageBox.Show($"{selectedShift.Date:yyyy-MM-dd} 근무 시간을 삭제하시겠습니까?",
                "삭제 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _dataService.DeleteWorkShift(selectedShift.Id);
                LoadWorkShifts();
                MessageBox.Show("근무 시간이 삭제되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        else
        {
            MessageBox.Show("삭제할 근무 시간을 선택해주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    /// <summary>
    /// 월별 매출 조회
    /// </summary>
    private void btnMonthlyRevenue_Click(object sender, EventArgs e)
    {
        var selectedDate = dateTimePickerMonth.Value;
        var monthlyRevenue = _dataService.GetMonthlyRevenue(selectedDate.Year, selectedDate.Month);
        labelMonthlyRevenue.Text = $"{selectedDate:yyyy년 MM월} 총 매출: {monthlyRevenue:C}";
    }

    /// <summary>
    /// 기간별 운행 현황 조회
    /// </summary>
    private void btnPeriodStats_Click(object sender, EventArgs e)
    {
        var startDate = dateTimePickerPeriodStart.Value;
        var endDate = dateTimePickerPeriodEnd.Value;

        var stats = _dataService.GetOperationStats(startDate, endDate);

        var statsMessage = $"기간: {startDate:yyyy-MM-dd} ~ {endDate:yyyy-MM-dd}\n" +
                          $"총 근무 횟수: {stats.TotalShifts}회\n" +
                          $"완료된 근무: {stats.CompletedShifts}회 ({stats.CompletionRate:F1}%)\n" +
                          $"총 매출: {stats.TotalRevenue:C}\n" +
                          $"총 근무시간: {stats.TotalWorkingHours:F1}시간\n" +
                          $"시간당 평균 매출: {stats.AverageRevenuePerHour:C}";

        MessageBox.Show(statsMessage, "운행 현황", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>
    /// 일별 마감 처리
    /// </summary>
    private void btnDailySettlement_Click(object sender, EventArgs e)
    {
        var selectedDate = dateTimePickerSettlement.Value.Date;

        try
        {
            // 이미 마감된 날짜인지 확인
            if (_dataService.IsDateSettled(selectedDate))
            {
                var result = MessageBox.Show(
                    $"{selectedDate:yyyy-MM-dd}는 이미 마감되었습니다.\n다시 마감하시겠습니까?",
                    "마감 확인",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                    return;
            }

            // 해당 날짜의 근무 기록이 있는지 확인
            var dailyRevenue = _dataService.GetDailyRevenue(selectedDate);
            if (dailyRevenue == 0)
            {
                MessageBox.Show(
                    $"{selectedDate:yyyy-MM-dd}에 등록된 근무 기록이 없습니다.",
                    "마감 불가",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // 일별 마감 처리
            _dataService.CreateDailySettlement(selectedDate);

            MessageBox.Show(
                $"{selectedDate:yyyy-MM-dd} 일별 마감이 완료되었습니다.\n총 매출: {dailyRevenue:C}",
                "마감 완료",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"일별 마감 처리 중 오류가 발생했습니다: {ex.Message}",
                "오류",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
