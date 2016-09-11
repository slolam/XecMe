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
    public partial class ParametersControl : UserControl
    {
        public ParametersControl()
        {
            InitializeComponent();
        }

        public object DataSource
        {
            get
            {
                return dvParameters.DataSource;
            }
            set
            {
                dvParameters.DataSource = value;
            }
        }
    }
}
