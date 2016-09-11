#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file ZipHelper.cs is part of XecMe.
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
using System.Linq;
using System.Text;
using System.IO;
using Ionic.Zip;

namespace XecMe.Common
{
    public static class ZipHelper
    {
        /// <summary>
        /// Buffer size for coping the 
        /// </summary>
        private const int BUFFER_SIZE = 1024;


        public static void AddFilesToZip(string zipFile, Dictionary<string, string> files)
        {
            Guard.ArgumentNotNullOrEmptyString(zipFile, "zipFile");

            if ((files == null) || (files.Count == 0))
            {
                throw new ArgumentOutOfRangeException("files");
            }

            using (ZipFile zip = new ZipFile(zipFile))
            {
                foreach (string item in files.Keys)
                {
                    zip.AddFile(files[item], item);
                }
                zip.Save();
            }
        }

        public static void AddFilesToZip(string zipFile, List<string> files, string pathInArchive)
        {
            Guard.ArgumentNotNullOrEmptyString(zipFile, "zipFile");

            if ((files == null) || (files.Count == 0))
            {
                throw new ArgumentOutOfRangeException("files");
            }

            using (ZipFile zip = new ZipFile(zipFile))
            {
                foreach (string item in files)
                {
                    zip.AddFile(item, pathInArchive);
                }
                zip.Save();
            }
        }

        public static void CopyStream(Stream src, Stream dest)
        {
            byte[] data = new byte[BUFFER_SIZE];
            int bytesRead = 0;
            for (bytesRead = src.Read(data, 0, BUFFER_SIZE); bytesRead > 0; bytesRead = src.Read(data, 0, BUFFER_SIZE))
            {
                dest.Write(data, 0, bytesRead);
            }
        }

        public static void DeleteFilesFromZip(string zipFile, List<string> files)
        {
            Guard.ArgumentNotNullOrEmptyString(zipFile, "zipFile");

            if (files == null
                || files.Count == 0)
            {
                throw new ArgumentOutOfRangeException("files");
            }

            using (ZipFile zip = new ZipFile(zipFile))
            {
                zip.RemoveEntries(files);
                zip.Save();
            }
        }

        public static void ExtractFilesFromZip(string zipFile, Dictionary<string, string> files)
        {
            Guard.ArgumentNotNullOrEmptyString(zipFile, "zipFile");
            if ((files == null) || (files.Count == 0))
            {
                throw new ArgumentOutOfRangeException("files");
            }

            if (files.Comparer.GetType() != StringComparer.OrdinalIgnoreCase.GetType())
            {
                Dictionary<string, string> td = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (string item in files.Keys)
                {
                    td.Add(item, files[item]);
                }
                files = td;
            }

            using (ZipFile zip = new ZipFile(zipFile))
            {
                var items = from x in zip.Entries
                            where files.ContainsKey(x.FileName)
                            select x;

                foreach (ZipEntry item in items)
                {
                    using (FileStream file = new FileStream(files[item.FileName], FileMode.Create, FileAccess.Write))
                    {
                        item.Extract(file);
                    }
                }
            }
        }

        public static void ExtractFilesFromZip(string zipFile, string targetPath, List<string> files)
        {
            Guard.ArgumentNotNullOrEmptyString(zipFile, "zipFile");
            if ((files == null) || (files.Count == 0))
            {
                throw new ArgumentOutOfRangeException("files");
            }
            Guard.ArgumentNotNullOrEmptyString(targetPath, "targetPath");
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            Dictionary<string, string> filesList = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < files.Count; i++)
            {
                string f = Path.Combine(targetPath, files[i]);
                filesList.Add(files[i], f);
            }
            ExtractFilesFromZip(zipFile, filesList);
        }

        public static IList<string> ExtractFilesFromZip(string zipFile, List<string> files)
        {
            Guard.ArgumentNotNullOrEmptyString(zipFile, "zipFile");
            if ((files == null) || (files.Count == 0))
            {
                throw new ArgumentOutOfRangeException("files");
            }
            IList<string> retVal = new List<string>();
            Dictionary<string, string> filesList = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string dir = Path.GetTempFileName();
            Directory.CreateDirectory(dir);
            for (int i = 0; i < files.Count; i++)
            {
                string file = Path.Combine(dir, files[i]);
                filesList.Add(files[i], file);
                retVal.Add(file);
            }
            ExtractFilesFromZip(zipFile, filesList);
            return retVal;
        }

        public static void ExtractAllFiles(string zipFile, string targetPath)
        {
            Guard.ArgumentNotNullOrEmptyString(zipFile, "zipFile");
            Guard.ArgumentNotNullOrEmptyString(targetPath, "targetPath");
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            using (ZipFile zip = new ZipFile(zipFile))
            {
                zip.ExtractAll(targetPath, ExtractExistingFileAction.OverwriteSilently);
            }
        }
    }


}
