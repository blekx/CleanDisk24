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
        // private MyPlace Browser1 = Database.emptyFakeRoot;
        // private MyPlace Browser2 = Database.emptyFakeRoot;
        private static MyPlace Browser1 = Database.prodigyNTB;
        private static MyPlace Browser2 = Database.prodigyPC;

        public static async void SetBrowsedDirectory(MyPlace source, MyPlace area, Database DB)//attepmt for universal solution/useless
        {
            area = await ScanAndAdd_Only_OneDirectory_async(source, DB);
        }
        public static async Task<string> SetBrowser1_async(MyPlace selectedDirectory, Database DB) // Remake REF ...or no
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
        public static async void SetBrowser2(MyPlace selectedDirectory, Database DB)
        {
            Browser2 = await ScanAndAdd_Only_OneDirectory_async(selectedDirectory, DB);
        }
        public static ObservableCollection<MyRootPlace> GetChosenRoots(Database DB) => DB.SetOfChosenRoots;
        public static ObservableCollection<MyItemDirectoryOrFile> GetDirectoryChildren_RootsSub()
            => new ObservableCollection<MyItemDirectoryOrFile>(Browser1.Items);
        public static ObservableCollection<MyItemDirectoryOrFile> GetDirectoryChildren_ChosenSub()
            => new ObservableCollection<MyItemDirectoryOrFile>(Browser2.Items);

        /// <summary>
        /// 3 overloads, this takes "MyPlace" and redirects to the 3rd.
        /// </summary>
        /// <param name="directoryImage"></param>
        /// <returns></returns>
        public static async Task<MyPlace> ScanAndAdd_Only_OneDirectory_async(MyPlace directoryImage, Database DB)
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
        public static async Task<MyPlace> ScanAndAdd_Only_OneDirectory_async(DirectoryInfo scannedDirectory, Database DB)
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
        public static async Task<MyPlace> ScanAndAdd_Only_OneDirectory_async(DuoDirInfo currentlyScannedDirectory, Database DB)
        {
            DirectoryInfo[] moreDirInfos = await ScanDirectory_Async(currentlyScannedDirectory);
            //ERROR HERE !!!!!!!!!!!!
            //await Task.Run(() => ScandAndAddFilesHere_Async(currentlyScannedDirectory.MyPlace, currentlyScannedDirectory.DirectoryInfo, DB));
            foreach (DirectoryInfo foundDirInfo in moreDirInfos)
            {
                MyDirectory foundDir = new MyDirectory(currentlyScannedDirectory.MyPlace, foundDirInfo);
                AddDirectoryToDBList(foundDir, DB);
            }
            return currentlyScannedDirectory.MyPlace;
        }
    }
}
