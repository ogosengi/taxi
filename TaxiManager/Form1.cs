using System.Linq;
using TaxiManager.Models;
using TaxiManager.Services;

namespace TaxiManager;

public partial class Form1 : Form
{
    private TaxiDataService _dataService;
    private ExcelExportService _excelService;

    public Form1()
    {
        InitializeComponent();
        _dataService = new TaxiDataService();
        _excelService = new ExcelExportService();
        InitializeYearComboBox();
        LoadWorkShifts();
        LoadSettlements();
        InitializeInputs();

        // 폼 크기 변경 이벤트 핸들러 등록
        this.Resize += Form1_Resize;

        // 숫자 입력 컨트롤의 텍스트 자동 선택 이벤트 등록
        SetupTextSelectionEvents();

        // 시간 변경 시 야간근무 자동 체크 이벤트 등록
        SetupTimeChangeEvents();

        // F1 키 처리를 위한 KeyPreview 설정
        this.KeyPreview = true;
        this.KeyDown += Form1_KeyDown;
    }

    /// <summary>
    /// 년도 콤보박스 초기화
    /// </summary>
    private void InitializeYearComboBox()
    {
        comboBoxYear.Items.Clear();
        for (int year = DateTime.Now.Year - 5; year <= DateTime.Now.Year + 2; year++)
        {
            comboBoxYear.Items.Add(year);
        }
        comboBoxYear.SelectedItem = DateTime.Now.Year;
    }

    /// <summary>
    /// 근무 시간 목록을 로드
    /// </summary>
    private void LoadWorkShifts()
    {
        var workShifts = _dataService.GetAllWorkShifts().OrderBy(x => x.Date).ToList();
        dataGridView1.DataSource = workShifts;

        // 컬럼 헤더를 한글로 설정
        if (dataGridView1.Columns.Count > 0)
        {
            dataGridView1.Columns["Id"].HeaderText = "ID";
            dataGridView1.Columns["Date"].HeaderText = "날짜";
            dataGridView1.Columns["StartTime"].HeaderText = "시작시간";
            dataGridView1.Columns["EndTime"].HeaderText = "종료시간";
            dataGridView1.Columns["IsNightShift"].HeaderText = "야간근무";
            dataGridView1.Columns["Notes"].HeaderText = "메모";
            dataGridView1.Columns["ShiftType"].HeaderText = "근무타입";
            dataGridView1.Columns["WorkingHours"].HeaderText = "근무시간";

            // 컬럼 너비를 동적으로 조정
            AdjustGridColumnWidths();
        }
    }

