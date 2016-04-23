using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;

namespace XecMe.Installer
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();
        }
    }
}