namespace XecMeConfig.Controls
{
    partial class ScheduledTaskControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.repeatTrack = new System.Windows.Forms.NumericUpDown();
            this.lblMin = new System.Windows.Forms.Label();
            this.dtStartTime = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.dtStartDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbRecursion = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbTz = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.grpWeekly = new System.Windows.Forms.GroupBox();
            this.chkWed = new System.Windows.Forms.CheckBox();
            this.chkSat = new System.Windows.Forms.CheckBox();
            this.chkFri = new System.Windows.Forms.CheckBox();
            this.chkTue = new System.Windows.Forms.CheckBox();
            this.chkThu = new System.Windows.Forms.CheckBox();
            this.chkMon = new System.Windows.Forms.CheckBox();
            this.chkSun = new System.Windows.Forms.CheckBox();
            this.grpMonthly = new System.Windows.Forms.GroupBox();
            this.chkOct = new System.Windows.Forms.CheckBox();
            this.chkApr = new System.Windows.Forms.CheckBox();
            this.chkDec = new System.Windows.Forms.CheckBox();
            this.chkSep = new System.Windows.Forms.CheckBox();
            this.chkJun = new System.Windows.Forms.CheckBox();
            this.chkNov = new System.Windows.Forms.CheckBox();
            this.chkMar = new System.Windows.Forms.CheckBox();
            this.chkAug = new System.Windows.Forms.CheckBox();
            this.chkMay = new System.Windows.Forms.CheckBox();
            this.chkJul = new System.Windows.Forms.CheckBox();
            this.chkFeb = new System.Windows.Forms.CheckBox();
            this.chkJan = new System.Windows.Forms.CheckBox();
            this.chkDays = new System.Windows.Forms.CheckedListBox();
            this.parameters = new XecMeConfig.Controls.ParametersControl();
            ((System.ComponentModel.ISupportInitialize)(this.repeatTrack)).BeginInit();
            this.grpWeekly.SuspendLayout();
            this.grpMonthly.SuspendLayout();
            this.SuspendLayout();
            // 
            // repeatTrack
            // 
            this.repeatTrack.Location = new System.Drawing.Point(248, 137);
            this.repeatTrack.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.repeatTrack.Maximum = new decimal(new int[] {
            365,
            0,
            0,
            0});
            this.repeatTrack.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.repeatTrack.Name = "repeatTrack";
            this.repeatTrack.Size = new System.Drawing.Size(240, 31);
            this.repeatTrack.TabIndex = 6;
            this.repeatTrack.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblMin
            // 
            this.lblMin.AutoSize = true;
            this.lblMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMin.Location = new System.Drawing.Point(20, 22);
            this.lblMin.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblMin.Name = "lblMin";
            this.lblMin.Size = new System.Drawing.Size(53, 13);
            this.lblMin.TabIndex = 7;
            this.lblMin.Text = "Start date";
            // 
            // dtStartTime
            // 
            this.dtStartTime.CustomFormat = "hh:mm:ss tt";
            this.dtStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtStartTime.Location = new System.Drawing.Point(248, 80);
            this.dtStartTime.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.dtStartTime.Name = "dtStartTime";
            this.dtStartTime.Size = new System.Drawing.Size(236, 31);
            this.dtStartTime.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(18, 80);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Task time";
            // 
            // dtStartDate
            // 
            this.dtStartDate.CustomFormat = "MM/dd/yyyy";
            this.dtStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtStartDate.Location = new System.Drawing.Point(248, 22);
            this.dtStartDate.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.dtStartDate.Name = "dtStartDate";
            this.dtStartDate.Size = new System.Drawing.Size(236, 31);
            this.dtStartDate.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(20, 253);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Recursion";
            // 
            // cmbRecursion
            // 
            this.cmbRecursion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRecursion.FormattingEnabled = true;
            this.cmbRecursion.Location = new System.Drawing.Point(248, 253);
            this.cmbRecursion.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cmbRecursion.Name = "cmbRecursion";
            this.cmbRecursion.Size = new System.Drawing.Size(238, 33);
            this.cmbRecursion.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(20, 137);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Repeat";
            // 
            // cmbTz
            // 
            this.cmbTz.FormattingEnabled = true;
            this.cmbTz.Location = new System.Drawing.Point(248, 195);
            this.cmbTz.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cmbTz.Name = "cmbTz";
            this.cmbTz.Size = new System.Drawing.Size(648, 33);
            this.cmbTz.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(20, 195);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Time zome";
            // 
            // grpWeekly
            // 
            this.grpWeekly.Controls.Add(this.chkWed);
            this.grpWeekly.Controls.Add(this.chkSat);
            this.grpWeekly.Controls.Add(this.chkFri);
            this.grpWeekly.Controls.Add(this.chkTue);
            this.grpWeekly.Controls.Add(this.chkThu);
            this.grpWeekly.Controls.Add(this.chkMon);
            this.grpWeekly.Controls.Add(this.chkSun);
            this.grpWeekly.Location = new System.Drawing.Point(24, 297);
            this.grpWeekly.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.grpWeekly.Name = "grpWeekly";
            this.grpWeekly.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.grpWeekly.Size = new System.Drawing.Size(872, 88);
            this.grpWeekly.TabIndex = 14;
            this.grpWeekly.TabStop = false;
            this.grpWeekly.Text = "Weekly";
            // 
            // chkWed
            // 
            this.chkWed.AutoSize = true;
            this.chkWed.Location = new System.Drawing.Point(624, 24);
            this.chkWed.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chkWed.Name = "chkWed";
            this.chkWed.Size = new System.Drawing.Size(145, 29);
            this.chkWed.TabIndex = 0;
            this.chkWed.Text = "Wednesday";
            this.chkWed.UseVisualStyleBackColor = true;
            // 
            // chkSat
            // 
            this.chkSat.AutoSize = true;
            this.chkSat.Location = new System.Drawing.Point(426, 51);
            this.chkSat.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chkSat.Name = "chkSat";
            this.chkSat.Size = new System.Drawing.Size(117, 29);
            this.chkSat.TabIndex = 0;
            this.chkSat.Text = "Saturday";
            this.chkSat.UseVisualStyleBackColor = true;
            // 
            // chkFri
            // 
            this.chkFri.AutoSize = true;
            this.chkFri.Location = new System.Drawing.Point(224, 51);
            this.chkFri.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chkFri.Name = "chkFri";
            this.chkFri.Size = new System.Drawing.Size(91, 29);
            this.chkFri.TabIndex = 0;
            this.chkFri.Text = "Friday";
            this.chkFri.UseVisualStyleBackColor = true;
            // 
            // chkTue
            // 
            this.chkTue.AutoSize = true;
            this.chkTue.Location = new System.Drawing.Point(426, 24);
            this.chkTue.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chkTue.Name = "chkTue";
            this.chkTue.Size = new System.Drawing.Size(114, 29);
            this.chkTue.TabIndex = 0;
            this.chkTue.Text = "Tuesday";
            this.chkTue.UseVisualStyleBackColor = true;
            // 
            // chkThu
            // 
            this.chkThu.AutoSize = true;
            this.chkThu.Location = new System.Drawing.Point(14, 53);
            this.chkThu.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chkThu.Name = "chkThu";
            this.chkThu.Size = new System.Drawing.Size(121, 29);
            this.chkThu.TabIndex = 0;
            this.chkThu.Text = "Thursday";
            this.chkThu.UseVisualStyleBackColor = true;
            // 
            // chkMon
            // 
            this.chkMon.AutoSize = true;
            this.chkMon.Location = new System.Drawing.Point(224, 24);
            this.chkMon.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chkMon.Name = "chkMon";
            this.chkMon.Size = new System.Drawing.Size(108, 29);
            this.chkMon.TabIndex = 0;
            this.chkMon.Text = "Monday";
            this.chkMon.UseVisualStyleBackColor = true;
            // 
            // chkSun
            // 
            this.chkSun.AutoSize = true;
            this.chkSun.Location = new System.Drawing.Point(14, 25);
            this.chkSun.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chkSun.Name = "chkSun";
            this.chkSun.Size = new System.Drawing.Size(104, 29);
            this.chkSun.TabIndex = 0;
            this.chkSun.Text = "Sunday";
            this.chkSun.UseVisualStyleBackColor = true;
            // 
            // grpMonthly
            // 
            this.grpMonthly.Controls.Add(this.chkOct);
            this.grpMonthly.Controls.Add(this.chkApr);
            this.grpMonthly.Controls.Add(this.chkDec);
            this.grpMonthly.Controls.Add(this.chkSep);
            this.grpMonthly.Controls.Add(this.chkJun);
            this.grpMonthly.Controls.Add(this.chkNov);
            this.grpMonthly.Controls.Add(this.chkMar);
            this.grpMonthly.Controls.Add(this.chkAug);
            this.grpMonthly.Controls.Add(this.chkMay);
            this.grpMonthly.Controls.Add(this.chkJul);
            this.grpMonthly.Controls.Add(this.chkFeb);
            this.grpMonthly.Controls.Add(this.chkJan);
            this.grpMonthly.Location = new System.Drawing.Point(26, 388);
            this.grpMonthly.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.grpMonthly.Name = "grpMonthly";
            this.grpMonthly.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.grpMonthly.Size = new System.Drawing.Size(870, 114);
            this.grpMonthly.TabIndex = 15;
            this.grpMonthly.TabStop = false;
            this.grpMonthly.Text = "Monthly";
            // 
            // chkOct
            // 
            this.chkOct.AutoSize = true;
            this.chkOct.Location = new System.Drawing.Point(190, 76);
            this.chkOct.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chkOct.Name = "chkOct";
            this.chkOct.Size = new System.Drawing.Size(107, 29);
            this.chkOct.TabIndex = 0;
            this.chkOct.Text = "October";
            this.chkOct.UseVisualStyleBackColor = true;
            // 
            // chkApr
            // 
            this.chkApr.AutoSize = true;
            this.chkApr.Location = new System.Drawing.Point(536, 21);
            this.chkApr.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chkApr.Name = "chkApr";
            this.chkApr.Size = new System.Drawing.Size(74, 29);
            this.chkApr.TabIndex = 0;
            this.chkApr.Text = "April";
            this.chkApr.UseVisualStyleBackColor = true;
            // 
            // chkDec
            // 
            this.chkDec.AutoSize = true;
            this.chkDec.Location = new System.Drawing.Point(536, 76);
            this.chkDec.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chkDec.Name = "chkDec";
            this.chkDec.Size = new System.Drawing.Size(129, 29);
            this.chkDec.TabIndex = 0;
            this.chkDec.Text = "December";
            this.chkDec.UseVisualStyleBackColor = true;
            // 
            // chkSep
            // 
            this.chkSep.AutoSize = true;
            this.chkSep.Location = new System.Drawing.Point(14, 76);
            this.chkSep.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chkSep.Name = "chkSep";
            this.chkSep.Size = new System.Drawing.Size(135, 29);
            this.chkSep.TabIndex = 0;
            this.chkSep.Text = "September";
            this.chkSep.UseVisualStyleBackColor = true;
            // 
            // chkJun
            // 
            this.chkJun.AutoSize = true;
            this.chkJun.Location = new System.Drawing.Point(190, 52);
            this.chkJun.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chkJun.Name = "chkJun";
            this.chkJun.Size = new System.Drawing.Size(78, 29);
            this.chkJun.TabIndex = 0;
            this.chkJun.Text = "June";
            this.chkJun.UseVisualStyleBackColor = true;
            // 
            // chkNov
            // 
            this.chkNov.AutoSize = true;
            this.chkNov.Location = new System.Drawing.Point(374, 76);
            this.chkNov.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chkNov.Name = "chkNov";
            this.chkNov.Size = new System.Drawing.Size(129, 29);
            this.chkNov.TabIndex = 0;
            this.chkNov.Text = "November";
            this.chkNov.UseVisualStyleBackColor = true;
            // 
            // chkMar
            // 
            this.chkMar.AutoSize = true;
            this.chkMar.Location = new System.Drawing.Point(374, 21);
            this.chkMar.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chkMar.Name = "chkMar";
            this.chkMar.Size = new System.Drawing.Size(91, 29);
            this.chkMar.TabIndex = 0;
            this.chkMar.Text = "March";
            this.chkMar.UseVisualStyleBackColor = true;
            // 
            // chkAug
            // 
            this.chkAug.AutoSize = true;
            this.chkAug.Location = new System.Drawing.Point(536, 52);
            this.chkAug.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chkAug.Name = "chkAug";
            this.chkAug.Size = new System.Drawing.Size(98, 29);
            this.chkAug.TabIndex = 0;
            this.chkAug.Text = "August";
            this.chkAug.UseVisualStyleBackColor = true;
            // 
            // chkMay
            // 
            this.chkMay.AutoSize = true;
            this.chkMay.Location = new System.Drawing.Point(14, 52);
            this.chkMay.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chkMay.Name = "chkMay";
            this.chkMay.Size = new System.Drawing.Size(72, 29);
            this.chkMay.TabIndex = 0;
            this.chkMay.Text = "May";
            this.chkMay.UseVisualStyleBackColor = true;
            // 
            // chkJul
            // 
            this.chkJul.AutoSize = true;
            this.chkJul.Location = new System.Drawing.Point(374, 52);
            this.chkJul.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chkJul.Name = "chkJul";
            this.chkJul.Size = new System.Drawing.Size(70, 29);
            this.chkJul.TabIndex = 0;
            this.chkJul.Text = "July";
            this.chkJul.UseVisualStyleBackColor = true;
            // 
            // chkFeb
            // 
            this.chkFeb.AutoSize = true;
            this.chkFeb.Location = new System.Drawing.Point(190, 21);
            this.chkFeb.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chkFeb.Name = "chkFeb";
            this.chkFeb.Size = new System.Drawing.Size(117, 29);
            this.chkFeb.TabIndex = 0;
            this.chkFeb.Text = "Febraury";
            this.chkFeb.UseVisualStyleBackColor = true;
            // 
            // chkJan
            // 
            this.chkJan.AutoSize = true;
            this.chkJan.Location = new System.Drawing.Point(14, 22);
            this.chkJan.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chkJan.Name = "chkJan";
            this.chkJan.Size = new System.Drawing.Size(108, 29);
            this.chkJan.TabIndex = 0;
            this.chkJan.Text = "January";
            this.chkJan.UseVisualStyleBackColor = true;
            // 
            // chkDays
            // 
            this.chkDays.FormattingEnabled = true;
            this.chkDays.Location = new System.Drawing.Point(26, 514);
            this.chkDays.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chkDays.Name = "chkDays";
            this.chkDays.Size = new System.Drawing.Size(870, 56);
            this.chkDays.TabIndex = 16;
            // 
            // parameters
            // 
            this.parameters.DataSource = null;
            this.parameters.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.parameters.Location = new System.Drawing.Point(0, 579);
            this.parameters.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.parameters.Name = "parameters";
            this.parameters.Size = new System.Drawing.Size(913, 190);
            this.parameters.TabIndex = 17;
            // 
            // ScheduledTaskControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.parameters);
            this.Controls.Add(this.chkDays);
            this.Controls.Add(this.grpMonthly);
            this.Controls.Add(this.grpWeekly);
            this.Controls.Add(this.cmbTz);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cmbRecursion);
            this.Controls.Add(this.dtStartTime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dtStartDate);
            this.Controls.Add(this.repeatTrack);
            this.Controls.Add(this.lblMin);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "ScheduledTaskControl";
            this.Size = new System.Drawing.Size(913, 769);
            ((System.ComponentModel.ISupportInitialize)(this.repeatTrack)).EndInit();
            this.grpWeekly.ResumeLayout(false);
            this.grpWeekly.PerformLayout();
            this.grpMonthly.ResumeLayout(false);
            this.grpMonthly.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown repeatTrack;
        private System.Windows.Forms.Label lblMin;
        private System.Windows.Forms.DateTimePicker dtStartTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtStartDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbRecursion;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbTz;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox grpWeekly;
        private System.Windows.Forms.CheckBox chkWed;
        private System.Windows.Forms.CheckBox chkSat;
        private System.Windows.Forms.CheckBox chkFri;
        private System.Windows.Forms.CheckBox chkTue;
        private System.Windows.Forms.CheckBox chkThu;
        private System.Windows.Forms.CheckBox chkMon;
        private System.Windows.Forms.CheckBox chkSun;
        private System.Windows.Forms.GroupBox grpMonthly;
        private System.Windows.Forms.CheckBox chkOct;
        private System.Windows.Forms.CheckBox chkApr;
        private System.Windows.Forms.CheckBox chkDec;
        private System.Windows.Forms.CheckBox chkSep;
        private System.Windows.Forms.CheckBox chkJun;
        private System.Windows.Forms.CheckBox chkNov;
        private System.Windows.Forms.CheckBox chkMar;
        private System.Windows.Forms.CheckBox chkAug;
        private System.Windows.Forms.CheckBox chkMay;
        private System.Windows.Forms.CheckBox chkJul;
        private System.Windows.Forms.CheckBox chkFeb;
        private System.Windows.Forms.CheckBox chkJan;
        private System.Windows.Forms.CheckedListBox chkDays;
        private ParametersControl parameters;
    }
}
