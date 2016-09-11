using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XecMe.Common;

namespace XecMe.Core.Reporting
{
    public class ReportContext
    {
        private string _applicationName;
        private Dictionary<string, object> _dataSources;
        private OutputFormat _format;
        private bool _isLandscape;
        private Dictionary<string, string> _parameters;
        private string _printerName;
        private string _reportName;

        public ReportContext(string applicationName, string reportName)
        {
            Guard.ArgumentNotNullOrEmptyString(applicationName, "applicationName");
            Guard.ArgumentNotNullOrEmptyString(reportName, "reportName");
            this._applicationName = applicationName;
            this._reportName = reportName;
        }

        
        public string ApplicationName
        {
            get
            {
                return this._applicationName;
            }
        }

        public IDictionary<string, object> DataSources
        {
            get
            {
                if (this._dataSources == null)
                {
                    this._dataSources = new Dictionary<string, object>();
                }
                return this._dataSources;
            }
        }

        public OutputFormat Format
        {
            get
            {
                return this._format;
            }
            set
            {
                this._format = value;
            }
        }

        public bool IsLandscape
        {
            get
            {
                return this._isLandscape;
            }
            set
            {
                this._isLandscape = value;
            }
        }

        public IDictionary<string, string> Parameters
        {
            get
            {
                if (this._parameters == null)
                {
                    this._parameters = new Dictionary<string, string>();
                }
                return this._parameters;
            }
        }

        public string PrinterName
        {
            get
            {
                return this._printerName;
            }
            set
            {
                this._printerName = value;
            }
        }

        public string ReportName
        {
            get
            {
                return this._reportName;
            }
        }
    }

    #region OutputFormat
    public enum OutputFormat
    {
        Pdf,
        Excel,
        Tif
    }
    #endregion

    #region ReportType
    public enum ReportType
    {
        MsReports,
        MsWebReports
    }
    #endregion

}
