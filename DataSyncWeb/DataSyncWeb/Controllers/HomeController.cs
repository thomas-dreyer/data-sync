using DataSyncWeb.Core;
using System;
using System.IO;
using DataSyncWeb.Helpers;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Threading;
using DataSyncWeb.BusinessLogic;
using DataSyncWeb.DataAccessLayer;
using System.Linq;

namespace DataSyncWeb.Controllers
{
    public class HomeController : Controller
    {
        private DataComparer comparer = new DataComparer();
        private DataManager dm = new DataManager();

        private void startBackup(int jobId, string source, string destination)
        {
            DataComparer comparer = new DataComparer();
            FolderEntry sourceFolder = comparer.IndexDirectory(source);
            FolderEntry destinationFolder = comparer.IndexDirectory(destination);
            comparer.progress += Comparer_progress;
            comparer.SyncFolders(jobId, sourceFolder, destinationFolder);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        public JsonResult Backup(string source, string destination)
        {
            BackupJob job = new BackupJob()
            {
                Progress = 0,
                ScheduleTime = DateTime.Now,
                UserId = 1,
                Destination = destination,
                Source = source,
                StartTime = DateTime.Now,
                EndTime = new DateTime(1900, 1, 1)
            };
            job.BackupJobId = dm.InsertData(job);
            Thread backupThread = new Thread(() => startBackup(job.BackupJobId, source, destination));
            backupThread.Start();
            var result = new
            {
                Id = job.BackupJobId,
                progress = 0
            };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Progress(int Id)
        {
            decimal progressPerc = 0;
            decimal completePerc = (decimal)100.00;
            try
            {
                BackupJob job = dm.SelectData(new BackupJob(), string.Format("WHERE BackupJobId = {0}", Id)).FirstOrDefault();
                progressPerc = job.Progress;
            }
            catch (Exception ex)
            {
                progressPerc = completePerc;
            }
            bool complete = progressPerc == completePerc;
            var result = new
            {
                Complete = complete,
                Progress = progressPerc
            };
            return Json(result);

        }

        private void Comparer_progress(SyncInfo syncInfo)
        {
            //StreamWriter writer = System.IO.File.AppendText("c:\\temp\\backup_web.log");
            //writer.WriteLine(string.Format("{0} {1} {2} {3}", DateTime.Now.ToUniversalTime(), syncInfo.FileCount, syncInfo.FileName,  DataFormatHelper.FormatSize(syncInfo.FileSize)));
            //writer.Close();
        }

        public PartialViewResult BackupProgress(Guid unique)
        {
            return new PartialViewResult();
        }

    }
}