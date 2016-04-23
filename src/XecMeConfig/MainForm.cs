using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace XecMeConfig
{
    public partial class MainForm : Form
    {

        HashSet<string> _taskTypes = new HashSet<string>();
        //List<string> _taskTypeList = null;
        public MainForm()
        {
            InitializeComponent();
        }

        private string[] GetTaskTypes(string[] files)
        {
            string[] retVal;
            AppDomain tempDomain = AppDomain.CreateDomain("temp", AppDomain.CurrentDomain.Evidence, AppDomain.CurrentDomain.BaseDirectory, Path.GetFullPath(files[0]), true);
            tempDomain.AssemblyResolve += new ResolveEventHandler(tempDomain_AssemblyResolve);
            TypeFinder taskFinder = (TypeFinder) tempDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, (typeof(TypeFinder)).FullName);
            retVal = taskFinder.GetTasksTypes(files, "XecMe.Core.Tasks.ITask");
            AppDomain.Unload(tempDomain);
            return retVal;
        }

        Assembly tempDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Trace.TraceError(args.Name);
            MessageBox.Show(string.Format("Cannot load the dependency {0}", args.RequestingAssembly));
            return null;
        }


        private void loadTaskTypesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog selectFile = new OpenFileDialog())
            {
                selectFile.Multiselect = true;
                selectFile.ReadOnlyChecked = true;
                selectFile.AutoUpgradeEnabled = true;
                selectFile.CheckFileExists = true;
                selectFile.CheckPathExists = true;
                selectFile.Filter = "Application Assembly(*.dll)|*.dll";
                selectFile.Title = "Select Application Assembly";
                if (selectFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string[] types = GetTaskTypes(selectFile.FileNames);
                    for (int i = 0; i < types.Length; i++)
                    {
                        _taskTypes.Add(types[i]);
                    }
                    dgTask.DataSource = _taskTypes.ToList<string>();
                    
                    //_taskTypes.AddRange(GetTaskTypes(selectFile.FileNames));
                }
            }
        }

        private void clearTasksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _taskTypes.Clear();
            dgTask.DataSource = null;
            chkList.Items.Add("Monday");
            chkList.Items.Add("Tuesday");
            chkList.Items.Add("Wednesday");
            chkList.Items.Add("Thursday");
            chkList.Items.Add("Friday");
            chkList.Items.Add("Saturday");
            chkList.Items.Add("Sunday");
        }
    }
}
