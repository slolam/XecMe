namespace XecMeConfig.Controls
{
    partial class ParametersControl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dvParameters = new System.Windows.Forms.DataGridView();
            this.parameterName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.parameterValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dvParameters)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dvParameters);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(274, 99);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parameters";
            // 
            // dvParameters
            // 
            this.dvParameters.AllowUserToOrderColumns = true;
            this.dvParameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dvParameters.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.parameterName,
            this.parameterValue});
            this.dvParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dvParameters.Location = new System.Drawing.Point(3, 16);
            this.dvParameters.Name = "dvParameters";
            this.dvParameters.Size = new System.Drawing.Size(268, 80);
            this.dvParameters.TabIndex = 2;
            // 
            // Name
            // 
            this.parameterName.HeaderText = "Name";
            this.parameterName.Name = "Name";
            // 
            // Value
            // 
            this.parameterValue.HeaderText = "Value";
            this.parameterValue.Name = "Value";
            // 
            // ParametersControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "ParametersControl";
            this.Size = new System.Drawing.Size(274, 99);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dvParameters)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dvParameters;
        private System.Windows.Forms.DataGridViewTextBoxColumn parameterName;
        private System.Windows.Forms.DataGridViewTextBoxColumn parameterValue;

    }
}
