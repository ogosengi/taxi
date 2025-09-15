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
                Notes = textBoxNotes.Text,
                IsCompleted = checkBoxCompleted.Checked,
                BreakMinutes = (int)numericUpDownBreak.Value
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
        numericUpDownBreak.Value = 0;
        textBoxNotes.Text = "";
        checkBoxCompleted.Checked = false;
    }

    /// <summary>
    /// 미리 정의된 근무 시간 선택 변경 (완전 유연한 시간 설정 지원)
    /// </summary>
    private void comboBoxShiftType_SelectedIndexChanged(object sender, EventArgs e)
    {
        switch (comboBoxShiftType.SelectedIndex)
        {
            case 0: // 일반 근무 (10:00-15:00)
                dateTimePickerStart.Value = DateTime.Today.AddHours(10);
                dateTimePickerEnd.Value = DateTime.Today.AddHours(15);
                checkBoxNightShift.Checked = false;
                break;
            case 1: // 야간 근무 (19:00-02:00)
                dateTimePickerStart.Value = DateTime.Today.AddHours(19);
                dateTimePickerEnd.Value = DateTime.Today.AddHours(2);
                checkBoxNightShift.Checked = true;
                break;
            case 2: // 24시간 근무 (10:00-다음날 10:00)
                dateTimePickerStart.Value = DateTime.Today.AddHours(10);
                dateTimePickerEnd.Value = DateTime.Today.AddHours(10);
                checkBoxNightShift.Checked = true;
                break;
            case 3: // 사용자 정의 (현재 설정 유지)
                // 아무 변경 없음 - 사용자가 직접 설정
                break;
        }
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
}
