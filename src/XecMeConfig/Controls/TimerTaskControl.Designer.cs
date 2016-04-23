namespace XecMeConfig.Controls
{
    partial class TimerTaskControl
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
            this.lblMin = new System.Windows.Forms.Label();
            this.intervalTrack = new System.Windows.Forms.NumericUpDown();
            this.dtStartDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dtEndDate = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.dtStartTime = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.dtEndTime = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.recurrenceTrack = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbTz = new System.Windows.Forms.ComboBox();
            this.parameters = new XecMeConfig.Controls.ParametersControl();
            ((System.ComponentModel.ISupportInitialize)(this.intervalTrack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.recurrenceTrack)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMin
            // 
            this.lblMin.AutoSize = true;
            this.lblMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMin.Location = new System.Drawing.Point(6, 28);
            this.lblMin.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblMin.Name = "lblMin";
            this.lblMin.Size = new System.Drawing.Size(42, 13);
            this.lblMin.TabIndex = 5;
            this.lblMin.Text = "Interval";
            // 
            // intervalTrack
            // 
            this.intervalTrack.Location = new System.Drawing.Point(76, 16);
            this.intervalTrack.Margin = new System.Windows.Forms.Padding(6);
            this.intervalTrack.Maximum = new decimal(new int[] {
            86400000,
            0,
            0,
            0});
            this.intervalTrack.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.intervalTrack.Name = "intervalTrack";
            this.intervalTrack.Size = new System.Drawing.Size(260, 31);
            this.intervalTrack.TabIndex = 1;
            this.intervalTrack.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // dtStartDate
            // 
            this.dtStartDate.CustomFormat = "MM/dd/yyyy";
            this.dtStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtStartDate.Location = new System.Drawing.Point(76, 74);
            this.dtStartDate.Margin = new System.Windows.Forms.Padding(6);
            this.dtStartDate.Name = "dtStartDate";
            this.dtStartDate.Size = new System.Drawing.Size(260, 31);
            this.dtStartDate.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 86);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Start date";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 143);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "End Date";
            // 
            // dtEndDate
            // 
            this.dtEndDate.CustomFormat = "MM/dd/yyyy";
            this.dtEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtEndDate.Location = new System.Drawing.Point(76, 131);
            this.dtEndDate.Margin = new System.Windows.Forms.Padding(6);
            this.dtEndDate.Name = "dtEndDate";
            this.dtEndDate.Size = new System.Drawing.Size(260, 31);
            this.dtEndDate.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(384, 86);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Start time";
            // 
            // dtStartTime
            // 
            this.dtStartTime.CustomFormat = "hh:mm:ss tt";
            this.dtStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtStartTime.Location = new System.Drawing.Point(494, 74);
            this.dtStartTime.Margin = new System.Windows.Forms.Padding(6);
            this.dtStartTime.Name = "dtStartTime";
            this.dtStartTime.Size = new System.Drawing.Size(260, 31);
            this.dtStartTime.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(384, 143);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "End time";
            // 
            // dtEndTime
            // 
            this.dtEndTime.CustomFormat = "hh:mm:ss tt";
            this.dtEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtEndTime.Location = new System.Drawing.Point(494, 129);
            this.dtEndTime.Margin = new System.Windows.Forms.Padding(6);
            this.dtEndTime.Name = "dtEndTime";
            this.dtEndTime.Size = new System.Drawing.Size(260, 31);
            this.dtEndTime.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(384, 27);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Recurrence";
            // 
            // recurrenceTrack
            // 
            this.recurrenceTrack.Location = new System.Drawing.Point(494, 17);
            this.recurrenceTrack.Margin = new System.Windows.Forms.Padding(6);
            this.recurrenceTrack.Name = "recurrenceTrack";
            this.recurrenceTrack.Size = new System.Drawing.Size(260, 31);
            this.recurrenceTrack.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(6, 197);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Time zome";
            // 
            // cmbTz
            // 
            this.cmbTz.FormattingEnabled = true;
            this.cmbTz.Location = new System.Drawing.Point(76, 185);
            this.cmbTz.Margin = new System.Windows.Forms.Padding(6);
            this.cmbTz.Name = "cmbTz";
            this.cmbTz.Size = new System.Drawing.Size(678, 33);
            this.cmbTz.TabIndex = 7;
            // 
            // parameters
            // 
            this.parameters.DataSource = null;
            this.parameters.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.parameters.Location = new System.Drawing.Point(0, 425);
            this.parameters.Margin = new System.Windows.Forms.Padding(12);
            this.parameters.Name = "parameters";
            this.parameters.Size = new System.Drawing.Size(764, 262);
            this.parameters.TabIndex = 8;
            // 
            // TimerTaskControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.parameters);
            this.Controls.Add(this.cmbTz);
            this.Controls.Add(this.dtEndTime);
            this.Controls.Add(this.dtStartTime);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dtEndDate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dtStartDate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.recurrenceTrack);
            this.Controls.Add(this.intervalTrack);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblMin);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "TimerTaskControl";
            this.Size = new System.Drawing.Size(764, 687);
            ((System.ComponentModel.ISupportInitialize)(this.intervalTrack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.recurrenceTrack)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMin;
        private System.Windows.Forms.NumericUpDown intervalTrack;
        private System.Windows.Forms.DateTimePicker dtStartDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtEndDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtStartTime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtEndTime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown recurrenceTrack;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbTz;
        private ParametersControl parameters;
    }
}
