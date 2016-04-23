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
            this._name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dvParameters)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dvParameters);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(6);
            this.groupBox1.Size = new System.Drawing.Size(548, 190);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parameters";
            // 
            // dvParameters
            // 
            this.dvParameters.AllowUserToOrderColumns = true;
            this.dvParameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dvParameters.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._name,
            this._value});
            this.dvParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dvParameters.Location = new System.Drawing.Point(6, 30);
            this.dvParameters.Margin = new System.Windows.Forms.Padding(6);
            this.dvParameters.Name = "dvParameters";
            this.dvParameters.Size = new System.Drawing.Size(536, 154);
            this.dvParameters.TabIndex = 2;
            // 
            // _name
            // 
            this._name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._name.HeaderText = "Name";
            this._name.Name = "_name";
            // 
            // _value
            // 
            this._value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._value.HeaderText = "Value";
            this._value.Name = "_value";
            // 
            // ParametersControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "ParametersControl";
            this.Size = new System.Drawing.Size(548, 190);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dvParameters)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dvParameters;
        private System.Windows.Forms.DataGridViewTextBoxColumn parameterName;
        private System.Windows.Forms.DataGridViewTextBoxColumn parameterValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn _name;
        private System.Windows.Forms.DataGridViewTextBoxColumn _value;

    }
}
