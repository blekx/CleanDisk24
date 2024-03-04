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
        // private MyPlace Browser1 = Database.emptyFakeRoot;
        // private MyPlace Browser2 = Database.emptyFakeRoot;
        private MyPlace Browser1 = Database.prodigyNTB;
        private MyPlace Browser2 = Database.prodigyPC;

        public async void SetBrowsedDirectory(MyPlace source, MyPlace area)//attepmt for universal solution/useless
        {
            area = await ScanAndAdd_Only_OneDirectory_async(source);
        }
        public async Task<string> SetBrowser1_async(MyPlace selectedDirectory) // Remake REF ...or no
        {
            string setDirResult = $"Browser 1 unable to set {selectedDirectory}";
            if (Directory.Exists(selectedDirectory.WholePath)) //strange check
            {
                Browser1 = await ScanAndAdd_Only_OneDirectory_async(selectedDirectory);
                setDirResult = $"Browser1 set to: {selectedDirectory.WholePath}";
            }
            return setDirResult;
        }
        public async void SetBrowser2(MyPlace selectedDirectory)
        {
            Browser2 = await ScanAndAdd_Only_OneDirectory_async(selectedDirectory);
        }
        public ObservableCollection<MyRootPlace> GetChosenRoots() => DB.SetOfChosenRoots;
        public ObservableCollection<MyItemDirectoryOrFile> GetDirectoryChildren_RootsSub()
            => new ObservableCollection<MyItemDirectoryOrFile>(Browser1.Items);
        public ObservableCollection<MyItemDirectoryOrFile> GetDirectoryChildren_ChosenSub()
            => new ObservableCollection<MyItemDirectoryOrFile>(Browser2.Items);

        /// <summary>
        /// 3 overloads, this takes "MyPlace" and redirects to the 3rd.
        /// </summary>
        /// <param name="directoryImage"></param>
        /// <returns></returns>
        public async Task<MyPlace> ScanAndAdd_Only_OneDirectory_async(MyPlace directoryImage)
        {
            /*
            string givenPath;
            if (directoryImage.GetType() == typeof(MyRootDirectory)) givenPath = ((MyRootDirectory)directoryImage).WholePath;
            else throw new NotImplementedException("MyPlace different than MyRootDirectory.");
            DirectoryInfo scannedDirectory = new DirectoryInfo(givenPath);
            */
            DirectoryInfo scannedDirectory = new DirectoryInfo(directoryImage.WholePath);

            DuoDirInfo currentlyScannedDirectory = new DuoDirInfo(directoryImage, scannedDirectory);
            return await ScanAndAdd_Only_OneDirectory_async(currentlyScannedDirectory);
        }
        /// <summary>
        /// 3 overloads, this takes "System.IO.DirectoryInfo" and redirects to the 3rd.
        /// </summary>
        /// <param name="scannedDirectory"></param>
        /// <returns></returns>
        public async Task<MyPlace> ScanAndAdd_Only_OneDirectory_async(DirectoryInfo scannedDirectory)
        {
            MyPlace directoryImage = CreateMyDirectoryImage(scannedDirectory);
            DuoDirInfo currentlyScannedDirectory = new DuoDirInfo(directoryImage, scannedDirectory);
            return await ScanAndAdd_Only_OneDirectory_async(currentlyScannedDirectory);
        }
        /// <summary>
        /// 3 overloads, this takes "DuoDirInfo" and does the actual work.
        /// </summary>
        /// <param name="currentlyScannedDirectory" class="DuoDirInfo"></param>
        /// <returns></returns>
        public async Task<MyPlace> ScanAndAdd_Only_OneDirectory_async(DuoDirInfo currentlyScannedDirectory)
        {
            DirectoryInfo[] moreDirInfos = await ScanDirectory_Async(currentlyScannedDirectory);
            await Task.Run(() => ScandAndAddFilesHere_Async(currentlyScannedDirectory.MyPlace, currentlyScannedDirectory.DirectoryInfo));
            foreach (DirectoryInfo foundDirInfo in moreDirInfos)
            {
                MyDirectory foundDir = new MyDirectory(currentlyScannedDirectory.MyPlace, foundDirInfo);
                AddDirectoryToDBList(foundDir);
            }
            return currentlyScannedDirectory.MyPlace;
        }
    }
}
