using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using XecMe.Common;
using XecMe.Core.Reporting.Configuration;
using XecMe.Configuration;

namespace XecMe.Core.Reporting
{
    public abstract class BaseReportRunner : IReportRunner
{
    private Assembly _assembly;
    private ReportContext _context;
    private static StringDictionary _reportCache;
    private string _reportLocation;
    protected readonly object _sync = new object();

    public virtual void Dispose()
    {
    }

    public abstract Dictionary<string, string> Generate();

    public string GenerateZip()
    {
        Dictionary<string, string> fileList = this.Generate();
        if ((fileList == null) || (fileList.Count == 0))
        {
            return null;
        }
        string retVal = Path.GetTempFileName();
        ZipArchive z = new ZipArchive(null);
        
        //ZipHelper.AddFilesToZip(retVal, fileList);
        return retVal;
    }

    public static IReportRunner GetReportRunner(string runnerType)
    {
        Guard.ArgumentNotNullOrEmptyString(runnerType, "runnerType");
        ExtensionElement extnElement = ExtensionsSection.ThisSection.GetExtensions(ReportsSection.REPORT_RUNNERS)[runnerType];
        if (extnElement == null)
            throw new ArgumentOutOfRangeException(string.Format("Runner type {0} not configured", runnerType));
        return Reflection.CreateInstance<IReportRunner>(extnElement.Type);
    }

    public void Intialize(ReportContext context)
    {
        Guard.ArgumentNotNull(context, "context");
        this._assembly = null;
        this.ValidateContext(context);
        this._context = context;
    }

    public virtual void Print()
    {
    }

    private void ValidateContext(ReportContext context)
    {
        ReportApplication ra = ReportsSection.ThisSection.ReportApplications[context.ApplicationName];
        this._reportLocation = null;
        if (ra == null)
        {
            throw new ArgumentOutOfRangeException(string.Format("Reporting application \"{0}\" is not defined in the configuration", context.ApplicationName));
        }
        ReportDefinition rd = ra.ReportDefinitions[context.ReportName];
        if (rd == null)
        {
            throw new ArgumentOutOfRangeException(string.Format("Report definition \"{0}\" is not defined in \"{1}\" in the configuration", context.ApplicationName, context.ReportName));
        }
        if (!string.IsNullOrEmpty(rd.AssemblyName))
        {
            this._assembly = Assembly.Load(rd.AssemblyName);
            if (_reportCache == null)
            {
                _reportCache = new StringDictionary();
            }
            string reportCacheKey = this._assembly.FullName + "$" + rd.ReportLocation;
            if (_reportCache.ContainsKey(reportCacheKey))
            {
                this._reportLocation = _reportCache[reportCacheKey];
                if (!File.Exists(this._reportLocation))
                {
                    lock (_reportCache.SyncRoot)
                    {
                        _reportCache.Remove(reportCacheKey);
                    }
                    this._reportLocation = null;
                }
            }
            if (string.IsNullOrEmpty(this._reportLocation))
            {
                lock (_reportCache.SyncRoot)
                {
                    this._reportLocation = Path.GetTempFileName();
                    if (_reportCache.ContainsKey(reportCacheKey))
                    {
                        this._reportLocation = _reportCache[reportCacheKey];
                    }
                    else
                    {
                        using (Stream resource = this._assembly.GetManifestResourceStream(rd.ReportLocation))
                        {
                            using (Stream file = new FileStream(this._reportLocation, FileMode.Create))
                            {
                                byte[] data = new byte[1024];
                                for (int bytesRead = resource.Read(data, 0, 1024); bytesRead > 0; bytesRead = resource.Read(data, 0, 1024))
                                {
                                    file.Write(data, 0, bytesRead);
                                }
                            }
                        }
                        _reportCache.Add(reportCacheKey, this._reportLocation);
                    }
                }
            }
        }
        else
        {
            this._reportLocation = rd.ReportLocation;
        }
    }

    protected Assembly Assembly
    {
        get
        {
            return this._assembly;
        }
    }

    protected ReportContext Context
    {
        get
        {
            return this._context;
        }
    }

    protected string ReportLocation
    {
        get
        {
            return this._reportLocation;
        }
    }
}

 

}
