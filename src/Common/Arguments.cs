#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file Arguments.cs is part of XecMe.
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
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace XecMe.Common
{
    /// <summary>
    /// This class parses the command-line arguments and holds them in a StringDictionary
    /// </summary>
    public class Arguments
    {
        /// <summary>
        /// To hold the parsed parameters
        /// </summary>
        private StringDictionary _parameters;

        /// <summary>
        /// Constructor for parsing the command line parameters
        /// </summary>
        /// <param name="args">Arguments to be parsed</param>
        public Arguments(string[] args)
        {
            _parameters = new StringDictionary();
            Regex spliter = new Regex(@"^-{1,2}|^/|=|:",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            Regex remover = new Regex(@"^['""]?(.*?)['""]?$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string parameter = null;
            string[] parts;

            // Valid parameters forms:
            // {-,/,--}param{ ,=,:}((",')value(",'))
            // Examples: 
            // -param1 value1 --param2 /param3:"Test-:-work" 
            //   /param4=happy -param5 '--=nice=--'
            foreach (string str in args)
            {
                parts = spliter.Split(str, 3);

                switch (parts.Length)
                {
                    case 1:
                        if (parameter != null)
                        {
                            if (!_parameters.ContainsKey(parameter))
                            {
                                parts[0] =
                                    remover.Replace(parts[0], "$1");

                                _parameters.Add(parameter, parts[0]);
                            }
                            parameter = null;
                        }
                        break;

                    case 2:
                        if (parameter != null)
                        {
                            if (!_parameters.ContainsKey(parameter))
                                _parameters.Add(parameter, "true");
                        }
                        parameter = parts[1];
                        break;

                    case 3:
                        if (parameter != null)
                        {
                            if (!_parameters.ContainsKey(parameter))
                                _parameters.Add(parameter, "true");
                        }

                        parameter = parts[1];

            
                        if (!_parameters.ContainsKey(parameter))
                        {
                            parts[2] = remover.Replace(parts[2], "$1");
                            _parameters.Add(parameter, parts[2]);
                        }

                        parameter = null;
                        break;
                }
            }

            if (parameter != null)
            {
                if (!_parameters.ContainsKey(parameter))
                    _parameters.Add(parameter, "true");
            }
        }

        /// <summary>
        /// Gets the value of the parameter from the parsed list
        /// </summary>
        /// <param name="param">Name of the parameter</param>
        /// <returns>Returns the value of the parameter</returns>
        public string this[string param]
        {
            get
            {
                return _parameters[param];
            }
        }
    }
}
