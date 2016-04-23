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
    public partial class EventTaskControl : BaseControl
    {
        public EventTaskControl()
        {
            InitializeComponent();
            cmbThreadOptions.DataSource = Enum.GetNames(typeof(ThreadOptions));
        }

        public override BaseTask Entity
        {
            get
            {
                if (base.Entity == null)
                    base.Entity = new EventTask();

                EventTask entity = base.Entity as EventTask;

                entity.EventTopic = txtTopicName.Text;
                entity.Timeout = (int)timeoutTrack.Value;
                entity.ThreadOption = (ThreadOptions)Enum.Parse(typeof(ThreadOptions), cmbThreadOptions.SelectedValue.ToString());

                return entity;
            }
            set
            {
                EventTask entity = value as EventTask;
                if (entity != null)
                {
                    txtTopicName.Text = entity.EventTopic;
                    timeoutTrack.Value = entity.Timeout;
                    cmbThreadOptions.SelectedValue = entity.ThreadOption.ToString();

                    base.Entity = entity;
                }
            }
        }

        public override bool ValidateEntity()
        {
            if (txtTopicName.Text.Trim().Length == 0)
            {
                MessageBox.Show("Topic name missing");
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
