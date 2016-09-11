using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XecMe.Core.Reporting
{
    /// <summary>
    /// Report runners for generating the reports. Each new type of the reports technology 
    /// has to implement this interface to generate reports
    /// </summary>
    public interface IReportRunner : IDisposable
    {
        Dictionary<string, string> Generate();
        string GenerateZip();
        void Intialize(ReportContext context);
        void Print();
    }


}
