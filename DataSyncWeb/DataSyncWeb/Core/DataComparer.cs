using DataSyncWeb.DataAccessLayer;
using DataSyncWeb.BusinessLogic;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace DataSyncWeb.Core
{
    public delegate void DataSyncProgressHandler(SyncInfo syncInfo);
    /// <summary>
    /// Functions as the primary data / file comparer.
    /// </summary>
    public class DataComparer
    {

        #region Fields

        DataManager dm = new DataManager();

        #endregion

        #region Events

        public event DataSyncProgressHandler progress;

        #endregion

        #region Private Functions

        private void Progress(SyncInfo syncInfo)
        {
            if (progress != null)
            {
                progress(syncInfo);
            }
        }

        #endregion

        #region Functions


        public FolderEntry IndexDirectory(string source)
        {
            FolderEntry folder = null;
            if (!Directory.Exists(source))
            {
                Directory.CreateDirectory(source);
            }
            DirectoryInfo dir = new DirectoryInfo(source);
            folder = new FolderEntry(dir.Name);
            folder.Path = dir.FullName;
            foreach (var file in dir.GetFiles())
            {
                FileEntry fileEntry = new FileEntry(file.Name, file.Length, file.CreationTime, file.LastWriteTime);
                folder.AddFile(fileEntry);
            }
            foreach (var directory in dir.GetDirectories())
            {
                FolderEntry subFolder = IndexDirectory(directory.FullName);
                if (subFolder != null)
                {
                    folder.AddFolder(subFolder);
                }
            }
            return folder;
        }

        public void DeleteDirectoryRecursive(string directory)
        {
            DirectoryInfo dir = new DirectoryInfo(directory);
            foreach (var file in dir.GetFiles())
            {
                File.Delete(file.FullName);
            }
            foreach (var dirs in dir.GetDirectories())
            {
                DeleteDirectoryRecursive(dirs.FullName);
            }
            Directory.Delete(directory);
        }

        public void SyncFolders(int JobId, FolderEntry source, FolderEntry destination)
        {
            List<BackupJobDetail> jobDetails = readSyncFolders(JobId, source, destination);
            BackupJob job = dm.SelectData(new BackupJob(), string.Format("WHERE BackupJobId = {0}", JobId)).FirstOrDefault();
            int files = 0;
            long totalBytes = 0;
            foreach (BackupJobDetail jobDetail in jobDetails)
            {
                files++;
                totalBytes += jobDetail.FileSize;
            }
            double prog = 0;
            foreach (BackupJobDetail jobDetail in jobDetails)
            {
                jobDetail.DateSynced = DateTime.Now;
                File.Copy(jobDetail.FileNameFrom, jobDetail.FileNameTo, true);
                dm.InsertData(jobDetail);
                prog++;
                job.Progress = (decimal)Math.Round((prog / files) * 100, 2);
                if (files == prog)
                {
                    job.EndTime = DateTime.Now;
                }
                dm.UpdateData(job);
                Progress(new SyncInfo() { FileCount = 1, FileSize = jobDetail.FileSize, FileName = jobDetail.FileNameTo });
            }
            
            foreach (var item in destination.Files.Keys)
            {
                // delete file if not in source.
                if (!source.Files.ContainsKey(item))
                {
                    File.Delete(Path.Combine(destination.Path, destination.Files[item].FileName));
                }
            }
            // destroy all folders that do not exist in the source.
            foreach (var item in destination.Folders)
            {
                if (!source.Folders.ContainsKey(item.Key))
                {
                    DeleteDirectoryRecursive(item.Value.Path);
                }
               // SyncFolders(JobId, item.Value, destination.Folders[item.Key]);
            }
        }

        private List<BackupJobDetail> readSyncFolders(int JobId, FolderEntry source, FolderEntry destination)
        {
            List<BackupJobDetail> jobDetails = new List<BackupJobDetail>();
            foreach (var item in source.Folders)
            {
                if (!destination.Folders.ContainsKey(item.Key))
                {
                    FolderEntry newFolder = new FolderEntry(item.Key);
                    destination.AddFolder(newFolder);
                    Directory.CreateDirectory(destination.Folders[item.Key].Path);
                }
                jobDetails.AddRange(readSyncFolders(JobId,item.Value, destination.Folders[item.Key]));
            }

            foreach (var item in source.Files.Keys)
            {
                bool copy = false;
                // compare files that exist
                if (destination.Files.ContainsKey(item))
                {
                    // compare file size
                    copy = (destination.Files[item].FileSize != source.Files[item].FileSize) || (destination.Files[item].DateModified != source.Files[item].DateModified);
                }
                else
                {
                    // file does not exist.
                    copy = true;
                }
                if (copy)
                {
                    BackupJobDetail jobDetail = new BackupJobDetail()
                    {
                        BackupJobId = JobId,
                        FileNameFrom = Path.Combine(source.Path, item),
                        FileNameTo = Path.Combine(destination.Path, item),
                        FileSize = source.Files[item].FileSize,
                        DateSynced = DateTime.Now
                    };
                    jobDetails.Add(jobDetail);
                }
            }
            return jobDetails;
        }

        #endregion

    }
}