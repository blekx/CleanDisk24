using CleanDisk24.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanDisk24.DataWorker
{
    public static partial class DataWorkerAgent
    {
        public static void ScanAndAddDirectory_Initialize_Old(DirectoryInfo di, DB DB)
        {
            MyPlace directory = CreateMyDirectoryImage(di, DB);
            ScanDirectoryAndAddSub_Recurrent(directory, di, DB);
            ScandAndAddFilesHere_Old(directory, di, DB);
        }
        public static async void ScanAndAddDirectory_Initialize(DirectoryInfo rootDirInfo, DB DB)
        {
            MyPlace rootPlace = CreateMyDirectoryImage(rootDirInfo, DB);
            Stack<DuoDirInfo> directoriesToBeScanned = new Stack<DuoDirInfo>();
            directoriesToBeScanned.Push(new DuoDirInfo(rootPlace, rootDirInfo));
            DuoDirInfo currentlyScannedDirectory;
            int stackSize;
            do
            {
                currentlyScannedDirectory = directoriesToBeScanned.Pop();
                DirectoryInfo[] moreDirInfos = await ScanDirectory_Async(currentlyScannedDirectory);
                //currentlyScannedDirectory.MyPlace, currentlyScannedDirectory.DirectoryInfo);
                await Task.Run(() => ScandAndAddFilesHere_Async(currentlyScannedDirectory.MyPlace, currentlyScannedDirectory.DirectoryInfo, DB));
                //ScandAndAddFilesHere_Async2(rootPlace, rootDirInfo);
                foreach (DirectoryInfo foundDirInfo in moreDirInfos)
                {
                    MyDirectory foundDir = new MyDirectory(currentlyScannedDirectory.MyPlace, foundDirInfo);
                    AddDirectoryToDBList(foundDir, DB);
                    directoriesToBeScanned.Push(new DuoDirInfo(foundDir, foundDirInfo));
                }
                currentlyScannedDirectory = directoriesToBeScanned.Pop();
                stackSize = directoriesToBeScanned.Count();
                //} while (currentlyScannedDirectory != null);
            } while (stackSize > 0);

            //ScanDirectoryAndAddSub_Recurrent_Async(rootPlace, rootDirInfo);
        }

        internal static void StartupLoadData()
        {
            throw new NotImplementedException();
        }

        private static void ScandAndAddFilesHere_Old(MyPlace md, DirectoryInfo di, DB DB)
        {
            foreach (FileInfo fileinfo in di.GetFiles())
            {
                MyFile mf = new MyFile(md, fileinfo);
                md.Items.Add(mf);
                DB.AllFiles.Add(mf);
            }
        }
        private static async Task<bool> ScandAndAddFilesHere_Async_Wrong(MyPlace md, DirectoryInfo di, DB DB)
        {
            Task t = Task.Run(() =>
            {
                FileInfo[] files

                //var files
                = di.GetFiles();
                foreach (FileInfo fileinfo in files)
                {
                    MyFile mf = new MyFile(md, fileinfo);
                    md.Items.Add(mf);
                    DB.AllFiles.Add(mf);
                }
            });
            return t.IsCompleted;
        }
        private static async Task<int> ScandAndAddFilesHere_Async(MyPlace md, DirectoryInfo di, DB DB)
        {
            FileInfo[] files = await Task.Run(() => di.GetFiles());
            //FileInfo[] files = di.GetFiles();
            foreach (FileInfo fileInfo in files)
            {
                MyFile mf = new MyFile(md, fileInfo);
                md.Items.Add(mf);
                DB.AllFiles.Add(mf);
            }
            return files.Count();
        }
        private static Task<FileInfo[]> GetFiles(DirectoryInfo di) => Task.FromResult(di.GetFiles());
        //FileInfo[] files;
        //files = di.GetFiles();
        //files;
        private static async void ScanDirectoryAndAddSub_Recurrent_Async(MyPlace md, DirectoryInfo di, DB DB)
        {
            DirectoryInfo[] subDirectories = await Task.Run(() => di.GetDirectories());
            foreach (DirectoryInfo subDir in subDirectories)
            {
                MyDirectory msd = CreateMyDirectoryImage(md, subDir);
                AddDirectoryToDBList(msd, DB);
                ScanDirectoryAndAddSub_Recurrent(msd, subDir, DB);
            }
            ScandAndAddFilesHere_Old(md, di, DB);
        }
        ///<summary>This directory must have a parent.</summary>
        private static async Task<DirectoryInfo[]> ScanDirectory_Async(DuoDirInfo scannedDirectory)
        //MyPlace parent, DirectoryInfo directory)
        {
            DirectoryInfo[] subDirectories = await Task.Run(() => scannedDirectory.DirectoryInfo.GetDirectories());
            return subDirectories;//Task.FromResult(subDirectories);
        }

        private static void ScanDirectoryAndAddSub_Recurrent(MyPlace md, DirectoryInfo di, DB DB)
        {
            foreach (DirectoryInfo subDir in di.GetDirectories())
            {
                MyDirectory msd = CreateMyDirectoryImage(md, subDir);
                AddDirectoryToDBList(msd, DB);
                ScanDirectoryAndAddSub_Recurrent(msd, subDir, DB);
            }
            ScandAndAddFilesHere_Old(md, di, DB);
        }

    }
}
