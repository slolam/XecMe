using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XecMeConfig.Entities;

namespace XecMeConfig.Controls
{
    public partial class RunOnceControl : BaseControl
    {
        public RunOnceControl()
        {
            InitializeComponent();
        }


        public override BaseTask Entity
        {
            get
            {
                if (base.Entity == null)
                    base.Entity = new RunOnceTask();

                RunOnceTask entity = base.Entity as RunOnceTask;

                entity.Delay = (int)delayTrack.Value; 

                return entity; ;
            }
            set
            {
                base.Entity = value as RunOnceTask;
            }
        }

        protected override List<Parameter> DataSource
        {
            set
            {
                parameters.DataSource = value;
            }
        }

    }

    
}
