namespace XecMeConfig.Controls
{
    partial class ParallelTaskControl
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
            this.idleTrack = new System.Windows.Forms.NumericUpDown();
            this.maxTrack = new System.Windows.Forms.NumericUpDown();
            this.minTrack = new System.Windows.Forms.NumericUpDown();
            this.parameters = new XecMeConfig.Controls.ParametersControl();
            ((System.ComponentModel.ISupportInitialize)(this.idleTrack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxTrack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minTrack)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMin
            // 
            this.lblMin.AutoSize = true;
            this.lblMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMin.Location = new System.Drawing.Point(19, 14);
            this.lblMin.Name = "lblMin";
            this.lblMin.Size = new System.Drawing.Size(97, 13);
            this.lblMin.TabIndex = 0;
            this.lblMin.Text = "Minimum Instances";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(19, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Maximum Instances";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(19, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Idle Period (in ms)";
            // 
            // idleTrack
            // 
            this.idleTrack.Location = new System.Drawing.Point(133, 74);
            this.idleTrack.Maximum = new decimal(new int[] {
            1800000,
            0,
            0,
            0});
            this.idleTrack.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.idleTrack.Name = "idleTrack";
            this.idleTrack.Size = new System.Drawing.Size(120, 20);
            this.idleTrack.TabIndex = 3;
            this.idleTrack.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // maxTrack
            // 
            this.maxTrack.Location = new System.Drawing.Point(133, 44);
            this.maxTrack.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.maxTrack.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.maxTrack.Name = "maxTrack";
            this.maxTrack.Size = new System.Drawing.Size(120, 20);
            this.maxTrack.TabIndex = 2;
            this.maxTrack.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // minTrack
            // 
            this.minTrack.Location = new System.Drawing.Point(133, 14);
            this.minTrack.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.minTrack.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.minTrack.Name = "minTrack";
            this.minTrack.Size = new System.Drawing.Size(120, 20);
            this.minTrack.TabIndex = 1;
            this.minTrack.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // parameters
            // 
            this.parameters.DataSource = null;
            this.parameters.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.parameters.Location = new System.Drawing.Point(0, 100);
            this.parameters.Name = "parameters";
            this.parameters.Size = new System.Drawing.Size(525, 131);
            this.parameters.TabIndex = 4;
            // 
            // ParallelTaskControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.parameters);
            this.Controls.Add(this.minTrack);
            this.Controls.Add(this.maxTrack);
            this.Controls.Add(this.idleTrack);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblMin);
            this.Name = "ParallelTaskControl";
            this.Size = new System.Drawing.Size(525, 231);
            ((System.ComponentModel.ISupportInitialize)(this.idleTrack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxTrack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minTrack)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown idleTrack;
        private System.Windows.Forms.NumericUpDown maxTrack;
        private System.Windows.Forms.NumericUpDown minTrack;
        private ParametersControl parameters;
    }
}
