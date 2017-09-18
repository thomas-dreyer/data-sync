using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace DataSyncWeb.Core
{
    /// <summary>
    /// Functions as the primary data / file comparer.
    /// </summary>
    public static class DataComparer
    {

        #region Fields

        #endregion

        #region Functions

        public static string FormatSize(long bytes)
        {
            string[] levels = new string[] { "bytes", "Kilobytes", "Megabytes", "Gigabytes", "Terabytes" };
            int level = 0;
            while (bytes > 1024)
            {
                bytes /= 1024;
                level++;
            }
            return string.Format("{0} {1}", bytes, levels[level]);
        }

        public static FolderEntry IndexDirectory(string source)
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

        public static void DeleteDirectoryRecursive(string directory)
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

        public static Guid SyncFolders(FolderEntry source, FolderEntry destination)
        {
            foreach (var item in source.Folders)
            {
                if (!destination.Folders.ContainsKey(item.Key))
                {
                    FolderEntry newFolder = new FolderEntry(item.Key);
                    destination.AddFolder(newFolder);
                    Directory.CreateDirectory(destination.Folders[item.Key].Path);
                }
                SyncFolders(item.Value, destination.Folders[item.Key]);
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
                    File.Copy(Path.Combine(source.Path, item), Path.Combine(destination.Path, item), true);
                }
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
                SyncFolders(item.Value, destination.Folders[item.Key]);
            }
            return new Guid();
        }

        #endregion

    }
}