﻿using CleanDisk24.Database;
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
        // private MyPlace Browser1 = DB.emptyFakeRoot;
        // private MyPlace Browser2 = DB.emptyFakeRoot;
        private static MyPlace Browser1 { get; set; } = DB.prodigyNTB;
        private static MyPlace Browser2 { get; set; } = DB.prodigyPC;

        public static async void SetBrowsedDirectory(MyPlace source, MyPlace area, DB DB)//attepmt for universal solution/useless
        {
            area = await ScanAndAdd_Only_OneDirectory_async(source, DB);
        }
        public static async Task<string> SetBrowser1_async(MyPlace selectedDirectory, DB DB) // Remake REF ...or no
        {
            string setDirResult = $"Browser 1 unable to set {selectedDirectory}";
            if (Directory.Exists(selectedDirectory.WholePath)) //strange check
            {
                // debugging
                Browser1 = //selectedDirectory;//
                                             await ScanAndAdd_Only_OneDirectory_async(selectedDirectory, DB);
                setDirResult = $"Browser1 set to: {selectedDirectory.WholePath}";
            }
            return setDirResult;
        }
        public static async void SetBrowser2(MyPlace selectedDirectory, DB DB)
        {
            Browser2 = await ScanAndAdd_Only_OneDirectory_async(selectedDirectory, DB);
        }
        public static ObservableCollection<MyRootPlace> GetChosenRoots(DB DB) => DB.SetOfChosenRoots;
        public static ObservableCollection<MyItemDirectoryOrFile> GetDirectoryChildren_RootsSub()
            => new ObservableCollection<MyItemDirectoryOrFile>(Browser1.Items);
        public static ObservableCollection<MyItemDirectoryOrFile> GetDirectoryChildren_ChosenSub()
            => new ObservableCollection<MyItemDirectoryOrFile>(Browser2.Items);

        /// <summary>
        /// 3 overloads, this takes "MyPlace" and redirects to the 3rd.
        /// </summary>
        /// <param name="directoryImage"></param>
        /// <returns></returns>
        public static async Task<MyPlace> ScanAndAdd_Only_OneDirectory_async(MyPlace directoryImage, DB DB)
        {
            /*
            string givenPath;
            if (directoryImage.GetType() == typeof(MyRootDirectory)) givenPath = ((MyRootDirectory)directoryImage).WholePath;
            else throw new NotImplementedException("MyPlace different than MyRootDirectory.");
            DirectoryInfo scannedDirectory = new DirectoryInfo(givenPath);
            */
            DirectoryInfo scannedDirectory = new DirectoryInfo(directoryImage.WholePath);

            DuoDirInfo currentlyScannedDirectory
                = new DuoDirInfo(directoryImage, scannedDirectory);
            //currentlyScannedDirectory = await ((directoryImage,scannnedDiectory) => { (new DuoDirInfo(directoryImage, scannedDirectory)); });
            return await ScanAndAdd_Only_OneDirectory_async(currentlyScannedDirectory, DB);
        }
        /// <summary>
        /// 3 overloads, this takes "System.IO.DirectoryInfo" and redirects to the 3rd.
        /// </summary>
        /// <param name="scannedDirectory"></param>
        /// <returns></returns>
        public static async Task<MyPlace> ScanAndAdd_Only_OneDirectory_async(DirectoryInfo scannedDirectory, DB DB)
        {
            MyPlace directoryImage = CreateMyDirectoryImage(scannedDirectory, DB);
            DuoDirInfo currentlyScannedDirectory = new DuoDirInfo(directoryImage, scannedDirectory);
            return await ScanAndAdd_Only_OneDirectory_async(currentlyScannedDirectory, DB);
        }
        /// <summary>
        /// 3 overloads, this takes "DuoDirInfo" and does the actual work.
        /// </summary>
        /// <param name="currentlyScannedDirectory" class="DuoDirInfo"></param>
        /// <returns></returns>
        public static async Task<MyPlace> ScanAndAdd_Only_OneDirectory_async_Old(DuoDirInfo currentlyScannedDirectory, DB DB)
        {
            // 1/2 Directories:
            DirectoryInfo[] moreDirInfos = await ScanDirectory_Async(currentlyScannedDirectory);
            // 2/2 Files:
            //ERROR HERE: !!!!!!!!!!!!
            //await Task.Run(() => ScandAndAddFilesHere_Async(currentlyScannedDirectory.MyPlace, currentlyScannedDirectory.DirectoryInfo, DB));
            //FIXED: 
            await ScandAndAddFilesHere_Async(currentlyScannedDirectory.MyPlace, currentlyScannedDirectory.DirectoryInfo, DB);
            foreach (DirectoryInfo foundDirInfo in moreDirInfos)
            {
                MyDirectory foundDir = new MyDirectory(currentlyScannedDirectory.MyPlace, foundDirInfo);
                AddDirectoryToDBList(foundDir, DB);
            }
            return currentlyScannedDirectory.MyPlace;
        }
        public static async Task<MyPlace> ScanAndAdd_Only_OneDirectory_async_Attempt2didntWorkWell(DuoDirInfo currentlyScannedDirectory, DB DB)
        {
            /*
            Task<int> MoreFiles = Task.Run(async () =>
            {
                return await ScandAndAddFilesHere_Async(currentlyScannedDirectory.MyPlace, currentlyScannedDirectory.DirectoryInfo, DB);
            });*/
            DirectoryInfo[] moreDirInfos = await ScanDirectory_Async(currentlyScannedDirectory);
            foreach (DirectoryInfo foundDirInfo in moreDirInfos)
            {
                MyDirectory foundDir = new MyDirectory(currentlyScannedDirectory.MyPlace, foundDirInfo);
                AddDirectoryToDBList(foundDir, DB);
            }
            int filesAdded = await //MoreFiles;
                ScandAndAddFilesHere_Async(currentlyScannedDirectory.MyPlace, currentlyScannedDirectory.DirectoryInfo, DB);
            DB.Log($"{filesAdded.ToString()} files added in {currentlyScannedDirectory}");
            return currentlyScannedDirectory.MyPlace;
        }
        public static async Task<MyPlace> ScanAndAdd_Only_OneDirectory_async(DuoDirInfo currentlyScannedDirectory, DB DB)
        {
            DirectoryInfo[] moreDirInfos = await ScanDirectory_Async(currentlyScannedDirectory);
            foreach (DirectoryInfo foundDirInfo in moreDirInfos)
            {
                MyDirectory foundDir = new MyDirectory(currentlyScannedDirectory.MyPlace, foundDirInfo);
                AddDirectoryToDBList(foundDir, DB);
            }
            //int filesAdded = 
                await ScandAndAddFilesHere_Async(currentlyScannedDirectory.MyPlace, currentlyScannedDirectory.DirectoryInfo, DB);
            //DB.Log($"{filesAdded.ToString()} files added in {currentlyScannedDirectory}");
            return currentlyScannedDirectory.MyPlace;
        }

        internal static MyPlace GetBrowser1Directory(DB DB)
        {
            return Browser1;
        }

        internal static MyPlace GetBrowser2Directory(DB DB)
        {
            return Browser2;
        }

        internal static MyRootPlace GetCurrentlyRemovedChoosen_MyRootPlace(DB DB)
        {
            return DB.CurrentlyRemoved;
        }

        internal static void SetupCurrentlyRemovedTo(DB DB, MyRootPlace dir)
        {
            DB.CurrentlyRemoved = dir;
        }

        internal static MyDirectory SetBrowser1IntoDB(DB DB, MyDirectory dir)
        {
            DB.Browser1 = dir;
            return dir;
        }
        internal static MyDirectory SetBrowser2IntoDB(DB DB, MyDirectory dir)
        {
            DB.Browser2 = dir;
            return dir;
        }
    }
}
