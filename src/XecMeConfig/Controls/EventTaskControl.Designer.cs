namespace XecMeConfig.Controls
{
    partial class EventTaskControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbThreadOptions = new System.Windows.Forms.ComboBox();
            this.txtTopicName = new System.Windows.Forms.TextBox();
            this.timeoutTrack = new System.Windows.Forms.NumericUpDown();
            this.parameters = new XecMeConfig.Controls.ParametersControl();
            ((System.ComponentModel.ISupportInitialize)(this.timeoutTrack)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMin
            // 
            this.lblMin.AutoSize = true;
            this.lblMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMin.Location = new System.Drawing.Point(6, 39);
            this.lblMin.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblMin.Name = "lblMin";
            this.lblMin.Size = new System.Drawing.Size(63, 13);
            this.lblMin.TabIndex = 1;
            this.lblMin.Text = "Topic name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 97);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Timeout";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 154);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Thread options";
            // 
            // cmbThreadOptions
            // 
            this.cmbThreadOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbThreadOptions.FormattingEnabled = true;
            this.cmbThreadOptions.Items.AddRange(new object[] {
            "BackgroundParallel",
            "BackgroundSerial",
            "Publisher"});
            this.cmbThreadOptions.Location = new System.Drawing.Point(176, 142);
            this.cmbThreadOptions.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cmbThreadOptions.Name = "cmbThreadOptions";
            this.cmbThreadOptions.Size = new System.Drawing.Size(520, 33);
            this.cmbThreadOptions.TabIndex = 3;
            // 
            // txtTopicName
            // 
            this.txtTopicName.Location = new System.Drawing.Point(176, 27);
            this.txtTopicName.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtTopicName.Name = "txtTopicName";
            this.txtTopicName.Size = new System.Drawing.Size(520, 31);
            this.txtTopicName.TabIndex = 1;
            // 
            // timeoutTrack
            // 
            this.timeoutTrack.Location = new System.Drawing.Point(176, 85);
            this.timeoutTrack.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.timeoutTrack.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.timeoutTrack.Name = "timeoutTrack";
            this.timeoutTrack.Size = new System.Drawing.Size(240, 31);
            this.timeoutTrack.TabIndex = 2;
            // 
            // parameters
            // 
            this.parameters.DataSource = null;
            this.parameters.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.parameters.Location = new System.Drawing.Point(0, 194);
            this.parameters.Margin = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.parameters.Name = "parameters";
            this.parameters.Size = new System.Drawing.Size(702, 171);
            this.parameters.TabIndex = 4;
            // 
            // EventTaskControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.parameters);
            this.Controls.Add(this.timeoutTrack);
            this.Controls.Add(this.txtTopicName);
            this.Controls.Add(this.cmbThreadOptions);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblMin);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "EventTaskControl";
            this.Size = new System.Drawing.Size(702, 365);
            ((System.ComponentModel.ISupportInitialize)(this.timeoutTrack)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbThreadOptions;
        private System.Windows.Forms.TextBox txtTopicName;
        private System.Windows.Forms.NumericUpDown timeoutTrack;
        private ParametersControl parameters;
    }
}
