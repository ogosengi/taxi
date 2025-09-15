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
        labelShiftType = new Label();
        comboBoxShiftType = new ComboBox();
        checkBoxCompleted = new CheckBox();
        textBoxNotes = new TextBox();
        labelNotes = new Label();
        numericUpDownRevenue = new NumericUpDown();
        labelRevenue = new Label();
        checkBoxNightShift = new CheckBox();
        dateTimePickerEnd = new DateTimePicker();
        labelEndTime = new Label();
        dateTimePickerStart = new DateTimePicker();
        labelStartTime = new Label();
        dateTimePicker1 = new DateTimePicker();
        labelDate = new Label();
        btnAddShift = new Button();
        groupBoxList = new GroupBox();
        btnDeleteShift = new Button();
        dataGridView1 = new DataGridView();
        groupBoxRevenue = new GroupBox();
        labelMonthlyRevenue = new Label();
        btnMonthlyRevenue = new Button();
        dateTimePickerMonth = new DateTimePicker();
        labelMonth = new Label();
        groupBoxInput.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericUpDownRevenue).BeginInit();
        groupBoxList.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
        groupBoxRevenue.SuspendLayout();
        SuspendLayout();
        //
        // groupBoxInput
        //
        groupBoxInput.Controls.Add(labelShiftType);
        groupBoxInput.Controls.Add(comboBoxShiftType);
        groupBoxInput.Controls.Add(checkBoxCompleted);
        groupBoxInput.Controls.Add(textBoxNotes);
        groupBoxInput.Controls.Add(labelNotes);
        groupBoxInput.Controls.Add(numericUpDownRevenue);
        groupBoxInput.Controls.Add(labelRevenue);
        groupBoxInput.Controls.Add(checkBoxNightShift);
        groupBoxInput.Controls.Add(dateTimePickerEnd);
        groupBoxInput.Controls.Add(labelEndTime);
        groupBoxInput.Controls.Add(dateTimePickerStart);
        groupBoxInput.Controls.Add(labelStartTime);
        groupBoxInput.Controls.Add(dateTimePicker1);
        groupBoxInput.Controls.Add(labelDate);
        groupBoxInput.Controls.Add(btnAddShift);
        groupBoxInput.Location = new Point(12, 12);
        groupBoxInput.Name = "groupBoxInput";
        groupBoxInput.Size = new Size(350, 280);
        groupBoxInput.TabIndex = 0;
        groupBoxInput.TabStop = false;
        groupBoxInput.Text = "근무 시간 입력";
        //
        // labelShiftType
        //
        labelShiftType.AutoSize = true;
        labelShiftType.Location = new Point(6, 25);
        labelShiftType.Name = "labelShiftType";
        labelShiftType.Size = new Size(67, 15);
        labelShiftType.TabIndex = 14;
        labelShiftType.Text = "근무 타입:";
        //
        // comboBoxShiftType
        //
        comboBoxShiftType.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBoxShiftType.FormattingEnabled = true;
        comboBoxShiftType.Items.AddRange(new object[] { "오전 근무 (10:00-15:00)", "야간 근무 (19:00-02:00)", "사용자 정의" });
        comboBoxShiftType.Location = new Point(79, 22);
        comboBoxShiftType.Name = "comboBoxShiftType";
        comboBoxShiftType.Size = new Size(200, 23);
        comboBoxShiftType.TabIndex = 13;
        comboBoxShiftType.SelectedIndexChanged += comboBoxShiftType_SelectedIndexChanged;
        //
        // checkBoxCompleted
        //
        checkBoxCompleted.AutoSize = true;
        checkBoxCompleted.Location = new Point(79, 215);
        checkBoxCompleted.Name = "checkBoxCompleted";
        checkBoxCompleted.Size = new Size(62, 19);
        checkBoxCompleted.TabIndex = 12;
        checkBoxCompleted.Text = "마감됨";
        checkBoxCompleted.UseVisualStyleBackColor = true;
        //
        // textBoxNotes
        //
        textBoxNotes.Location = new Point(79, 180);
        textBoxNotes.Name = "textBoxNotes";
        textBoxNotes.Size = new Size(200, 23);
        textBoxNotes.TabIndex = 11;
        //
        // labelNotes
        //
        labelNotes.AutoSize = true;
        labelNotes.Location = new Point(6, 183);
        labelNotes.Name = "labelNotes";
        labelNotes.Size = new Size(35, 15);
        labelNotes.TabIndex = 10;
        labelNotes.Text = "메모:";
        //
        // numericUpDownRevenue
        //
        numericUpDownRevenue.DecimalPlaces = 0;
        numericUpDownRevenue.Location = new Point(79, 151);
        numericUpDownRevenue.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
        numericUpDownRevenue.Name = "numericUpDownRevenue";
        numericUpDownRevenue.Size = new Size(200, 23);
        numericUpDownRevenue.TabIndex = 9;
        //
        // labelRevenue
        //
        labelRevenue.AutoSize = true;
        labelRevenue.Location = new Point(6, 153);
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
        checkBoxNightShift.TabIndex = 7;
        checkBoxNightShift.Text = "야간 근무";
        checkBoxNightShift.UseVisualStyleBackColor = true;
        //
        // dateTimePickerEnd
        //
        dateTimePickerEnd.CustomFormat = "HH:mm";
        dateTimePickerEnd.Format = DateTimePickerFormat.Custom;
        dateTimePickerEnd.Location = new Point(79, 97);
        dateTimePickerEnd.Name = "dateTimePickerEnd";
        dateTimePickerEnd.ShowUpDown = true;
        dateTimePickerEnd.Size = new Size(200, 23);
        dateTimePickerEnd.TabIndex = 6;
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
        dateTimePickerStart.CustomFormat = "HH:mm";
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
        // dateTimePicker1
        //
        dateTimePicker1.Location = new Point(79, 51);
        dateTimePicker1.Name = "dateTimePicker1";
        dateTimePicker1.Size = new Size(200, 23);
        dateTimePicker1.TabIndex = 2;
        //
        // labelDate
        //
        labelDate.AutoSize = true;
        labelDate.Location = new Point(6, 57);
        labelDate.Name = "labelDate";
        labelDate.Size = new Size(35, 15);
        labelDate.TabIndex = 1;
        labelDate.Text = "날짜:";
        //
        // btnAddShift
        //
        btnAddShift.Location = new Point(204, 240);
        btnAddShift.Name = "btnAddShift";
        btnAddShift.Size = new Size(75, 23);
        btnAddShift.TabIndex = 0;
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
        groupBoxList.Size = new Size(600, 400);
        groupBoxList.TabIndex = 1;
        groupBoxList.TabStop = false;
        groupBoxList.Text = "근무 시간 목록";
        //
        // btnDeleteShift
        //
        btnDeleteShift.Location = new Point(519, 371);
        btnDeleteShift.Name = "btnDeleteShift";
        btnDeleteShift.Size = new Size(75, 23);
        btnDeleteShift.TabIndex = 1;
        btnDeleteShift.Text = "삭제";
        btnDeleteShift.UseVisualStyleBackColor = true;
        btnDeleteShift.Click += btnDeleteShift_Click;
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
        dataGridView1.Size = new Size(588, 343);
        dataGridView1.TabIndex = 0;
        //
        // groupBoxRevenue
        //
        groupBoxRevenue.Controls.Add(labelMonthlyRevenue);
        groupBoxRevenue.Controls.Add(btnMonthlyRevenue);
        groupBoxRevenue.Controls.Add(dateTimePickerMonth);
        groupBoxRevenue.Controls.Add(labelMonth);
        groupBoxRevenue.Location = new Point(12, 310);
        groupBoxRevenue.Name = "groupBoxRevenue";
        groupBoxRevenue.Size = new Size(350, 102);
        groupBoxRevenue.TabIndex = 2;
        groupBoxRevenue.TabStop = false;
        groupBoxRevenue.Text = "매출 조회";
        //
        // labelMonthlyRevenue
        //
        labelMonthlyRevenue.AutoSize = true;
        labelMonthlyRevenue.Font = new Font("맑은 고딕", 9F, FontStyle.Bold);
        labelMonthlyRevenue.Location = new Point(6, 70);
        labelMonthlyRevenue.Name = "labelMonthlyRevenue";
        labelMonthlyRevenue.Size = new Size(127, 15);
        labelMonthlyRevenue.TabIndex = 3;
        labelMonthlyRevenue.Text = "월별 매출이 여기 표시";
        //
        // btnMonthlyRevenue
        //
        btnMonthlyRevenue.Location = new Point(204, 40);
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
        dateTimePickerMonth.Location = new Point(79, 40);
        dateTimePickerMonth.Name = "dateTimePickerMonth";
        dateTimePickerMonth.Size = new Size(119, 23);
        dateTimePickerMonth.TabIndex = 1;
        //
        // labelMonth
        //
        labelMonth.AutoSize = true;
        labelMonth.Location = new Point(6, 46);
        labelMonth.Name = "labelMonth";
        labelMonth.Size = new Size(35, 15);
        labelMonth.TabIndex = 0;
        labelMonth.Text = "월별:";
        //
        // Form1
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 430);
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
        ResumeLayout(false);
    }

    #endregion

    private GroupBox groupBoxInput;
    private Label labelShiftType;
    private ComboBox comboBoxShiftType;
    private CheckBox checkBoxCompleted;
    private TextBox textBoxNotes;
    private Label labelNotes;
    private NumericUpDown numericUpDownRevenue;
    private Label labelRevenue;
    private CheckBox checkBoxNightShift;
    private DateTimePicker dateTimePickerEnd;
    private Label labelEndTime;
    private DateTimePicker dateTimePickerStart;
    private Label labelStartTime;
    private DateTimePicker dateTimePicker1;
    private Label labelDate;
    private Button btnAddShift;
    private GroupBox groupBoxList;
    private Button btnDeleteShift;
    private DataGridView dataGridView1;
    private GroupBox groupBoxRevenue;
    private Label labelMonthlyRevenue;
    private Button btnMonthlyRevenue;
    private DateTimePicker dateTimePickerMonth;
    private Label labelMonth;
}
