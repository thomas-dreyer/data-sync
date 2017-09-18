using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataSyncWeb.Core
{
    public class FileEntry
    {
        #region Properties

        public string FileName { get; }
        public long FileSize { get; }
        public DateTime DateCreated { get; }
        public DateTime DateModified { get; }

        #endregion

        #region Constructor

        public FileEntry()
        { }

        public FileEntry(string fileName, long fileSize, DateTime createdDateTime, DateTime modifiedDateTime)
        {
            FileName = fileName;
            FileSize = fileSize;
            DateCreated = createdDateTime;
            DateModified = modifiedDateTime;
        }

        #endregion
    }
}