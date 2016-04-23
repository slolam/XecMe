using System;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;

namespace Axp.Fx.Common
{
    public enum Delimiter { Tab, Comma, Semicolon, Pipe, Other };
    /// <summary>
    /// Summary description for FeedReader.
    /// </summary>
    public class FeedReader
    {


        #region members variables
        private string path = Directory.GetCurrentDirectory();
        private bool hasHeader = true;
        private Delimiter delimiter = Delimiter.Tab;
        private string customDelimiter = "\t";

        #endregion

        #region Properties
        public bool HasHeader
        {
            get
            {
                return hasHeader;
            }
            set
            {
                hasHeader = value;
            }
        }
        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
            }
        }
        public Delimiter Delimited
        {
            get
            {
                return delimiter;
            }
            set
            {
                delimiter = value;
            }
        }
        public string CustomDelimiter
        {
            get { return customDelimiter; }
            set { customDelimiter = value; }
        }
        private string ConnectionString
        {
            get
            {
                return
                    @"Provider=Microsoft.Jet.OLEDB.4.0;" +
                    @"Data Source=" + path + ";" +
                    @"Extended Properties=" + Convert.ToChar(34).ToString() +
                    @"Text;" + Options + Convert.ToChar(34).ToString();
            }
        }

        private string Options
        {
            get
            {
                string option = null;
                if (HasHeader)
                    option = "HDR=Yes;MaxScanRows=0;";
                else
                    option = "HDR=No;MaxScanRows=0;";
                switch (delimiter)
                {
                    default:
                    case Delimiter.Tab:
                        option += "FMT=TabDelimited;";
                        option += "Format=TabDelimited;";
                        break;
                    case Delimiter.Comma:
                        option += "FMT=CsvDelimited;";
                        option += "Format=CsvDelimited;";
                        break;
                    case Delimiter.Semicolon:
                        option += "FMT=Semicolon;";
                        option += "Format=Semicolon;";
                        break;
                    case Delimiter.Pipe:
                        option += "FMT=Delimited(|);";
                        option += "Format=Delimited(|);";
                        break;
                    case Delimiter.Other:
                        option += string.Format("FMT=Delimited({0});", customDelimiter);
                        option += string.Format("Format=Delimited({0});", customDelimiter);
                        break;
                }
                return option;
            }
        }


        #endregion
        public FeedReader()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        #region Methods
        public DataTable GetDataTable(string tableName)
        {
            DataTable retVal = new DataTable();
            using (TextFieldParser reader = new TextFieldParser(System.IO.Path.Combine(Path, tableName)))
            {
                #region
                switch (Delimited)
                {
                    case Delimiter.Tab:
                        reader.SetDelimiters("\t");
                        break;
                    case Delimiter.Comma:
                        reader.SetDelimiters(",");
                        break;
                    case Delimiter.Semicolon:
                        reader.SetDelimiters(";");
                        break;
                    case Delimiter.Semicolon:
                        reader.SetDelimiters(";");
                        break;
                    case Delimiter.Pipe:
                        reader.SetDelimiters("|");
                        break;
                    case Delimiter.Other:
                        reader.SetDelimiters(CustomDelimiter);
                        break;
                }
                #endregion
                reader.TextFieldType = FieldType.Delimited;

                string[] fields;
                
                if (HasHeader)
                {
                    fields = reader.ReadFields();
                    for (int i = 0; i < fields.Length; i++)
                    {
                        retVal.Columns.Add(fields[i]);
                    }
                }
                while ((fields = reader.ReadFields()) != null)
                {
                    retVal.Rows.Add(fields);
                }
            }
            return retVal;
        }
        private DataTable GetDataTable_Old(string tableName)
        {
            DataTable retVal = new DataTable();
            using (OleDbConnection oConn = new OleDbConnection(ConnectionString))
            {
                oConn.Open();
                using (OleDbDataAdapter oAdpr = new OleDbDataAdapter(string.Format("select * from [{0}]", tableName.Replace('.','_')), oConn))
                {
                    oAdpr.Fill(retVal);
                }
                if (hasHeader)
                    retVal = SetFirstRowAsColumnHeaders(retVal);
            }
            return retVal;
        }

        private DataTable SetFirstRowAsColumnHeaders(DataTable dataTable)
        {
            DataTable retVal = new DataTable();
            int count = dataTable.Columns.Count;
            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    //Set the column name from the first row which is the header.
                    DataColumn col = dataTable.Columns[i];
                    col.ColumnName = dataTable.Rows[0][i].ToString().ToUpper();
                    retVal.Columns.Add(col);
                }
                dataTable.Rows.RemoveAt(0);
            }
            count = dataTable.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                retVal.Rows.Add(dataTable.Rows[i]);
            }
            return retVal;
        }


        #endregion
    }
}
