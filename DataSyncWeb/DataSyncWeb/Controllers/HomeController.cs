using DataSyncWeb.Core;
using System;
using System.Web.Mvc;

namespace DataSyncWeb.Controllers
{
    public class HomeController : Controller
    {
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

        public ActionResult Backup(string source, string destination)
        {
            FolderEntry sourceFolder = DataComparer.IndexDirectory(source);
            FolderEntry destinationFolder = DataComparer.IndexDirectory(destination);
            Guid unique = DataComparer.SyncFolders(sourceFolder, destinationFolder);
            return View(unique);
        }

        public PartialViewResult BackupProgress(Guid unique)
        {
            return new PartialViewResult();
        }

    }
}