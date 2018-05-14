using System;

namespace DataSyncWeb.BusinessLogic
{
    public class BackupJobDetail
    {
        public int BackupJobDetailId { get; set; }
        public int BackupJobId { get; set; }
        public string FileNameFrom { get; set; }
        public string FileNameTo { get; set; }
        public long FileSize { get; set; }
        public DateTime DateSynced { get; set; }
    }
}