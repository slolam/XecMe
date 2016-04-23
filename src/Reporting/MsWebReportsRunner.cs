using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Reporting.WinForms;
using System.Drawing.Printing;
using System.Drawing.Imaging;

namespace XecMe.Core.Reporting
{
    public class MsReportsRunner : BaseReportRunner
{
    private int _currentPage;
    private Dictionary<string, string> _files;
    private IList<Stream> _streams;

    protected void Cleanup()
    {
        if (this._streams != null)
        {
            for (int i = 0; i < _streams.Count; i++)
            {
                using(_streams[i]);
            }
            _streams.Clear();
            _streams = null;
        }
    }

    private Stream CreateStream(string name, string fileNameExtension, Encoding encoding, string mimeType, bool willSeek)
    {
        string fileName = Path.GetTempFileName();
        Stream stream = new FileStream(fileName, FileMode.Create);
        this._streams.Add(stream);
        this._files.Add(name + "." + fileNameExtension, fileName);
        return stream;
    }

    public override void Dispose()
    {
        this.Cleanup();
        base.Dispose();
        GC.SuppressFinalize(this);
    }

    ~MsReportsRunner()
    {
        this.Dispose();
    }

    public override Dictionary<string, string> Generate()
    {
        string deviceInfo = null;
        using (LocalReport report = this.LoadReport())
        {
            Warning[] warnings;
            this.Cleanup();
            this._streams = new List<Stream>();
            this._files = new Dictionary<string, string>();
            report.Render(this.Context.Format.ToString(), deviceInfo, new CreateStreamCallback(CreateStream), out warnings);
            for (int i = 0; i < _streams.Count; i++)
            {
                this._streams[i].Close();
            }
            return this._files;
        }
    }

    private LocalReport LoadReport()
    {
        LocalReport report = new LocalReport();
        report.ReportPath = this.ReportLocation;
        foreach (string ds in this.Context.DataSources.Keys)
        {
            report.DataSources.Add(new ReportDataSource(ds, this.Context.DataSources[ds]));
        }
        report.Refresh();
        if (this.Context.Parameters.Count > 0)
        {
            ReportParameter[] parameters = new ReportParameter[Context.Parameters.Count];
            int i = 0;
            foreach (string pm in this.Context.Parameters.Keys)
            {
                ReportParameter param = new ReportParameter(pm);
                param.Values.Add(this.Context.Parameters[pm]);
                parameters[i] = param;
                i++;
            }
            report.SetParameters(parameters);
        }
        return report;
    }

    public override void Print()
    {
        string deviceInfo = "<DeviceInfo><OutputFormat>EMF</OutputFormat></DeviceInfo>";
        using (LocalReport report = this.LoadReport())
        {
            Warning[] warnings;
            Cleanup();
            _streams = new List<Stream>();
            _files = new Dictionary<string, string>();
            report.Render("Image", deviceInfo, new CreateStreamCallback(CreateStream), out warnings);
            for (int i = 0; i <= _streams.Count; i++)
            {
                Stream stream = this._streams[i];
                stream.Position = 0L;
            }
        }
        this.PrintReport();
    }

    private void PrintPage(object sender, PrintPageEventArgs e)
    {
        Metafile metaDoc = new Metafile(_streams[_currentPage]);
        e.Graphics.DrawImage(metaDoc, e.PageBounds.Location);
        _currentPage++;
        e.HasMorePages = _currentPage < _streams.Count;
    }

    private void PrintReport()
    {
        if (_streams != null 
            && _streams.Count != 0)
        {
            using (PrintDocument printDoc = new PrintDocument())
            {
                printDoc.PrintController = new StandardPrintController();
                printDoc.PrinterSettings.PrinterName = this.Context.PrinterName;
                if (!printDoc.PrinterSettings.IsValid)
                {
                    throw new InvalidPrinterException(printDoc.PrinterSettings);
                }
                printDoc.DefaultPageSettings.Landscape = this.Context.IsLandscape;
                printDoc.PrintPage += new PrintPageEventHandler(this.PrintPage);
                printDoc.Print();
                printDoc.PrintPage -= new PrintPageEventHandler(this.PrintPage);
            }
        }
    }
}

 

}
