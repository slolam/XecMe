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
    public partial class ParallelTaskControl : BaseControl
    {
        public ParallelTaskControl()
        {
            InitializeComponent();
        }

        public override BaseTask Entity
        {
            get
            {
                if (base.Entity == null)
                    base.Entity = new ParallelTask();

                ParallelTask entity = base.Entity as ParallelTask;

                entity.MinimumInstances = (int)minTrack.Value;
                entity.MaximumInstances = (int)maxTrack.Value;
                entity.IdlePollingPeriod = (int)idleTrack.Value;
                
                return entity;
            }
            set
            {
                ParallelTask entity = value as ParallelTask;
                if (entity != null)
                {
                    maxTrack.Value = entity.MaximumInstances;
                    minTrack.Value = entity.MinimumInstances;
                    idleTrack.Value = entity.IdlePollingPeriod;

                    base.Entity = entity;
                }
            }
        }

        private void maxTrack_Scroll(object sender, EventArgs e)
        {
            minTrack.Maximum = maxTrack.Value;

            if (minTrack.Value > maxTrack.Value)
                minTrack.Value = maxTrack.Value;
            
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
