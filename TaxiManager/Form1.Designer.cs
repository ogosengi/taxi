namespace TaxiManager;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        groupBoxInput = new GroupBox();
        textBoxNotes = new TextBox();
        labelNotes = new Label();
        numericUpDownRevenue = new NumericUpDown();
        labelRevenue = new Label();
        checkBoxNightShift = new CheckBox();
        dateTimePickerEnd = new DateTimePicker();
        labelEndTime = new Label();
        dateTimePickerStart = new DateTimePicker();
        labelStartTime = new Label();
        comboBoxYear = new ComboBox();
        numericUpDownMonth = new NumericUpDown();
        numericUpDownDay = new NumericUpDown();
        labelYear = new Label();
        labelMonth = new Label();
        labelDay = new Label();
        btnAddShift = new Button();
        groupBoxList = new GroupBox();
        btnDeleteShift = new Button();
        dataGridView1 = new DataGridView();
        groupBoxRevenue = new GroupBox();
        labelMonthlyRevenue = new Label();
        btnMonthlyRevenue = new Button();
        dateTimePickerMonth = new DateTimePicker();
        labelMonthSelector = new Label();
        btnPeriodStats = new Button();
        dateTimePickerPeriodEnd = new DateTimePicker();
        labelPeriodEnd = new Label();
        dateTimePickerPeriodStart = new DateTimePicker();
        labelPeriodStart = new Label();
        btnDailySettlement = new Button();
        dateTimePickerSettlement = new DateTimePicker();
        labelSettlement = new Label();
        groupBoxSettlements = new GroupBox();
        dataGridViewSettlements = new DataGridView();
        btnDeleteSettlement = new Button();
        btnViewSettlements = new Button();
        groupBoxInput.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericUpDownRevenue).BeginInit();
        groupBoxList.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
        groupBoxRevenue.SuspendLayout();
        groupBoxSettlements.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dataGridViewSettlements).BeginInit();
        SuspendLayout();
        //
        // groupBoxInput
        //
        groupBoxInput.Controls.Add(textBoxNotes);
        groupBoxInput.Controls.Add(labelNotes);
        groupBoxInput.Controls.Add(checkBoxNightShift);
        groupBoxInput.Controls.Add(dateTimePickerEnd);
        groupBoxInput.Controls.Add(labelEndTime);
        groupBoxInput.Controls.Add(dateTimePickerStart);
        groupBoxInput.Controls.Add(labelStartTime);
        groupBoxInput.Controls.Add(comboBoxYear);
        groupBoxInput.Controls.Add(numericUpDownMonth);
        groupBoxInput.Controls.Add(numericUpDownDay);
        groupBoxInput.Controls.Add(labelYear);
        groupBoxInput.Controls.Add(labelMonth);
        groupBoxInput.Controls.Add(labelDay);
        groupBoxInput.Controls.Add(btnAddShift);
        groupBoxInput.Location = new Point(12, 12);
        groupBoxInput.Name = "groupBoxInput";
        groupBoxInput.Size = new Size(350, 230);
        groupBoxInput.TabIndex = 0;
        groupBoxInput.TabStop = false;
        groupBoxInput.Text = "근무 시간 입력";
        //
        // textBoxNotes
        //
        textBoxNotes.Location = new Point(79, 155);
        textBoxNotes.Name = "textBoxNotes";
        textBoxNotes.Size = new Size(140, 23);
        textBoxNotes.TabIndex = 7;
        //
        // labelNotes
        //
        labelNotes.AutoSize = true;
        labelNotes.Location = new Point(6, 158);
        labelNotes.Name = "labelNotes";
        labelNotes.Size = new Size(35, 15);
        labelNotes.TabIndex = 10;
        labelNotes.Text = "메모:";
        //
        // numericUpDownRevenue
        //
        numericUpDownRevenue.DecimalPlaces = 0;
        numericUpDownRevenue.Location = new Point(79, 47);
        numericUpDownRevenue.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
        numericUpDownRevenue.Name = "numericUpDownRevenue";
        numericUpDownRevenue.Size = new Size(119, 23);
        numericUpDownRevenue.TabIndex = 11;
        //
        // labelRevenue
        //
        labelRevenue.AutoSize = true;
        labelRevenue.Location = new Point(6, 53);
        labelRevenue.Name = "labelRevenue";
        labelRevenue.Size = new Size(35, 15);
        labelRevenue.TabIndex = 8;
        labelRevenue.Text = "매출:";
        //
        // checkBoxNightShift
        //
        checkBoxNightShift.AutoSize = true;
        checkBoxNightShift.Location = new Point(79, 126);
        checkBoxNightShift.Name = "checkBoxNightShift";
        checkBoxNightShift.Size = new Size(74, 19);
        checkBoxNightShift.TabIndex = 6;
        checkBoxNightShift.Text = "야간 근무";
        checkBoxNightShift.UseVisualStyleBackColor = true;
        //
        // dateTimePickerEnd
        //
        dateTimePickerEnd.CustomFormat = "HH:00";
        dateTimePickerEnd.Format = DateTimePickerFormat.Custom;
        dateTimePickerEnd.Location = new Point(79, 97);
        dateTimePickerEnd.Name = "dateTimePickerEnd";
        dateTimePickerEnd.ShowUpDown = true;
        dateTimePickerEnd.Size = new Size(200, 23);
        dateTimePickerEnd.TabIndex = 5;
        //
        // labelEndTime
        //
        labelEndTime.AutoSize = true;
        labelEndTime.Location = new Point(6, 103);
        labelEndTime.Name = "labelEndTime";
        labelEndTime.Size = new Size(67, 15);
        labelEndTime.TabIndex = 5;
        labelEndTime.Text = "종료 시간:";
        //
        // dateTimePickerStart
        //
        dateTimePickerStart.CustomFormat = "HH:00";
        dateTimePickerStart.Format = DateTimePickerFormat.Custom;
        dateTimePickerStart.Location = new Point(79, 78);
        dateTimePickerStart.Name = "dateTimePickerStart";
        dateTimePickerStart.ShowUpDown = true;
        dateTimePickerStart.Size = new Size(200, 23);
        dateTimePickerStart.TabIndex = 4;
        //
        // labelStartTime
        //
        labelStartTime.AutoSize = true;
        labelStartTime.Location = new Point(6, 84);
        labelStartTime.Name = "labelStartTime";
        labelStartTime.Size = new Size(67, 15);
        labelStartTime.TabIndex = 3;
        labelStartTime.Text = "시작 시간:";
        //
        // comboBoxYear
        //
        comboBoxYear.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBoxYear.Location = new Point(79, 51);
        comboBoxYear.Name = "comboBoxYear";
        comboBoxYear.Size = new Size(70, 23);
        comboBoxYear.TabIndex = 1;
        //
        // numericUpDownMonth
        //
        numericUpDownMonth.Location = new Point(179, 51);
        numericUpDownMonth.Maximum = 12;
        numericUpDownMonth.Minimum = 1;
        numericUpDownMonth.Name = "numericUpDownMonth";
        numericUpDownMonth.Size = new Size(50, 23);
        numericUpDownMonth.TabIndex = 2;
        numericUpDownMonth.Value = 1;
        //
        // numericUpDownDay
        //
        numericUpDownDay.Location = new Point(259, 51);
        numericUpDownDay.Maximum = 31;
        numericUpDownDay.Minimum = 1;
        numericUpDownDay.Name = "numericUpDownDay";
        numericUpDownDay.Size = new Size(50, 23);
        numericUpDownDay.TabIndex = 3;
        numericUpDownDay.Value = 1;
        //
        // labelYear
        //
        labelYear.AutoSize = true;
        labelYear.Location = new Point(6, 54);
        labelYear.Name = "labelYear";
        labelYear.Size = new Size(31, 15);
        labelYear.TabIndex = 1;
        labelYear.Text = "날짜:";
        //
        // labelMonth
        //
        labelMonth.AutoSize = true;
        labelMonth.Location = new Point(155, 54);
        labelMonth.Name = "labelMonth";
        labelMonth.Size = new Size(19, 15);
        labelMonth.TabIndex = 2;
        labelMonth.Text = "월";
        //
        // labelDay
        //
        labelDay.AutoSize = true;
        labelDay.Location = new Point(235, 54);
        labelDay.Name = "labelDay";
        labelDay.Size = new Size(19, 15);
        labelDay.TabIndex = 3;
        labelDay.Text = "일";
        //
        // btnAddShift
        //
        btnAddShift.Location = new Point(225, 155);
        btnAddShift.Name = "btnAddShift";
        btnAddShift.Size = new Size(84, 23);
        btnAddShift.TabIndex = 8;
        btnAddShift.Text = "추가";
        btnAddShift.UseVisualStyleBackColor = true;
        btnAddShift.Click += btnAddShift_Click;
        //
        // groupBoxList
        //
        groupBoxList.Controls.Add(btnDeleteShift);
        groupBoxList.Controls.Add(dataGridView1);
        groupBoxList.Location = new Point(380, 12);
        groupBoxList.Name = "groupBoxList";
        groupBoxList.Size = new Size(600, 280);
        groupBoxList.TabIndex = 1;
        groupBoxList.TabStop = false;
        groupBoxList.Text = "근무 시간 목록";
        groupBoxList.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        //
        // btnDeleteShift
        //
        btnDeleteShift.Location = new Point(519, 251);
        btnDeleteShift.Name = "btnDeleteShift";
        btnDeleteShift.Size = new Size(75, 23);
        btnDeleteShift.TabIndex = 1;
        btnDeleteShift.Text = "삭제";
        btnDeleteShift.UseVisualStyleBackColor = true;
        btnDeleteShift.Click += btnDeleteShift_Click;
        btnDeleteShift.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        //
        // dataGridView1
        //
        dataGridView1.AllowUserToAddRows = false;
        dataGridView1.AllowUserToDeleteRows = false;
        dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridView1.Location = new Point(6, 22);
        dataGridView1.MultiSelect = false;
        dataGridView1.Name = "dataGridView1";
        dataGridView1.ReadOnly = true;
        dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dataGridView1.Size = new Size(588, 220);
        dataGridView1.TabIndex = 0;
        dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        //
        // groupBoxRevenue
        //
        groupBoxRevenue.Controls.Add(numericUpDownRevenue);
        groupBoxRevenue.Controls.Add(labelRevenue);
        groupBoxRevenue.Controls.Add(btnDailySettlement);
        groupBoxRevenue.Controls.Add(dateTimePickerSettlement);
        groupBoxRevenue.Controls.Add(labelSettlement);
        groupBoxRevenue.Controls.Add(btnPeriodStats);
        groupBoxRevenue.Controls.Add(dateTimePickerPeriodEnd);
        groupBoxRevenue.Controls.Add(labelPeriodEnd);
        groupBoxRevenue.Controls.Add(dateTimePickerPeriodStart);
        groupBoxRevenue.Controls.Add(labelPeriodStart);
        groupBoxRevenue.Controls.Add(labelMonthlyRevenue);
        groupBoxRevenue.Controls.Add(btnMonthlyRevenue);
        groupBoxRevenue.Controls.Add(dateTimePickerMonth);
        groupBoxRevenue.Controls.Add(labelMonthSelector);
        groupBoxRevenue.Location = new Point(12, 298);
        groupBoxRevenue.Name = "groupBoxRevenue";
        groupBoxRevenue.Size = new Size(350, 190);
        groupBoxRevenue.TabIndex = 2;
        groupBoxRevenue.TabStop = false;
        groupBoxRevenue.Text = "매출 조회";
        //
        // labelMonthlyRevenue
        //
        labelMonthlyRevenue.AutoSize = true;
        labelMonthlyRevenue.Font = new Font("맑은 고딕", 9F, FontStyle.Bold);
        labelMonthlyRevenue.Location = new Point(6, 80);
        labelMonthlyRevenue.Name = "labelMonthlyRevenue";
        labelMonthlyRevenue.Size = new Size(127, 15);
        labelMonthlyRevenue.TabIndex = 3;
        labelMonthlyRevenue.Text = "월별 매출이 여기 표시";
        //
        // btnMonthlyRevenue
        //
        btnMonthlyRevenue.Location = new Point(204, 50);
        btnMonthlyRevenue.Name = "btnMonthlyRevenue";
        btnMonthlyRevenue.Size = new Size(75, 23);
        btnMonthlyRevenue.TabIndex = 2;
        btnMonthlyRevenue.Text = "조회";
        btnMonthlyRevenue.UseVisualStyleBackColor = true;
        btnMonthlyRevenue.Click += btnMonthlyRevenue_Click;
        //
        // dateTimePickerMonth
        //
        dateTimePickerMonth.CustomFormat = "yyyy-MM";
        dateTimePickerMonth.Format = DateTimePickerFormat.Custom;
        dateTimePickerMonth.Location = new Point(79, 50);
        dateTimePickerMonth.Name = "dateTimePickerMonth";
        dateTimePickerMonth.Size = new Size(119, 23);
        dateTimePickerMonth.TabIndex = 1;
        //
        // labelMonthSelector
        //
        labelMonthSelector.AutoSize = true;
        labelMonthSelector.Location = new Point(6, 56);
        labelMonthSelector.Name = "labelMonthSelector";
        labelMonthSelector.Size = new Size(35, 15);
        labelMonthSelector.TabIndex = 0;
        labelMonthSelector.Text = "월별:";
        //
        // btnPeriodStats
        //
        btnPeriodStats.Location = new Point(204, 150);
        btnPeriodStats.Name = "btnPeriodStats";
        btnPeriodStats.Size = new Size(75, 23);
        btnPeriodStats.TabIndex = 9;
        btnPeriodStats.Text = "통계";
        btnPeriodStats.UseVisualStyleBackColor = true;
        btnPeriodStats.Click += btnPeriodStats_Click;
        //
        // dateTimePickerPeriodEnd
        //
        dateTimePickerPeriodEnd.Location = new Point(79, 150);
        dateTimePickerPeriodEnd.Name = "dateTimePickerPeriodEnd";
        dateTimePickerPeriodEnd.Size = new Size(119, 23);
        dateTimePickerPeriodEnd.TabIndex = 8;
        //
        // labelPeriodEnd
        //
        labelPeriodEnd.AutoSize = true;
        labelPeriodEnd.Location = new Point(6, 156);
        labelPeriodEnd.Name = "labelPeriodEnd";
        labelPeriodEnd.Size = new Size(47, 15);
        labelPeriodEnd.TabIndex = 7;
        labelPeriodEnd.Text = "종료일:";
        //
        // dateTimePickerPeriodStart
        //
        dateTimePickerPeriodStart.Location = new Point(79, 120);
        dateTimePickerPeriodStart.Name = "dateTimePickerPeriodStart";
        dateTimePickerPeriodStart.Size = new Size(119, 23);
        dateTimePickerPeriodStart.TabIndex = 6;
        //
        // labelPeriodStart
        //
        labelPeriodStart.AutoSize = true;
        labelPeriodStart.Location = new Point(6, 126);
        labelPeriodStart.Name = "labelPeriodStart";
        labelPeriodStart.Size = new Size(47, 15);
        labelPeriodStart.TabIndex = 5;
        labelPeriodStart.Text = "시작일:";
        //
        // btnDailySettlement
        //
        btnDailySettlement.Location = new Point(285, 20);
        btnDailySettlement.Name = "btnDailySettlement";
        btnDailySettlement.Size = new Size(59, 50);
        btnDailySettlement.TabIndex = 12;
        btnDailySettlement.Text = "일별마감";
        btnDailySettlement.UseVisualStyleBackColor = true;
        btnDailySettlement.Click += btnDailySettlement_Click;
        //
        // dateTimePickerSettlement
        //
        dateTimePickerSettlement.Location = new Point(79, 20);
        dateTimePickerSettlement.Name = "dateTimePickerSettlement";
        dateTimePickerSettlement.Size = new Size(119, 23);
        dateTimePickerSettlement.TabIndex = 10;
        //
        // labelSettlement
        //
        labelSettlement.AutoSize = true;
        labelSettlement.Location = new Point(6, 26);
        labelSettlement.Name = "labelSettlement";
        labelSettlement.Size = new Size(47, 15);
        labelSettlement.TabIndex = 10;
        labelSettlement.Text = "마감일:";
        //
        // groupBoxSettlements
        //
        groupBoxSettlements.Controls.Add(btnDeleteSettlement);
        groupBoxSettlements.Controls.Add(btnViewSettlements);
        groupBoxSettlements.Controls.Add(dataGridViewSettlements);
        groupBoxSettlements.Location = new Point(380, 298);
        groupBoxSettlements.Name = "groupBoxSettlements";
        groupBoxSettlements.Size = new Size(600, 190);
        groupBoxSettlements.TabIndex = 3;
        groupBoxSettlements.TabStop = false;
        groupBoxSettlements.Text = "일별 마감 자료 관리";
        groupBoxSettlements.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        //
        // dataGridViewSettlements
        //
        dataGridViewSettlements.AllowUserToAddRows = false;
        dataGridViewSettlements.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridViewSettlements.Location = new Point(6, 50);
        dataGridViewSettlements.Name = "dataGridViewSettlements";
        dataGridViewSettlements.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dataGridViewSettlements.Size = new Size(588, 130);
        dataGridViewSettlements.TabIndex = 0;
        dataGridViewSettlements.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        //
        // btnDeleteSettlement
        //
        btnDeleteSettlement.Location = new Point(519, 21);
        btnDeleteSettlement.Name = "btnDeleteSettlement";
        btnDeleteSettlement.Size = new Size(75, 23);
        btnDeleteSettlement.TabIndex = 2;
        btnDeleteSettlement.Text = "삭제";
        btnDeleteSettlement.UseVisualStyleBackColor = true;
        btnDeleteSettlement.Click += btnDeleteSettlement_Click;
        btnDeleteSettlement.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        //
        // btnViewSettlements
        //
        btnViewSettlements.Location = new Point(438, 21);
        btnViewSettlements.Name = "btnViewSettlements";
        btnViewSettlements.Size = new Size(75, 23);
        btnViewSettlements.TabIndex = 1;
        btnViewSettlements.Text = "조회";
        btnViewSettlements.UseVisualStyleBackColor = true;
        btnViewSettlements.Click += btnViewSettlements_Click;
        btnViewSettlements.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        //
        // Form1
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 500);
        MinimumSize = new Size(1000, 500);
        Controls.Add(groupBoxSettlements);
        Controls.Add(groupBoxRevenue);
        Controls.Add(groupBoxList);
        Controls.Add(groupBoxInput);
        Name = "Form1";
        Text = "택시 운행 관리 프로그램";
        groupBoxInput.ResumeLayout(false);
        groupBoxInput.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericUpDownRevenue).EndInit();
        groupBoxList.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
        groupBoxRevenue.ResumeLayout(false);
        groupBoxRevenue.PerformLayout();
        groupBoxSettlements.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dataGridViewSettlements).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private GroupBox groupBoxInput;
    private TextBox textBoxNotes;
    private Label labelNotes;
    private NumericUpDown numericUpDownRevenue;
    private Label labelRevenue;
    private CheckBox checkBoxNightShift;
    private DateTimePicker dateTimePickerEnd;
    private Label labelEndTime;
    private DateTimePicker dateTimePickerStart;
    private Label labelStartTime;
    private ComboBox comboBoxYear;
    private NumericUpDown numericUpDownMonth;
    private NumericUpDown numericUpDownDay;
    private Label labelYear;
    private Label labelMonth;
    private Label labelDay;
    private Button btnAddShift;
    private GroupBox groupBoxList;
    private Button btnDeleteShift;
    private DataGridView dataGridView1;
    private GroupBox groupBoxRevenue;
    private Label labelMonthlyRevenue;
    private Button btnMonthlyRevenue;
    private DateTimePicker dateTimePickerMonth;
    private Label labelMonthSelector;
    private Button btnPeriodStats;
    private DateTimePicker dateTimePickerPeriodEnd;
    private Label labelPeriodEnd;
    private DateTimePicker dateTimePickerPeriodStart;
    private Label labelPeriodStart;
    private Button btnDailySettlement;
    private DateTimePicker dateTimePickerSettlement;
    private Label labelSettlement;
    private GroupBox groupBoxSettlements;
    private DataGridView dataGridViewSettlements;
    private Button btnDeleteSettlement;
    private Button btnViewSettlements;
}
