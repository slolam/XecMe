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
    public partial class TimerTaskControl : BaseControl
    {
        public TimerTaskControl()
        {
            InitializeComponent();
            cmbTz.DisplayMember = "Name";
            cmbTz.ValueMember = "Id";
            cmbTz.DataSource = XecMeConfig.Entities.TimeZone.TimeZones;
        }

        public override BaseTask Entity
        {
            get
            {
                if (base.Entity == null)
                    base.Entity = new TimerTask();

                TimerTask entity = base.Entity as TimerTask;

                entity.StartDateTime = dtStartDate.Value;
                entity.EndDateTime = dtEndDate.Value;
                entity.StartTime = dtStartTime.Value.TimeOfDay;
                entity.EndTime = dtEndTime.Value.TimeOfDay;

                entity.Recurrence = (long)recurrenceTrack.Value;
                entity.Interval = (long)intervalTrack.Value;
                entity.TimeZone = cmbTz.SelectedValue as string;

                return base.Entity;
            }
            set
            {
                base.Entity = value as TimerTask;
            }
        }

        public override bool ValidateEntity()
        {
            if(dtStartDate.Value > dtEndDate.Value)
            {
                MessageBox.Show("Start date cannot be greater than End date");
                return false;
            }

            if (dtStartTime.Value > dtEndTime.Value)
            {
                MessageBox.Show("Start time cannot be greater than End time");
                return false;
            }

            return true;
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
