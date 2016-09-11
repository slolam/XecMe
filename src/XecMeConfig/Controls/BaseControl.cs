using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XecMeConfig.Entities;

namespace XecMeConfig.Controls
{
    public class BaseControl : UserControl
    {
        private BaseTask _entity;
        public virtual BaseTask Entity
        {
            get
            {
                return _entity;
            }
            set
            {
                _entity = value;
                
                DataSource = value == null ? null : value.Parameters;
            }
        }

        //protected ParametersControl parametersControl;

        public virtual bool ValidateEntity()
        { return false; }

        protected virtual List<Parameter> DataSource
        {
            set { }
        }
    }
}
