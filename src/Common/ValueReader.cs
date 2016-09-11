#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file ValueReader.cs is part of XecMe.
/// 
/// XecMe is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
/// 
/// XecMe is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License along with XecMe. If not, see http://www.gnu.org/licenses/.
/// 
/// History:
/// ______________________________________________________________
/// Created         01-2013             Shailesh Lolam

#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace XecMe.Common.Data
{
    public class ValueReader
    {

        private Dictionary<string, int> _ordinal;
        private IDataReader _reader = null;

        public ValueReader(IDataReader reader)
        {
            reader.NotNull(nameof(reader));
            this._reader = reader;
            this._ordinal = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            this.LoadColumnOrdinals();
        }

        public bool GetBoolean(int ordinal)
        {
            if (this._reader.IsDBNull(ordinal))
            {
                return false;
            }
            return this._reader.GetBoolean(ordinal);
        }

        public bool GetBoolean(string column)
        {
            return this.GetBoolean(this._ordinal[column]);
        }

        public bool GetBoolean(int ordinal, bool defValue)
        {
            if (this._reader.IsDBNull(ordinal))
            {
                return defValue;
            }
            return this._reader.GetBoolean(ordinal);
        }

        public bool GetBoolean(string column, bool defVal)
        {
            return this.GetBoolean(this._ordinal[column], defVal);
        }

        public DateTime GetDateTime(int ordinal)
        {
            if (this._reader.IsDBNull(ordinal))
            {
                return DateTime.MinValue;
            }
            return this._reader.GetDateTime(ordinal);
        }

        public DateTime GetDateTime(string column)
        {
            return this.GetDateTime(this._ordinal[column]);
        }

        public DateTime GetDateTime(int ordinal, DateTime defValue)
        {
            if (this._reader.IsDBNull(ordinal))
            {
                return defValue;
            }
            return this._reader.GetDateTime(ordinal);
        }

        public DateTime GetDateTime(string column, DateTime defVal)
        {
            return this.GetDateTime(this._ordinal[column], defVal);
        }

        public decimal GetDecimal(int ordinal)
        {
            if (this._reader.IsDBNull(ordinal))
            {
                return decimal.Zero;
            }
            return this._reader.GetDecimal(ordinal);
        }

        public decimal GetDecimal(string column)
        {
            return this.GetDecimal(this._ordinal[column]);
        }

        public decimal GetDecimal(int ordinal, decimal defValue)
        {
            if (this._reader.IsDBNull(ordinal))
            {
                return defValue;
            }
            return this._reader.GetDecimal(ordinal);
        }

        public decimal GetDecimal(string column, decimal defVal)
        {
            return this.GetDecimal(this._ordinal[column], defVal);
        }

        public double GetDouble(int ordinal)
        {
            if (this._reader.IsDBNull(ordinal))
            {
                return 0.0;
            }
            return this._reader.GetDouble(ordinal);
        }

        public double GetDouble(string column)
        {
            return this.GetDouble(this._ordinal[column]);
        }

        public double GetDouble(int ordinal, double defValue)
        {
            if (this._reader.IsDBNull(ordinal))
            {
                return defValue;
            }
            return this._reader.GetDouble(ordinal);
        }

        public double GetDouble(string column, double defVal)
        {
            return this.GetDouble(this._ordinal[column], defVal);
        }

        public float GetFloat(int ordinal)
        {
            if (this._reader.IsDBNull(ordinal))
            {
                return 0f;
            }
            return this._reader.GetFloat(ordinal);
        }

        public float GetFloat(string column)
        {
            return this.GetFloat(this._ordinal[column]);
        }

        public float GetFloat(int ordinal, float defValue)
        {
            if (this._reader.IsDBNull(ordinal))
            {
                return defValue;
            }
            return this._reader.GetFloat(ordinal);
        }

        public float GetFloat(string column, float defVal)
        {
            return this.GetFloat(this._ordinal[column], defVal);
        }

        public int GetInt(int ordinal)
        {
            if (this._reader.IsDBNull(ordinal))
            {
                return 0;
            }
            return this._reader.GetInt32(ordinal);
        }

        public int GetInt(string column)
        {
            return this.GetInt(this._ordinal[column]);
        }

        public int GetInt(int ordinal, int defValue)
        {
            if (this._reader.IsDBNull(ordinal))
            {
                return defValue;
            }
            return this._reader.GetInt32(ordinal);
        }

        public int GetInt(string column, int defVal)
        {
            return this.GetInt(this._ordinal[column], defVal);
        }

        public long GetLong(int ordinal)
        {
            if (this._reader.IsDBNull(ordinal))
            {
                return 0L;
            }
            return this._reader.GetInt64(ordinal);
        }

        public long GetLong(string column)
        {
            return this.GetLong(this._ordinal[column]);
        }

        public long GetLong(int ordinal, long defValue)
        {
            if (this._reader.IsDBNull(ordinal))
            {
                return defValue;
            }
            return this._reader.GetInt64(ordinal);
        }

        public long GetLong(string column, long defVal)
        {
            return this.GetLong(this._ordinal[column], defVal);
        }

        public string GetString(int ordinal)
        {
            if (this._reader.IsDBNull(ordinal))
            {
                return null;
            }
            return this._reader.GetString(ordinal);
        }

        public string GetString(string column)
        {
            return this.GetString(this._ordinal[column]);
        }

        public string GetString(int ordinal, string defValue)
        {
            if (this._reader.IsDBNull(ordinal))
            {
                return defValue;
            }
            return this._reader.GetString(ordinal);
        }

        public string GetString(string column, string defVal)
        {
            return this.GetString(this._ordinal[column], defVal);
        }

        public object GetValue(int ordinal)
        {
            if (this._reader.IsDBNull(ordinal))
            {
                return null;
            }
            return this._reader.GetValue(ordinal);
        }

        public object GetValue(string column)
        {
            return this.GetValue(this._ordinal[column]);
        }

        public object GetValueWithDefault(int ordinal)
        {
            if (this._reader.IsDBNull(ordinal))
            {
                Type t = this._reader.GetFieldType(ordinal);
                if (t.IsClass)
                {
                    return null;
                }
                return Activator.CreateInstance(t);
            }
            return this._reader.GetValue(ordinal);
        }

        public object GetValueWithDefault(string column)
        {
            return this.GetValueWithDefault(this._ordinal[column]);
        }

        private void LoadColumnOrdinals()
        {
            this._ordinal.Clear();
            for (int i = 0; i < _reader.FieldCount; i++)
            {
                this._ordinal.Add(this._reader.GetName(i), i);
            }
        }

        public bool NextResult()
        {
            bool NextResult = this._reader.NextResult();
            if (NextResult)
            {
                this.LoadColumnOrdinals();
            }
            return NextResult;
        }

        public bool Read()
        {
            return this._reader.Read();
        }

    }


}
