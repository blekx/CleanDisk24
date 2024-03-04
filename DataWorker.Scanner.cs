using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanDisk24
{
    public partial class DataWorker
    {
        public void ScanAndAddDirectory_Initialize_Old(DirectoryInfo di)
        {
            MyPlace directory = CreateMyDirectoryImage(di);
            ScanDirectoryAndAddSub_Recurrent(directory, di);
            ScandAndAddFilesHere_Old(directory, di);
        }
        public async void ScanAndAddDirectory_Initialize(DirectoryInfo rootDirInfo)
        {
            MyPlace rootPlace = CreateMyDirectoryImage(rootDirInfo);
            Stack<DuoDirInfo> directoriesToBeScanned = new Stack<DuoDirInfo>();
            directoriesToBeScanned.Push(new DuoDirInfo(rootPlace, rootDirInfo));
            DuoDirInfo currentlyScannedDirectory;
            int stackSize;
            do
            {
                currentlyScannedDirectory = directoriesToBeScanned.Pop();
                DirectoryInfo[] moreDirInfos = await ScanDirectory_Async(currentlyScannedDirectory);
                //currentlyScannedDirectory.MyPlace, currentlyScannedDirectory.DirectoryInfo);
                await Task.Run(() => ScandAndAddFilesHere_Async(currentlyScannedDirectory.MyPlace, currentlyScannedDirectory.DirectoryInfo));
                //ScandAndAddFilesHere_Async2(rootPlace, rootDirInfo);
                foreach (DirectoryInfo foundDirInfo in moreDirInfos)
                {
                    MyDirectory foundDir = new MyDirectory(currentlyScannedDirectory.MyPlace, foundDirInfo);
                    AddDirectoryToDBList(foundDir);
                    directoriesToBeScanned.Push(new DuoDirInfo(foundDir, foundDirInfo));
                }
                currentlyScannedDirectory = directoriesToBeScanned.Pop();
                stackSize = directoriesToBeScanned.Count();
                //} while (currentlyScannedDirectory != null);
            } while (stackSize > 0);

            //ScanDirectoryAndAddSub_Recurrent_Async(rootPlace, rootDirInfo);
        }

        internal void StartupLoadData()
        {
            throw new NotImplementedException();
        }

        private void ScandAndAddFilesHere_Old(MyPlace md, DirectoryInfo di)
        {
            foreach (FileInfo fileinfo in di.GetFiles())
            {
                MyFile mf = new MyFile(md, fileinfo);
                md.Items.Add(mf);
                DB.AllFiles.Add(mf);
            }
        }
        private async Task<bool> ScandAndAddFilesHere_Async_Wrong(MyPlace md, DirectoryInfo di)
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
        private async void ScandAndAddFilesHere_Async(MyPlace md, DirectoryInfo di)
        {
            FileInfo[] files = await Task.Run(() => di.GetFiles());
            foreach (FileInfo fileInfo in files)
            {
                MyFile mf = new MyFile(md, fileInfo);
                md.Items.Add(mf);
                DB.AllFiles.Add(mf);
            }
        }
        private Task<FileInfo[]> GetFiles(DirectoryInfo di) => Task.FromResult(di.GetFiles());
        //FileInfo[] files;
        //files = di.GetFiles();
        //files;
        private async void ScanDirectoryAndAddSub_Recurrent_Async(MyPlace md, DirectoryInfo di)
        {
            DirectoryInfo[] subDirectories = await Task.Run(() => di.GetDirectories());
            foreach (DirectoryInfo subDir in subDirectories)
            {
                MyDirectory msd = CreateMyDirectoryImage(md, subDir);
                AddDirectoryToDBList(msd);
                ScanDirectoryAndAddSub_Recurrent(msd, subDir);
            }
            ScandAndAddFilesHere_Old(md, di);
        }
        ///<summary>This directory must have a parent.</summary>
        private async Task<DirectoryInfo[]> ScanDirectory_Async(DuoDirInfo scannedDirectory)
        //MyPlace parent, DirectoryInfo directory)
        {
            DirectoryInfo[] subDirectories = await Task.Run(() => scannedDirectory.DirectoryInfo.GetDirectories());
            return subDirectories;//Task.FromResult(subDirectories);
        }

        private void ScanDirectoryAndAddSub_Recurrent(MyPlace md, DirectoryInfo di)
        {
            foreach (DirectoryInfo subDir in di.GetDirectories())
            {
                MyDirectory msd = CreateMyDirectoryImage(md, subDir);
                AddDirectoryToDBList(msd);
                ScanDirectoryAndAddSub_Recurrent(msd, subDir);
            }
            ScandAndAddFilesHere_Old(md, di);
        }

    }
}
