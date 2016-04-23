namespace XecMeConfig
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.loadTaskTypesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearTasksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabEditTask = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgTasks = new System.Windows.Forms.DataGridView();
            this.dgName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgTask = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.tasTasks = new System.Windows.Forms.TabPage();
            this.chkList = new System.Windows.Forms.CheckedListBox();
            this.menuStrip1.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabEditTask.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgTasks)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadTaskTypesToolStripMenuItem,
            this.clearTasksToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(598, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuLoad";
            // 
            // loadTaskTypesToolStripMenuItem
            // 
            this.loadTaskTypesToolStripMenuItem.Name = "loadTaskTypesToolStripMenuItem";
            this.loadTaskTypesToolStripMenuItem.Size = new System.Drawing.Size(77, 20);
            this.loadTaskTypesToolStripMenuItem.Text = "Load Tasks";
            this.loadTaskTypesToolStripMenuItem.Click += new System.EventHandler(this.loadTaskTypesToolStripMenuItem_Click);
            // 
            // clearTasksToolStripMenuItem
            // 
            this.clearTasksToolStripMenuItem.Name = "clearTasksToolStripMenuItem";
            this.clearTasksToolStripMenuItem.Size = new System.Drawing.Size(78, 20);
            this.clearTasksToolStripMenuItem.Text = "Clear Tasks";
            this.clearTasksToolStripMenuItem.Click += new System.EventHandler(this.clearTasksToolStripMenuItem_Click);
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabEditTask);
            this.tabMain.Controls.Add(this.tasTasks);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 24);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(598, 371);
            this.tabMain.TabIndex = 3;
            // 
            // tabEditTask
            // 
            this.tabEditTask.Controls.Add(this.splitContainer1);
            this.tabEditTask.Location = new System.Drawing.Point(4, 22);
            this.tabEditTask.Name = "tabEditTask";
            this.tabEditTask.Padding = new System.Windows.Forms.Padding(3);
            this.tabEditTask.Size = new System.Drawing.Size(590, 345);
            this.tabEditTask.TabIndex = 0;
            this.tabEditTask.Text = "Tasks";
            this.tabEditTask.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgTasks);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.chkList);
            this.splitContainer1.Size = new System.Drawing.Size(584, 339);
            this.splitContainer1.SplitterDistance = 194;
            this.splitContainer1.TabIndex = 0;
            // 
            // dgTasks
            // 
            this.dgTasks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgTasks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgName,
            this.dgTask});
            this.dgTasks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgTasks.Location = new System.Drawing.Point(0, 0);
            this.dgTasks.Name = "dgTasks";
            this.dgTasks.Size = new System.Drawing.Size(194, 339);
            this.dgTasks.TabIndex = 0;
            // 
            // dgName
            // 
            this.dgName.HeaderText = "Name";
            this.dgName.Name = "dgName";
            // 
            // dgTask
            // 
            this.dgTask.HeaderText = "Task";
            this.dgTask.Name = "dgTask";
            // 
            // tasTasks
            // 
            this.tasTasks.Location = new System.Drawing.Point(4, 22);
            this.tasTasks.Name = "tasTasks";
            this.tasTasks.Padding = new System.Windows.Forms.Padding(3);
            this.tasTasks.Size = new System.Drawing.Size(590, 345);
            this.tasTasks.TabIndex = 1;
            this.tasTasks.Text = "Task & Config";
            this.tasTasks.UseVisualStyleBackColor = true;
            // 
            // chkList
            // 
            this.chkList.Location = new System.Drawing.Point(20, 20);
            this.chkList.Name = "chkList";
            this.chkList.Size = new System.Drawing.Size(120, 94);
            this.chkList.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 395);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "XecMe Configuration";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabMain.ResumeLayout(false);
            this.tabEditTask.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgTasks)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem loadTaskTypesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearTasksToolStripMenuItem;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabEditTask;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabPage tasTasks;
        private System.Windows.Forms.DataGridView dgTasks;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgName;
        private System.Windows.Forms.DataGridViewComboBoxColumn dgTask;
        private System.Windows.Forms.CheckedListBox chkList;
    }
}