    /// <summary>
    /// 새 근무 시간 추가 버튼 클릭
    /// </summary>
    private void btnAddShift_Click(object sender, EventArgs e)
    {
        try
        {
            var selectedDate = new DateTime(
                (int)(comboBoxYear.SelectedItem ?? DateTime.Now.Year),
                (int)numericUpDownMonth.Value,
                (int)numericUpDownDay.Value);

            var workShift = new TaxiWorkShift
            {
                Date = selectedDate,
                StartTime = TimeOnly.FromDateTime(dateTimePickerStart.Value),
                EndTime = TimeOnly.FromDateTime(dateTimePickerEnd.Value),
                IsNightShift = checkBoxNightShift.Checked,
                Notes = textBoxNotes.Text
            };

            _dataService.AddWorkShift(workShift);
            LoadWorkShifts();

            // 추가된 데이터에 커서 이동
            ScrollToLastAddedWorkShift(workShift.Date);

            // 근무시간 입력 시 해당 날짜를 마감일에 자동 대입
            dateTimePickerSettlement.Value = selectedDate;

            ClearInputs();
            // 근무 시간 추가 완료 (팝업 제거)
        }
        catch (Exception ex)
        {
            MessageBox.Show($"오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// 입력 필드를 부분 초기화 (날짜/시간은 유지, 메모만 초기화)
    /// </summary>
    private void ClearInputs()
    {
        // 날짜와 시간, 야간근무 체크는 유지하고 메모만 초기화
        textBoxNotes.Text = "";
        textBoxNotes.Focus(); // 메모 입력란에 포커스
    }

    /// <summary>
    /// 모든 입력 필드를 완전 초기화 (최초 실행 시에만 사용)
    /// </summary>
    private void InitializeInputs()
    {
        comboBoxYear.SelectedItem = DateTime.Now.Year;
        numericUpDownMonth.Value = DateTime.Now.Month;
        numericUpDownDay.Value = DateTime.Now.Day;
        dateTimePickerStart.Value = DateTime.Today.AddHours(10);
        dateTimePickerEnd.Value = DateTime.Today.AddHours(15);
        checkBoxNightShift.Checked = false;
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
    /// 기간별 운행 현황 조회 - 통계 파일로 내보내기
    /// </summary>
    private void btnPeriodStats_Click(object sender, EventArgs e)
    {
        try
        {
            var startDate = dateTimePickerPeriodStart.Value;
            var endDate = dateTimePickerPeriodEnd.Value;

            var stats = _dataService.GetOperationStats(startDate, endDate);

            // 저장 파일 대화상자
            using var saveDialog = new SaveFileDialog
            {
                Filter = "Text Files|*.txt",
                Title = "운행 통계 리포트를 텍스트 파일로 저장",
                DefaultExt = "txt",
                FileName = $"택시운행통계_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}_{DateTime.Now:HHmm}.txt"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                // 통계 내보내기 서비스 사용
                var statsExportService = new Services.StatsExportService();
                statsExportService.ExportStatsToTextFile(stats, startDate, endDate, saveDialog.FileName);

                MessageBox.Show($"통계 리포트가 성공적으로 저장되었습니다.\n저장 위치: {saveDialog.FileName}",
                    "통계 리포트 저장 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 저장된 파일을 열어볼지 묻기
                if (MessageBox.Show("저장된 통계 파일을 지금 열어보시겠습니까?", "파일 열기",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = saveDialog.FileName,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"파일을 여는 중 오류가 발생했습니다: {ex.Message}", "오류",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"통계 리포트 생성 중 오류가 발생했습니다: {ex.Message}", "오류",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// 일별 마감 처리
    /// </summary>
    private void btnDailySettlement_Click(object sender, EventArgs e)
    {
        var selectedDate = dateTimePickerSettlement.Value.Date;
        var dailyRevenue = numericUpDownRevenue.Value;

        try
        {
            // 매출이 입력되지 않았는지 확인
            if (dailyRevenue <= 0)
            {
                MessageBox.Show(
                    "매출을 입력해주세요.",
                    "매출 입력 필요",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

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
            var workShifts = _dataService.GetAllWorkShifts()
                .Where(x => x.Date.Date == selectedDate.Date)
                .ToList();

            if (!workShifts.Any())
            {
                MessageBox.Show(
                    $"{selectedDate:yyyy-MM-dd}에 등록된 근무 기록이 없습니다.",
                    "마감 불가",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // 일별 마감 처리 (매출과 함께)
            _dataService.CreateDailySettlementWithRevenue(selectedDate, dailyRevenue);

            // 일별 마감 완료 후 마감자료 목록 새로고침
            LoadSettlements();

            // 추가된 마감자료에 커서 이동
            ScrollToLastAddedSettlement(selectedDate);

            // 매출 입력 초기화
            numericUpDownRevenue.Value = 0;

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

    /// <summary>
    /// 일별 마감 목록을 로드
    /// </summary>
    private void LoadSettlements()
    {
        var settlements = _dataService.GetAllDailySettlements().OrderBy(x => x.Date).ToList();
        dataGridViewSettlements.DataSource = settlements;

        // 컬럼 헤더를 한글로 설정
        if (dataGridViewSettlements.Columns.Count > 0)
        {
            dataGridViewSettlements.Columns["Id"].HeaderText = "ID";
            dataGridViewSettlements.Columns["Date"].HeaderText = "마감일";
            dataGridViewSettlements.Columns["TotalRevenue"].HeaderText = "총 매출";
            dataGridViewSettlements.Columns["TotalWorkingHours"].HeaderText = "총 근무시간";
            dataGridViewSettlements.Columns["Notes"].HeaderText = "메모";
            dataGridViewSettlements.Columns["AverageRevenuePerHour"].HeaderText = "시간당 평균";
            dataGridViewSettlements.Columns["AverageRevenuePerHour"].DefaultCellStyle.Format = "F1";

            // 컬럼 너비를 동적으로 조정
            AdjustGridColumnWidths();
        }
    }

    /// <summary>
    /// 일별 마감 자료 조회 버튼 클릭
    /// </summary>
    private void btnViewSettlements_Click(object sender, EventArgs e)
    {
        LoadSettlements();
        // 일별 마감 자료 조회 완료 (팝업 제거)
    }

    /// <summary>
    /// 선택된 일별 마감 자료 삭제
    /// </summary>
    private void btnDeleteSettlement_Click(object sender, EventArgs e)
    {
        if (dataGridViewSettlements.SelectedRows.Count > 0)
        {
            var selectedSettlement = dataGridViewSettlements.SelectedRows[0].DataBoundItem as DailySettlement;
            if (selectedSettlement != null && MessageBox.Show($"{selectedSettlement.Date:yyyy-MM-dd} 마감 자료를 삭제하시겠습니까?",
                "삭제 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    _dataService.DeleteDailySettlement(selectedSettlement.Id);
                    LoadSettlements();
                    MessageBox.Show("마감 자료가 삭제되었습니다.", "삭제 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"삭제 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        else
        {
            MessageBox.Show("삭제할 마감 자료를 선택해주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    /// <summary>
    /// 추가된 근무시간에 그리드 커서 이동
    /// </summary>
    private void ScrollToLastAddedWorkShift(DateTime targetDate)
    {
        for (int i = 0; i < dataGridView1.Rows.Count; i++)
        {
            var row = dataGridView1.Rows[i];
            if (row.DataBoundItem is TaxiWorkShift workShift && workShift.Date.Date == targetDate.Date)
            {
                // 마지막으로 추가된 데이터로 이동 (날짜가 같은 것 중 마지막)
                dataGridView1.ClearSelection();
                row.Selected = true;
                dataGridView1.FirstDisplayedScrollingRowIndex = i;
                dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[0];
                break;
            }
        }
    }

    /// <summary>
    /// 추가된 마감자료에 그리드 커서 이동
    /// </summary>
    private void ScrollToLastAddedSettlement(DateTime targetDate)
    {
        for (int i = 0; i < dataGridViewSettlements.Rows.Count; i++)
        {
            var row = dataGridViewSettlements.Rows[i];
            if (row.DataBoundItem is DailySettlement settlement && settlement.Date.Date == targetDate.Date)
            {
                dataGridViewSettlements.ClearSelection();
                row.Selected = true;
                dataGridViewSettlements.FirstDisplayedScrollingRowIndex = i;
                dataGridViewSettlements.CurrentCell = dataGridViewSettlements.Rows[i].Cells[0];
                break;
            }
        }
    }

    /// <summary>
    /// 폼 크기 변경 시 그리드 크기 조정
    /// </summary>
    private void Form1_Resize(object? sender, EventArgs e)
    {
        // 폼이 최소화되었을 때는 처리하지 않음
        if (this.WindowState == FormWindowState.Minimized)
            return;

        // 그리드 컬럼 너비를 다시 조정
        AdjustGridColumnWidths();
    }

    /// <summary>
    /// 숫자 입력 컨트롤의 텍스트 자동 선택 이벤트 설정
    /// </summary>
    private void SetupTextSelectionEvents()
    {
        // NumericUpDown 컨트롤들의 텍스트 자동 선택 이벤트 등록
        numericUpDownMonth.Enter += NumericUpDown_Enter;
        numericUpDownDay.Enter += NumericUpDown_Enter;

        // TextBox 컨트롤의 텍스트 자동 선택 이벤트 등록
        textBoxNotes.Enter += TextBox_Enter;
    }

    /// <summary>
    /// 시간 변경 시 야간근무 자동 체크 이벤트 설정
    /// </summary>
    private void SetupTimeChangeEvents()
    {
        dateTimePickerStart.ValueChanged += TimeChanged;
        dateTimePickerEnd.ValueChanged += TimeChanged;
    }

    /// <summary>
    /// 시간 변경 시 야간근무 자동 체크
    /// </summary>
    private void TimeChanged(object? sender, EventArgs e)
    {
        var startTime = TimeOnly.FromDateTime(dateTimePickerStart.Value);
        var endTime = TimeOnly.FromDateTime(dateTimePickerEnd.Value);

        // 종료시간이 00:00이거나 시작시간보다 작으면 야간근무로 자동 체크
        if (endTime < startTime || (endTime.Hour == 0 && endTime.Minute == 0))
        {
            checkBoxNightShift.Checked = true;
        }
        else
        {
            // 같은 날 내 근무인 경우 야간근무 체크 해제 (사용자가 수동으로 체크할 수 있음)
            // checkBoxNightShift.Checked = false; // 주석처리: 사용자가 수동으로 체크한 경우를 고려
        }
    }

    /// <summary>
    /// NumericUpDown 컨트롤 포커스 시 텍스트 전체 선택
    /// </summary>
    private void NumericUpDown_Enter(object? sender, EventArgs e)
    {
        if (sender is NumericUpDown numericUpDown)
        {
            // NumericUpDown의 텍스트박스 부분을 선택
            numericUpDown.Select(0, numericUpDown.Text.Length);
        }
    }

    /// <summary>
    /// TextBox 컨트롤 포커스 시 텍스트 전체 선택
    /// </summary>
    private void TextBox_Enter(object? sender, EventArgs e)
    {
        if (sender is TextBox textBox)
        {
            textBox.SelectAll();
        }
    }

    /// <summary>
    /// F1 키 처리 - 근무시간 입력 일자로 이동
    /// </summary>
    private void Form1_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.F1)
        {
            // F1 키를 누르면 근무시간 입력의 월 입력 칸으로 포커스 이동
            numericUpDownMonth.Focus();
            numericUpDownMonth.Select(0, numericUpDownMonth.Text.Length);
            e.Handled = true; // 기본 F1 처리 방지
        }
    }

    /// <summary>
    /// 그리드 컬럼 너비를 화면 크기에 맞게 조정
    /// </summary>
    private void AdjustGridColumnWidths()
    {
        try
        {
            // 근무 시간 목록 그리드 컬럼 너비 조정
            if (dataGridView1.Columns.Count > 0)
            {
                var totalWidth = dataGridView1.Width - 50; // 스크롤바 여백
                dataGridView1.Columns["Id"].Width = (int)(totalWidth * 0.05);
                dataGridView1.Columns["Date"].Width = (int)(totalWidth * 0.14);
                dataGridView1.Columns["StartTime"].Width = (int)(totalWidth * 0.12);
                dataGridView1.Columns["EndTime"].Width = (int)(totalWidth * 0.12);
                dataGridView1.Columns["IsNightShift"].Width = (int)(totalWidth * 0.10);
                dataGridView1.Columns["Notes"].Width = (int)(totalWidth * 0.22);
                dataGridView1.Columns["ShiftType"].Width = (int)(totalWidth * 0.15);
                dataGridView1.Columns["WorkingHours"].Width = (int)(totalWidth * 0.10);
            }

            // 마감 자료 그리드 컬럼 너비 조정
            if (dataGridViewSettlements.Columns.Count > 0)
            {
                var totalWidth = dataGridViewSettlements.Width - 50; // 스크롤바 여백
                dataGridViewSettlements.Columns["Id"].Width = (int)(totalWidth * 0.05);
                dataGridViewSettlements.Columns["Date"].Width = (int)(totalWidth * 0.12);
                dataGridViewSettlements.Columns["TotalRevenue"].Width = (int)(totalWidth * 0.12);
                dataGridViewSettlements.Columns["TotalWorkingHours"].Width = (int)(totalWidth * 0.12);
                dataGridViewSettlements.Columns["Notes"].Width = (int)(totalWidth * 0.20);
                dataGridViewSettlements.Columns["AverageRevenuePerHour"].Width = (int)(totalWidth * 0.15);
            }
        }
        catch (Exception)
        {
            // 컬럼이 아직 생성되지 않았거나 오류가 발생한 경우 무시
        }
    }

    /// <summary>
    /// 엑셀로 내보내기 버튼 클릭
    /// </summary>
    private void btnExportExcel_Click(object sender, EventArgs e)
    {
        try
        {
            using var saveDialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                Title = "택시 운행 데이터를 엑셀로 저장",
                DefaultExt = "xlsx",
                FileName = $"택시운행데이터_{DateTime.Now:yyyyMMddHHmm}.xlsx"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                // 기간 설정 (전체 데이터)
                var allWorkShifts = _dataService.GetAllWorkShifts();
                var allSettlements = _dataService.GetAllDailySettlements();

                if (!allWorkShifts.Any() && !allSettlements.Any())
                {
                    MessageBox.Show("내보낼 데이터가 없습니다.", "알림",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var startDate = allWorkShifts.Any() ? allWorkShifts.Min(x => x.Date) : DateTime.Today;
                var endDate = allWorkShifts.Any() ? allWorkShifts.Max(x => x.Date) : DateTime.Today;

                // 통계 정보 생성
                var stats = _dataService.GetOperationStats(startDate, endDate);

                // 엑셀 작업용 통합 리포트로 내보내기 (편집 가능한 형태)
                _excelService.ExportWorkableReportToExcel(allWorkShifts, allSettlements, stats,
                    startDate, endDate, saveDialog.FileName);

                MessageBox.Show($"엑셀 파일이 성공적으로 저장되었습니다.\n저장 위치: {saveDialog.FileName}",
                    "엑셀 내보내기 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 저장된 파일을 열어볼지 묻기
                if (MessageBox.Show("저장된 엑셀 파일을 열어보시겠습니까?", "파일 열기",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = saveDialog.FileName,
                        UseShellExecute = true
                    });
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"엑셀 내보내기 중 오류가 발생했습니다: {ex.Message}",
                "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
