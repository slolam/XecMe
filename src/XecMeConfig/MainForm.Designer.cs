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
            this.eventTaskControl1 = new XecMeConfig.Controls.EventTaskControl();
            this.tasTasks = new System.Windows.Forms.TabPage();
            this._name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this._task = new System.Windows.Forms.DataGridViewComboBoxColumn();
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
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(12, 4, 0, 4);
            this.menuStrip1.Size = new System.Drawing.Size(1196, 27);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuLoad";
            // 
            // loadTaskTypesToolStripMenuItem
            // 
            this.loadTaskTypesToolStripMenuItem.Name = "loadTaskTypesToolStripMenuItem";
            this.loadTaskTypesToolStripMenuItem.Size = new System.Drawing.Size(77, 19);
            this.loadTaskTypesToolStripMenuItem.Text = "Load Tasks";
            this.loadTaskTypesToolStripMenuItem.Click += new System.EventHandler(this.loadTaskTypesToolStripMenuItem_Click);
            // 
            // clearTasksToolStripMenuItem
            // 
            this.clearTasksToolStripMenuItem.Name = "clearTasksToolStripMenuItem";
            this.clearTasksToolStripMenuItem.Size = new System.Drawing.Size(78, 19);
            this.clearTasksToolStripMenuItem.Text = "Clear Tasks";
            this.clearTasksToolStripMenuItem.Click += new System.EventHandler(this.clearTasksToolStripMenuItem_Click);
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabEditTask);
            this.tabMain.Controls.Add(this.tasTasks);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 27);
            this.tabMain.Margin = new System.Windows.Forms.Padding(6);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(1196, 733);
            this.tabMain.TabIndex = 3;
            // 
            // tabEditTask
            // 
            this.tabEditTask.Controls.Add(this.splitContainer1);
            this.tabEditTask.Location = new System.Drawing.Point(4, 34);
            this.tabEditTask.Margin = new System.Windows.Forms.Padding(6);
            this.tabEditTask.Name = "tabEditTask";
            this.tabEditTask.Padding = new System.Windows.Forms.Padding(6);
            this.tabEditTask.Size = new System.Drawing.Size(1188, 695);
            this.tabEditTask.TabIndex = 0;
            this.tabEditTask.Text = "Tasks";
            this.tabEditTask.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(6, 6);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(6);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgTasks);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.eventTaskControl1);
            this.splitContainer1.Size = new System.Drawing.Size(1176, 683);
            this.splitContainer1.SplitterDistance = 390;
            this.splitContainer1.SplitterWidth = 8;
            this.splitContainer1.TabIndex = 0;
            // 
            // dgTasks
            // 
            this.dgTasks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgTasks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._name,
            this._type,
            this._task});
            this.dgTasks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgTasks.Location = new System.Drawing.Point(0, 0);
            this.dgTasks.Margin = new System.Windows.Forms.Padding(6);
            this.dgTasks.Name = "dgTasks";
            this.dgTasks.Size = new System.Drawing.Size(390, 683);
            this.dgTasks.TabIndex = 0;
            // 
            // eventTaskControl1
            // 
            this.eventTaskControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eventTaskControl1.Entity = null;
            this.eventTaskControl1.Location = new System.Drawing.Point(0, 0);
            this.eventTaskControl1.Margin = new System.Windows.Forms.Padding(6);
            this.eventTaskControl1.Name = "eventTaskControl1";
            this.eventTaskControl1.Size = new System.Drawing.Size(778, 683);
            this.eventTaskControl1.TabIndex = 1;
            // 
            // tasTasks
            // 
            this.tasTasks.Location = new System.Drawing.Point(4, 34);
            this.tasTasks.Margin = new System.Windows.Forms.Padding(6);
            this.tasTasks.Name = "tasTasks";
            this.tasTasks.Padding = new System.Windows.Forms.Padding(6);
            this.tasTasks.Size = new System.Drawing.Size(1188, 695);
            this.tasTasks.TabIndex = 1;
            this.tasTasks.Text = "Task & Config";
            this.tasTasks.UseVisualStyleBackColor = true;
            // 
            // _name
            // 
            this._name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._name.HeaderText = "Name";
            this._name.Name = "_name";
            // 
            // _type
            // 
            this._type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._type.HeaderText = "Type";
            this._type.Name = "_type";
            // 
            // _task
            // 
            this._task.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._task.HeaderText = "Task";
            this._task.Name = "_task";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1196, 760);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(6);
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
        private Controls.EventTaskControl eventTaskControl1;
        private System.Windows.Forms.DataGridViewTextBoxColumn _name;
        private System.Windows.Forms.DataGridViewComboBoxColumn _type;
        private System.Windows.Forms.DataGridViewComboBoxColumn _task;
    }
}

