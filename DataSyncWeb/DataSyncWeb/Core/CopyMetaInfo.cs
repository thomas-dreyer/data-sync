namespace DataSyncWeb.Core
{
    public class SyncInfo
    {

        #region Properties

        public string FileName { get; set; }
        public int FileCount { get; set; }
        public long FileSize { get; set; }

        #endregion

        #region Overrides

        public static SyncInfo operator +(SyncInfo cpl, SyncInfo cpr)
        {
            return new SyncInfo(cpl.FileCount + cpr.FileCount, cpl.FileSize + cpr.FileSize);
        }

        public static SyncInfo operator -(SyncInfo cpl, SyncInfo cpr)
        {
            return new SyncInfo(cpl.FileCount - cpr.FileCount, cpl.FileSize - cpr.FileSize);
        }

        #endregion

        #region Constructor

        public SyncInfo()
        {
            FileCount = 0;
            FileSize = 0;
            FileName = string.Empty;
        }

        public SyncInfo(int fileCount, long fileSize)
        {
            FileCount = fileCount;
            FileSize = fileSize;
            FileName = string.Empty;
        }
        public SyncInfo(int fileCount, long fileSize, string filename)
        {
            FileCount = fileCount;
            FileSize = fileSize;
            FileName = filename;
        }

        #endregion

    }
}