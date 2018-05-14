using System;

namespace DataSyncWeb.BusinessLogic
{
    public class BackupJob
    {
        public int BackupJobId { get; set; }
        public int UserId { get; set; }
        public DateTime ScheduleTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public decimal Progress { get; set; }
    }
}