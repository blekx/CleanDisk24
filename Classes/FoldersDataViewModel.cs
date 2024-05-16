using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using CleanDisk24.Database;
using CleanDisk24.DataWorker;

namespace CleanDisk24
{
    public class FoldersDataViewModel : INotifyPropertyChanged
    {
        private DB DB { get; set; }
        //private DataWorkerAgent DB;

        //public FoldersDataViewModel(DB db) { DB = db; }
        /*public FoldersDataViewModel(DataWorkerOld DB)
        {
            this.DB = DB;
        }*/
        public FoldersDataViewModel(DB db)
        {
            DB = db;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        //private class ModelItems
        public ObservableCollection<MyItemDirectoryOrFile> TestModelItems => new ObservableCollection<MyItemDirectoryOrFile>() { MyItemDirectoryOrFile.Def() };

        public ObservableCollection<MyRootPlace> AllRoots// => new ObservableCollection<MyRoot>();
        {
            get
            {
                return DB.AllRoots;
            }
        }

        //public ObservableCollection<MyRootDrive> ModelHarddrives => DB.GetAllUnits();
        public ObservableCollection<MyRootDrive> ModelHarddrives => DataWorkerAgent.GetAllDiscs(DB);
        public ObservableCollection<MyRootPlace> ModelRoots => DataWorkerAgent.GetAllRoots(DB); // panel 1/4
        /*
         * using System;
using System.IO;

class Program
{
    static void Main()
    {
        DriveInfo[] allDrives = DriveInfo.GetDrives();

        Console.WriteLine("List of Hard Disks:");
        foreach (DriveInfo d in allDrives)
        {
            if (d.DriveType == DriveType.Fixed)
            {
                Console.WriteLine($"Drive: {d.Name}");
                Console.WriteLine($"  Volume Label: {d.VolumeLabel}");
                Console.WriteLine($"  File System: {d.DriveFormat}");
                Console.WriteLine($"  Total Size: {FormatBytes(d.TotalSize)}");
                Console.WriteLine($"  Available Free Space: {FormatBytes(d.AvailableFreeSpace)}");
                Console.WriteLine();
            }
        }
    }

    static string FormatBytes(long bytes)
    {
        const long KB = 1024;
        const long MB = KB * 1024;
        const long GB = MB * 1024;

        if (bytes >= GB)
            return $"{bytes / GB} GB";
        else if (bytes >= MB)
            return $"{bytes / MB} MB";
        else if (bytes >= KB)
            return $"{bytes / KB} KB";
        else
            return $"{bytes} bytes";
    }
}
         */
        public ObservableCollection<MyItemDirectoryOrFile> ModelRoots_Sub => TestModelItems; //DB.GetDirectoryChildren_RootsSub(); // panel 2/4
        public ObservableCollection<MyRootPlace> ModelChosenRoots => DataWorkerAgent.GetAllRoots(DB);// DB.GetChosenRoots(); // panel 3/4
        public ObservableCollection<MyItemDirectoryOrFile> ModelChosen_Sub => DataWorkerAgent.GetDirectoryChildren_ChosenSub(); // panel 4/4
        public MyPlace ModelRootsHead => DataWorkerAgent.GetBrowser2Directory(DB);
        public MyPlace ModelChosenRootsHead => DataWorkerAgent.GetBrowser2Directory(DB);
    }
}
