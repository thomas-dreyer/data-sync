using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataSyncWeb.Core
{
    public class FolderEntry
    {
        #region Properties


        public Dictionary<string, FileEntry> Files { get; }
        public Dictionary<string, FolderEntry> Folders { get; }
        public string Name { get; set; }
        public string Path { get; set; }
        public SyncInfo FolderMetaData { get; set; }

        #endregion

        #region Functions

        public void AddFolder(FolderEntry folderEntry)
        {
            if (folderEntry != null)
            {
                if (!Folders.ContainsKey(folderEntry.Name))
                {
                    folderEntry.Path = System.IO.Path.Combine(Path, folderEntry.Name);
                    Console.WriteLine("Added Folder: {0} Path {1}", folderEntry.Name, folderEntry.Path);
                    Folders.Add(folderEntry.Name, folderEntry);
                    FolderMetaData += folderEntry.FolderMetaData;
                }
            }
        }

        public void AddFile(FileEntry fileEntry)
        {
            if (!Files.ContainsKey(fileEntry.FileName))
            {
                Files.Add(fileEntry.FileName, fileEntry);
                FolderMetaData.FileCount++;
                FolderMetaData.FileSize += fileEntry.FileSize;
            }
        }

        #endregion

        #region Constructor

        public FolderEntry(string name)
        {
            Name = name;
            Files = new Dictionary<string, FileEntry>();
            Folders = new Dictionary<string, FolderEntry>();
            FolderMetaData = new SyncInfo();
        }

        #endregion

    }
}